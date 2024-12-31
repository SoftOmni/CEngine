using CEngine.ParsingResults;
using CEngine.SegmentedStrings;

namespace CEngine.Structure.Identifiers;

public partial class Identifier : CodeElement
{
    internal HashSet<CodeElement> Usages { get; } = [];
    
    public string Value { get; private set; }

    private const char UniversalCharacterNameCharacter = '\\';

    private const char SimpleUniversalCharacterNameCharacter = 'u';
    
    private const char ComplexUniversalCharacterNameCharacter = 'U';

    public Identifier(IStringBuilder code) : base(code)
    {
        Value = string.Empty;
    }

    public Identifier(IStringBuilder code, bool doNotCheckCode) : base(code, doNotCheckCode)
    {
        Value = string.Empty;
    }

    public Identifier(IStringBuilder code, CodeElement parent, int parentOffset, int parentIndex) : base(code, parent,
        parentOffset, parentIndex)
    {
        Value = string.Empty;
    }

    public Identifier(IStringBuilder code, CodeElement parent, int parentOffset, int parentIndex, bool doNotCheckCode) :
        base(code, parent, parentOffset, parentIndex, doNotCheckCode)
    {
        Value = string.Empty;
    }

    protected override Func<IStringBuilder, ParsingResult> Validator => Check;


    protected override CodeElement Clone()
    {
        throw new NotImplementedException();
    }

    public bool AddUsage(CodeElement usage)
    {
        return Usages.Add(usage);
    }

    public bool RemoveUsage(CodeElement usage)
    {
        return Usages.Remove(usage);
    }

    public bool ContainsUsage(CodeElement usage)
    {
        return Usages.Contains(usage);
    }

    public IReadOnlySet<CodeElement> GetUsages()
    {
        return Usages;
    }
}

