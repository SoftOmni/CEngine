using System.Diagnostics;
using CEngine.ParsingResults;
using CEngine.SegmentedStrings;
using CEngine.Structure.Identifiers;

namespace CEngine.Structure.Constants.Integers;

public partial class Integer
{
    public const char SpecialFormatLead = '0';

    public const char HexadecimalLeadCharacterUppercase = 'X';

    public const char HexadecimalLeadCharacterLowercase = 'x';

    public const char BinaryLeadCharacterUppercase = 'B';

    public const char BinaryLeadCharacterLowercase = 'b';

    public const char UnsignedSuffixLowercase = 'u';

    public const char UnsignedSuffixUppercase = 'U';

    public const char LongSuffixLowercase = 'l';

    public const char LongSuffixUppercase = 'L';

    public static ParsingResult Check(IStringBuilder code)
    {
        if (code.Length == 0)
        {
            return ParsingResult.CreateFailure(0,
                "Error: an integer cannot be empty.");
        }

        char firstCharacter = code[0];

        ProgressiveParsingResult endParsingResult = CheckSuffix(code);
        if (endParsingResult.IsFailure)
            return endParsingResult.ToParsingResult();

        int lastCharacter = endParsingResult.NewIndex;

        return firstCharacter == SpecialFormatLead
            ? CheckSpeciallyFormattedIntegers(code, lastCharacter)
            : CheckDecimalConstant(code, lastCharacter);
    }

    private static ProgressiveParsingResult CheckSuffix(IStringBuilder code)
    {
        int length = 0;
        int index = code.Length - 1;
        while (index >= 0 && length <= 3 && code[index].IsSuffix())
        {
            length += 1;
            index -= 1;
        }

        if (length > 3)
        {
            return ProgressiveParsingResult.CreateFailure(index,
                "Error: the integer suffix must be at most 3 characters but is longer.");
        }

        if (index <= 3)
        {
            return ProgressiveParsingResult.CreateFailure(0,
                "Error: the integer suffix is the entire integer. This is not valid.", 1);
        }

        return length switch
        {
            0 or 1 => ProgressiveParsingResult.CreateSuccess(code.Length - 1 - length),
            2 when code[^1].IsLongSuffix() && code[^2].IsLongSuffix() => ProgressiveParsingResult.CreateFailure(code.Length - 1,
                $"Error: a long-long suffix must be either \"{LongSuffixLowercase}{LongSuffixLowercase}\" or " +
                $"{LongSuffixUppercase}{LongSuffixUppercase} but yours was {code[^2]}{code[^1]}"),
            2 => ProgressiveParsingResult.CreateSuccess(code.Length - 1 - length),
            3 => CheckThreeLongSuffix(code),
            _ => throw new UnreachableException($"Unexpected length: {length}")
        };
    }

