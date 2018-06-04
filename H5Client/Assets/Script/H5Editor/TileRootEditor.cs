using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class TileRootEditor : H5ObjectBase
{
    private Dictionary<ushort, H5TileBase> mTileDic = new Dictionary<ushort, H5TileBase>();
    public Dictionary<ushort, H5TileBase> TileDic { get { return mTileDic; } }
    public TILE_TYPE DefaultTileType;
    public byte CountX;
    public byte CountY;

    void Update()
    {

    }

    public void CreateTiles()
    {
        ClearTiles();

        for (int i = 0; i < CountX * CountY; ++i)
        {
            byte x = (byte)(i / CountX);
            byte y = (byte)(i % CountX);
            SpawnTile(x, y, DefaultTileType);
        }
    }

    public void ClearTiles()
    {
        var e = mTileDic.GetEnumerator();
        while (e.MoveNext())
            DestroyImmediate(e.Current.Value.GO);
        mTileDic.Clear();

        var tiles = GetComponentsInChildren<H5TileBase>();
        for (int i = 0; i < tiles.Length; ++i)
        {
            DestroyImmediate(tiles[i].GO);
        }
    }

    private H5TileBase SpawnTile(byte x, byte y, TILE_TYPE type)
    {
        var tileObjPrefab = Resources.Load("Prefab/Tile") as GameObject;
        var tileObj = GameObject.Instantiate(tileObjPrefab);

        if (tileObj == null)
            return null;

        var coord = LogicHelper.GetCoordinateFromXY(x, y);
        var h5Tile = tileObj.AddComponent<H5TileBase>();
        h5Tile.TM.SetParent(TM);
        h5Tile.InitTile(type, coord);
        h5Tile.PlaceOnWorld(H5TileBase.TileSize * x, H5TileBase.TileSize * y);

        mTileDic.Add(coord, h5Tile);

        return h5Tile;
    }

    public override void InitObject()
    {
    }
}

[CustomEditor(typeof(TileRootEditor))]
public class TileRootEditorObject : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        TileRootEditor editor = target as TileRootEditor;

        if (GUILayout.Button("Create Tiles"))
            editor.CreateTiles();

        if (GUILayout.Button("Clear Tiles"))
            editor.ClearTiles();

        //GUILayout.BeginHorizontal();
        //GUILayout.EndHorizontal();
        //EditorGUILayout.LabelField(labelValue);
        //floatValue = EditorGUILayout.FloatField(floatValue);
        //enumValue = (EnumType)EditorGUILayout.EnumPopup(Name, enumValue);
        //intValue = EditorGUILayout.IntField(intValue);
        //slideValue = EditorGUILayout.Slider(slideValue, slideLeft, slideRight);
        //if (GUILayout.Button(ButtonName)) Function();
    }
}