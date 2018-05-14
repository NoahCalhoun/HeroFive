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
}
