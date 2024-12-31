using System.Numerics;
using CEngine.ParsingResults;
using CEngine.SegmentedStrings;
using CEngine.Types;

namespace CEngine.Structure.Constants.Integers;

public partial class Integer :
    Literal<BigInteger, IntegerType>,
    IUnsignedNumber<Integer>,
    IBinaryInteger<Integer>
{
    public IntegerRepresentation Representation { get; private set; }

    public UnsignedSuffixRepresentation UnsignedSuffixRepresentation { get; private set; }

    public bool IsUnsigned { get; private set; }

    public bool IsSigned => !IsUnsigned;

    public LongSuffixRepresentation LongSuffixRepresentation { get; private set; }

    public bool IsLong { get; private set; }

    public bool IsLongLong { get; private set; }

    public SuffixOrder SuffixOrder { get; private set; }

    public Integer(IStringBuilder code) :
        base(code, BuildValue, DeduceTypeFromCode(code))
    {
        Representation = IntegerRepresentation.Decimal;
    }

    public Integer(BigInteger value, IntegerType type) :
        base(GetCodeFromValue(value), delegate { return value; }, type, true)
    {
        if (value.Sign < 0)
            throw new ArgumentException($"Value of an integer constant cannot be negative but was {value}.", nameof(value));
    }

    internal Integer(IStringBuilder code, bool doNotCheckCode) :
        base(code, BuildValue, DeduceTypeFromCode(code), doNotCheckCode)
    {
        Representation = IntegerRepresentation.Decimal;
    }

    public Integer(IStringBuilder code, CodeElement parent, int parentOffset, int parentIndex) :
        base(code, parent, parentOffset, parentIndex, BuildValue, DeduceTypeFromCode(code))
    {
        Representation = IntegerRepresentation.Decimal;
    }

    public Integer(BigInteger value, CodeElement parent, int parentOffset, int parentIndex, IntegerType type) :
        base(GetCodeFromValue(value), parent, parentOffset, parentIndex, delegate { return value; }, type, true)
    {
        if (value.Sign < 0)
            throw new ArgumentException($"Value of an integer constant cannot be negative but was {value}.", nameof(value));
    }

    internal Integer(IStringBuilder code, CodeElement parent, int parentOffset, int parentIndex,
        bool doNotCheckCode) :
        base(code, parent, parentOffset, parentIndex, BuildValue, DeduceTypeFromCode(code), doNotCheckCode)
    {
        Representation = IntegerRepresentation.Decimal;
    }

    protected override Func<IStringBuilder, ParsingResult> Validator => Check;

    protected override CodeElement Clone()
    {
        throw new NotImplementedException();
    }
}