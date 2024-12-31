namespace CEngine;

public class InvalidEnumValueException<T>(int value) : 
    ArgumentException($"Invalid enum constant value {value} for the enum {nameof(T)}")
    where T : Enum
{
    
}