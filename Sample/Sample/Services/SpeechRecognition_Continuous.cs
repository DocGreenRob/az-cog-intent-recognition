using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.CognitiveServices.Speech.Intent;
using System;
using System.Threading.Tasks;

public class SpeechRecognition_Continuous
{
    // Your CLU project name and deployment name.
    private readonly string _cluProjectName;
    private readonly string _cluDeploymentName;
    private readonly string _speechKey;
    private readonly string _speechRegion;
    private readonly string _languageKey;
    private readonly string _languageEndpoint;
    private readonly IntentRecognition_SingleShot _intentRecgonition_SingleShot;

    public SpeechRecognition_Continuous(string speechKey,
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
        using (var speechRecognizer = new SpeechRecognizer(speechConfig, audioConfig))
        {
            // manage the state of speech recognition
            var stopRecognition = new TaskCompletionSource<int>();

            speechRecognizer.Recognizing += (s, e) =>
            {
                Console.WriteLine($"RECOGNIZING: Text={e.Result.Text}");
                
            };

            speechRecognizer.Recognized += (s, e) =>
            {
                if (e.Result.Reason == ResultReason.RecognizedSpeech)
                {
                    Console.WriteLine($"RECOGNIZED: Text={e.Result.Text}");
                }
                else if (e.Result.Reason == ResultReason.NoMatch)
                {
                    Console.WriteLine($"NOMATCH: Speech could not be recognized.");
                }
            };

            speechRecognizer.Canceled += (s, e) =>
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

            speechRecognizer.SessionStopped += (s, e) =>
            {
                Console.WriteLine("\n    Session stopped event.");
                stopRecognition.TrySetResult(0);
            };

            await speechRecognizer.StartContinuousRecognitionAsync();

            // Waits for completion. Use Task.WaitAny to keep the task rooted.
            Task.WaitAny(new[] { stopRecognition.Task });

            // Make the following call at some point to stop recognition:
            // await speechRecognizer.StopContinuousRecognitionAsync();
        }
    }
}