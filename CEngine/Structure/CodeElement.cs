using System.Collections;
using System.Numerics;
using System.Text;
using CEngine.SegmentedStrings;
using JavaScriptEngine.Core;

namespace CEngine.Structure;

/// <summary>
///     An abstract and simple code element with underlying code.
/// </summary>
public abstract class CodeElement : IComparable<CodeElement>, IEqualityComparer<CodeElement>, IEnumerable<char>,
    IEqualityOperators<CodeElement?, CodeElement?, bool>, IEqualityOperators<CodeElement?, object?, bool>,
    IEqualityOperators<CodeElement?, string?, bool>, IEquatable<CodeElement?>, IEquatable<string?>, IEquatable<object?>,
    ICopiable<CodeElement>
{
    protected readonly Guid Id = Guid.NewGuid();

    /// <summary>
    ///     The underlying code of the code element.
    /// </summary>
    protected IStringBuilder UnderlyingCode;

    /// <summary>
    ///     The functions used to check the valididty of code.
    /// </summary>
    protected abstract Func<IStringBuilder, ParsingResults.ParsingResult> Validator { get; }


    protected List<CodeElement> Children { get; private set; }

    protected int ParentIndex { get; private set; }

    protected CodeElement(IStringBuilder code)
    {
        UnderlyingCode = new SegmentedString();
        Code = code;
        Parent = null;
        ParentIndex = -1;
        Children = [];
        Previous = null;
        Next = null;
    }

    protected CodeElement(IStringBuilder code, bool doNotCheckCode)
    {
        UnderlyingCode = new SegmentedString();
        if (doNotCheckCode)
        {
            UnderlyingCode = code;
        }
        else
        {
            Code = code;
        }
        
        Parent = null;
        ParentIndex = -1;
        Children = [];
        Previous = null;
        Next = null;
    }

    protected CodeElement(IStringBuilder code, CodeElement parent, int parentOffset,
        int parentIndex)
    {
        UnderlyingCode = new StringSegment(parent.UnderlyingCode, parentOffset);
        Code = code;
        Parent = parent;
        if (parentIndex < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(parentIndex), parentIndex, "The parent index must be greater than or equal to 0.");
        }
        
        ParentIndex = parentIndex;
        Children = [];
        Previous = null;
        Next = null;
    }

    protected CodeElement(IStringBuilder code, CodeElement parent, int parentOffset,
        int parentIndex, bool doNotCheckCode)
    {
        UnderlyingCode =
                new StringSegment(parent.UnderlyingCode, parentOffset);

        if (doNotCheckCode)
        {
            UnderlyingCode.Append(code);
        }
        else
        {
            Code = code;
        }
 
        Parent = parent;
        
        if (parentIndex < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(parentIndex), parentIndex, "The parent index must be greater than or equal to 0.");
        }
        
        ParentIndex = parentIndex;
        Children = [];
        Previous = null;
        Next = null;
    }

    protected CodeElement(IStringBuilder code, List<CodeElement> children)
    {
        UnderlyingCode = new SegmentedString();
        Code = code;
        Parent = null;
        ParentIndex = -1;
        Children = children;
        Previous = null;
        Next = null;
    }

    protected CodeElement(IStringBuilder code, List<CodeElement> children, bool doNotCheckCode)
    {
        UnderlyingCode = new SegmentedString();
        if (doNotCheckCode)
        {
            UnderlyingCode.Append(code);
        }
        else
        {
            Code = code;
        }
        
        Parent = null;
        ParentIndex = -1;
        Children = children;
        Previous = null;
        Next = null;
    }

    protected CodeElement(IStringBuilder code, CodeElement parent, int parentOffset,
        int parentIndex, List<CodeElement> children)
    {
        UnderlyingCode = new StringSegment(parent.UnderlyingCode, parentOffset);
        Code = code;
        Parent = parent;
        
        if (parentIndex < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(parentIndex), parentIndex, "The parent index must be greater than or equal to 0.");
        }
        
        ParentIndex = parentIndex;
        Children = children;
        Previous = null;
        Next = null;
    }

    protected CodeElement(IStringBuilder code, CodeElement parent, int parentOffset,
        int parentIndex, List<CodeElement> children, bool doNotCheckCode)
    {
        UnderlyingCode =
                new StringSegment(parent.UnderlyingCode, parentOffset);

        if (doNotCheckCode)
        {
            UnderlyingCode.Append(code);
        }
        else
        {
            Code = code;
        }
 
        Parent = parent;
        
        if (parentIndex < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(parentIndex), parentIndex, "The parent index must be greater than or equal to 0.");
        }
        
        ParentIndex = parentIndex;
        Children = children;
        Previous = null;
        Next = null;
    }

    protected CodeElement? Parent { get; private set; }

    public void CopyFrom(CodeElement other)
    {
        throw new NotImplementedException();
    }

    protected CodeElement ShallowClone()
    {
        CodeElement copy = Clone();
        ShallowCopyOverNeighbors(copy);

        return copy;
    }

    protected void ShallowCopyOverNeighbors(CodeElement element)
    {
        if (element.HasPreviousElement)
        {
            Previous = element.Previous;
        }

        if (element.HasNextElement)
        {
            Next = element.Next;
        }
    }

    protected CodeElement DeepCopy()
    {
        CodeElement clonedSelf = Clone();

        clonedSelf.Previous = CopyNeighborsBackward(Previous, clonedSelf);
        clonedSelf.Next = CopyNeighborsForward(Next, clonedSelf);

        return clonedSelf;
    }

    protected void DeepCopyOverNeighbors(CodeElement original, CodeElement element)
    {
        element.Previous = CopyNeighborsBackward(original.Previous, element);
        element.Next = CopyNeighborsForward(element.Next, element);
    }

    protected CodeElement? CopyNeighborsBackward(CodeElement? previous, CodeElement forward)
    {
        CodeElement? previousCopy = null;
        while (previous is not null)
        {
            CodeElement newCopy = previous.Clone();
            newCopy.Next = forward;
            if (previousCopy is not null)
            {
                previousCopy.Previous = newCopy;
            }

            previousCopy = newCopy;
            previous = previous.Previous;
        }

        return previousCopy;
    }

    protected CodeElement? CopyNeighborsForward(CodeElement? next, CodeElement backward)
    {
        CodeElement? nextCopy = null;
        while (next is not null)
        {
            CodeElement newCopy = next.Clone();
            newCopy.Previous = backward;
            if (nextCopy is not null)
            {
                nextCopy.Next = newCopy;
            }

            nextCopy = newCopy;
            next = next.Next;
        }

        return nextCopy;
    }

    protected abstract void BuildInnerStructures();

    public IStringBuilder Code
    {
        get => UnderlyingCode;
        set
        {
            ParsingResults.ParsingResult checkResult = CheckCode(value);
            if (checkResult.IsFailure)
            {
                string errorMessage = GenerateErrorMessage(value.ToString(), checkResult.Value, checkResult.Message);
                throw new InvalidCodeException(errorMessage);
            }

            UnderlyingCode.Clear();
            UnderlyingCode.Append(value);
            BuildInnerStructures();
        }
    }

    public int CodeLength => UnderlyingCode.Length;

    protected abstract CodeElement Clone();

    protected ParsingResults.ParsingResult CheckCode(IStringBuilder code)
    {
        return Validator(code);
    }

    public static ParsingResults.ParsingResult CheckCodeElement<TCodeElement>(IStringBuilder code)
        where TCodeElement : CodeElement, ICheckable
    {
        return TCodeElement.Check(code);
    }

    public bool IsValidCode(IStringBuilder code)
    {
        ParsingResults.ParsingResult codeCheckResult = CheckCode(code);
        return codeCheckResult.IsSuccess;
    }

    public static bool IsValidCodeElement<TCodeElement>(IStringBuilder code) where TCodeElement : CodeElement, ICheckable
    {
        ParsingResults.ParsingResult checkCodeResult = TCodeElement.Check(code);
        return checkCodeResult.IsSuccess;
    }

    public bool IsInvalidCode(IStringBuilder code)
    {
        ParsingResults.ParsingResult codeCheckResult = CheckCode(code);
        return codeCheckResult.IsFailure;
    }

    public static bool IsInvalidCodeElement<TCodeElement>(IStringBuilder code) where TCodeElement : CodeElement, ICheckable
    {
        ParsingResults.ParsingResult checkCodeResult = TCodeElement.Check(code);
        return checkCodeResult.IsFailure;
    }

    /// <summary>
    ///     Helper method that generates the error message for an invalid line break code.
    /// </summary>
    /// <param name="code">
    ///     The invalid line break code.
    /// </param>
    /// <param name="invalidIndex">
    ///     The index of the faulty character in the line break code.
    /// </param>
    /// <param name="message">
    ///     The message to be displayed.
    /// </param>
    /// <returns>
    ///     The error message with the given line break code and index.
    /// </returns>
    protected static string GenerateErrorMessage(string code, int invalidIndex, string message)
    {
        string trimmedCode = code.TrimEnd();
        StringBuilder errorMessageBuilder = new StringBuilder();
        errorMessageBuilder.AppendLine();
        errorMessageBuilder.AppendLine(trimmedCode);

        StringBuilder leftPaddingBuilder = new StringBuilder();
        leftPaddingBuilder.Append(' ', invalidIndex);
        string leftPadding = leftPaddingBuilder.ToString();

        errorMessageBuilder.Append(leftPadding);
        errorMessageBuilder.Append('⇑');

        int numberOfCharsInCode = trimmedCode.Length;
        int numberOfProblematicChars = numberOfCharsInCode - invalidIndex - 1;

        for (int i = 0; i < numberOfProblematicChars; i++)
        {
            errorMessageBuilder.Append('↑');
        }

        errorMessageBuilder.AppendLine(leftPadding);
        errorMessageBuilder.Append(leftPadding);
        errorMessageBuilder.Append('║');

        const string invalidCodeErrorMessage = "NOT READ BECAUSE OF FAULT: NOT A PART OF CODE ELEMENT";
        int invalidCodeErrorMessageLength = invalidCodeErrorMessage.Length;

        int leftPaddingOfErrorMessageAmount = 0;
        if (numberOfProblematicChars > invalidCodeErrorMessageLength)
        {
            int differenceToMakeUpOnBothSides = numberOfProblematicChars - invalidCodeErrorMessageLength;
            leftPaddingOfErrorMessageAmount = differenceToMakeUpOnBothSides / 2;
        }

        StringBuilder leftPaddingOfErrorMessageBuilder = new StringBuilder();

        for (int i = 0; i < leftPaddingOfErrorMessageAmount; i++)
        {
            leftPaddingOfErrorMessageBuilder.Append(' ');
        }

        string leftPaddingOfErrorMessage = leftPaddingOfErrorMessageBuilder.ToString();
        errorMessageBuilder.Append(leftPaddingOfErrorMessage);
        errorMessageBuilder.AppendLine(invalidCodeErrorMessage);

        errorMessageBuilder.Append(leftPadding);
        errorMessageBuilder.AppendLine("║");
        errorMessageBuilder.Append(leftPadding);
        errorMessageBuilder.Append('╚');
        errorMessageBuilder.Append(message);

        string errorMessage = errorMessageBuilder.ToString();
        return errorMessage;
    }

    protected static string GenerateErrorMessageStatic(string code, int invalidIndex, string message)
    {
        return GenerateErrorMessage(code, invalidIndex, message);
    }


    /// <summary>
    /// The previous element of code in the code.
    /// If this is the first element and there are no previous elements,
    /// this value will be null.
    /// </summary>
    public CodeElement? Previous { get; set; }

    public bool HasPreviousElement => Previous is not null;

    public bool IsFirstElement => Previous is null;

    public bool HasNoPreviousElement => Previous is null;

    /// <summary>
    /// The next element of code in the code.
    /// If this is the last element and there are no previous elements,
    /// this value will be null.
    /// </summary>
    public CodeElement? Next { get; set; }

    public bool HasNextElement => Next is not null;

    public bool IsLastElement => Next is null;

    public bool HasNoNextElement => Next is null;

    public static CodeElement GetFirstElement(CodeElement codeElement)
    {
        CodeElement currentElement = codeElement;
        while (currentElement.HasPreviousElement)
        {
            currentElement = currentElement.Previous!;
        }

        return currentElement;
    }

    public CodeElement GetFirstElement()
    {
        return GetFirstElement(this);
    }

    public static CodeElement GetLastElement(CodeElement codeElement)
    {
        CodeElement currentElement = codeElement;
        while (currentElement.HasNextElement)
        {
            currentElement = currentElement.Next!;
        }

        return currentElement;
    }

    public CodeElement GetLastElement()
    {
        return GetLastElement(this);
    }

    public static int GetNumberOfElementsPreceeding(CodeElement codeElement)
    {
        CodeElement currentElement = codeElement;
        int numberOfElementsPreceeding = 0;

        while (currentElement.HasNextElement)
        {
            currentElement = currentElement.Next!;
            numberOfElementsPreceeding += 1;
        }

        return numberOfElementsPreceeding;
    }

    public int GetNumberOfElementsPreceeding()
    {
        return GetNumberOfElementsPreceeding(this);
    }

    public static int GetNumberOfElementsAhead(CodeElement codeElement)
    {
        CodeElement currentElement = codeElement;
        int numberOfElementsAhead = 0;

        while (currentElement.HasNextElement)
        {
            currentElement = currentElement.Next!;
            numberOfElementsAhead += 1;
        }

        return numberOfElementsAhead;
    }

    public int GetNumberOfElementsAhead()
    {
        return GetNumberOfElementsAhead(this);
    }

    public override string ToString()
    {
        return Code.ToString();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public IEnumerator<char> GetEnumerator()
    {
        return Code.GetEnumerator();
    }

    bool IEquatable<CodeElement?>.Equals(CodeElement? other)
    {
        return other is not null && Equals(other);
    }

    protected virtual bool Equals(CodeElement other)
    {
        return UnderlyingCode == other.UnderlyingCode;
    }

    public bool Equals(string? other)
    {
        if (other is null)
        {
            return false;
        }

        return Code.ToString() == other;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj))
        {
            return false;
        }

        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        if (obj is string s)
        {
            return s == Code.ToString();
        }

        if (obj is CodeElement codeElement)
        {
            return codeElement.UnderlyingCode == UnderlyingCode;
        }

        return false;
    }

    public virtual bool Equals(CodeElement? x, CodeElement? y)
    {
        if (x is null && y is null)
        {
            return true;
        }

        if (x is null || y is null)
        {
            return false;
        }

        return x.UnderlyingCode == y.UnderlyingCode;
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    public int GetHashCode(CodeElement obj)
    {
        return obj.GetHashCode();
    }

    private sealed class UnderlyingCodeEqualityComparer : IEqualityComparer<CodeElement>
    {
        public bool Equals(CodeElement? x, CodeElement? y)
        {
            if (ReferenceEquals(x, y))
            {
                return true;
            }

            if (ReferenceEquals(x, null))
            {
                return false;
            }

            if (ReferenceEquals(y, null))
            {
                return false;
            }

            if (x.GetType() != y.GetType())
            {
                return false;
            }

            return x.UnderlyingCode == y.UnderlyingCode;
        }

        public int GetHashCode(CodeElement obj)
        {
            return obj.UnderlyingCode.GetHashCode();
        }
    }

    public static IEqualityComparer<CodeElement> EqualityCodeElementComparer { get; } =
        new UnderlyingCodeEqualityComparer();

    public int CompareTo(CodeElement? other)
    {
        if (ReferenceEquals(this, other))
        {
            return 0;
        }

        if (ReferenceEquals(null, other))
        {
            return 1;
        }
        
        return string.Compare(Code.ToString(), other.Code.ToString(), StringComparison.Ordinal);
    }

    private sealed class UnderlyingCodeRelationalComparer : IComparer<CodeElement>
    {
        public int Compare(CodeElement? x, CodeElement? y)
        {
            if (ReferenceEquals(x, y))
            {
                return 0;
            }

            if (ReferenceEquals(null, y))
            {
                return 1;
            }

            if (ReferenceEquals(null, x))
            {
                return -1;
            }

            return string.Compare(x.Code.ToString(), y.Code.ToString(), StringComparison.Ordinal);
        }
    }

    public static IComparer<CodeElement> UnderlyingCodeComparer { get; } =
        new UnderlyingCodeRelationalComparer();

    public static bool operator ==(CodeElement? firstCodeElement, CodeElement? secondCodeElement)
    {
        if (ReferenceEquals(firstCodeElement, secondCodeElement))
        {
            return true;
        }

        if (ReferenceEquals(firstCodeElement, null))
        {
            return false;
        }

        if (ReferenceEquals(secondCodeElement, null))
        {
            return false;
        }

        return firstCodeElement.Code == secondCodeElement.Code;
    }

    public static bool operator !=(CodeElement? firstCodeElement, CodeElement? secondCodeElement)
    {
        return !(firstCodeElement == secondCodeElement);
    }

    public static bool operator ==(CodeElement? codeElement, string? @string)
    {
        if (codeElement is null && @string is null)
        {
            return true;
        }

        if (ReferenceEquals(codeElement, null))
        {
            return false;
        }

        if (ReferenceEquals(@string, null))
        {
            return false;
        }

        return codeElement.Code.ToString() == @string;
    }

    public static bool operator !=(CodeElement? codeElement, string? @string)
    {
        return !(codeElement == @string);
    }

    public static bool operator ==(string? @string, CodeElement? codeElement)
    {
        return codeElement == @string;
    }

    public static bool operator !=(string? @string, CodeElement? codeElement)
    {
        return !(@string == codeElement);
    }

    public static bool operator <(CodeElement firstCodeElement, CodeElement secondCodeElement)
    {
        return firstCodeElement.Code.Length < secondCodeElement.Code.Length;
    }

    public static bool operator <(string @string, CodeElement codeElement)
    {
        return @string.Length < codeElement.Code.Length;
    }

    public static bool operator <(CodeElement codeElement, string @string)
    {
        return codeElement.Code.Length < @string.Length;
    }

    public static bool operator <=(CodeElement firstCodeElement, CodeElement secondCodeElement)
    {
        return firstCodeElement.Code.Length <= secondCodeElement.Code.Length;
    }

    public static bool operator <=(string @string, CodeElement codeElement)
    {
        return @string.Length <= codeElement.Code.Length;
    }

    public static bool operator <=(CodeElement codeElement, string @string)
    {
        return codeElement.Code.Length <= @string.Length;
    }

    public static bool operator >(CodeElement firstCodeElement, CodeElement secondCodeElement)
    {
        return firstCodeElement.Code.Length > secondCodeElement.Code.Length;
    }

    public static bool operator >(string @string, CodeElement codeElement)
    {
        return @string.Length > codeElement.Code.Length;
    }

    public static bool operator >(CodeElement codeElement, string @string)
    {
        return codeElement.Code.Length > @string.Length;
    }

    public static bool operator >=(CodeElement firstCodeElement, CodeElement secondCodeElement)
    {
        return firstCodeElement.Code.Length >= secondCodeElement.Code.Length;
    }

    public static bool operator >=(string @string, CodeElement codeElement)
    {
        return @string.Length >= codeElement.Code.Length;
    }

    public static bool operator >=(CodeElement codeElement, string @string)
    {
        return codeElement.Code.Length >= @string.Length;
    }

    public static bool operator ==(CodeElement? left, object? right)
    {
        if (left is null && right is null)
        {
            return true;
        }

        if (left is null || right is null)
        {
            return false;
        }

        return right switch
        {
            CodeElement rightAsCodeElement => left == rightAsCodeElement,
            string rightAsString => left == rightAsString,
            _ => false
        };
    }

    public static bool operator !=(CodeElement? left, object? right)
    {
        return !(left == right);
    }

    protected static int GetNextSignificantCharacterIndex(string code, int index)
    {
        int codeLength = code.Length;
        if (index >= codeLength)
        {
            return -1;
        }

        char character = code[index];
        while (char.IsWhiteSpace(character))
        {
            index += 1;
            if (index >= codeLength)
            {
                return -1;
            }

            character = code[index];
        }

        return index;
    }

    protected class SignificantSubstringIndexResponse
    {
        public const int NegativeIndexResponse = -3;

        public const int IndexTooLargeResponse = -2;

        public const int EmptySignificantSubstringIndexResponse = -1;

        public int Start { get; }

        public int End { get; }

        private SignificantSubstringIndexResponse(int start, int end)
        {
            Start = start;
            End = end;
        }

        public static SignificantSubstringIndexResponse New(int start, int end) =>
            new(start, end);

        public static SignificantSubstringIndexResponse NewNegativeIndex() =>
            new(NegativeIndexResponse, NegativeIndexResponse);

        public static SignificantSubstringIndexResponse NewIndexTooLarge() =>
            new(IndexTooLargeResponse, IndexTooLargeResponse);

        public static SignificantSubstringIndexResponse NewEmpty() =>
            new(EmptySignificantSubstringIndexResponse, EmptySignificantSubstringIndexResponse);

        public bool WasPassedNegativeIndex => Start == NegativeIndexResponse;

        public bool WasPassedIndexTooLarge => End == IndexTooLargeResponse;

        public bool IsEmpty => Start == EmptySignificantSubstringIndexResponse;

        public bool IsProblematic => WasPassedNegativeIndex || WasPassedIndexTooLarge;

        public override bool Equals(object? obj)
        {
            return obj is SignificantSubstringIndexResponse significantSubstringIndexResponse
                   && Start == significantSubstringIndexResponse.Start
                   && End == significantSubstringIndexResponse.End;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Start, End);
        }

        public override string ToString()
        {
            return $"SignificantSubstringIndexResponse(Start={Start}, End={End})";
        }

        public static bool operator ==(SignificantSubstringIndexResponse? left,
            SignificantSubstringIndexResponse? right)
        {
            if (left is null && right is null)
            {
                return true;
            }

            if (left is null || right is null)
            {
                return false;
            }

            return left.Start == right.Start && left.End == right.End;
        }

        public static bool operator !=(SignificantSubstringIndexResponse? left,
            SignificantSubstringIndexResponse? right)
        {
            return !(left == right);
        }
    }

    protected static (int start, int end) GetSignificantSubstringIndexes(string code, int index = 0)
    {
        const int firstIndex = 0;

        const int negativeIndexResponse = -3;
        const int tooGreatIndexResponse = -2;
        const int emptySignificantSubStringResponse = -1;

        if (index < firstIndex)
        {
            return (negativeIndexResponse, negativeIndexResponse);
        }

        int codeLength = code.Length;
        if (index >= codeLength)
        {
            return (tooGreatIndexResponse, tooGreatIndexResponse);
        }

        while (index < codeLength && char.IsWhiteSpace(code[index]))
        {
            index += 1;
        }

        if (index >= codeLength)
        {
            return (emptySignificantSubStringResponse,
                emptySignificantSubStringResponse); // This means the significant sub string is empty
        }

        int lastIndex = codeLength - 1;
        while (lastIndex >= index && char.IsWhiteSpace(code[lastIndex]))
        {
            lastIndex -= 1;
        }

        if (lastIndex < index)
        {
            // This should never happen as normally the previous if check should mean this is impossible.
            // For extra resiliency, this code exists as well (means the significant sub string is empty).
            return (emptySignificantSubStringResponse, emptySignificantSubStringResponse);
        }

        return (index, lastIndex);
    }

    public class NotACodeElement() : CodeElement(new SegmentedString())
    {
        public static NotACodeElement Empty => new();

        protected override void BuildInnerStructures()
        { }

        protected override CodeElement Clone()
        {
            return Empty;
        }

        protected override Func<IStringBuilder, ParsingResults.ParsingResult> Validator { get; } =
            delegate { return ParsingResults.ParsingResult.CreateSuccess(); };
    }
}