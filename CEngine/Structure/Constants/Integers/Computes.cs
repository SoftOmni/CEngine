using System.Numerics;

namespace CEngine.Structure.Constants.Integers;

public partial class Integer
{
    public static Integer Abs(Integer value)
    {
        return value;
    }

    public static Integer Log2(Integer value)
    {
        return new Integer(BigInteger.Log2(value.Value), value.Type);
    }
}