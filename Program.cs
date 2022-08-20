using System.Diagnostics;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using System.Text.Json;
using Azure.Storage.Blobs;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.Extensions.Configuration;
using TextToAudioBot;
using File = System.IO.File;

IConfiguration config = new ConfigurationBuilder()
    .AddJsonFile("Properties/appsettings.json")
    .Build();
Settings settings = config.GetRequiredSection("Settings").Get<Settings>();

List<VoiceEntity>? voices;
using (StreamReader r = new StreamReader("Properties/supportedVoices.json"))
{
    string json = r.ReadToEnd();
    voices = JsonSerializer.Deserialize<List<VoiceEntity>>(json);
}

var botClient = new TelegramBotClient(settings.TelegramBotToken);
var me = await botClient.GetMeAsync();
using var cts = new CancellationTokenSource();
botClient.StartReceiving(HandleUpdateAsync, PollingErrorHandler, null, cts.Token);

Console.WriteLine($"Start listening for @{me.Username}");
Console.ReadLine();

// stop the bot
cts.Cancel();

Task PollingErrorHandler(ITelegramBotClient bot, Exception ex, CancellationToken ct)
{
    Console.WriteLine($"Exception while polling for updates: {ex}");
    return Task.CompletedTask;
}

async Task HandleUpdateAsync(ITelegramBotClient bot, Update update, CancellationToken ct)
{
    try
    {
        await (update.Type switch
        {
            UpdateType.InlineQuery => BotOnInlineQueryReceived(bot, update.InlineQuery!),
            _ => Task.CompletedTask
        });
    }
#pragma warning disable CA1031
    catch (Exception ex)
    {
        Console.WriteLine($"Exception while handling {update.Type}: {ex}");
    }
#pragma warning restore CA1031
}

async Task BotOnInlineQueryReceived(ITelegramBotClient bot, InlineQuery inlineQuery)
{
    var results = new List<InlineQueryResult>();
    if (inlineQuery.Query.Length is > 0 and < 100)
    {
        int index = 0;
        foreach (var voice in voices)
        {
            results.Add(new InlineQueryResultAudio(
                id:$"{index++}",
                audioUrl: await GetAudioUrl(
                    VoiceName: voice.VoiceName,
                    Text: inlineQuery.Query
                    ),
                title:voice.Title));
        }
    }

    foreach (var file  in Directory.GetFiles("./voices"))
        File.Delete(file);

    await bot.AnswerInlineQueryAsync(inlineQuery.Id, results, cacheTime: 5);
}

async Task<string> GetAudioUrl(string VoiceName, string Text)
{
    Directory.CreateDirectory("./voices/");
    string wavFileName = VoiceName + Guid.NewGuid() + ".wav";
    string oggFileName = VoiceName + Guid.NewGuid() + ".ogg";
    string wavFilePath = Path.Combine("./voices/", wavFileName);
    string oggFilePath = Path.Combine("./voices/", oggFileName);
    
    var speechConfig = SpeechConfig.FromSubscription(settings.SubscriptionKey, settings.ServiceRegion);
    speechConfig.SpeechSynthesisVoiceName = VoiceName;
    using var audioConfig = AudioConfig.FromWavFileOutput(wavFilePath);
    using var synthesizer = new SpeechSynthesizer(speechConfig, audioConfig);
    await synthesizer.SpeakTextAsync(text: Text);

    Process process = new Process();
    process.StartInfo.RedirectStandardOutput = true;
    process.StartInfo.RedirectStandardError = true;
    process.StartInfo.FileName = "ffmpeg";

    var command = $"-i {wavFilePath} -c:a libopus {oggFilePath}";
    process.StartInfo.Arguments = command;
    process.StartInfo.UseShellExecute = false;
    process.StartInfo.CreateNoWindow = true;
    process.Start();
    process.WaitForExit();
    
    BlobServiceClient blobServiceClient = new BlobServiceClient(settings.BlobConnectionString);
    BlobContainerClient blobContainerClient = blobServiceClient.GetBlobContainerClient("voices");
    BlobClient blobClient = blobContainerClient.GetBlobClient(oggFileName);
    await blobClient.UploadAsync(oggFilePath, true);

    return blobClient.Uri.ToString();
}