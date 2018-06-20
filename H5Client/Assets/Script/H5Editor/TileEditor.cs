using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(H5TileBase))]
public class TileEditor : Editor
{
    TILE_TYPE CurrentTileType = TILE_TYPE.TILE_TYPE_NONE;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        H5TileBase tile = target as H5TileBase;

        if (CurrentTileType == TILE_TYPE.TILE_TYPE_NONE)
        {
            CurrentTileType = tile.m_TileType;
            tile.Refresh();
        }

        if (CurrentTileType != tile.m_TileType)
        {
            CurrentTileType = tile.m_TileType;
            tile.InitTile(CurrentTileType, H5TileBase.InvalidCoordinate);
        }

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
