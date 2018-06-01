using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public enum TILE_TYPE
{
    TILE_TYPE_NONE,
    TILE_TYPE_NORMAL,
    TILE_TYPE_WATER,
}

public enum TILE_NEIGHBOR
{
    Up,
    Down,
    Left,
    Right,
    Max
}

[StructLayout(LayoutKind.Explicit)]
struct Coordinate
{
    [FieldOffset(0)]
    public byte x;

    [FieldOffset(1)]
    public byte y;

    [FieldOffset(0)]
    public ushort xy;
}

public class H5TileBase : H5ObjectBase
{
    public static readonly float TileSize = 1f;

    private H5TileBase[] mNeighbors = new H5TileBase[(int)TILE_NEIGHBOR.Max];
    private Material Material;

    private ushort Coordinate;
    private int NeighborFlag;
    private int SettingFlag;

    private Coordinate m_Coordinate;

    public bool IsWalkable { get { return m_TileType == TILE_TYPE.TILE_TYPE_NORMAL; } }

    public TILE_TYPE m_TileType;
    public override void InitObject()
    {
        Vector4 m_TileUV = new Vector4(0, 0, 0, 0);

        var loadMaterial = Resources.Load("Material/Tile") as Material;
        loadMaterial.SetTexture("_MainTex", ResourceManager.Instance.LoadImage("Tile", "Texture/FT2/Map/st51a"));

        switch (m_TileType)
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

    public void InitTile(TILE_TYPE type, ushort coord)
    {
        m_TileType = type;
        Coordinate = coord;

        Vector4 m_TileUV = new Vector4(0, 0, 0, 0);

        var loadMaterial = Resources.Load("Material/Tile") as Material;
        var renderer = GetComponent<MeshRenderer>();
        renderer.material = loadMaterial;
        Material = renderer.material;

        switch (m_TileType)
        {
            case TILE_TYPE.TILE_TYPE_NONE:
                m_TileUV.Set(0, 0, 1, 1);
                Material.SetTexture("_MainTex", ResourceManager.Instance.LoadImage("TileTest", "Texture/FT2/Map/st51a"));
                break;

            case TILE_TYPE.TILE_TYPE_NORMAL:
                m_TileUV.Set(0, 0, 1, 1);
                Material.SetTexture("_MainTex", ResourceManager.Instance.LoadImage("NormalTile", "Texture/FT2/Map/st26f"));
                break;

            case TILE_TYPE.TILE_TYPE_WATER:
                m_TileUV.Set(0, 0, 1, 1);
                Material.SetTexture("_MainTex", ResourceManager.Instance.LoadImage("WaterTile", "Texture/FT2/Map/st33h"));
                break;
        }

        Material.SetVector("_UVPos", m_TileUV);
    }

    public void SetNeighbor(TILE_NEIGHBOR type, H5TileBase tile)
    {
        var typeInt = (int)type;
        if (typeInt < 0 || typeInt >= (int)TILE_NEIGHBOR.Max)
            return;

        mNeighbors[typeInt] = (tile != null && tile.IsWalkable) ? tile : null;
    }

    public void SetPicked(bool picked)
    {
        var pickFlag = 1 << 5;
        if (picked)
            NeighborFlag |= pickFlag;
        else if ((NeighborFlag & pickFlag) == pickFlag)
            NeighborFlag ^= pickFlag;
    }

    public void ClearFlag()
    {
        NeighborFlag = 0;
        SettingFlag = -1;
    }

    public void SetFlag(TILE_NEIGHBOR neighbor)
    {
        if (neighbor == TILE_NEIGHBOR.Max)
            return;

        var flag = 1 << ((int)neighbor + 1);
        NeighborFlag |= flag;
    }

    void Update()
    {
        if (SettingFlag != NeighborFlag)
        {
            Material.SetInt("_Neighbor", NeighborFlag);
            SettingFlag = NeighborFlag;
        }
    }

    public HashSet<ushort> GetBound(int bound)
    {
        return BoundCheckRecursive(bound, null);
    }

    HashSet<ushort> BoundCheckRecursive(int bound, HashSet<ushort> tiles)
    {
        if (tiles == null)
            tiles = new HashSet<ushort>();

        if (bound < 0)
            return tiles;

        if (tiles.Contains(Coordinate) == false)
            tiles.Add(Coordinate);

        for (TILE_NEIGHBOR i = 0; i < TILE_NEIGHBOR.Max; ++i)
        {
            var neighbor = mNeighbors[(int)i];
            if (neighbor != null)
            {
                neighbor.BoundCheckRecursive(bound - 1, tiles);
            }
        }

        return tiles;
    }
}
