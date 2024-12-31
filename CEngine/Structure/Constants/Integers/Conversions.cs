using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using CEngine.Types;

namespace CEngine.Structure.Constants.Integers;

public partial class Integer
{
    public static implicit operator BigInteger(Integer integer)
    {
        return integer.Value;
    }

    public static explicit operator Integer(BigInteger bigInt)
    {
        return new Integer(bigInt, IntegerType.TypeFitForValue(bigInt));
    }

    public static bool TryConvertFromChecked<TOther>(TOther value, [MaybeNullWhen(false)] out Integer result) where TOther : INumberBase<TOther>
    {
        throw new NotImplementedException();
    }

    public static bool TryConvertFromSaturating<TOther>(TOther value, [MaybeNullWhen(false)] out Integer result) where TOther : INumberBase<TOther>
    {
        throw new NotImplementedException();
    }

    public static bool TryConvertFromTruncating<TOther>(TOther value, [MaybeNullWhen(false)] out Integer result) where TOther : INumberBase<TOther>
    {
        throw new NotImplementedException();
    }

    public static bool TryConvertToChecked<TOther>(Integer value, [MaybeNullWhen(false)] out TOther result) where TOther : INumberBase<TOther>
    {
        throw new NotImplementedException();
    }

    public static bool TryConvertToSaturating<TOther>(Integer value, [MaybeNullWhen(false)] out TOther result) where TOther : INumberBase<TOther>
    {
        throw new NotImplementedException();
    }

    public static bool TryConvertToTruncating<TOther>(Integer value, [MaybeNullWhen(false)] out TOther result) where TOther : INumberBase<TOther>
    {
        throw new NotImplementedException();
    }

    
}