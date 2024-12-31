using System.Diagnostics;
using System.Numerics;
using System.Text;
using CEngine.SegmentedStrings;
using CEngine.Types;

namespace CEngine.Structure.Constants.Integers;

public partial class Integer
{
    protected override void BuildInnerStructures()
    {
        char firstCharacter = Code[0];

        int lastIndex = ReadSuffix();

        if (firstCharacter == SpecialFormatLead)
        {
            BuildSpeciallyFormatedInteger(lastIndex);
            return;
        }

        Representation = IntegerRepresentation.Decimal;
        BuildDecimal(lastIndex);
    }

    private static BigInteger BuildValue(IStringBuilder code)
    {
        char firstCharacter = code[0];

        int lastIndex = ReadSuffix(code);

        if (firstCharacter == SpecialFormatLead)
        {
            return BuildSpeciallyFormatedInteger(code, lastIndex);
        }
        
        return BuildDecimal(code, lastIndex);
    }

    private int ReadSuffix()
    {
        IsUnsigned = false;
        UnsignedSuffixRepresentation = UnsignedSuffixRepresentation.Signed;

        IsLong = false;
        IsLongLong = false;

        LongSuffixRepresentation = LongSuffixRepresentation.Regular;

        int length = 0;
        int index = CodeLength - 1;

        SuffixOrder = Configuration.SuffixOrder;

        while (length < 3 && index >= 0 && Code[index].IsSuffix())
        {
            if (Code[index].IsUnsignedSuffix())
            {
                UnsignedSuffixRepresentation = Code[index] == UnsignedSuffixUppercase
                    ? UnsignedSuffixRepresentation.UnsignedUppercase
                    : UnsignedSuffixRepresentation.UnsignedLowercase;

                IsUnsigned = true;
            }
            else if (Code[index].IsLongSuffix() && IsLong)
            {
                LongSuffixRepresentation = Code[index] == LongSuffixUppercase
                    ? LongSuffixRepresentation.LongUppercase
                    : LongSuffixRepresentation.LongLowercase;

                IsLong = false;
                IsLongLong = true;
            }
            else if (Code[index].IsLongSuffix())
            {
                LongSuffixRepresentation = Code[index] == LongSuffixUppercase
                    ? LongSuffixRepresentation.LongLongUppercase
                    : LongSuffixRepresentation.LongLongLowercase;

                IsLong = true;
            }


            index -= 1;
            length += 1;
        }

        if (length is 2 or 3)
        {
            SuffixOrder = Code[CodeLength - 1].IsUnsignedSuffix()
                ? SuffixOrder.LongUnsigned
                : SuffixOrder.UnsignedLong;
        }

        return index;
    }

    private static int ReadSuffix(IStringBuilder code)
    {
        int length = 0;
        int index = code.Length - 1;

        while (length < 3 && index >= 0 && code[index].IsSuffix())
        {
            index -= 1;
            length += 1;
        }
        
        return index;
    }

    private void BuildSpeciallyFormatedInteger(int lastIndex)
    {
        if (Code.Length == 1)
        {
            Representation = IntegerRepresentation.Decimal;
            UnderlyingValue = BigInteger.Zero;
            return;
        }

        char directingCharacter = Code[1];
        switch (directingCharacter)
        {
            case BinaryLeadCharacterUppercase or BinaryLeadCharacterLowercase:
                BuildBinary(lastIndex);
                break;
            case HexadecimalLeadCharacterUppercase or HexadecimalLeadCharacterLowercase:
                BuildHexadecimal(lastIndex);
                break;
            default:
                BuildOctal(lastIndex);
                break;
        }
    }

    private static BigInteger BuildSpeciallyFormatedInteger(IStringBuilder code, int lastIndex)
    {
        if (code.Length == 1)
            return BigInteger.Zero;
        
        char directingCharacter = code[1];
        return directingCharacter switch
        {
            BinaryLeadCharacterUppercase 
                or BinaryLeadCharacterLowercase => BuildBinary(code, lastIndex),
            HexadecimalLeadCharacterUppercase 
                or HexadecimalLeadCharacterLowercase => BuildHexadecimal(code, lastIndex),
            _ => BuildOctal(code, lastIndex)
        };
    }

    private void BuildDecimal(int lastIndex)
    {
        Representation = IntegerRepresentation.Decimal;
        UnderlyingValue = BuildDecimal(Code, lastIndex);
    }

