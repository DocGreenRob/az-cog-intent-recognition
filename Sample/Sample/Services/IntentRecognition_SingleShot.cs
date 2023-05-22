using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.CognitiveServices.Speech.Intent;
using Newtonsoft.Json;
using Sample.Models;
using System;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

public class IntentRecognition_SingleShot
{
    // Your CLU project name and deployment name.
    private readonly string _cluProjectName;
    private readonly string _cluDeploymentName;
    private readonly string _speechKey;
    private readonly string _speechRegion;
    private readonly string _languageKey;
    private readonly string _languageEndpoint;

    public IntentRecognition_SingleShot(string speechKey, 
                                string speechRegion,
                                string languageKey,
                                string languageEndpoint,
                                string projectName,
                                string deploymentName)
    {
        _speechKey = speechKey;
        _speechRegion = speechRegion;
        _languageKey = languageKey;
        _languageEndpoint = languageEndpoint;
        _cluProjectName = projectName;
        _cluDeploymentName = deploymentName;
    }

    public async Task DoWork()
    {
        var speechConfig = SpeechConfig.FromSubscription(_speechKey, _speechRegion);
        speechConfig.SpeechRecognitionLanguage = "en-US";

        using var audioConfig = AudioConfig.FromDefaultMicrophoneInput();

        // Creates an intent recognizer in the specified language using microphone as audio input.
        using (var intentRecognizer = new IntentRecognizer(speechConfig, audioConfig))
        {
            var cluModel = new ConversationalLanguageUnderstandingModel(
                                                                        _languageKey,
                                                                        _languageEndpoint,
                                                                        _cluProjectName,
                                                                        _cluDeploymentName);
            var collection = new LanguageUnderstandingModelCollection();
            collection.Add(cluModel);
            intentRecognizer.ApplyLanguageModels(collection);
            

            Console.WriteLine("Speak into your microphone.");
            var recognitionResult = await intentRecognizer.RecognizeOnceAsync().ConfigureAwait(false);

            // Checks result.
            if (recognitionResult.Reason == ResultReason.RecognizedIntent)
            {
                Console.WriteLine($"RECOGNIZED: Text={recognitionResult.Text}");
                Console.WriteLine($"    Intent Id: {recognitionResult.IntentId}.");
                var jsonResult = recognitionResult.Properties.GetProperty(PropertyId.LanguageUnderstandingServiceResponse_JsonResult);
                Console.WriteLine($"    Language Understanding JSON: {jsonResult}.");
                var intentResponseDto = JsonConvert.DeserializeObject<IntentRecognitionResultDto>(jsonResult);
                Console.WriteLine($"Product Suggestion: {intentResponseDto.result.prediction.entities.First().category}");
            }
            else if (recognitionResult.Reason == ResultReason.RecognizedSpeech)
            {
                Console.WriteLine($"RECOGNIZED: Text={recognitionResult.Text}");
                Console.WriteLine($"    Intent not recognized.");
            }
            else if (recognitionResult.Reason == ResultReason.NoMatch)
            {
                Console.WriteLine($"NOMATCH: Speech could not be recognized.");
            }
            else if (recognitionResult.Reason == ResultReason.Canceled)
            {
                var cancellation = CancellationDetails.FromResult(recognitionResult);
                Console.WriteLine($"CANCELED: Reason={cancellation.Reason}");

                if (cancellation.Reason == CancellationReason.Error)
                {
                    Console.WriteLine($"CANCELED: ErrorCode={cancellation.ErrorCode}");
                    Console.WriteLine($"CANCELED: ErrorDetails={cancellation.ErrorDetails}");
                    Console.WriteLine($"CANCELED: Did you update the subscription info?");
                }
            }
        }
    }
}