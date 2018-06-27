using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

public enum OBJECT_TYPE
{
    OBJECT_TEST,
    OBJECT_TILE
}

public enum H5Direction
{
    Up,     //+Y
    Down,   //-Y
    Left,   //-X
    Right,  //+X
    Max
}

public class WorldManager : MonoBehaviour
{
    private static WorldManager mInstance;
    public static WorldManager Instance { get { if (mInstance == null) mInstance = GameObject.FindGameObjectWithTag("World").GetComponent<WorldManager>(); return mInstance; } }

    private Transform WorldRoot;
    public Transform TileRoot;
    public Transform CharacterRoot;
    public Transform CameraRoot;

    private Dictionary<ushort, H5TileBase> TileDic = new Dictionary<ushort, H5TileBase>();

    private static float CameraFloorDist = 10f;
    private static float CameraSkyDist = 8f;

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

    public H5ObjectBase SpawnTestObject(float x, float z)
    {
        var testObjectPrefab = Resources.Load("Prefab/TestObject") as GameObject;
        var testObject = GameObject.Instantiate(testObjectPrefab);

        if (testObject == null)
            return null;
        
        var h5Test = testObject.AddComponent<H5TestObject>();
        if (h5Test == null)
            return null;

        h5Test.TM.SetParent(WorldRoot);
        h5Test.InitObject();
        h5Test.PlaceOnTilePosition(x, z);

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

    H5CharacterBase SpawnCharacter(byte x, byte y, int tableId)
    {
        var spawnTile = TileDic[LogicHelper.GetCoordinateFromXY(x, y)];
        if (spawnTile == null || spawnTile.IsEmpty == false)
            return null;

        return SpawnCharacter(CharacterRoot, spawnTile, tableId);
    }

    public static H5CharacterBase SpawnCharacter(Transform root, H5TileBase tile, int tableId)
    {
        var characterObjPrefab = Resources.Load("Prefab/Character") as GameObject;
        var characterObj = GameObject.Instantiate(characterObjPrefab);

        if (characterObj == null)
            return null;
        
        var h5Character = characterObj.AddComponent<H5CharacterBase>();
        h5Character.TM.SetParent(root);
        h5Character.InitCharacter(tableId);

        tile.OnTile(h5Character);
        h5Character.OwnTile = tile;
        h5Character.TM.position = tile.TM.position;

        return h5Character;
    }

    public static void FocusCameraOnTile(H5TileBase tile, bool setAngle = false)
    {
        Vector3 lookAt = tile.TM.position;

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

    void FocusCameraOnTile(byte x, byte y, bool setAngle = false)
    {
        var coordinate = LogicHelper.GetCoordinateFromXY(x, y);
        if (TileDic.ContainsKey(coordinate) == false)
            return;

        FocusCameraOnTile(TileDic[coordinate], setAngle);
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
            if (LogicHelper.MousePickingOnWorld(out hit, 1 << 11) && hit.collider != null)
            {
                var character = hit.collider.gameObject.GetComponentInParent<H5CharacterBase>();

                if (character != null)
                {
                    if (UIManager.Instance.IsWindowOpened(UIWindowType.TestUI) == false)
                    {
                        var data = new H5TestUI.H5TestUIData() { CharacterType = character.Type };
                        UIManager.Instance.OpenWindow(UIWindowType.TestUI, data);
                    }
                    else
                    {
                        UIManager.Instance.CloseWindow(UIWindowType.TestUI);
                    }
                }
            }
            else if (LogicHelper.MousePickingOnWorld(out hit, 1 << 10) && hit.collider != null)
            {
                var e = TileDic.GetEnumerator();
                while (e.MoveNext())
                {
                    if (e.Current.Value.GO == hit.collider.gameObject)
                    {
                        byte bx = LogicHelper.GetXFromCoordinate(e.Current.Key);
                        byte by = LogicHelper.GetYFromCoordinate(e.Current.Key);
                        FocusCameraOnTile(bx, by);

                        // 요기 오또케 처리해야하나욘
                        if (e.Current.Value.m_TileType != TILE_TYPE.TILE_TYPE_NORMAL)
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

                        test.Move(bx, by);
                        break;
                    }
                }
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            IsMousePicked = true;

            H5TileBase tile = GetTile_MousePick();
            if (tile != null)
            {
                SkillData knockBack = new SkillData();
                knockBack.ForceType = ForceType.KnockBack;
                knockBack.ForceDir = (H5Direction)Random.Range(0, 4);
                knockBack.Force = 5;

                test.SkillSystem.DoSkillActive(knockBack, tile.m_Coordinate);
            }
            //int i = Random.Range(0, 4);
            //test.KnockBackTo((H5Direction)i, 5);
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

    public static void SetTilesNeighbor(Dictionary<ushort, H5TileBase> tileDic)
    {
        var e = tileDic.GetEnumerator();
        while (e.MoveNext())
        {
            var tile = e.Current.Value;
            var coord = e.Current.Key;
            var curx = LogicHelper.GetXFromCoordinate(coord);
            var cury = LogicHelper.GetYFromCoordinate(coord);

            for (H5Direction i = 0; i < H5Direction.Max; ++i)
            {
                var neighborx = curx;
                var neighbory = cury;

                switch (i)
                {
                    case H5Direction.Up:
                        neighbory += 1;
                        break;
                    case H5Direction.Down:
                        neighbory -= 1;
                        break;
                    case H5Direction.Left:
                        neighborx -= 1;
                        break;
                    case H5Direction.Right:
                        neighborx += 1;
                        break;
                    default:
                        continue;
                }

                var neighborkey = LogicHelper.GetCoordinateFromXY(neighborx, neighbory);
                tile.SetNeighbor(i, tileDic.ContainsKey(neighborkey) ? tileDic[neighborkey] : null);
            }
        }
    }

    void SetTilesNeighbor()
    {
        SetTilesNeighbor(TileDic);
    }

    void SetBoundEdge(HashSet<ushort> bound)
    {
        if (bound == null || bound.Count <= 0)
            return;

        var e = bound.GetEnumerator();
        while (e.MoveNext())
        {
            var curCoord = e.Current;
            for (H5Direction i = 0; i < H5Direction.Max; ++i)
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

        test = SpawnCharacter(1, 1, 1);
        SpawnCharacter(0, 3, 1);
        SpawnCharacter(3, 0, 1);
        SpawnCharacter(3, 2, 1);
        SpawnCharacter(2, 3, 1);
        SpawnCharacter(6, 7, 1);

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
        while (e.MoveNext())
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

    public H5TileBase GetTile_MousePick()
    {
        RaycastHit hit;
        if (LogicHelper.MousePickingOnWorld(out hit, 1 << 10) && hit.collider != null)
        {
            var e = TileDic.GetEnumerator();
            while (e.MoveNext())
            {
                if (e.Current.Value.GO == hit.collider.gameObject)
                    return e.Current.Value;

            }
        }

        return null;
    }
}
