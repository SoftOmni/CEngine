using CEngine.ParsingResults;
using CEngine.SegmentedStrings;

namespace CEngine.Structure.Identifiers;

public partial class Identifier
{
    public static ParsingResult Check(IStringBuilder code)
    {
        if (code.Length == 0)
        {
            return ParsingResult.CreateFailure(0, "Error: an Identifier must not be empty.");
        }

        ProgressiveParsingResult parsingResult = CheckIdentifierCharacter(code, 0, c => c.IsCNonDigit());
        if (parsingResult.IsFailure)
        {
            return parsingResult.ToParsingResult();
        }

        int index = parsingResult.NewIndex;

        while (index < code.Length)
        {
            parsingResult = CheckIdentifierCharacter(code, index, c => c.IsCNonDigit() || c.IsCDigit());
            if (parsingResult.IsFailure)
                return parsingResult.ToParsingResult();

            index = parsingResult.NewIndex;
        }

        return ParsingResult.CreateSuccess();
    }

    private static ProgressiveParsingResult CheckIdentifierCharacter(IStringBuilder code, int index,
        Predicate<char> predicate)
    {
        char character = code[index];
        if (predicate(character))
        {
            return ProgressiveParsingResult.CreateSuccess(index + 1);
        }

        if (character == UniversalCharacterNameCharacter)
            return CheckUniversalCharacterName(code, index + 1);

        return ProgressiveParsingResult.CreateFailure(0, "Error: an Identifier must start " +
                                                         "with either an underscore or an uppercase or lowercase " +
                                                         "letter (only ASCII characters are allowed).\n" +
                                                         "That would be one of the following:\n" +
                                                         "'_', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', " +
                                                         "'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', " +
                                                         "'s', 't', 'u', 'v', 'w', 'x', 'y', 'z'," +
                                                         "'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', " +
                                                         "'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', " +
                                                         "'V', 'W', 'X', 'Y' or 'Z'");
    }

    private const string HexAllowedCharactersErrorMessage = "The allowed characters are:\n" +
                                                            "'0', '1', '2', '3', '4', '5', '6', " +
                                                            "'7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F', " +
                                                            "'a', 'b', 'c', 'd', 'e' or 'f'";

    private static ProgressiveParsingResult CheckUniversalCharacterName(IStringBuilder code, int index)
    {
        if (index >= code.Length)
        {
            return ProgressiveParsingResult.CreateFailure(index,
                $"Error: expected '{SimpleUniversalCharacterNameCharacter}' or " +
                $"'{ComplexUniversalCharacterNameCharacter}' after " +
                "universal character name start but code ended.");
        }

        char character = code[index];
        if (character is not 'u' and not 'U')
        {
            return ProgressiveParsingResult.CreateFailure(index,
                $"Error: expected '{SimpleUniversalCharacterNameCharacter}' or " +
                $"'{ComplexUniversalCharacterNameCharacter}' after " +
                $"universal character name start but got '{character}' (U+{character:2X}).",
                index + 1);
        }

        if (character is SimpleUniversalCharacterNameCharacter)
        {
            return CheckUniversalCharacterNameSimple(code, index);
        }
        
        // character is complex universal character name character
        return CheckUniversalCharacterNameComplex(code, index);
    }

    private static ProgressiveParsingResult CheckUniversalCharacterNameSimple(IStringBuilder code, int index)
    {
        const int neededLength = 4;
        const string type = "simple";

        if (index + neededLength >= code.Length)
        {
            return ProgressiveParsingResult.CreateFailure(code.Length, $"Error: expected after " +
                                                                       $"a simple universal character name (\"{UniversalCharacterNameCharacter}" +
                                                                       $"{SimpleUniversalCharacterNameCharacter}\")" +
                                                                       $" at least {neededLength} characters but the code ended {code.Length - index} characters before.");
        }

        char firstHexCharacter = code[index + 1];
        char secondHexCharacter = code[index + 2];
        char thirdHexCharacter = code[index + 3];
        char forthHexCharacter = code[index + 4];

        if (!firstHexCharacter.IsHexDigit())
        {
            return CreateUniversalCharacterNameFailure(index + 1, "first", type,
                SimpleUniversalCharacterNameCharacter, firstHexCharacter);
        }

        if (!secondHexCharacter.IsHexDigit())
        {
            return CreateUniversalCharacterNameFailure(index + 2, "second", type,
                SimpleUniversalCharacterNameCharacter, secondHexCharacter);
        }

        if (!thirdHexCharacter.IsHexDigit())
        {
            return CreateUniversalCharacterNameFailure(index + 3, "third", type,
                SimpleUniversalCharacterNameCharacter, thirdHexCharacter);
        }

        if (!forthHexCharacter.IsHexDigit())
        {
            return CreateUniversalCharacterNameFailure(index + 4, "forth", type,
                SimpleUniversalCharacterNameCharacter, forthHexCharacter);
        }

        return ProgressiveParsingResult.CreateSuccess(index + neededLength + 1);
    }

