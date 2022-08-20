# Text2VoiceMessageBot
Text2VoiceMessageBot allows you to make voice messages from text using Azure Speech-To-Text Service.

## Appsettings.json setup:
Appsettings.json must have `Settings` field with this values:

```
"Settings" : {
        "TelegramBotToken": "<YOUR_BOT_TOKEN>",
        "SubscriptionKey" : "<YOUR_SUBSCRIPTION_KEY>",
        "ServiceRegion": "<YOUR_SERVICE_REGION>",
        "BlobAccessKey": "<YOUR_BLOB_ACCESS_KEY>",
        "BlobConnectionString": "<YOUR_BLOB_CONNECTION_STRING>"
    }
```

## SupportedVoices.json setup:
SupportedVoices.json is array of object in this format:

```
{
      "Title": "Title that will be appeared over the text field",
      "Gender": "Voice gender",
      "Locale": "Voice locale",
      "VoiceName": "Voice name"
}
```
### Example:
```
[
    {
      "Title": "\uD83C\uDDFA\uD83C\uDDE6 Ostap",
      "Gender": "Male",
      "Locale": "uk-UA",
      "VoiceName": "uk-UA-OstapNeural"
    },
    {
      "Title": "\uD83C\uDDFA\uD83C\uDDE6 Polina",
      "Gender": "Female",
      "Locale": "uk-UA",
      "VoiceName": "uk-UA-PolinaNeural"
    }
  ]
```

List of all supported voices you can find here: [Microsoft Docs](https://docs.microsoft.com/en-us/azure/cognitive-services/speech-service/language-support).
