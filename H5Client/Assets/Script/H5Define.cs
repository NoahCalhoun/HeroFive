using System.Runtime.InteropServices;

public enum UIWindowType
{
    None = -1,
    TestUI,
    Loading,
    UIMax
}

[StructLayout(LayoutKind.Explicit)]
public struct ACoordinate
{
    [FieldOffset(0)]
    public byte y;

    [FieldOffset(1)]
    public byte x;

    [FieldOffset(0)]
    public ushort xy;

    public ACoordinate(ushort _xy)
    {
        x = 0;
        y = 0;
        this.xy = _xy;
    }

    public ACoordinate(byte _x, byte _y)
    {
        this.xy = 0;
        this.x = _x;
        this.y = _y;
    }

    public bool IsValid { get { return x != byte.MaxValue && y != byte.MaxValue; } }

    public static ACoordinate operator +(ACoordinate acoord, RCoordinate _rcoord)
    {
        var newX = acoord.x + _rcoord.x;
        var newY = acoord.y + _rcoord.y;
        return new ACoordinate(newX < byte.MinValue || newX > byte.MaxValue ? byte.MaxValue : (byte)newX
            , newY < byte.MinValue || newY > byte.MaxValue ? byte.MaxValue : (byte)newY);
    }
}

[StructLayout(LayoutKind.Explicit)]
public struct RCoordinate
{
    [FieldOffset(0)]
    public short y;

    [FieldOffset(1)]
    public short x;

    [FieldOffset(0)]
    public uint xy;

    public RCoordinate(uint _xy)
    {
        x = 0;
        y = 0;
        this.xy = _xy;
    }

    public RCoordinate(short _x, short _y)
    {
        this.xy = 0;
        this.x = _x;
        this.y = _y;
    }
}

public static class H5Define
{
}

public interface IMovable
{
    void Move(byte x, byte y);
    void KnockBack(H5Direction dir, byte count);
}