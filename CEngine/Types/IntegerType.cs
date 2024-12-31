using System.Numerics;
using CEngine.Structure.Constants.Integers;

namespace CEngine.Types;

public class IntegerType : Type
{
    public bool IsUnsigned { get; private set; }

    public BigInteger MaxValue { get; }

    public BigInteger MinValue { get; }

    protected IntegerType(bool isUnsigned, BigInteger maxValue, BigInteger minValue)
    {
        IsUnsigned = isUnsigned;
        MaxValue = maxValue;
        MinValue = minValue;
    }

    public static IntegerType CombineIntegerTypesInOperation(Integer left, Integer right)
    {
        throw new NotImplementedException();
    }

    public static IntegerType TypeFitForValue(BigInteger value)
    {
        if (value < IntType.Int.MaxValue)
            return IntType.Int;

        if (value < LongType.Long.MaxValue)
            return LongType.Long;

        return LongLongType.LongLong;
    }
    
    protected static BigInteger MaximumValue(bool isUnsigned, int elementLength)
    {
        BigInteger @base = BigInteger.One;
        @base <<= isUnsigned ? elementLength : elementLength - 1;

        return ~@base;
    }

    protected static BigInteger MinimumValue(bool isUnsigned, int elementLength)
    {
        if (isUnsigned)
            return BigInteger.Zero;
        
        BigInteger @base = BigInteger.One;
        @base <<= elementLength;

        return ~@base;
    }
}

public sealed class ShortType : IntegerType
{
    private ShortType(bool isUnsigned) : base(isUnsigned,
        MaximumValue(isUnsigned, (int)Configuration.ShortSize),
        MinimumValue(isUnsigned, (int)Configuration.ShortSize))
    { }

    public static readonly ShortType Short = new(false);

    public static readonly ShortType UnsignedShort = new(true);
}

public class IntType : IntegerType
{
    private IntType(bool isUnsigned) :
        base(isUnsigned,
            MaximumValue(isUnsigned, (int)Configuration.IntSize),
            MinimumValue(isUnsigned, (int)Configuration.IntSize))
    { }

    public static readonly IntType Int = new(false);

    public static readonly IntType UnsignedInt = new(true);
}

public sealed class LongType : IntegerType
{
    private LongType(bool isUnsigned) : base(isUnsigned,
        MaximumValue(isUnsigned, (int)Configuration.LongSize),
        MinimumValue(isUnsigned, (int)Configuration.LongSize))
    { }

    public static readonly LongType Long = new(false);

    public static readonly LongType UnsignedLong = new(true);
}

public sealed class LongLongType : IntegerType
{
    private LongLongType(bool isUnsigned) : base(isUnsigned,
        MaximumValue(isUnsigned, (int)Configuration.LongLongSize),
        MinimumValue(isUnsigned, (int)Configuration.LongLongSize))
    { }
    
    public static readonly LongLongType LongLong = new(false);

    public static readonly LongLongType UnsignedLongLong = new(true);
}