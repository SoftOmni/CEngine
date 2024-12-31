using CEngine.SegmentedStrings;

namespace CEngine.Structure;

public interface ICheckable
{
    public static abstract ParsingResults.ParsingResult Check(IStringBuilder code);
}