    private static ProgressiveParsingResult CheckThreeLongSuffix(IStringBuilder code)
    {
        int successfulIndex = code.Length - 4;

        char firstCharacter = code[^3];
        char secondCharacter = code[^2];
        char thirdCharacter = code[^1];

        return firstCharacter switch
        {
            UnsignedSuffixUppercase or UnsignedSuffixLowercase => secondCharacter switch
            {
                UnsignedSuffixUppercase or UnsignedSuffixLowercase => ProgressiveParsingResult.CreateFailure(
                    code.Length - 2,
                    "Error: you specified an integer is unsigned as " + "the first character of the integer suffix. " +
                    "Only one character unsigned flag is allowed in the " +
                    "integer unsigned and your second character is an unsigned flag " +
                    $"(combo: {firstCharacter}{secondCharacter}{thirdCharacter})."),
                LongSuffixUppercase when thirdCharacter.IsUnsignedSuffix() => ProgressiveParsingResult.CreateFailure(
                    code.Length - 1,
                    "Error: you specified an integer is unsigned as " + "the first character of the integer suffix. " +
                    "Only one character unsigned flag is allowed in the " +
                    "integer unsigned and your third character is an unsigned flag " +
                    $"(combo: {firstCharacter}{secondCharacter}{thirdCharacter})."),
                LongSuffixLowercase when thirdCharacter.IsUnsignedSuffix() => ProgressiveParsingResult.CreateFailure(
                    code.Length - 1,
                    "Error: you specified an integer is unsigned as " + "the first character of the integer suffix. " +
                    "Only one character unsigned flag is allowed in the " +
                    "integer unsigned and your third character is an unsigned flag " +
                    $"(combo: {firstCharacter}{secondCharacter}{thirdCharacter})."),
                LongSuffixUppercase when thirdCharacter is LongSuffixLowercase =>
                    ProgressiveParsingResult.CreateFailure(code.Length - 1,
                        $"Error: a long-long suffix must be either " +
                        $"\"{LongSuffixLowercase}{LongSuffixLowercase}\" or " +
                        $"{LongSuffixUppercase}{LongSuffixUppercase} but yours " +
                        $"was {secondCharacter}{thirdCharacter}"),
                LongSuffixLowercase when thirdCharacter is LongSuffixUppercase =>
                    ProgressiveParsingResult.CreateFailure(code.Length - 1,
                        $"Error: a long-long suffix must be either " +
                        $"\"{LongSuffixLowercase}{LongSuffixLowercase}\" or " +
                        $"{LongSuffixUppercase}{LongSuffixUppercase} but yours " +
                        $"was {secondCharacter}{thirdCharacter}"),
                LongSuffixUppercase or LongSuffixLowercase => ProgressiveParsingResult.CreateSuccess(successfulIndex),
                _ => throw new UnreachableException($"firstCharacter: '{firstCharacter}'|" +
                                                    $"secondCharacter: '{secondCharacter}'|" +
                                                    $"thirdCharacter: '{thirdCharacter}'")
            },
            LongSuffixUppercase => secondCharacter switch
            {
                UnsignedSuffixUppercase or UnsignedSuffixLowercase => ProgressiveParsingResult.CreateFailure(
                    code.Length - 2,
                    "Error: an unsigned suffix specifier must be either " +
                    "the first or third character of an integer suffix " +
                    "when the integer suffix is 3 characters but " +
                    "it was the second character of the integer suffix"),
                LongSuffixUppercase when !thirdCharacter.IsUnsignedSuffix() => ProgressiveParsingResult.CreateFailure(
                    code.Length - 1,
                    "Error: a long specifier must be two characters " +
                    "long at the maximum but was three characters long. Your combination " +
                    $"was {firstCharacter}{secondCharacter}{thirdCharacter}"),
                LongSuffixUppercase when thirdCharacter.IsUnsignedSuffix() => ProgressiveParsingResult.CreateSuccess(
                    successfulIndex),
                LongSuffixLowercase => ProgressiveParsingResult.CreateFailure(code.Length - 2,
                    $"Error: a long-long suffix must be either " +
                    $"\"{LongSuffixLowercase}{LongSuffixLowercase}\" or " +
                    $"{LongSuffixUppercase}{LongSuffixUppercase} but yours " +
                    $"was {firstCharacter}{secondCharacter}"),
                _ => throw new UnreachableException($"firstCharacter: '{firstCharacter}'|" +
                                                    $"secondCharacter: '{secondCharacter}'|" +
                                                    $"thirdCharacter: '{thirdCharacter}'")
            },
            LongSuffixLowercase => secondCharacter switch
            {
                UnsignedSuffixUppercase or UnsignedSuffixLowercase => ProgressiveParsingResult.CreateFailure(
                    code.Length - 2,
                    "Error: an unsigned suffix specifier must be either " +
                    "the first or third character of an integer suffix " +
                    "when the integer suffix is 3 characters but " +
                    "it was the second character of the integer suffix"),
                LongSuffixUppercase => ProgressiveParsingResult.CreateFailure(code.Length - 2,
                    $"Error: a long-long suffix must be either " +
                    $"\"{LongSuffixLowercase}{LongSuffixLowercase}\" or " +
                    $"{LongSuffixUppercase}{LongSuffixUppercase} but yours " +
                    $"was {firstCharacter}{secondCharacter}"),
                LongSuffixLowercase when !thirdCharacter.IsUnsignedSuffix() => ProgressiveParsingResult.CreateFailure(
                    code.Length - 1,
                    "Error: a long specifier must be two characters " +
                    "long at the maximum but was three characters long. Your combination " +
                    $"was {firstCharacter}{secondCharacter}{thirdCharacter}"),
                LongSuffixLowercase when thirdCharacter.IsUnsignedSuffix() => ProgressiveParsingResult.CreateSuccess(
                    successfulIndex),
                _ => throw new UnreachableException($"firstCharacter: '{firstCharacter}'|" +
                                                    $"secondCharacter: '{secondCharacter}'|" +
                                                    $"thirdCharacter: '{thirdCharacter}'")
            },
            _ => throw new UnreachableException($"firstCharacter: '{firstCharacter}'|" +
                                                $"secondCharacter: '{secondCharacter}' +" +
                                                $"thirdCharacter: '{thirdCharacter}'")
        };
    }

