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

    public static void PlaceOnCell(this H5ObjectBase obj, int _iRow, int _iColumn)
    {
        obj.TM.rotation = Quaternion.Euler(0, 45, 0);
        GameObject p = GetTile(_iRow, _iColumn);
        if(p==null)
        {
            int i = 0;
        }
        obj.TM.position = GetTileTransform(_iRow, _iColumn).position;
        //obj.TM.position = new Vector3(x, 0, z);
    }

    public static GameObject GetTile(int _iRow, int _iColumn)
    {
        string sFind = string.Format("/Tile/{0}/{1}", _iRow, _iColumn);
        return GameObject.Find(sFind);
    }

    public static Transform GetTileTransform(int _iRow, int _iColumn)
    {
        string sFind = string.Format("/Tile/{0}/{1}", _iRow, _iColumn);
        return GameObject.Find(sFind).GetComponent<Transform>();
    }
}
