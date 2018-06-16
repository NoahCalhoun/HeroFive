using System.Collections;
using System.Collections.Generic;

using UnityEngine;
public class H5TestObject : H5ObjectBase
{
    MovementSystem m_MovementSystem;

    public override void InitObject()
    {
        var loadMaterial = Resources.Load("Material/Sprite") as Material;
        //loadMaterial.SetTexture("_MainTex", ResourceManager.Instance.LoadImage("Test", "Texture/TestImage"));
        GetComponent<MeshRenderer>().material = loadMaterial;

        TM.localScale = new Vector3(2, 2, 1);

        var texture = ResourceManager.Instance.LoadTexture(Episode.FT2, "char_00a");
        var sprite = texture.GetSpriteData("Idle00");
        GetComponent<MeshRenderer>().material.SetTexture("_MainTex", texture);
        GetComponent<MeshRenderer>().material.SetColor("_CutoffColor", sprite.Cutoff);
        GetComponent<MeshRenderer>().material.SetVector("_SetUV", sprite.ShaderSetUV);
        GetComponent<MeshRenderer>().material.SetVector("_TestUV", sprite.ShaderTestUV);
    }

    public void MoveTo(byte _x, byte _y)
    {
    }

    private void Update()
    {
    }
}
