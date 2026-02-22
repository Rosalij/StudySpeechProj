using Microsoft.CognitiveServices.Speech;

public class AzureSpeechService
{
    private readonly string _key;
    private readonly string _region;

 public AzureSpeechService(IConfiguration config)
{
    _key = config["AzureSpeech:Key"] 
        ?? throw new ArgumentException("Azure Speech key not configured");

    _region = config["AzureSpeech:Region"] 
        ?? throw new ArgumentException("Azure Speech region not configured");
}
public async Task<byte[]> TextToSpeechAsync(string text)
{
    var config = SpeechConfig.FromSubscription(_key, _region);
    config.SpeechSynthesisVoiceName = "en-GB-OllieMultilingualNeural";

    using var synthesizer = new SpeechSynthesizer(config, null);

    var result = await synthesizer.SpeakTextAsync(text);

    if (result.Reason == ResultReason.SynthesizingAudioCompleted)
    {
        return result.AudioData;
    }

    if (result.Reason == ResultReason.Canceled)
    {
        var cancellation = SpeechSynthesisCancellationDetails.FromResult(result);
        throw new Exception($"Speech failed: {cancellation.Reason} - {cancellation.ErrorDetails}");
    }

    throw new Exception($"Speech failed: {result.Reason}");
}
}