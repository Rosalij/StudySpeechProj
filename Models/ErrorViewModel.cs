namespace StudySpeech.Models;
// Represents the model for error view, containing the request ID and a property to determine if the request ID should be shown.
public class ErrorViewModel
{
    public string? RequestId { get; set; }

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}
