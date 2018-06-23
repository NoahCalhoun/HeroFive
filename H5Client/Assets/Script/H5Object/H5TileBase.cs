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

[StructLayout(LayoutKind.Explicit)]
public struct Coordinate
{
    [FieldOffset(0)]
    public byte y;

    [FieldOffset(1)]
    public byte x;

    [FieldOffset(0)]
    public ushort xy;

    public Coordinate(ushort _xy)
    {
        x = 0;
        y = 0;
        this.xy = _xy;
    }

    public Coordinate(byte _x, byte _y)
    {
        this.xy = 0;
        this.x = _x;
        this.y = _y;
    }
}

public class H5TileBase : H5ObjectBase
{
    Vector4 m_TileUV = new Vector4(0, 0, 0, 0);

    public static readonly float TileSize = 1f;
    public static readonly ushort InvalidCoordinate = ushort.MaxValue;

    private H5TileBase[] m_Neighbors = new H5TileBase[(int)H5Direction.Max];
    public H5TileBase GetNeighbor(H5Direction _direction) { return m_Neighbors[(int)_direction]; }
    private Material TileRendererMaterial;
    private Material TileMaterial;

    public ushort Coordinate;
    private int NeighborFlag;
    private int SettingFlag;

    H5ObjectBase ObjectOnTile;

    public Coordinate m_Coordinate { get; private set; }

    // 인자로 H5Character를 넣어서 해당 캐릭터가 이동할 수 있는지에 대한 판단을 하는건 어떨까
    // ex) 물 타일의 경우 비행 유닛은 이동 가능, 지상 타입은 이동 불가
    public bool IsWalkable()
    {
        if (ObjectOnTile != null ) return false;

        switch (m_TileType)
        {
            case TILE_TYPE.TILE_TYPE_WATER:
                return false;
        }

        return true;
    }

    public TILE_TYPE m_TileType;
    public override void InitObject()
    {
        Vector4 m_TileUV = new Vector4(0, 0, 0, 0);

        var loadMaterial = Resources.Load("Material/Tile") as Material;
        //loadMaterial.SetTexture("_MainTex", ResourceManager.Instance.LoadImage("Tile", "Texture/FT2/Map/st51a"));
       
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
        var h5Renderer = GetComponentInChildren<H5TileRenderer>();
        h5Renderer.GO.GetComponent<MeshRenderer>().material = loadMaterial;

        TM.localScale = new Vector3(5, 5, 1);
    }

    public void InitTile(TILE_TYPE type, ushort coord)
    {
        //Coordinate Side
        if (coord != InvalidCoordinate)
        {
            Coordinate = coord;
            m_Coordinate = new Coordinate(Coordinate);
        }
        
        //Material Side
        if (type != TILE_TYPE.TILE_TYPE_NONE)
        {
            m_TileType = type;

            Vector4 m_TileUV = new Vector4(0, 0, 0, 0);

            var tileRenderer = GetComponent<MeshRenderer>();

            bool noMat = tileRenderer.sharedMaterial == null;
            if (noMat)
            {
                var loadMaterial = Resources.Load("Material/Tile") as Material;
                tileRenderer.material = loadMaterial;
            }
#if UNITY_EDITOR
            TileMaterial = noMat ? tileRenderer.material : tileRenderer.sharedMaterial;
#else
            TileMaterial = tileRenderer.material;
#endif
            //-----------------------------------------------------------
            var h5Renderer = GetComponentInChildren<H5TileRenderer>();
            var renderer = h5Renderer.GO.GetComponent<MeshRenderer>();

            noMat = renderer.sharedMaterial == null;
            if (noMat)
            {
                var loadMaterial = Resources.Load("Material/TileRenderer") as Material;
                renderer.material = loadMaterial;
            }
#if UNITY_EDITOR
            TileRendererMaterial = noMat ? renderer.material : renderer.sharedMaterial;
#else
            TileRendererMaterial = renderer.material;
#endif

            TileRendererMaterial.SetTexture("_MainTex", ResourceManager.Instance.LoadImage(RESOURCE_TYPE.Tile, "Tile"));

            float sliceX = 7f;              // 이미지 X축 7칸
            float sliceY = 9.5f;            // 이미지 Y축 9.5칸
            float sizeX = 1f / sliceX;      // 이미지 X축 배율
            float sizeY = 1f / sliceY;      // 이미지 Y축 배율

            float offsetIndexX = 0f;
            float offsetIndexY = 0f;

            switch (m_TileType)
            {
                case TILE_TYPE.TILE_TYPE_NORMAL:
                    {
                        offsetIndexX = 0f;        // 7칸 중 1번째 (인덱스 = 1 - 1 = 0)
                        offsetIndexY = 8.5f;      // 9.5칸 중 9.5번째 (인덱스 9.5 - 1 = 8.5)
                    }
                    break;

                case TILE_TYPE.TILE_TYPE_WATER:
                    {
                        offsetIndexX = 2f;        // 7칸 중 3번째 (인덱스 = 3 - 1 = 2)
                        offsetIndexY = 8.5f;      // 9.5칸 중 9.5번째 (인덱스 9.5 - 1 = 8.5)
                    }
                    break;

                default:
                    {
                        m_TileUV.Set(0, 0, 1, 1);
                        TileRendererMaterial.SetVector("_UVPos", m_TileUV);
                    }
                    return;
            }

            m_TileUV.Set(sizeX, sizeY, sizeX * offsetIndexX, sizeY * offsetIndexY);
            TileRendererMaterial.SetVector("_UVPos", m_TileUV);
            TileRendererMaterial.SetColor("_CutoffColor", new Color32(0, 0, 255, 255));
        }
    }

    public void SetNeighbor(H5Direction type, H5TileBase tile)
    {
        var typeInt = (int)type;
        if (typeInt < 0 || typeInt >= (int)H5Direction.Max)
            return;

        m_Neighbors[typeInt] = (tile != null && tile.IsWalkable()) ? tile : null;
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

    public void SetFlag(H5Direction neighbor)
    {
        if (neighbor == H5Direction.Max)
            return;

        var flag = 1 << ((int)neighbor + 1);
        NeighborFlag |= flag;
    }

    void Update()
    {
        if (SettingFlag != NeighborFlag)
        {
            TileMaterial.SetInt("_Neighbor", NeighborFlag);
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

        for (H5Direction i = 0; i < H5Direction.Max; ++i)
        {
            var neighbor = m_Neighbors[(int)i];
            if (neighbor != null)
            {
                neighbor.BoundCheckRecursive(bound - 1, tiles);
            }
        }

        return tiles;
    }

    public void Refresh()
    {
        InitTile(m_TileType, Coordinate);
    }

    public bool OnTile(H5ObjectBase obj)
    {
        if (obj == null || ObjectOnTile != null) return false;

        ObjectOnTile = obj;

        return true;
    }

    public bool OnTile(H5CharacterBase objChar)
    {
        if (objChar == null || !OnTile((H5ObjectBase)objChar)) return false;

        objChar.OwnTile = this;

        return true;
    }

    public bool LeaveTile(H5ObjectBase obj)
    {
        if (ObjectOnTile != obj) return false;

        ObjectOnTile = null;

        return true;
    }
}
