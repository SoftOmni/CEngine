namespace CEngine.Structure;

public interface ICopiable<in T>
{
    public void CopyFrom(T other);
}