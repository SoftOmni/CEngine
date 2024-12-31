namespace CEngine.Structure.Constants.Integers;

public partial class Integer
{
    public static bool TryReadBigEndian(ReadOnlySpan<byte> source, bool isUnsigned, out Integer value)
    {
        throw new NotImplementedException();
    }

    public static bool TryReadLittleEndian(ReadOnlySpan<byte> source, bool isUnsigned, out Integer value)
    {
        throw new NotImplementedException();
    }

    public bool TryWriteBigEndian(Span<byte> destination, out int bytesWritten)
    {
        throw new NotImplementedException();
    }

    public bool TryWriteLittleEndian(Span<byte> destination, out int bytesWritten)
    {
        throw new NotImplementedException();
    }

    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        throw new NotImplementedException();
    }

    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
    {
        throw new NotImplementedException();
    }
}