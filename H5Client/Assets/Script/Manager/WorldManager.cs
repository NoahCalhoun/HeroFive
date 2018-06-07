using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

public enum OBJECT_TYPE
{
    OBJECT_TEST,
    OBJECT_TILE
}

public class WorldManager : MonoBehaviour
{
    private static WorldManager mInstance;
    public static WorldManager Instance { get { if (mInstance == null) mInstance = GameObject.FindGameObjectWithTag("World").GetComponent<WorldManager>(); return mInstance; } }

    private Transform WorldRoot;
    public Transform TileRoot;
    public Transform CharacterRoot;

    private Dictionary<ushort, H5TileBase> TileDic = new Dictionary<ushort, H5TileBase>();

    private float CameraFloorDist = 10f;
    private float CameraSkyDist = 8f;

    private List<H5TileBase> Path;
    
    private bool IsMousePicked;

    H5CharacterBase test;

    // Use this for initialization
    void Start()
    {
        WorldRoot = GameObject.FindGameObjectWithTag("World").transform;

        StartCoroutine(TestInit());
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

    public H5CharacterBase SpawnCharacter(byte x, byte y, CharacterType type)
    {
        var characterObjPrefab = Resources.Load("Prefab/Character") as GameObject;
        var characterObj = GameObject.Instantiate(characterObjPrefab);

        if (characterObj == null)
            return null;

        var coord = LogicHelper.GetCoordinateFromXY(x, y);
        var h5Character = characterObj.AddComponent<H5CharacterBase>();
        h5Character.TM.SetParent(CharacterRoot);
        h5Character.InitCharacter(type);

        var spawnTile = TileDic[LogicHelper.GetCoordinateFromXY(x, y)];
        if (spawnTile != null)
        {
            h5Character.TM.position = spawnTile.TM.position;
        }
        else
        {
            h5Character.TM.position = new Vector3(0, 0, 0);
        }

        return h5Character;
    }

    public void FocusCameraOnTile(byte x, byte y, bool setAngle = false)
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
        var camPos = lookAt - lookVec;
        Camera.main.transform.position = camPos;

        if (setAngle)
        {
            var lookDir = lookVec.normalized;
            var rightDir = Vector3.Cross(lookDir, Vector3.up).normalized;
            var upDir = Vector3.Cross(rightDir, lookDir).normalized;

            Matrix4x4 camWorld = new Matrix4x4(
                new Vector4(rightDir.x, rightDir.y, rightDir.z, 0),
                new Vector4(upDir.x, upDir.y, upDir.z, 0),
                new Vector4(lookDir.x, lookDir.y, lookDir.z, 0),
                new Vector4(0, 0, 0, 1));

            Camera.main.transform.localRotation = camWorld.rotation;
        }
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
            if (Path != null)
                Path.Clear();
            
            RaycastHit hit;
            if (LogicHelper.MousePickingOnWorld(out hit, 1 << 10) && hit.collider != null)
            {
                var e = TileDic.GetEnumerator();
                while (e.MoveNext())
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
                            while (eb.MoveNext())
                            {
                                if (TileDic.ContainsKey(eb.Current))
                                    TileDic[eb.Current].SetPicked(true);
                            }
                            SetBoundEdge(bound);
                        }

                        Path = MoveManager.Instance.FindPath(TileDic[(1 << 8) | 1], e.Current.Value);

                        test.MoveTo(bx, by);
                        break;
                    }
                }
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            IsMousePicked = true;
        }
        else if (Input.GetMouseButtonUp(1))
        {
            IsMousePicked = false;
        }

        if (IsMousePicked)
        {
            var camUp = Camera.main.transform.up;
            var camRight = Camera.main.transform.right;
            var camForward = Camera.main.transform.forward;
            var movedCamPos = Camera.main.transform.position - camUp * Input.GetAxis("Mouse Y") * 0.5f - camRight * Input.GetAxis("Mouse X") * 0.5f;
            var rayStart = movedCamPos - camForward * 100f;
            Plane camField = new Plane(Vector3.up, -CameraSkyDist);
            float dist;
            if (camField.Raycast(new Ray(rayStart, camForward), out dist))
            {
                Camera.main.transform.position = rayStart + camForward * dist;
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

    IEnumerator LoadMapScene(string name)
    {
        ClearTileDic();

        var load = SceneManager.LoadSceneAsync(name, LoadSceneMode.Additive);
        while (load.isDone == false) { yield return null; }

        var tiles = GameObject.FindGameObjectWithTag("TileRoot").GetComponent<TileRootEditor>().GetComponentsInChildren<H5TileBase>();
        for (int i = 0; i < tiles.Length; ++i)
        {
            var tile = tiles[i];
            tile.Refresh();
            TileDic.Add(LogicHelper.GetCoordinateFromXY(tile.m_Coordinate.x, tile.m_Coordinate.y), tile);
        }

        yield break;
    }

    IEnumerator TestInit()
    {
        yield return StartCoroutine(LoadMapScene("TestMap"));

        SetTilesNeighbor();

        FocusCameraOnTile(5, 5, true);

        test = SpawnCharacter(1, 1, CharacterType.Monarch);
        SpawnCharacter(0, 3, CharacterType.Tanker);
        SpawnCharacter(3, 0, CharacterType.Dealer);
        SpawnCharacter(3, 2, CharacterType.Positioner);
        SpawnCharacter(2, 3, CharacterType.Supporter);
        SpawnCharacter(6, 7, CharacterType.Monster);

        yield break;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (Path != null && Path.Count > 0)
        {
            Gizmos.DrawLine(TileDic[(1 << 8) | 1].TM.position, Path[0].TM.position);
            for (int i = 0; i < Path.Count - 1; ++i)
            {
                Gizmos.DrawLine(Path[i].TM.position, Path[i + 1].TM.position);
            }
        }
    }

    void ClearTileDic()
    {
        var e = TileDic.GetEnumerator();
        while(e.MoveNext())
        {
            DestroyImmediate(e.Current.Value.GO);
        }
        TileDic.Clear();
    }

    public H5TileBase GetTile(byte _x, byte _y)
    {
        return TileDic[LogicHelper.GetCoordinateFromXY(_x, _y)];
    }

    public H5TileBase GetTile(ushort _xy)
    {
        return TileDic[_xy];
    }

    public H5TileBase GetTile(Transform _tm)
    {
        return TileDic[LogicHelper.GetCoordinateFromXY((byte)_tm.position.x, (byte)_tm.position.z)];
    }
}
