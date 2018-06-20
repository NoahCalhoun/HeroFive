using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class H5TileRenderer : H5ObjectBase
{
    private Material mMaterial;
    public Material Material { get { if (mMaterial == null) InitObject(); return mMaterial; } }

    public override void InitObject()
    {
        mMaterial = GetComponent<MeshRenderer>().material;
    }
}