    private static ParsingResult CheckSpeciallyFormattedIntegers(IStringBuilder code, int finalIndex)
    {
        if (code.Length == 1)
        {
            return ParsingResult.CreateSuccess();
        }

        char directingCharacter = code[1];
        return directingCharacter switch
        {
            BinaryLeadCharacterUppercase => CheckBinaryConstant(code, finalIndex),
            BinaryLeadCharacterLowercase => CheckBinaryConstant(code, finalIndex),
            HexadecimalLeadCharacterUppercase => CheckHexadecimalConstant(code, finalIndex),
            HexadecimalLeadCharacterLowercase => CheckHexadecimalConstant(code, finalIndex),
            '0' => CheckOctalConstant(code, finalIndex),
            '1' => CheckOctalConstant(code, finalIndex),
            '2' => CheckOctalConstant(code, finalIndex),
            '3' => CheckOctalConstant(code, finalIndex),
            '4' => CheckOctalConstant(code, finalIndex),
            '5' => CheckOctalConstant(code, finalIndex),
            '6' => CheckOctalConstant(code, finalIndex),
            '7' => CheckOctalConstant(code, finalIndex),
            _ => ParsingResult.CreateFailure(1, "Error: an integer constant must either be a binary constant" +
                                                $" starting with \"{SpecialFormatLead}{BinaryLeadCharacterUppercase}\" or " +
                                                $"\"{SpecialFormatLead}{BinaryLeadCharacterLowercase}\" and then containing either '0' or '1' characters, " +
                                                "a hexadecimal constant starting with " +
                                                $"\"{SpecialFormatLead}{HexadecimalLeadCharacterUppercase}\" or " +
                                                $"\"{SpecialFormatLead}{HexadecimalLeadCharacterLowercase}\"\n " +
                                                "and then followed by " +
                                                "'0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F', 'a', 'b', 'c', 'D', 'e' or 'f'," +
                                                $" an octal constant starting with '{SpecialFormatLead}' and then made up of octal digits: " +
                                                "'0', '1', '2', '3', '4', '5', '6' or '7', a decimal constant starting with " +
                                                "made up of '0', '1', '2', '3', '4', '5', '6', '7', '8' or '9' characters " +
                                                "but the second character was not compatible with either of these descriptions:" +
                                                $"'{directingCharacter}' ({directingCharacter:2X}")
        };
    }

    private static ParsingResult CheckDecimalConstant(IStringBuilder code, int finalIndex)
    {
        char firstCharacter = code[0];
        if (!firstCharacter.IsCDigit())
        {
            return ParsingResult.CreateFailure(0, "Error: a decimal constant must start with either " +
                                                  "'1', '2', '3', '4', '5', '6', '7', '8', or '9' (NOT '0') " +
                                                  $"but started with '{firstCharacter}' ({firstCharacter:2X})");
        }

        for (int i = 1; i <= finalIndex; i++)
        {
            char character = code[i];
            if (!character.IsCDigit())
            {
                return ParsingResult.CreateFailure(i, "Error: a decimal constant must contain either " +
                                                      "'0', '1', '2', '3', '4', '5', '6', '7', '8', or '9' " +
                                                      $"but started with '{character}' ({character:2X})");
            }
        }

        return ParsingResult.CreateSuccess();
    }

    private static ParsingResult CheckOctalConstant(IStringBuilder code, int finalIndex)
    {
        for (int i = 1; i <= finalIndex; i++)
        {
            char character = code[i];
            if (!character.IsOctalDigit())
            {
                return ParsingResult.CreateFailure(i, "Error: an octal constant must start with '0' and" +
                                                      "then be made up of either one of the following characters:\n" +
                                                      $"'0', '1', '2', '3', '4', '5', '6' or '7' but encountered '{character}' ({character:2X})");
            }
        }

        return ParsingResult.CreateSuccess();
    }

    private static ParsingResult CheckHexadecimalConstant(IStringBuilder code, int finalIndex)
    {
        for (int i = 2; i <= finalIndex; i++)
        {
            char character = code[i];
            if (!character.IsHexDigit())
            {
                return ParsingResult.CreateFailure(i, "Error: a hexadecimal constant must start with " +
                                                      $"\"0{HexadecimalLeadCharacterUppercase}\" or " +
                                                      $"\"0{HexadecimalLeadCharacterLowercase}\" and " +
                                                      "then be made up of either one of the following characters:\n" +
                                                      "'0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', " +
                                                      "'D', 'E', 'F', 'a', 'b', 'c', 'd', 'e' or 'f' " +
                                                      $"but encountered '{character}' ({character:2X}) instead");
            }
        }

        return ParsingResult.CreateSuccess();
    }

    private static ParsingResult CheckBinaryConstant(IStringBuilder code, int finalIndex)
    {
        for (int i = 2; i <= finalIndex; i++)
        {
            char character = code[i];
            if (!character.IsBinaryDigit())
            {
                return ParsingResult.CreateFailure(i, "Error: a binary constant must start with " +
                                                      $"\"0{BinaryLeadCharacterUppercase}\" or \"0{BinaryLeadCharacterLowercase}\" and " +
                                                      "then be made up of '0' and '1' characters " +
                                                      $"but encountered '{character}' ({character:2X}) instead");
            }
        }

        return ParsingResult.CreateSuccess();
    }
}

internal static class CharacterExtensions
{
    public static bool IsOctalDigit(this char character) => character is >= '0' and <= '7';

    public static bool IsBinaryDigit(this char character) => character is '0' or '1';

    public static bool IsUnsignedSuffix(this char character) =>
        character is Integer.UnsignedSuffixUppercase
            or Integer.UnsignedSuffixLowercase;

    public static bool IsLongSuffix(this char character) =>
        character is Integer.LongSuffixUppercase
            or Integer.LongSuffixLowercase;

    public static bool IsSuffix(this char character) => IsUnsignedSuffix(character)
                                                   || IsLongSuffix(character);
}