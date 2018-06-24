public enum UIWindowType
{
    None = -1,
    TestUI,
    Loading,
    UIMax
}

public static class H5Define
{
}

public interface IMovable
{
    void Move(byte x, byte y);
    void KnockBack(H5Direction dir, byte count);
}