using System.Numerics;
using CEngine.Structure.Constants.Integers;

namespace CEngine;

public class Configuration
{
    public static bool SupportMicrosoftExtensions { get; }
    
    public static SuffixOrder SuffixOrder { get; }
    
    public static BigInteger ShortSize { get; }
    
    public static BigInteger IntSize { get; }
    
    public static BigInteger LongSize { get; }
    
    public static BigInteger LongLongSize { get; }
    
    public static BigInteger DoubleSize { get; }
    
    public static BigInteger DoubleLongSize { get; }
}