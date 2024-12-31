using System.Numerics;
using CEngine.Types;

namespace CEngine.Structure.Constants.Integers;

public partial class Integer
{
    public static Integer operator ++(Integer value)
    {
        value.Value++;
        return value;
    }

    public static Integer operator --(Integer value)
    {
        value.Value--;
        return value;
    }

    public static Integer operator +(Integer left, Integer right)
    {
        return new Integer(left.Value + right.Value, IntegerType.CombineIntegerTypesInOperation(left, right));
    }

    public void Add(Integer value)
    {
        Value += value.Value;
    }

    public void Add(BigInteger value)
    {
        if (BigInteger.IsNegative(Value + value))
            throw new ArithmeticException($"Error: You attempted to add {value} to {Value} " +
                                          $"but the result is negative {Value + value} and cannot be added.\n" +
                                          $"Integer constants cannot hold strictly negative values.");

        Value += value;
    }

    public static Integer operator -(Integer left, Integer right)
    {
        return new Integer(left.Value - right.Value, IntegerType.CombineIntegerTypesInOperation(left, right));
    }

    public void Subtract(Integer value)
    {
        Subtract(value.Value);
    }

    public void Subtract(BigInteger value)
    {
        if (BigInteger.IsNegative(Value - value))
            throw new ArithmeticException($"Error: You attempted to subtract {value} from {Value} " +
                                          $"but the result is negative {Value - value} and cannot be subtracted.\n" +
                                          $"Integer constants cannot hold strictly negative values.");

        Value -= value;
    }

    public static Integer operator *(Integer left, Integer right)
    {
        return new Integer(left.Value * right.Value, IntegerType.CombineIntegerTypesInOperation(left, right));
    }

    public void Multiply(Integer value)
    {
        Value *= value.Value;
    }

    public void Multiply(BigInteger value)
    {
        if (BigInteger.IsNegative(value))
            throw new ArithmeticException($"Error: You attempted to multiply by a negative value {value}" +
                                          $"but the result of the multiplication would thus be negative.\n" +
                                          $"Integer constants cannot hold strictly negative values.");

        Value *= value;
    }

    public static Integer operator /(Integer left, Integer right)
    {
        return new Integer(left.Value + right.Value, IntegerType.CombineIntegerTypesInOperation(left, right));
    }

    public void Divide(Integer value)
    {
        if (value.Value == BigInteger.Zero)
            throw new DivideByZeroException($"Error: You attempted to divide {value.Value} by 0.");

        Value /= value.Value;
    }

    public void Divide(BigInteger value)
    {
        if (value == BigInteger.Zero)
            throw new DivideByZeroException($"Error: You attempted to divide {value} by 0.");

        if (BigInteger.IsNegative(value))
            throw new ArithmeticException($"Error: You attempted to divide by a negative value {value}" +
                                          $"but the result of the multiplication would thus be negative.\n" +
                                          $"Integer constants cannot hold strictly negative values.");


        Value /= value;
    }

    public static Integer operator %(Integer left, Integer right)
    {
        return new Integer(left.Value % right.Value, IntegerType.CombineIntegerTypesInOperation(left, right));
    }

    public void Modulo(Integer value)
    {
        if (value.Value == BigInteger.Zero)
            throw new DivideByZeroException($"Error: You attempted to divide {value.Value} by 0.");

        Value %= value.Value;
        if (BigInteger.IsNegative(Value))
            Value += value.Value;
    }

    public void Modulo(BigInteger value)
    {
        if (value == BigInteger.Zero)
            throw new DivideByZeroException($"Error: You attempted to divide {value} by 0.");

        if (BigInteger.IsNegative(value))
            throw new ArithmeticException(
                $"Error: You attempted to perform a modulus with a negative value ({Value} % {value})." +
                "This is not permitted.");

        Value %= value;
        if (BigInteger.IsNegative(value))
            Value += value;
    }

    public static Integer operator -(Integer value)
    {
        if (value.Value == BigInteger.Zero)
            return value;

        throw new ArithmeticException("Error: This operation would result in a negative integer.\n" +
                                      "Integer constants cannot hold strictly negative values.");
    }

    public void Negate()
    {
        if (Value == BigInteger.Zero)
            return;

        throw new ArithmeticException("This operation would result in a negative integer.\n" +
                                      "Integer constants cannot hold strictly negative values.");
    }

    public static Integer operator +(Integer value)
    {
        return value;
    }

    public void Identity()
    {
        // Nothing to be done
    }

    public static bool operator true(Integer value)
    {
        return !value.Value.IsZero;
    }

    public static bool operator false(Integer value)
    {
        return value.Value.IsZero;
    }

    public static Integer operator &(Integer left, Integer right)
    {
        return new Integer(left.Value & right.Value, IntegerType.CombineIntegerTypesInOperation(left, right));
    }

