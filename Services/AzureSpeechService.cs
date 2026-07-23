using Microsoft.CognitiveServices.Speech;
/* this class provides integration with Azure Speech Services for text-to-speech functionality */
public class AzureSpeechService
{
// Private fields to store the Azure Speech API key and region, initialized in the constructor.
    private readonly string _key;
    private readonly string _region;


// Constructor that initializes the AzureSpeechService with configuration settings for the API key and region.
 public AzureSpeechService(IConfiguration config)
{ // Retrieve the Azure Speech API key and region from the configuration settings, throwing an exception if they are not configured.
    _key = config["AzureSpeech:Key"] 
        ?? throw new ArgumentException("Azure Speech key not configured");

    _region = config["AzureSpeech:Region"] 
        ?? throw new ArgumentException("Azure Speech region not configured");
}

// Converts the provided text to speech using Azure Speech Services and returns the resulting audio data as a byte array.
public async Task<byte[]> TextToSpeechAsync(string text)
{ // Create a SpeechConfig object using the API key and region, and set the desired voice for speech synthesis.
    var config = SpeechConfig.FromSubscription(_key, _region);
    config.SpeechSynthesisVoiceName = "en-GB-OllieMultilingualNeural";

// Create a SpeechSynthesizer object with the configuration and use it to synthesize speech from the provided text.
    using var synthesizer = new SpeechSynthesizer(config, null);

    var result = await synthesizer.SpeakTextAsync(text);

    if (result.Reason == ResultReason.SynthesizingAudioCompleted)
    {// If the speech synthesis was successful, return the audio data as a byte array.
        return result.AudioData;
    }
// Handle cases where speech synthesis was canceled, throwing an exception with details about the cancellation reason and error.
    if (result.Reason == ResultReason.Canceled)
    {
        var cancellation = SpeechSynthesisCancellationDetails.FromResult(result);
        throw new Exception($"Speech failed: {cancellation.Reason} - {cancellation.ErrorDetails}");
    }
// If the speech synthesis failed for any other reason, throw a generic exception indicating that the speech synthesis failed.
    throw new Exception($"Speech failed: {result.Reason}");
}
}