namespace BusinessCardManager.Api.Importing.Exceptions;

public class BusinessCardImportException : Exception
{
    public BusinessCardImportException(string message)
        : this(message, [message])
    {
    }

    public BusinessCardImportException(string message, IReadOnlyList<string> errors)
        : base(message)
    {
        Errors = errors;
    }

    public IReadOnlyList<string> Errors { get; }
}
