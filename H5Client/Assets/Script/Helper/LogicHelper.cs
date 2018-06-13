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

    public static ushort GetNeighborCoordinate(this ushort host, TILE_NEIGHBOR type)
    {
        switch(type)
        {
            case TILE_NEIGHBOR.Up:
                return host.GetUpCoordinate();
            case TILE_NEIGHBOR.Down:
                return host.GetDownCoordinate();
            case TILE_NEIGHBOR.Left:
                return host.GetLeftCoordinate();
            case TILE_NEIGHBOR.Right:
                return host.GetRightCoordinate();
            default:
                return ushort.MaxValue;
        }
    }

    public static ushort GetUpCoordinate(this ushort host, byte count = 1)
    {
        var hostX = GetXFromCoordinate(host);
        var hostY = GetYFromCoordinate(host);
        hostY += count;
        return GetCoordinateFromXY(hostX, hostY);
    }

    public static ushort GetDownCoordinate(this ushort host, byte count = 1)
    {
        var hostX = GetXFromCoordinate(host);
        var hostY = GetYFromCoordinate(host);
        hostY -= count;
        return GetCoordinateFromXY(hostX, hostY);
    }

    public static ushort GetLeftCoordinate(this ushort host, byte count = 1)
    {
        var hostX = GetXFromCoordinate(host);
        var hostY = GetYFromCoordinate(host);
        hostX -= count;
        return GetCoordinateFromXY(hostX, hostY);
    }

    public static ushort GetRightCoordinate(this ushort host, byte count = 1)
    {
        var hostX = GetXFromCoordinate(host);
        var hostY = GetYFromCoordinate(host);
        hostX += count;
        return GetCoordinateFromXY(hostX, hostY);
    }

    public static bool MousePickingOnWorld(out RaycastHit hit, int layerMask)
    {
        var uiCamera = UIManager.Instance.UICameraRoot.gameObject.GetComponent<Camera>();
        var ray = uiCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit uiHit;
        Physics.Raycast(ray, out uiHit, float.PositiveInfinity, -1);
        if (uiHit.collider != null)
        {
            hit = new RaycastHit();
            return false;
        }

        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out hit, float.PositiveInfinity, layerMask);
        return true;
    }
}
