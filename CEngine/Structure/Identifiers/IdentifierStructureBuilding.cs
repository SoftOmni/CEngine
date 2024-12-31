using System.Diagnostics;
using System.Text;

namespace CEngine.Structure.Identifiers;

public partial class Identifier
{
    protected override void BuildInnerStructures()
    {
        StringBuilder valueBuilder = new();

        int index = 0;
        while (index < CodeLength)
        {
            char c = Code[index];
            if (c.IsCDigit() || c.IsCNonDigit())
            {
                valueBuilder.Append(c);
                index += 1;
            }

            index = BuildUniversalCharacterName(valueBuilder, index);
        }

        Value = valueBuilder.ToString();
    }

    private int BuildUniversalCharacterName(StringBuilder valueBuilder, int index)
    {
        char simpleOrComplexCharacter = Code[index + 1];

        char firstHexCharacter = Code[index + 2];
        char secondHexCharacter = Code[index + 3];
        char thirdHexCharacter = Code[index + 4];
        char forthHexCharacter = Code[index + 5];

        int firstHexValue = firstHexCharacter.ToHexValue();
        int secondHexValue = secondHexCharacter.ToHexValue();
        int thirdHexValue = thirdHexCharacter.ToHexValue();
        int forthHexValue = forthHexCharacter.ToHexValue();
        
        int baseCodePoint = firstHexValue << 3 + secondHexValue << 2 + thirdHexValue << 1 + forthHexValue;

        if (simpleOrComplexCharacter == SimpleUniversalCharacterNameCharacter)
        {
            valueBuilder.Append((char)baseCodePoint);
            return index + 6;
        }
        
        char fifthHexCharacter = Code[index + 6];
        char sixthHexCharacter = Code[index + 7];
        char seventhHexCharacter = Code[index + 8];
        char eighthHexCharacter = Code[index + 9];

        int fifthHexValue = fifthHexCharacter.ToHexValue();
        int sixthHexValue = sixthHexCharacter.ToHexValue();
        int seventhHexValue = seventhHexCharacter.ToHexValue();
        int eighthHexValue = eighthHexCharacter.ToHexValue();
        
        int followupCodePoint = fifthHexValue << 3 + sixthHexValue << 2 + seventhHexValue << 1 + eighthHexValue;
        int combinedCodePoint = baseCodePoint << 4 + followupCodePoint;
        
        char newCharacter = (char)combinedCodePoint;
        valueBuilder.Append(newCharacter);

        return index + 10;
    }

    public void SetValue(string value)
    {
        UnderlyingCode.Clear();

        foreach (char character in value)
        {
            if (character.IsCDigit() || character.IsCNonDigit())
            {
                UnderlyingCode.Append(character);
            }
            
            int codePoint = character;
            UnderlyingCode.Append(DeconstructCodePoint(codePoint));
        }
    }

    private static string DeconstructCodePoint(int codePoint)
    {
        const int limit = ushort.MaxValue;

        int limitFirst = codePoint & limit;
        string hexQuadFirst = DeconstructHexQuad(limitFirst);
        if (codePoint <= limit)
        {
            return $"{UniversalCharacterNameCharacter}{SimpleUniversalCharacterNameCharacter}{hexQuadFirst}";
        }

        int limitSecond = codePoint >> 8;
        string hexQuadSecond = DeconstructHexQuad(limitSecond);
        
        return
            $"{UniversalCharacterNameCharacter}{ComplexUniversalCharacterNameCharacter}{hexQuadSecond}{hexQuadFirst}";
    }

    private static string DeconstructHexQuad(int limitFirst)
    {
        int forth = limitFirst & 1;
        limitFirst >>= 1;
        
        int third = limitFirst & 1;
        limitFirst >>= 1;
        
        int second = limitFirst & 1;
        limitFirst >>= 1;
        
        int first = limitFirst & 1;
        
        string firstAsHex = first.ToString("X");
        string secondAsHex = second.ToString("X");
        string thirdAsHex = third.ToString("X");
        string forthAsHex = forth.ToString("X");

        return $"{firstAsHex}{secondAsHex}{thirdAsHex}{forthAsHex}";
    }
}

internal static class CharacterExtensions
{
    internal static int ToHexValue(this char character)
    {
        return character switch
        {
            >= '0' and <= '9' => character - '0',
            >= 'A' and <= 'Z' => character - 'A' + 10,
            >= 'a' and <= 'z' => character - 'a' + 10,
            _ => throw new UnreachableException()
        };
    } 
}