    private static BigInteger BuildDecimal(IStringBuilder code, int lastIndex)
    {
        BigInteger value = BigInteger.Zero;
        BigInteger power = new(10);

        for (int i = 0; i <= lastIndex; i++)
        {
            char character = code[i];

            value *= power;
            BigInteger charValue = new(character - '0');

            value += charValue;
        }

        return value;
    }

    private void BuildOctal(int lastIndex)
    {
        Representation = IntegerRepresentation.Octal;
        UnderlyingValue = BuildOctal(Code, lastIndex);
    }

    private static BigInteger BuildOctal(IStringBuilder code, int lastIndex)
    {
        BigInteger value = BigInteger.Zero;
        BigInteger power = new(8);
        
        for (int i = 1; i <= lastIndex; i++)
        {
            char character = code[i];

            value *= power;
            BigInteger charValue = new(character - '0');

            value += charValue;
        }
        
        return value;
    }

    private void BuildHexadecimal(int lastIndex)
    {
        Representation = Code[1] == HexadecimalLeadCharacterUppercase
            ? IntegerRepresentation.HexadecimalUppercase
            : IntegerRepresentation.HexadecimalLowercase;

        UnderlyingValue = BuildHexadecimal(Code, lastIndex);
    }

    private static BigInteger BuildHexadecimal(IStringBuilder code, int lastIndex)
    {
        BigInteger value = BigInteger.Zero;
        BigInteger power = new(16);
        
        for (int i = 2; i <= lastIndex; i++)
        {
            char character = code[i];

            value *= power;
            BigInteger charValue = new(character switch
            {
                '0' => 0,
                '1' => 1,
                '2' => 2,
                '3' => 3,
                '4' => 4,
                '5' => 5,
                '6' => 6,
                '7' => 7,
                '8' => 8,
                '9' => 9,
                'A' => 10,
                'a' => 10,
                'B' => 11,
                'b' => 11,
                'C' => 12,
                'c' => 12,
                'D' => 13,
                'd' => 13,
                'E' => 14,
                'e' => 14,
                'F' => 15,
                'f' => 15,
                _ => throw new UnreachableException()
            });

            value += charValue;
        }
        
        return value;
    }

    private void BuildBinary(int lastIndex)
    {
        Representation = Code[1] == BinaryLeadCharacterUppercase
            ? IntegerRepresentation.BinaryUppercase
            : IntegerRepresentation.BinaryLowercase;
        
        UnderlyingValue = BuildBinary(Code, lastIndex);
    }

    private static BigInteger BuildBinary(IStringBuilder code, int lastIndex)
    {
        BigInteger value = BigInteger.Zero;
        BigInteger power = new(2);
        
        for (int i = 2; i <= lastIndex; i++)
        {
            char character = code[i];

            value *= power;
            BigInteger charValue = new(character - '0');

            value += charValue;
        }
        
        return value;
    }

    private static IStringBuilder GetCodeFromValue(BigInteger value)
    {
        IntegerType integerType = IntegerType.TypeFitForValue(value);
    }

    protected override void SetValue(BigInteger value)
    {
        SetValue(value, Representation,
            UnsignedSuffixRepresentation,
            LongSuffixRepresentation,
            SuffixOrder);
    }

    public void SetValue(BigInteger value, IntegerRepresentation representation,
        UnsignedSuffixRepresentation unsignedSuffixRepresentation,
        LongSuffixRepresentation longSuffixRepresentation,
        SuffixOrder suffixOrder)
    {
        switch (representation)
        {
            case IntegerRepresentation.Decimal:
                SetDecimalValue(value, unsignedSuffixRepresentation, longSuffixRepresentation, suffixOrder);
                break;
            case IntegerRepresentation.Octal:
                SetOctalValue(value, unsignedSuffixRepresentation, longSuffixRepresentation, suffixOrder);
                break;
            case IntegerRepresentation.BinaryLowercase:
                SetBinaryLowercaseValue(value, unsignedSuffixRepresentation, longSuffixRepresentation,
                    suffixOrder);
                break;
            case IntegerRepresentation.BinaryUppercase:
                SetBinaryUppercaseValue(value, unsignedSuffixRepresentation, longSuffixRepresentation,
                    suffixOrder);
                break;
            case IntegerRepresentation.HexadecimalLowercase:
                SetHexadecimalLowercaseValue(value, unsignedSuffixRepresentation, longSuffixRepresentation,
                    suffixOrder);
                break;
            case IntegerRepresentation.HexadecimalUppercase:
                SetHexadecimalUppercaseValue(value, unsignedSuffixRepresentation, longSuffixRepresentation,
                    suffixOrder);
                break;
            default:
                throw new ArgumentException(
                    $"Error: the value of {(int)representation} does not represent any valid IntegerRepresentation enum constant.");
        }
    }

