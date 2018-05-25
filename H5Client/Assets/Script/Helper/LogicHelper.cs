using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public static class LogicHelper
{
    public static void PlaceOnWorld(this H5ObjectBase obj, float x, float z)
    {
        obj.TM.rotation = Quaternion.Euler(90, 0, 0);
        obj.TM.position = new Vector3(x, 0, z);
    }

    public static ushort GetCoordinateFromXY(byte x, byte y)
    {
        return (ushort)((x << 8) | y);
    }

    public static void GetXYFromCoordinate(ushort coordinate, out byte x, out byte y)
    {
        x = (byte)(coordinate >> 8);
        y = (byte)(coordinate & byte.MaxValue);
    }

    public static byte GetXFromCoordinate(ushort coordinate)
    {
        return (byte)(coordinate >> 8);
    }

    public static byte GetYFromCoordinate(ushort coordinate)
    {
        return (byte)(coordinate & byte.MaxValue);
    }
}
