using System;
using System.Threading.Tasks;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.CognitiveServices.Speech.Translation;

public class SpeechRecgonition_SingleShot
{
    private readonly string _speechKey;
    private readonly string _speechRegion;

    public SpeechRecgonition_SingleShot(string speechKey, string speechRegion)
    {
        _speechKey = speechKey;
        _speechRegion = speechRegion;
    }

    public async Task DoWork()
    {
        var speechTranslationConfig = SpeechTranslationConfig.FromSubscription(_speechKey, _speechRegion);
        speechTranslationConfig.SpeechRecognitionLanguage = "en-US";
        speechTranslationConfig.AddTargetLanguage("en");


        using var audioConfig = AudioConfig.FromDefaultMicrophoneInput();
        using var translationRecognizer = new TranslationRecognizer(speechTranslationConfig, audioConfig);

        Console.WriteLine("Speak into your microphone.");
        var translationRecognitionResult = await translationRecognizer.RecognizeOnceAsync();
        OutputSpeechRecognitionResult(translationRecognitionResult);
    }

    public void OutputSpeechRecognitionResult(TranslationRecognitionResult translationRecognitionResult)
    {
        switch (translationRecognitionResult.Reason)
        {
            case ResultReason.TranslatedSpeech:
                Console.WriteLine($"RECOGNIZED: Text={translationRecognitionResult.Text}");
                foreach (var element in translationRecognitionResult.Translations)
                {
                    Console.WriteLine($"TRANSLATED into '{element.Key}': {element.Value}");
                }
                break;
            case ResultReason.NoMatch:
                Console.WriteLine($"NOMATCH: Speech could not be recognized.");
                break;
            case ResultReason.Canceled:
                var cancellation = CancellationDetails.FromResult(translationRecognitionResult);
                Console.WriteLine($"CANCELED: Reason={cancellation.Reason}");

                if (cancellation.Reason == CancellationReason.Error)
                {
                    Console.WriteLine($"CANCELED: ErrorCode={cancellation.ErrorCode}");
                    Console.WriteLine($"CANCELED: ErrorDetails={cancellation.ErrorDetails}");
                    Console.WriteLine($"CANCELED: Did you set the speech resource key and region values?");
                }
                break;
        }
    }
}
