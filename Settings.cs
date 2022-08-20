namespace TextToAudioBot;

public sealed class Settings
{
    public string TelegramBotToken { get; set; } = string.Empty;
    public string SubscriptionKey { get; set; } = string.Empty;
    public string ServiceRegion { get; set; } = string.Empty;
    public string BlobAccessKey { get; set; } = string.Empty;
    public string BlobConnectionString { get; set; } = string.Empty;
}