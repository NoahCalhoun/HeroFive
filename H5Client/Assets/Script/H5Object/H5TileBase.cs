using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TILE_TYPE
{
    TILE_TYPE_NONE,
    TILE_TYPE_NORMAL,
    TILE_TYPE_WATER,
}

public class H5TileBase : H5ObjectBase
{
    public static readonly float TileSize = 1f;

    public TILE_TYPE m_TileType;
    public override void InitObject()
    {
        Vector4 m_TileUV = new Vector4(0, 0, 0, 0);

        var loadMaterial = Resources.Load("Material/Tile") as Material;
        loadMaterial.SetTexture("_MainTex", ResourceManager.Instance.LoadImage("Tile", "Texture/FT2/Map/st51a"));

        switch(m_TileType)
        {
            case TILE_TYPE.TILE_TYPE_NONE:
                m_TileUV.Set(0, 0, 1, 1);
                break;

            case TILE_TYPE.TILE_TYPE_NORMAL:
                m_TileUV.Set(0, 0, 1, 1);
                break;
        }
  
        loadMaterial.SetVector("_UVPos", m_TileUV);
        GetComponent<MeshRenderer>().material = loadMaterial;

        TM.localScale = new Vector3(5, 5, 1);
    }

    public void InitTile(TILE_TYPE type)
    {
        m_TileType = type;

        Vector4 m_TileUV = new Vector4(0, 0, 0, 0);

        var loadMaterial = Resources.Load("Material/Tile") as Material;
        var renderer = GetComponent<MeshRenderer>();
        renderer.material = loadMaterial;

        switch (m_TileType)
        {
            case TILE_TYPE.TILE_TYPE_NONE:
                m_TileUV.Set(0, 0, 1, 1);
                renderer.material.SetTexture("_MainTex", ResourceManager.Instance.LoadImage("TileTest", "Texture/FT2/Map/st51a"));
                break;

            case TILE_TYPE.TILE_TYPE_NORMAL:
                m_TileUV.Set(0, 0, 1, 1);
                renderer.material.SetTexture("_MainTex", ResourceManager.Instance.LoadImage("NormalTile", "Texture/FT2/Map/st26f"));
                break;

            case TILE_TYPE.TILE_TYPE_WATER:
                m_TileUV.Set(0, 0, 1, 1);
                renderer.material.SetTexture("_MainTex", ResourceManager.Instance.LoadImage("WaterTile", "Texture/FT2/Map/st33h"));
                break;
        }

        renderer.material.SetVector("_UVPos", m_TileUV);
    }
}
