namespace CEngine.Structure.Constants.Integers;

public enum IntegerRepresentation
{
    Decimal,
    Octal,
    BinaryLowercase,
    BinaryUppercase,
    HexadecimalLowercase,
    HexadecimalUppercase,
}

public enum UnsignedSuffixRepresentation
{
    Signed,
    UnsignedUppercase,
    UnsignedLowercase
}

public enum LongSuffixRepresentation
{
    Regular,
    LongUppercase,
    LongLowercase,
    LongLongUppercase,
    LongLongLowercase
}

public enum SuffixOrder
{
    UnsignedLong,
    LongUnsigned,
}

public static class UnsignedSuffixRepresentationExtensions
{
    private static HashSet<int> ValidValues { get; } = FillValidValues();
    
    public static bool IsSigned(this UnsignedSuffixRepresentation unsignedSuffixRepresentation)
    {
        return unsignedSuffixRepresentation is UnsignedSuffixRepresentation.Signed;
    }

    public static bool IsUnsigned(this UnsignedSuffixRepresentation unsignedSuffixRepresentation)
    {
        return unsignedSuffixRepresentation 
            is UnsignedSuffixRepresentation.UnsignedUppercase
            or UnsignedSuffixRepresentation.UnsignedLowercase;
    }

    public static int Length(this UnsignedSuffixRepresentation unsignedSuffixRepresentation)
    {
        return unsignedSuffixRepresentation switch
        {
            UnsignedSuffixRepresentation.Signed => 0,
            UnsignedSuffixRepresentation.UnsignedUppercase => 1,
            UnsignedSuffixRepresentation.UnsignedLowercase => 1,
            _ => throw new InvalidEnumValueException<UnsignedSuffixRepresentation>((int)unsignedSuffixRepresentation)
        };
    }

    public static bool IsValidValue(this UnsignedSuffixRepresentation unsignedSuffixRepresentation)
    {
        return IsValidValue((int)unsignedSuffixRepresentation);
    }

    public static bool IsValidValue(int unsignedSuffixRepresentationValue)
    {
        return ValidValues.Contains(unsignedSuffixRepresentationValue);
    }

    private static HashSet<int> FillValidValues()
    {
        HashSet<int> validValues = [];
        
        Array unsignedSuffixRepresentationValues = Enum.GetValues(typeof(UnsignedSuffixRepresentation));
        foreach (object? unsignedSuffixRepresentationValue in unsignedSuffixRepresentationValues)
        {
            if (unsignedSuffixRepresentationValue is null)
                continue;
            
            validValues.Add((int)unsignedSuffixRepresentationValue);
        }
        
        return validValues;
    }
}

public static class LongSuffixRepresentationExtensions
{
    private static HashSet<int> ValidValues { get; } = FillValidValues();
    
    public static bool IsLong(this LongSuffixRepresentation longSuffixRepresentation)
    {
        return longSuffixRepresentation 
            is LongSuffixRepresentation.LongUppercase
            or LongSuffixRepresentation.LongLowercase;
    }

    public static bool IsLongLong(this LongSuffixRepresentation longSuffixRepresentation)
    {
        return longSuffixRepresentation
            is LongSuffixRepresentation.LongLongUppercase
            or LongSuffixRepresentation.LongLowercase;
    }

    public static int Length(this LongSuffixRepresentation longSuffixRepresentation)
    {
        return longSuffixRepresentation switch
        {
            LongSuffixRepresentation.Regular => 0,
            LongSuffixRepresentation.LongUppercase => 1,
            LongSuffixRepresentation.LongLowercase => 1,
            LongSuffixRepresentation.LongLongUppercase => 2,
            LongSuffixRepresentation.LongLongLowercase => 2,
            _ => throw new InvalidEnumValueException<LongSuffixRepresentation>((int)longSuffixRepresentation)
        };
    }
    
    public static bool IsValidValue(this LongSuffixRepresentation longSuffixRepresentation)
    {
        return IsValidValue((int)longSuffixRepresentation);
    }

    public static bool IsValidValue(int longSuffixRepresentationValue)
    {
        return ValidValues.Contains(longSuffixRepresentationValue);
    }

    private static HashSet<int> FillValidValues()
    {
        HashSet<int> validValues = [];
        
        Array longSuffixRepresentationValues = Enum.GetValues(typeof(LongSuffixRepresentation));
        foreach (object? longSuffixRepresentationValue in longSuffixRepresentationValues)
        {
            if (longSuffixRepresentationValue is null)
                continue;
            
            validValues.Add((int)longSuffixRepresentationValue);
        }
        
        return validValues;
    }
}

public static class SuffixOrderExtensions
{
    private static HashSet<int> ValidValues { get; } = FillValidValues();
    
    public static bool IsValidValue(this SuffixOrder suffixOrder)
    {
        return IsValidValue((int)suffixOrder);
    }

    public static bool IsValidValue(int suffixOrderValue)
    {
        return ValidValues.Contains(suffixOrderValue);
    }

    private static HashSet<int> FillValidValues()
    {
        HashSet<int> validValues = [];
        
        Array suffixOrderValues = Enum.GetValues(typeof(UnsignedSuffixRepresentation));
        foreach (object? suffixOrderValue in suffixOrderValues)
        {
            if (suffixOrderValue is null)
                continue;
            
            validValues.Add((int)suffixOrderValue);
        }
        
        return validValues;
    }
}
