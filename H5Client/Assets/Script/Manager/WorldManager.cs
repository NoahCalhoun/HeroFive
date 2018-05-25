using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public enum OBJECT_TYPE
{
    OBJECT_TEST,
    OBJECT_TILE
}

public class WorldManager : MonoBehaviour
{
    private Transform WorldRoot;
    public Transform TileRoot;

    private Dictionary<ushort, H5TileBase> TileDic = new Dictionary<ushort, H5TileBase>();

    private float CameraFloorDist = 10f;
    private float CameraSkyDist = 8f;

    // Use this for initialization
    void Start()
    {
        WorldRoot = GameObject.FindGameObjectWithTag("World").transform;

        for (int i = 0; i < 100; ++i)
        {
            SpawnTile((byte)(i / 10), (byte)(i % 10), i > 73 ? TILE_TYPE.TILE_TYPE_WATER : TILE_TYPE.TILE_TYPE_NORMAL);
        }

        FocusCameraOnTile(5, 5);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public H5ObjectBase SpawnTestObject(float x, float z, string objwhere, OBJECT_TYPE objtype)
    {
        var testObjectPrefab = Resources.Load(objwhere) as GameObject;
        var testObject = GameObject.Instantiate(testObjectPrefab);

        if (testObject == null)
            return null;

        H5ObjectBase h5Test = null;

        switch (objtype)
        {
            case OBJECT_TYPE.OBJECT_TEST:
                h5Test = testObject.AddComponent<H5TestObject>();
                break;

            case OBJECT_TYPE.OBJECT_TILE:
                h5Test = testObject.AddComponent<H5TileBase>();
                break;
        }

        if (h5Test == null)
            return null;

        h5Test.TM.SetParent(WorldRoot);
        h5Test.InitObject();
        h5Test.PlaceOnWorld(x, z);

        return h5Test;
    }

    public H5TileBase SpawnTile(byte x, byte y, TILE_TYPE type)
    {
        var tileObjPrefab = Resources.Load("Prefab/Tile") as GameObject;
        var tileObj = GameObject.Instantiate(tileObjPrefab);

        if (tileObj == null)
            return null;

        var h5Tile = tileObj.AddComponent<H5TileBase>();
        h5Tile.TM.SetParent(TileRoot);
        h5Tile.InitTile(type);
        h5Tile.PlaceOnWorld(H5TileBase.TileSize * x, H5TileBase.TileSize * y);

        TileDic.Add(LogicHelper.GetCoordinateFromXY(x, y), h5Tile);

        return h5Tile;
    }

    public void FocusCameraOnTile(byte x, byte y)
    {
        var coordinate = LogicHelper.GetCoordinateFromXY(x, y);
        Vector3 lookAt;
        if (TileDic.ContainsKey(coordinate))
        {
            lookAt = TileDic[coordinate].TM.position;
        }
        else
        {
            lookAt = new Vector3(H5TileBase.TileSize * x, 0, H5TileBase.TileSize * y);
        }

        var camPos = lookAt + new Vector3(CameraFloorDist * 0.7072f, CameraSkyDist, CameraFloorDist * 0.7072f);
        var lookDir = (lookAt - camPos).normalized;
        var rightDir = Vector3.Cross(lookDir, Vector3.up).normalized;
        var upDir = Vector3.Cross(rightDir, lookDir).normalized;

        Matrix4x4 camWorld = new Matrix4x4(
            new Vector4(rightDir.x, rightDir.y, rightDir.z, 0),
            new Vector4(upDir.x, upDir.y, upDir.z, 0),
            new Vector4(lookDir.x, lookDir.y, lookDir.z, 0),
            new Vector4(0, 0, 0, 1));

        Camera.main.transform.position = camPos;
        Camera.main.transform.localRotation = camWorld.rotation;
    }
}
