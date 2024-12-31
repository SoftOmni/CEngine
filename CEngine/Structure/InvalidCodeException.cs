using System.Runtime.Serialization;

namespace JavaScriptEngine.Core;

[Serializable]
public class InvalidCodeException : Exception
{
    public InvalidCodeException()
    {
    }

    protected InvalidCodeException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public InvalidCodeException(string? message) : base(message)
    {
    }

    public InvalidCodeException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}