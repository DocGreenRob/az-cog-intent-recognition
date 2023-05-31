using System;
using System.Threading.Tasks;

class Program
{
    // "LANGUAGE_KEY", "LANGUAGE_ENDPOINT", "SPEECH_KEY", and "SPEECH_REGION"
    static readonly string languageKey = Environment.GetEnvironmentVariable("LANGUAGE_KEY");
    static readonly string languageEndpoint = Environment.GetEnvironmentVariable("LANGUAGE_ENDPOINT");
    static readonly string speechKey = Environment.GetEnvironmentVariable("SPEECH_KEY");
    static readonly string speechRegion = Environment.GetEnvironmentVariable("SPEECH_REGION");

    async static Task Main(string[] args)
    {
        //var speechRecognitionSingleShot = new SpeechRecgonition_SingleShot(speechKey, speechRegion);
        //await speechRecognitionSingleShot.DoWork();

        //var intentRecognitionSingleShot = new IntentRecognition_SingleShot(speechKey,
        //                                                                    speechRegion,
        //                                                                    languageKey,
        //                                                                    languageEndpoint,
        //                                                                    "OpenCS",
        //                                                                    "OpenCS.Deploy4");
        //await intentRecognitionSingleShot.DoWork();

        //var speechRecgonitionContinuous = new SpeechRecognition_Continuous(speechKey,
        //                                                                    speechRegion,
        //                                                                    languageKey,
        //                                                                    languageEndpoint,
        //                                                                    "OpenCS",
        //                                                                    "OpenCS.Deploy4");
        //await speechRecgonitionContinuous.DoWork();

        var intentRecgonitionContinuous = new IntentRecognition_Continuous2(speechKey,
                                                                            speechRegion,
                                                                            languageKey,
                                                                            languageEndpoint,
                                                                            "OpenCS",
                                                                            "OpenCS.Deploy4");
        await intentRecgonitionContinuous.ContinuousRecognitionWithFileAsync();
    }
}