    private static ProgressiveParsingResult CheckUniversalCharacterNameComplex(IStringBuilder code, int index)
    {
        const int neededLength = 8;

        if (index + neededLength >= code.Length)
        {
            return ProgressiveParsingResult.CreateFailure(code.Length, $"Error: expected after " +
                                                                       $"a simple universal character name (\"{UniversalCharacterNameCharacter}" +
                                                                       $"{ComplexUniversalCharacterNameCharacter}\")" +
                                                                       $" at least {neededLength} characters but the code ended {code.Length - index} characters before.");
        }

        const string type = "complex";

        char firstHexCharacter = code[index + 1];
        char secondHexCharacter = code[index + 2];
        char thirdHexCharacter = code[index + 3];
        char forthHexCharacter = code[index + 4];

        char fifthHexCharacter = code[index + 5];
        char sixthHexCharacter = code[index + 6];
        char seventhHexCharacter = code[index + 7];
        char eighthHexCharacter = code[index + 8];

        if (!firstHexCharacter.IsHexDigit())
        {
            return CreateUniversalCharacterNameFailure(index + 1, "first", type,
                ComplexUniversalCharacterNameCharacter, firstHexCharacter);
        }

        if (!secondHexCharacter.IsHexDigit())
        {
            return CreateUniversalCharacterNameFailure(index + 2, "second", type,
                ComplexUniversalCharacterNameCharacter, secondHexCharacter);
        }

        if (!thirdHexCharacter.IsHexDigit())
        {
            return CreateUniversalCharacterNameFailure(index + 3, "third", type,
                ComplexUniversalCharacterNameCharacter, thirdHexCharacter);
        }

        if (!forthHexCharacter.IsHexDigit())
        {
            return CreateUniversalCharacterNameFailure(index + 4, "forth", type,
                ComplexUniversalCharacterNameCharacter, forthHexCharacter);
        }

        if (!fifthHexCharacter.IsHexDigit())
        {
            return CreateUniversalCharacterNameFailure(index + 5, "fifth", type,
                ComplexUniversalCharacterNameCharacter, fifthHexCharacter);
        }

        if (!sixthHexCharacter.IsHexDigit())
        {
            return CreateUniversalCharacterNameFailure(index + 6, "sixth", type,
                ComplexUniversalCharacterNameCharacter, sixthHexCharacter);
        }

        if (!seventhHexCharacter.IsHexDigit())
        {
            return CreateUniversalCharacterNameFailure(index + 7, "seventh", type,
                ComplexUniversalCharacterNameCharacter, seventhHexCharacter);
        }

        if (!eighthHexCharacter.IsHexDigit())
        {
            return CreateUniversalCharacterNameFailure(index + 8, "eighth", type,
                ComplexUniversalCharacterNameCharacter, eighthHexCharacter);
        }

        return ProgressiveParsingResult.CreateSuccess(index + neededLength + 1);
    }

    private static ProgressiveParsingResult CreateUniversalCharacterNameFailure(int index,
        string numberString, string type, char typeCharacter, char character)
    {
        return ProgressiveParsingResult.CreateFailure(index, $"Error: expected the {numberString} digit after " +
                                                             $"a {type} universal character name (\"{UniversalCharacterNameCharacter}" +
                                                             $"{typeCharacter}\")" +
                                                             $"to be a hexadecimal digit but was '{character}' (U+{character:2X})\n" +
                                                             HexAllowedCharactersErrorMessage);
    }
}

internal static class CharExtensions
{
    internal static bool IsCDigit(this char c) => c is >= '0' and <= '9';

    internal static bool IsCNonDigit(this char c) => c is '_' or >= 'A' and <= 'Z' or >= 'a' and <= 'z';

    internal static bool IsHexDigit(this char c) => c is >= '0' and <= '9' or >= 'A' and <= 'F' or >= 'a' and <= 'f';
}