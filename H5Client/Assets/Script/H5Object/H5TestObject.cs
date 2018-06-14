using System.Collections;
using System.Collections.Generic;

using UnityEngine;
public class H5TestObject : H5ObjectBase
{
    MovementSystem m_MovementSystem;

    public override void InitObject()
    {
        var loadMaterial = Resources.Load("Material/TileRenderer") as Material;
        //loadMaterial.SetTexture("_MainTex", ResourceManager.Instance.LoadImage("Test", "Texture/TestImage"));
        GetComponent<MeshRenderer>().material = loadMaterial;

        TM.localScale = new Vector3(2, 2, 1);

        var texture = ResourceManager.Instance.LoadTexture(Episode.FT2, "char_00a");
        var sprite = texture.GetSprite("Idle00");
        GetComponent<MeshRenderer>().material.SetTexture("_MainTex", texture);
        GetComponent<MeshRenderer>().material.SetVector("_UVStartPos", new Vector4(sprite.uv[0].x, sprite.uv[0].y, sprite.uv[1].x, sprite.uv[1].y));
    }

    public void MoveTo(byte _x, byte _y)
    {
    }

    private void Update()
    {
    }
}
