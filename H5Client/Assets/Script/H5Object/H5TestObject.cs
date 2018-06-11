using System.Collections;
using System.Collections.Generic;

using UnityEngine;
public class H5TestObject : H5ObjectBase
{
    MovementSystem m_MovementSystem;

    public override void InitObject()
    {
        var loadMaterial = Resources.Load("Material/AlphaTest") as Material;
        //loadMaterial.SetTexture("_MainTex", ResourceManager.Instance.LoadImage("Test", "Texture/TestImage"));
        GetComponent<MeshRenderer>().material = loadMaterial;

        TM.localScale = new Vector3(5, 5, 1);
        m_MovementSystem = new MovementSystem();
    }

    public void MoveTo(byte _x, byte _y)
    {
        m_MovementSystem.CalcMovePath(_x, _y);
    }

    private void Update()
    {
        m_MovementSystem.Update();
    }
}
