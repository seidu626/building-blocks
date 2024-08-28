#nullable enable
using CSharpFunctionalExtensions;

namespace BuildingBlocks.Exceptions;

public sealed class Error : ValueObject
{
    private readonly string _code;
    private readonly string _message;
    private readonly string _stackTrace;

    public static implicit operator string(Error error)
    {
        return $"code: {error._code} - message: {error._message} - stackTrace: {error._stackTrace}";
    }

    public Error(string code, string message, Exception? exception = null)
    {
        _code = code;
        _message = message;
        FriendlyMessage = message;
        _stackTrace = exception?.StackTrace ?? string.Empty;
    }

    public string FriendlyMessage { get; set; }

    public string StackTrace => _stackTrace;


    public override string ToString()
    {
        return $"code: {_code} - message: {_message} - stackTrace: {_stackTrace}";
    }

    protected override IEnumerable<IComparable> GetEqualityComponents()
    {
        yield return _code;
    }
}