    public void SetDecimalValue(BigInteger value,
        UnsignedSuffixRepresentation unsignedSuffixRepresentation,
        LongSuffixRepresentation longSuffixRepresentation,
        SuffixOrder suffixOrder)
    {
        UnderlyingValue = value;
        Representation = IntegerRepresentation.Decimal;

        UnderlyingCode.Clear();
        UnderlyingCode.Append(value.ToString());

        AppendSuffix(unsignedSuffixRepresentation, longSuffixRepresentation, suffixOrder);
    }

    public void SetOctalValue(BigInteger value,
        UnsignedSuffixRepresentation unsignedSuffixRepresentation,
        LongSuffixRepresentation longSuffixRepresentation,
        SuffixOrder suffixOrder)
    {
        UnderlyingValue = value;
        Representation = IntegerRepresentation.Octal;

        UnderlyingCode.Clear();
        UnderlyingCode.Append(SpecialFormatLead);

        UnderlyingCode.Append(value.ToOctalString());

        AppendSuffix(unsignedSuffixRepresentation, longSuffixRepresentation, suffixOrder);
    }

    public void SetHexadecimalValue(BigInteger value,
        UnsignedSuffixRepresentation unsignedSuffixRepresentation,
        LongSuffixRepresentation longSuffixRepresentation,
        SuffixOrder suffixOrder,
        bool uppercase = true)
    {
        if (uppercase)
        {
            SetHexadecimalUppercaseValue(value,
                unsignedSuffixRepresentation,
                longSuffixRepresentation,
                suffixOrder);
        }
        else
        {
            SetHexadecimalLowercaseValue(value,
                unsignedSuffixRepresentation,
                longSuffixRepresentation,
                suffixOrder);
        }
    }

    public void SetHexadecimalUppercaseValue(BigInteger value,
        UnsignedSuffixRepresentation unsignedSuffixRepresentation,
        LongSuffixRepresentation longSuffixRepresentation,
        SuffixOrder suffixOrder)
    {
        UnderlyingValue = value;
        Representation = IntegerRepresentation.HexadecimalUppercase;

        UnderlyingCode.Clear();

        string hexadecimalStart = $"{SpecialFormatLead}{HexadecimalLeadCharacterUppercase}";

        UnderlyingCode.Append(hexadecimalStart);
        UnderlyingCode.Append(value.ToHexadecimalString(uppercase: true));

        AppendSuffix(unsignedSuffixRepresentation, longSuffixRepresentation, suffixOrder);
    }

    public void SetHexadecimalLowercaseValue(BigInteger value,
        UnsignedSuffixRepresentation unsignedSuffixRepresentation,
        LongSuffixRepresentation longSuffixRepresentation,
        SuffixOrder suffixOrder)
    {
        UnderlyingValue = value;
        Representation = IntegerRepresentation.HexadecimalLowercase;

        UnderlyingCode.Clear();

        string hexadecimalStart = $"{SpecialFormatLead}{HexadecimalLeadCharacterLowercase}";

        UnderlyingCode.Append(hexadecimalStart);
        UnderlyingCode.Append(value.ToHexadecimalString(uppercase: false));

        AppendSuffix(unsignedSuffixRepresentation, longSuffixRepresentation, suffixOrder);
    }

    public void SetBinaryValue(BigInteger value,
        UnsignedSuffixRepresentation unsignedSuffixRepresentation,
        LongSuffixRepresentation longSuffixRepresentation,
        SuffixOrder suffixOrder,
        bool uppercase = true)
    {
        if (uppercase)
        {
            SetBinaryUppercaseValue(value,
                unsignedSuffixRepresentation,
                longSuffixRepresentation,
                suffixOrder);
        }
        else
        {
            SetBinaryLowercaseValue(value,
                unsignedSuffixRepresentation,
                longSuffixRepresentation,
                suffixOrder);
        }
    }

