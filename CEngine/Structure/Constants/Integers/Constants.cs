using System.Numerics;
using CEngine.Types;

namespace CEngine.Structure.Constants.Integers;

public partial class Integer
{
    public static Integer AdditiveIdentity => Zero;

    public static Integer MultiplicativeIdentity => One;

    public static Integer One => new(BigInteger.One, IntType.Int);
   
    public static Integer Zero => new(BigInteger.Zero, IntType.Int);

    public static int Radix => 10;
}