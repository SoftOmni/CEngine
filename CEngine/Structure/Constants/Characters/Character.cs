using CEngine.ParsingResults;
using CEngine.SegmentedStrings;
using CEngine.Types;
using Type = CEngine.Types.Type;

namespace CEngine.Structure.Constants.Characters;

public partial class Character : Literal<char, CharType>
{
    public Character(IStringBuilder code, char value, CharType type) : 
        base(code, value, type)
    {
    }

    public Character(IStringBuilder code, char value, CharType type, bool doNotCheckCode) : 
        base(code, value, type, doNotCheckCode)
    {
    }

    public Character(IStringBuilder code, CodeElement parent, int parentOffset, int parentIndex, char value, CharType type) : 
        base(code, parent, parentOffset, parentIndex, value, type)
    {
    }

    public Character(IStringBuilder code, CodeElement parent, int parentOffset, int parentIndex, char value, CharType type, bool doNotCheckCode) : 
        base(code, parent, parentOffset, parentIndex, value, type, doNotCheckCode)
    {
    }

    protected override Func<IStringBuilder, ParsingResult> Validator { get; } = Check;

    protected override CodeElement Clone()
    {
        throw new NotImplementedException();
    }
    
    
}

