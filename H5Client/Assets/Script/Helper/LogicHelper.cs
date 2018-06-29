using System.Text;

using UnityEngine;

public static class LogicHelper
{
    public static void PlaceOnWorld(this H5ObjectBase obj, float x, float z)
    {
        obj.TM.rotation = Quaternion.Euler(90, 0, 0);
        obj.TM.position = new Vector3(x, 0, z);
    }

    public static void PlaceOnTile(this H5ObjectBase obj, H5TileBase tile)
    {
        obj.TM.position = new Vector3(tile.m_Coordinate.x, 0, tile.m_Coordinate.y);
        obj.TM.rotation = WorldManager.Instance.CameraRoot.rotation;
    }

    public static void PlaceOnTilePosition(this H5ObjectBase obj, float x, float z)
    {
        obj.TM.position = new Vector3(x, 0, z);
        obj.TM.rotation = WorldManager.Instance.CameraRoot.rotation;
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

    public static ushort GetNeighborCoordinate(this ushort host, H5Direction type)
    {
        switch(type)
        {
            case H5Direction.Up:
                return host.GetUpCoordinate();
            case H5Direction.Down:
                return host.GetDownCoordinate();
            case H5Direction.Left:
                return host.GetLeftCoordinate();
            case H5Direction.Right:
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

    public static bool MousePickingOnWorld(out RaycastHit hit, int layerMask, bool ckeckUI = true)
    {
        Ray ray;
        if (ckeckUI)
        {
            var uiCamera = UIManager.Instance.UICameraRoot.gameObject.GetComponent<Camera>();
            ray = uiCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit uiHit;
            Physics.Raycast(ray, out uiHit, float.PositiveInfinity, -1);
            if (uiHit.collider != null)
            {
                hit = new RaycastHit();
                return false;
            }
        }

        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out hit, float.PositiveInfinity, layerMask);
        return true;
    }

    public static void Clear(this StringBuilder sb)
    {
        sb.Length = 0;
    }

    public static bool IsFront(this H5Direction dir)
    {
        return dir == H5Direction.Up || dir == H5Direction.Right;
    }

    public static bool IsMirror(this H5Direction dir)
    {
        return dir == H5Direction.Up || dir == H5Direction.Down;
    }

    public static H5Direction VectorToDirection(this Vector3 dir)
    {
        float[] factor = new float[(int)H5Direction.Max];
        factor[(int)H5Direction.Up] = Vector3.Dot(dir, Vector3.forward);
        factor[(int)H5Direction.Down] = Vector3.Dot(dir, -Vector3.forward);
        factor[(int)H5Direction.Left] = Vector3.Dot(dir, -Vector3.right);
        factor[(int)H5Direction.Right] = Vector3.Dot(dir, Vector3.right);

        int maxIdx = -1;
        float maxfloat = 0;
        for (int i = 0; i < (int)H5Direction.Max; ++i)
        {
            if (maxfloat < factor[i])
            {
                maxIdx = i;
                maxfloat = factor[i];
            }
        }

        return maxIdx == -1 ? H5Direction.Max : (H5Direction)maxIdx;
    }

    public static H5Direction Reverse(this H5Direction dir)
    {
        switch (dir)
        {
            case H5Direction.Up:
                return H5Direction.Down;

            case H5Direction.Down:
                return H5Direction.Up;

            case H5Direction.Right:
                return H5Direction.Left;

            case H5Direction.Left:
                return H5Direction.Right;
        }

        return H5Direction.Max;
    }

    public static H5Direction Relation(this H5Direction dir, H5Direction test)
    {
        switch (dir)
        {
            case H5Direction.Up:
                {
                    switch (test)
                    {
                        case H5Direction.Up:
                            return H5Direction.Up;
                        case H5Direction.Down:
                            return H5Direction.Down;
                        case H5Direction.Left:
                            return H5Direction.Left;
                        case H5Direction.Right:
                            return H5Direction.Right;
                    }
                    break;
                }

            case H5Direction.Down:
                {
                    switch (test)
                    {
                        case H5Direction.Up:
                            return H5Direction.Down;
                        case H5Direction.Down:
                            return H5Direction.Up;
                        case H5Direction.Left:
                            return H5Direction.Right;
                        case H5Direction.Right:
                            return H5Direction.Left;
                    }
                    break;
                }

            case H5Direction.Left:
                {
                    switch (test)
                    {
                        case H5Direction.Up:
                            return H5Direction.Right;
                        case H5Direction.Down:
                            return H5Direction.Left;
                        case H5Direction.Left:
                            return H5Direction.Up;
                        case H5Direction.Right:
                            return H5Direction.Down;
                    }
                    break;
                }

            case H5Direction.Right:
                {
                    switch (test)
                    {
                        case H5Direction.Up:
                            return H5Direction.Left;
                        case H5Direction.Down:
                            return H5Direction.Right;
                        case H5Direction.Left:
                            return H5Direction.Down;
                        case H5Direction.Right:
                            return H5Direction.Up;
                    }
                    break;
                }
        }

        return H5Direction.Max;
    }

    public static RCoordinate Rotate(this RCoordinate rCoord, H5Direction rot)
    {
        short newX = 0, newY = 0;
        switch (rot)
        {
            case H5Direction.Up:
                {
                    newX = rCoord.x;
                    newY = rCoord.y;
                    break;
                }

            case H5Direction.Down:
                {
                    newX = (short)(-rCoord.x);
                    newY = (short)(-rCoord.y);
                    break;
                }

            case H5Direction.Left:
                {
                    newX = (short)(-rCoord.y);
                    newY = rCoord.x;
                    break;
                }

            case H5Direction.Right:
                {
                    newX = rCoord.y;
                    newY = (short)(-rCoord.x);
                    break;
                }

            case H5Direction.Max:
                {
                    newX = short.MaxValue;
                    newY = short.MaxValue;
                    break;
                }
        }

        return new RCoordinate(newX, newY);
    }
}
