using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TILE_TYPE
{
    TILE_TYPE_NONE,
    TILE_TYPE_NORMAL
}
public class H5TileBase : H5ObjectBase
{

    public virtual void InitObject()
    {
        var loadMaterial = Resources.Load("Material/Tile") as Material;
    }
}