    public void SetBinaryUppercaseValue(BigInteger value,
        UnsignedSuffixRepresentation unsignedSuffixRepresentation,
        LongSuffixRepresentation longSuffixRepresentation,
        SuffixOrder suffixOrder)
    {
        UnderlyingValue = value;
        Representation = IntegerRepresentation.BinaryUppercase;

        UnderlyingCode.Clear();

        string binaryStart = $"{SpecialFormatLead}{BinaryLeadCharacterUppercase}";

        UnderlyingCode.Append(binaryStart);
        UnderlyingCode.Append(value.ToBinaryString());

        AppendSuffix(unsignedSuffixRepresentation, longSuffixRepresentation, suffixOrder);
    }

    public void SetBinaryLowercaseValue(BigInteger value,
        UnsignedSuffixRepresentation unsignedSuffixRepresentation,
        LongSuffixRepresentation longSuffixRepresentation,
        SuffixOrder suffixOrder)
    {
        UnderlyingValue = value;
        Representation = IntegerRepresentation.BinaryLowercase;

        UnderlyingCode.Clear();

        string binaryStart = $"{SpecialFormatLead}{BinaryLeadCharacterLowercase}";

        UnderlyingCode.Append(binaryStart);
        UnderlyingCode.Append(value.ToBinaryString());

        AppendSuffix(unsignedSuffixRepresentation, longSuffixRepresentation, suffixOrder);
    }

    private void AppendSuffix(UnsignedSuffixRepresentation unsignedSuffixRepresentation,
        LongSuffixRepresentation longSuffixRepresentation,
        SuffixOrder suffixOrder)
    {
        if (!unsignedSuffixRepresentation.IsValidValue())
        {
            throw new InvalidEnumValueException<UnsignedSuffixRepresentation>((int)unsignedSuffixRepresentation);
        }

        if (!longSuffixRepresentation.IsValidValue())
        {
            throw new InvalidEnumValueException<LongSuffixRepresentation>((int)longSuffixRepresentation);
        }

        if (!suffixOrder.IsValidValue())
        {
            throw new InvalidEnumValueException<SuffixOrder>((int)suffixOrder);
        }
        
        UnsignedSuffixRepresentation = unsignedSuffixRepresentation;
        LongSuffixRepresentation = longSuffixRepresentation;
        SuffixOrder = suffixOrder;
        
        AppendSuffix();
    }

    private void AppendSuffix()
    {
        int length = UnsignedSuffixRepresentation.Length() + LongSuffixRepresentation.Length();
        if (length == 0)
            return;

        string unsignedSuffixString = UnsignedSuffixRepresentation switch
        {
            UnsignedSuffixRepresentation.Signed => "",
            UnsignedSuffixRepresentation.UnsignedUppercase => UnsignedSuffixUppercase.ToString(),
            UnsignedSuffixRepresentation.UnsignedLowercase => UnsignedSuffixUppercase.ToString(),
            _ => throw new UnreachableException($"{nameof(UnsignedSuffixRepresentation)} value: {(int)UnsignedSuffixRepresentation}")
        };

        string longSuffixString = LongSuffixRepresentation switch
        {
            LongSuffixRepresentation.Regular => "",
            LongSuffixRepresentation.LongUppercase => LongSuffixUppercase.ToString(),
            LongSuffixRepresentation.LongLowercase => LongSuffixLowercase.ToString(),
            LongSuffixRepresentation.LongLongUppercase => $"{LongSuffixUppercase}{LongSuffixUppercase}",
            LongSuffixRepresentation.LongLongLowercase => $"{LongSuffixLowercase}{LongSuffixLowercase}",
            _ => throw new UnreachableException($"{nameof(LongSuffixRepresentation)} value: {(int)LongSuffixRepresentation}")
        };

        if (SuffixOrder is SuffixOrder.LongUnsigned)
        {
            Code.Append(longSuffixString);
            Code.Append(unsignedSuffixString);
        }
        else
        {
            Code.Append(unsignedSuffixString);
            Code.Append(longSuffixString);
        }

        IsUnsigned = UnsignedSuffixRepresentation.IsUnsigned();
        IsLong = LongSuffixRepresentation.IsLong();
        IsLongLong = LongSuffixRepresentation.IsLongLong();
    }
    