    public void And(Integer value)
    {
        Value &= value.Value;
    }

    public void And(BigInteger value)
    {
        Value &= value;
    }

    public static Integer operator |(Integer left, Integer right)
    {
        return new Integer(left.Value | right.Value, IntegerType.CombineIntegerTypesInOperation(left, right));
    }

    public void Or(Integer value)
    {
        Value |= value.Value;
    }

    public void Or(BigInteger value)
    {
        if (BigInteger.IsNegative(value))
            throw new ArithmeticException(
                $"Error: You attempted to perform a bitwise or operation between {Value} and {value} " +
                $"({Value} | {value}). This would result in a negative integer.\n" +
                "Integer constants cannot hold strictly negative values.");

        Value |= value;
    }

    public static Integer operator ^(Integer left, Integer right)
    {
        return new Integer(left.Value ^ right.Value, IntegerType.CombineIntegerTypesInOperation(left, right));
    }

    public void Xor(Integer value)
    {
        Value ^= value.Value;
    }

    public void Xor(BigInteger value)
    {
        if (BigInteger.IsNegative(value))
            throw new ArithmeticException(
                $"Error: You attempted to perform an xor operation between {Value} and {value} " +
                $"({Value} ^ {value}). This would result in a strictly negative integer.\n" +
                "Integer constants cannot hold strictly negative values.");
    }

    public static Integer operator ~(Integer value)
    {
        throw new ArithmeticException("Error: The one's complement of any positive integer is a negative integer.\n" +
                                      "Integer constants cannot hold strictly negative values.");
    }

    public void OnesComplement()
    {
        throw new ArithmeticException("Error: The one's complement of any positive integer is a negative integer.\n" +
                                      "Integer constants cannot hold strictly negative values.");
    }

    public static bool operator ==(Integer? left, Integer? right)
    {
        if (left is null)
            return right is null;

        if (right is null)
            return false;

        return left.Value == right.Value;
    }

    public static bool operator ==(Integer? left, BigInteger? right)
    {
        if (left is null)
            return right is null;

        if (right is null)
            return false;

        return left.Value == right;
    }

    public static bool operator ==(BigInteger? left, Integer? right)
    {
        return right == left;
    }

    public static bool operator ==(Integer? left, BigInteger right)
    {
        if (left is null)
            return false;

        return left.Value == right;
    }

    public static bool operator ==(BigInteger left, Integer? right)
    {
        return right == left;
    }

    public static bool operator !=(Integer? left, Integer? right)
    {
        return !(left == right);
    }

    public static bool operator !=(Integer? left, BigInteger? right)
    {
        return !(left == right);
    }

    public static bool operator !=(BigInteger? left, Integer? right)
    {
        return !(left == right);
    }

    public static bool operator !=(Integer? left, BigInteger right)
    {
        return !(left == right);
    }

    public static bool operator !=(BigInteger left, Integer? right)
    {
        return !(left == right);
    }

    public static bool operator >(Integer left, Integer right)
    {
        return left.Value > right.Value;
    }

    public static bool operator >(Integer left, BigInteger right)
    {
        return left.Value > right;
    }

    public static bool operator >(BigInteger left, Integer right)
    {
        return left > right.Value;
    }

    public static bool operator >=(Integer left, Integer right)
    {
        return left.Value >= right.Value;
    }

    public static bool operator >=(Integer left, BigInteger right)
    {
        return left.Value >= right;
    }

    public static bool operator >=(BigInteger left, Integer right)
    {
        return left >= right.Value;
    }

    public static bool operator <(Integer left, Integer right)
    {
        return left.Value < right.Value;
    }

    public static bool operator <(Integer left, BigInteger right)
    {
        return left.Value < right;
    }

    public static bool operator <(BigInteger left, Integer right)
    {
        return left < right.Value;
    }

    public static bool operator <=(Integer left, Integer right)
    {
        return left.Value <= right.Value;
    }

    public static bool operator <=(Integer left, BigInteger right)
    {
        return left.Value <= right;
    }

    public static bool operator <=(BigInteger left, Integer right)
    {
        return left <= right.Value;
    }

    public static Integer operator <<(Integer value, int shiftAmount)
    {
        return new Integer(value.Value << shiftAmount, value.Type);
    }

    public void ShiftLeft(int shiftAmount)
    {
        Value <<= shiftAmount;
    }

    public static Integer operator >> (Integer value, int shiftAmount)
    {
        return new Integer(value.Value >> shiftAmount, value.Type);
    }

    public void ShiftRight(int shiftAmount)
    {
        Value >>= shiftAmount;
    }

    public static Integer operator >>> (Integer value, int shiftAmount)
    {
        return new Integer(value.Value >>> shiftAmount, value.Type);
    }
}