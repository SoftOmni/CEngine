using System.Numerics;

namespace CEngine.Structure.Constants.Integers;

public partial class Integer
{
    public static Integer MaxMagnitude(Integer x, Integer y)
    {
        return x > y ? x : y;
    }

    public static Integer MaxMagnitudeNumber(Integer x, Integer y)
    {
        return x > y ? x : y;
    }

    public static Integer MinMagnitude(Integer x, Integer y)
    {
        return x < y ? x : y;
    }

    public static Integer MinMagnitudeNumber(Integer x, Integer y)
    {
        return x < y ? x : y;
    }

    public int CompareTo(Integer? other)
    {
        return other is null ? 1 : Value.CompareTo(other.Value);
    }

    public int CompareTo(BigInteger? other)
    {
        return other is null ? 1 : Value.CompareTo(other);
    }

    public int CompareTo(BigInteger other)
    {
        return Value.CompareTo(other);
    }

    public int CompareTo(float? other)
    {
        return other is null ? 1 : CompareTo((float)other);
    }

    public int CompareTo(float other)
    {
        BigInteger otherFloor = (BigInteger)other;
        
        if (Value == otherFloor)
            return 0;
        
        if (Value > otherFloor)
            return 1;
        
        return -1;
    }

    public int CompareTo(double? other)
    {
        return other is null ? 1 : CompareTo((double)other);
    }

    public int CompareTo(double other)
    {
        BigInteger otherFloor = (BigInteger)other;
        
        if (Value == otherFloor)
            return 0;
        
        if (Value > otherFloor)
            return 1;
        
        return -1;
    }

    public int CompareTo(decimal? other)
    {
        return other is null ? 1 : CompareTo((decimal)other);
    }

    public int CompareTo(decimal other)
    {
        BigInteger otherFloor = (BigInteger)other;
        
        if (Value == otherFloor)
            return 0;
        
        if (Value > otherFloor)
            return 1;
        
        return -1;
    }

    public int CompareTo(object? obj)
    {
        return obj switch
        {
            null => 1,
            Integer integer => CompareTo(integer),
            BigInteger bigInteger => CompareTo(bigInteger),
            sbyte sbyteValue => CompareTo((BigInteger)sbyteValue),
            byte byteValue => CompareTo((BigInteger)byteValue),
            short shortValue => CompareTo((BigInteger)shortValue),
            ushort ushortValue => CompareTo((BigInteger)ushortValue),
            int intValue => CompareTo((BigInteger)intValue),
            uint uintValue => CompareTo((BigInteger)uintValue),
            long longValue => CompareTo((BigInteger)longValue),
            ulong ulongValue => CompareTo((BigInteger)ulongValue),
            float floatValue => CompareTo(floatValue),
            double doubleValue => CompareTo(doubleValue),
            decimal decimalValue => CompareTo(decimalValue),
            CodeElement codeElement => base.CompareTo(codeElement),
            _ => throw new ArgumentException($"Cannot compare {obj.GetType()} to {typeof(Integer)}")
        };
    }

    public bool Equals(Integer? other)
    {
        return other is not null && Value == other.Value;
    }

    public bool Equals(BigInteger? other)
    {
        return other is not null && Value == other.Value;
    }

    public bool Equals(BigInteger other)
    {
        return Value == other;
    }
    
    public bool Equals(float? other)
    {
        return other is not null && Value == (BigInteger)other;
    }

    public bool Equals(float other)
    {
        return Value == (BigInteger)other;
    }

    public bool Equals(double? other)
    {
        return other is not null && Value == (BigInteger)other;
    }

    public bool Equals(double other)
    {
        return Value == (BigInteger)other;
    }

    public bool Equals(decimal? other)
    {
        return other is not null && Value == (BigInteger)other;
    }

    public bool Equals(decimal other)
    {
        return Value == (BigInteger)other;
    }

    public override bool Equals(object? obj)
    {
        return obj switch
        {
            Integer integer => Equals(integer),
            BigInteger bigInteger => Equals(bigInteger),
            sbyte sbyteValue => Equals((BigInteger)sbyteValue),
            byte byteValue => Equals((BigInteger)byteValue),
            short shortValue => Equals((BigInteger)shortValue),
            ushort ushortValue => Equals((BigInteger)ushortValue),
            int intValue => Equals((BigInteger)intValue),
            uint uintValue => Equals((BigInteger)uintValue),
            long longValue => Equals((BigInteger)longValue),
            ulong ulongValue => Equals((BigInteger)ulongValue),
            float floatValue => Equals(floatValue),
            double doubleValue => Equals(doubleValue),
            decimal decimalValue => Equals(decimalValue),
            CodeElement codeElement => Equals(codeElement),
            _ => base.Equals(obj)
        };
    }
}