    private static IntegerType DeduceTypeFromCode(IStringBuilder code)
    {
        if (Check(code).IsFailure)
            return IntType.Int;
        
        bool isUnsigned = false;
        bool isLong = false;
        bool isLongLong = false;
        
        int index = code.Length - 1;
        int length = 0;

        while (length < 3 && index >= 0 && code[index].IsSuffix())
        {
            if (code[index].IsLongSuffix() && isLong)
                isLongLong = true;
            else if (code[index].IsLongSuffix())
                isLong = true;
            else if (code[index].IsUnsignedSuffix())
                isUnsigned = true;
                
            
            index -= 1;
            length += 1;
        }
        
        if (isLongLong)
            return isUnsigned ? LongLongType.UnsignedLongLong : LongLongType.LongLong;

        if (isLong)
            return isUnsigned ? LongType.UnsignedLong : LongType.Long;

        return isUnsigned ? IntType.UnsignedInt : IntType.Int;
    }
}

/// <summary>
/// Extension methods to convert <see cref="System.Numerics.BigInteger"/>
/// instances to hexadecimal, octal, and binary strings.
/// </summary>
public static class BigIntegerExtensions
{
    /// <summary>
    /// Converts a <see cref="BigInteger"/> to a binary string.
    /// </summary>
    /// <param name="bigint">A <see cref="BigInteger"/>.</param>
    /// <returns>
    /// A <see cref="System.String"/> containing a binary
    /// representation of the supplied <see cref="BigInteger"/>.
    /// </returns>
    public static string ToBinaryString(this BigInteger bigint)
    {
        byte[] bytes = bigint.ToByteArray();
        int idx = bytes.Length - 1;

        // Create a StringBuilder having appropriate capacity.
        StringBuilder base2 = new(bytes.Length * 8);

        // Convert first byte to binary.
        string binary = Convert.ToString(bytes[idx], 2);

        // Ensure leading zero exists if value is positive.
        if (binary[0] != '0' && bigint.Sign == 1)
        {
            base2.Append('0');
        }

        // Append binary string to StringBuilder.
        base2.Append(binary);

        // Convert remaining bytes adding leading zeros.
        for (idx--; idx >= 0; idx--)
        {
            base2.Append(Convert.ToString(bytes[idx], 2).PadLeft(8, '0'));
        }

        return base2.ToString();
    }

    /// <summary>
    /// Converts a <see cref="BigInteger"/> to a hexadecimal string.
    /// </summary>
    /// <param name="bigint">A <see cref="BigInteger"/>.</param>
    /// <param name="uppercase">Weather to uppercase the hex letters.</param>
    /// <returns>
    /// A <see cref="System.String"/> containing a hexadecimal
    /// representation of the supplied <see cref="BigInteger"/>.
    /// </returns>
    public static string ToHexadecimalString(this BigInteger bigint, bool uppercase = false)
    {
        return bigint.ToString(uppercase ? "X" : "x");
    }

    /// <summary>
    /// Converts a <see cref="BigInteger"/> to an octal string.
    /// </summary>
    /// <param name="bigint">A <see cref="BigInteger"/>.</param>
    /// <returns>
    /// A <see cref="System.String"/> containing an octal
    /// representation of the supplied <see cref="BigInteger"/>.
    /// </returns>
    public static string ToOctalString(this BigInteger bigint)
    {
        byte[] bytes = bigint.ToByteArray();
        int idx = bytes.Length - 1;

        // Create a StringBuilder having appropriate capacity.
        StringBuilder base8 = new(((bytes.Length / 3) + 1) * 8);

        // Calculate how many bytes are extra when byte array is split
        // into three-byte (24-bit) chunks.
        int extra = bytes.Length % 3;

        // If no bytes are extra, use three bytes for first chunk.
        if (extra == 0)
        {
            extra = 3;
        }

        // Convert first chunk (24-bits) to integer value.
        int int24 = 0;
        for (; extra != 0; extra--)
        {
            int24 <<= 8;
            int24 += bytes[idx--];
        }

        // Convert 24-bit integer to octal without adding leading zeros.
        string octal = Convert.ToString(int24, 8);

        // Ensure leading zero exists if value is positive.
        if (octal[0] != '0' && bigint.Sign == 1)
        {
            base8.Append('0');
        }

        // Append first converted chunk to StringBuilder.
        base8.Append(octal);

        // Convert remaining 24-bit chunks, adding leading zeros.
        for (; idx >= 0; idx -= 3)
        {
            int24 = (bytes[idx] << 16) + (bytes[idx - 1] << 8) + bytes[idx - 2];
            base8.Append(Convert.ToString(int24, 8).PadLeft(8, '0'));
        }

        return base8.ToString();
    }
}