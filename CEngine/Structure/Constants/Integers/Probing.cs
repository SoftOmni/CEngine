using System.Numerics;

namespace CEngine.Structure.Constants.Integers;

public partial class Integer
{
    public static bool IsCanonical(Integer value)
    {
        return true;
    }

    public static bool IsComplexNumber(Integer value)
    {
        return false;
    }

    public static bool IsEvenInteger(Integer value)
    {
        return value.Value.IsEven;
    }

    public static bool IsFinite(Integer value)
    {
        return true;
    }

    public static bool IsImaginaryNumber(Integer value)
    {
        return false;
    }

    public static bool IsInfinity(Integer value)
    {
        return false;
    }

    public static bool IsInteger(Integer value)
    {
        return true;
    }

    public static bool IsNaN(Integer value)
    {
        return false;
    }

    public static bool IsNegative(Integer value)
    {
        return false;
    }

    public static bool IsNegativeInfinity(Integer value)
    {
        return false;
    }

    public static bool IsNormal(Integer value)
    {
        return !value.Value.IsZero;
    }

    public static bool IsOddInteger(Integer value)
    {
        return !value.Value.IsEven;
    }

    public static bool IsPositive(Integer value)
    {
        return true;
    }

    public static bool IsPositiveInfinity(Integer value)
    {
        return false;
    }

    public static bool IsRealNumber(Integer value)
    {
       return true;
    }

    public static bool IsSubnormal(Integer value)
    {
        return false;
    }

    public static bool IsZero(Integer value)
    {
        return value.Value.IsZero;
    }

    public static bool IsPow2(Integer value)
    {
        return BigInteger.IsPow2(value.Value);
    }

    public int GetByteCount()
    {
        return Value.GetByteCount();
    }

    public int GetShortestBitLength()
    {
        throw new NotSupportedException("This method is not supported.");
    }

    public static Integer TrailingZeroCount(Integer value)
    {
        BigInteger trailingZeroCount = BigInteger.TrailingZeroCount(value.Value);
        
        return (Integer)trailingZeroCount;
    }

    public static Integer PopCount(Integer value)
    {
        return (Integer)BigInteger.PopCount(value.Value);
    }
}