namespace Weavly.Core.Shared.Persistence;

[Serializable]
public class DocumentNotFoundException : Exception
{
    public DocumentNotFoundException() { }

    public DocumentNotFoundException(string? message)
        : base(message) { }

    public DocumentNotFoundException(string? message, Exception? innerException)
        : base(message, innerException) { }
}
