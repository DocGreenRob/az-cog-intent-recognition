using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.CognitiveServices.Speech.Intent;
using System;
using System.Threading.Tasks;

public class IntentRecognition_Continuous2
{
    // Your CLU project name and deployment name.
    private readonly string _cluProjectName;
    private readonly string _cluDeploymentName;
    private readonly string _speechKey;
    private readonly string _speechRegion;
    private readonly string _languageKey;
    private readonly string _languageEndpoint;

    public IntentRecognition_Continuous2(string speechKey,
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

            Console.WriteLine("Begin conversation...");

            // manage the state of speech recognition
            var stopRecognition = new TaskCompletionSource<int>();


            intentRecognizer.Recognizing += (s, e) =>
                {
                    Console.WriteLine($"RECOGNIZING: Text={e.Result.Text}");

                };

            intentRecognizer.Recognized += async (s, e) =>
            {
                if (e.Result.Reason == ResultReason.RecognizedIntent)
                {
                    Console.WriteLine($"RECOGNIZED: Text={e.Result.Text}");

                    var recognitionResult = await intentRecognizer.RecognizeOnceAsync().ConfigureAwait(false);

                    // Checks result.
                    if (recognitionResult.Reason == ResultReason.RecognizedIntent)
                    {
                        Console.WriteLine($"RECOGNIZED: Text={recognitionResult.Text}");
                        Console.WriteLine($"    Intent Id: {recognitionResult.IntentId}.");
                        Console.WriteLine($"    Language Understanding JSON: {recognitionResult.Properties.GetProperty(PropertyId.LanguageUnderstandingServiceResponse_JsonResult)}.");
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
                else if (e.Result.Reason == ResultReason.NoMatch)
                {
                    Console.WriteLine($"NOMATCH: Speech could not be recognized.");
                }
            };

            intentRecognizer.Canceled += (s, e) =>
            {
                Console.WriteLine($"CANCELED: Reason={e.Reason}");

                if (e.Reason == CancellationReason.Error)
                {
                    Console.WriteLine($"CANCELED: ErrorCode={e.ErrorCode}");
                    Console.WriteLine($"CANCELED: ErrorDetails={e.ErrorDetails}");
                    Console.WriteLine($"CANCELED: Did you set the speech resource key and region values?");
                }

                stopRecognition.TrySetResult(0);
            };

            intentRecognizer.SessionStopped += (s, e) =>
            {
                Console.WriteLine("\n    Session stopped event.");
                stopRecognition.TrySetResult(0);
            };

            await intentRecognizer.StartContinuousRecognitionAsync();

            // Waits for completion. Use Task.WaitAny to keep the task rooted.
            Task.WaitAny(new[] { stopRecognition.Task });


        }
    }
}