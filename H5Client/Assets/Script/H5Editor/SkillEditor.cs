using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

public class Boundary
{
    public H5Direction Direction = H5Direction.Up;
    public HashSet<RCoordinate> RelativeBound;

    public HashSet<ACoordinate> GetAbsoluteBound(ACoordinate pos, H5Direction dir, bool validCheck = true)
    {
        if (RelativeBound == null || RelativeBound.Count <= 0)
            return null;

        var returnSet = new HashSet<ACoordinate>();
        var e = RelativeBound.GetEnumerator();
        while (e.MoveNext())
        {
            var aPos = pos + e.Current.Rotate(Direction.Relation(dir));
            if (validCheck == false || aPos.IsValid)
                returnSet.Add(aPos);
        }

        return returnSet;
    }
}

public class SkillBaseData
{
    public float SkillTime;
    public List<KeyValuePair<float, AttackBaseData>> AttackList;
    public Boundary SkillBoundary;
}

public class HitBaseData
{
    //only : 유일값
    //flag : 중복 가능 값

    //데미지 관련
    //1. (only)물리적인 데미지 값
    //2. (flag)나중에 타입? 속성? 같은게 들어갈 것 같은데...
    //3. (flag)상태이상, 도 당연히 들어가겠지 뭐 (확률이 필요할런지는 미지수)

    //이동 관련
    //1. (only)밀기 or 당기기 or 날리기 : 4bit
    //2. (only)방향 (8방향 + 랜덤 + 안밀림) : 16bit
    //3. (only)몇 칸? : 8bit
}

public class AttackBaseData
{
    public Dictionary<RCoordinate, HitBaseData> Attack;

    public Boundary GetBoundary { get { return new Boundary() { RelativeBound = new HashSet<RCoordinate>(Attack.Keys) }; } }
}

public enum SkillEditType
{
    Attack,
    Skill,
    Demo
}

public enum HitType_Tool
{
    Push,
    Pull,
    Fly
}

public enum HitDirection_Tool
{
    None,
    Up,
    Down,
    Left,
    Right,
    UpLeft,
    UpRight,
    DownLeft,
    DownRight,
    Random,
}

public class SkillEditor : MonoBehaviour
{
    //public
    public SkillEditType EditType;
    public Dictionary<ushort, H5TileBase> TileDic = new Dictionary<ushort, H5TileBase>();

    //Hit
    public class HitData_Tool
    {
        public HitType_Tool HitType;
        public HitDirection_Tool HitDirection;
        public byte HitValue;

        public HitData_Tool Clone { get { return new HitData_Tool() { HitDirection = HitDirection, HitType = HitType, HitValue = HitValue }; } }
    }
    public HitData_Tool HitData = new HitData_Tool();

    //Attack
    public class AttackData_Tool
    {
        public Dictionary<ushort, HitData_Tool> Attack = new Dictionary<ushort, HitData_Tool>(); //Key : ACoord
    }
    public AttackData_Tool AttackData;

    //Skill

    void Start()
    {
        var tiles = GameObject.FindGameObjectWithTag("TileRoot").GetComponent<TileRootEditor>().GetComponentsInChildren<H5TileBase>();
        for (int i = 0; i < tiles.Length; ++i)
        {
            var tile = tiles[i];
            tile.Refresh();
            TileDic.Add(LogicHelper.GetCoordinateFromXY(tile.m_Coordinate.x, tile.m_Coordinate.y), tile);
        }

        WorldManager.SetTilesNeighbor(TileDic);

        var centerTile = TileDic[new ACoordinate(5, 5).xy];
        WorldManager.FocusCameraOnTile(centerTile, true);

        WorldManager.SpawnCharacter(transform, centerTile, 1).Direction = H5Direction.Up;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            OnMousePicking(true);
        else if (Input.GetMouseButtonDown(1))
            OnMousePicking(false);
    }

    void OnMousePicking(bool left)
    {
        bool right = !left;
        RaycastHit hit;
        if (LogicHelper.MousePickingOnWorld(out hit, 1 << 10, false) && hit.collider != null)
        {
            var tile = hit.collider.gameObject.GetComponent<H5TileBase>();
            if (tile == null)
                return;

            switch (EditType)
            {
                case SkillEditType.Attack:
                    {
                        if (AttackData == null)
                            AttackData = new AttackData_Tool();

                        bool controlled = false;

                        if (left && AttackData.Attack.ContainsKey(tile.m_Coordinate.xy) == false)
                        {
                            AttackData.Attack.Add(tile.m_Coordinate.xy, HitData.Clone);
                            controlled = true;
                        }
                        else if (right && AttackData.Attack.ContainsKey(tile.m_Coordinate.xy) == true)
                        {
                            AttackData.Attack.Remove(tile.m_Coordinate.xy);
                            controlled = true;
                        }

                        if (controlled)
                        {
                            var et = TileDic.GetEnumerator();
                            while (et.MoveNext())
                            {
                                et.Current.Value.ClearFlag();
                            }

                            var e = AttackData.Attack.GetEnumerator();
                            while (e.MoveNext())
                            {
                                TileDic[e.Current.Key].SetPicked(true);
                            }
                            WorldManager.SetBoundEdge(TileDic, new HashSet<ushort>(AttackData.Attack.Keys));
                        }
                        break;
                    }

                case SkillEditType.Skill:
                    {
                        break;
                    }

                case SkillEditType.Demo:
                    break;
            }

            //var bound = tile.GetBound(2);
            //if (bound != null && bound.Count > 0)
            //{
            //    var eb = bound.GetEnumerator();
            //    while (eb.MoveNext())
            //    {
            //        if (TileDic.ContainsKey(eb.Current))
            //            TileDic[eb.Current].SetPicked(true);
            //    }
            //    WorldManager.SetBoundEdge(TileDic, bound);
            //}
        }
    }
}

[CustomEditor(typeof(SkillEditor))]
public class SkillEditorObject : Editor
{
    public override void OnInspectorGUI()
    {
        var editor = target as SkillEditor;

        editor.EditType = (SkillEditType)EditorGUILayout.EnumPopup("EditType", editor.EditType);

        switch (editor.EditType)
        {
            case SkillEditType.Attack:
                {
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("Save Attack"))
                    {

                    }
                    if (GUILayout.Button("Load Attack"))
                    {

                    }
                    GUILayout.EndHorizontal();

                    editor.HitData.HitType = (HitType_Tool)EditorGUILayout.EnumPopup("Hit Type", editor.HitData.HitType);
                    editor.HitData.HitDirection = (HitDirection_Tool)EditorGUILayout.EnumPopup("Hit Direction", editor.HitData.HitDirection);
                    int value = EditorGUILayout.IntField("Hit Value", editor.HitData.HitValue);
                    editor.HitData.HitValue = (byte)Mathf.Clamp(value, byte.MinValue, byte.MaxValue);
                    break;
                }

            case SkillEditType.Skill:
                {
                    break;
                }

            case SkillEditType.Demo:
                {
                    break;
                }
        }
    }
}
