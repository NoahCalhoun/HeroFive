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

        SetTilesNeighbor();

        FocusCameraOnTile(5, 5);
    }

    // Update is called once per frame
    void Update()
    {
        OnMouseClickEvent();
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

        var coord = LogicHelper.GetCoordinateFromXY(x, y);
        var h5Tile = tileObj.AddComponent<H5TileBase>();
        h5Tile.TM.SetParent(TileRoot);
        h5Tile.InitTile(type, coord);
        h5Tile.PlaceOnWorld(H5TileBase.TileSize * x, H5TileBase.TileSize * y);

        TileDic.Add(coord, h5Tile);

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

        var lookVec = new Vector3(CameraFloorDist * 0.7072f, CameraSkyDist, CameraFloorDist * 0.7072f) * -1f;
        var lookDir = lookVec.normalized;
        var rightDir = Vector3.Cross(lookDir, Vector3.up).normalized;
        var upDir = Vector3.Cross(rightDir, lookDir).normalized;
        var camPos = lookAt - lookVec;

        Matrix4x4 camWorld = new Matrix4x4(
            new Vector4(rightDir.x, rightDir.y, rightDir.z, 0),
            new Vector4(upDir.x, upDir.y, upDir.z, 0),
            new Vector4(lookDir.x, lookDir.y, lookDir.z, 0),
            new Vector4(0, 0, 0, 1));

        Camera.main.transform.position = camPos;
        Camera.main.transform.localRotation = camWorld.rotation;
    }

    void OnMouseClickEvent()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var ef = TileDic.GetEnumerator();
            while (ef.MoveNext())
            {
                ef.Current.Value.ClearFlag();
            }

            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            Physics.Raycast(ray, out hit, float.PositiveInfinity, -1);
            if (hit.collider != null)
            {
                var e = TileDic.GetEnumerator();
                while(e.MoveNext())
                {
                    if (e.Current.Value.GO == hit.collider.gameObject)
                    {
                        byte bx = LogicHelper.GetXFromCoordinate(e.Current.Key);
                        byte by = LogicHelper.GetYFromCoordinate(e.Current.Key);
                        FocusCameraOnTile(bx, by);

                        if (e.Current.Value.IsWalkable == false)
                            return;

                        var bound = e.Current.Value.GetBound(2);
                        if (bound != null && bound.Count > 0)
                        {
                            var eb = bound.GetEnumerator();
                            while(eb.MoveNext())
                            {
                                if (TileDic.ContainsKey(eb.Current))
                                    TileDic[eb.Current].SetPicked(true);
                            }
                            SetBoundEdge(bound);
                        }
                        return;
                    }
                }
            }
        }
    }

    void SetTilesNeighbor()
    {
        var e = TileDic.GetEnumerator();
        while (e.MoveNext())
        {
            var tile = e.Current.Value;
            var coord = e.Current.Key;
            var curx = LogicHelper.GetXFromCoordinate(coord);
            var cury = LogicHelper.GetYFromCoordinate(coord);

            for (TILE_NEIGHBOR i = 0; i < TILE_NEIGHBOR.Max; ++i)
            {
                var neighborx = curx;
                var neighbory = cury;

                switch(i)
                {
                    case TILE_NEIGHBOR.Up:
                        neighbory += 1;
                        break;
                    case TILE_NEIGHBOR.Down:
                        neighbory -= 1;
                        break;
                    case TILE_NEIGHBOR.Left:
                        neighborx -= 1;
                        break;
                    case TILE_NEIGHBOR.Right:
                        neighborx += 1;
                        break;
                    default:
                        continue;
                }

                var neighborkey = LogicHelper.GetCoordinateFromXY(neighborx, neighbory);
                tile.SetNeighbor(i, TileDic.ContainsKey(neighborkey) ? TileDic[neighborkey] : null);
            }
        }
    }

    void SetBoundEdge(HashSet<ushort> bound)
    {
        if (bound == null || bound.Count <= 0)
            return;

        var e = bound.GetEnumerator();
        while(e.MoveNext())
        {
            var curCoord = e.Current;
            for (TILE_NEIGHBOR i = 0; i < TILE_NEIGHBOR.Max; ++i)
            {
                var checkCoord = e.Current.GetNeighborCoordinate(i);
                if (bound.Contains(checkCoord) == false)
                    TileDic[e.Current].SetFlag(i);
            }
        }
    }
}
