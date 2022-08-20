using System.Text.Json.Serialization;

namespace TextToAudioBot;

public sealed class VoiceEntity
{
    [JsonPropertyName("Title")]
    public string Title { get; set; } = string.Empty;
    [JsonPropertyName("Gender")]
    public string Gender { get; set; } = string.Empty;
    [JsonPropertyName("Locale")]
    public string Locale { get; set; } = string.Empty;
    [JsonPropertyName("VoiceName")]
    public string VoiceName { get; set; } = string.Empty;
}
