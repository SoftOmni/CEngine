using CEngine.SegmentedStrings;
using Type = CEngine.Types.Type;

namespace CEngine.Structure.Constants;

public abstract class Literal<TValue, TType> : CodeElement where TType : Type
{
    public TType Type { get; }

    protected TValue UnderlyingValue;

    public TValue Value
    {
        get => UnderlyingValue;
        set => SetValue(value);
    }

    protected Literal(IStringBuilder code, Func<IStringBuilder, TValue> valueBuilder, TType type) :
        base(code)
    {
        UnderlyingValue = valueBuilder(code);
        Type = type;
    }

    protected Literal(IStringBuilder code, Func<IStringBuilder, TValue> valueBuilder, TType type,
        bool doNotCheckCode) : base(code,
        doNotCheckCode)
    {
        UnderlyingValue = valueBuilder(code);
        Type = type;
    }

    protected Literal(IStringBuilder code, CodeElement parent, int parentOffset,
        int parentIndex, Func<IStringBuilder, TValue> valueBuilder, TType type) : base(code, parent,
        parentOffset, parentIndex)
    {
        UnderlyingValue = valueBuilder(code);
        Type = type;
    }

    protected Literal(IStringBuilder code, CodeElement parent, int parentOffset,
        int parentIndex, Func<IStringBuilder, TValue> valueBuilder, TType type, bool doNotCheckCode) : base(
        code, parent, parentOffset,
        parentIndex, doNotCheckCode)
    {
        UnderlyingValue = valueBuilder(code);
        Type = type;
    }

    public TValue GetValue()
    {
        return UnderlyingValue;
    }

    protected abstract void SetValue(TValue value);
}