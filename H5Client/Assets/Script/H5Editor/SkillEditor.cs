using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;

using UnityEngine;
using UnityEditor;
using System;

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
    public static readonly string NoNameStr = "NoName";

    //public
    public SkillEditType EditType;
    public Dictionary<ushort, H5TileBase> TileDic = new Dictionary<ushort, H5TileBase>();
    public Dictionary<ushort, ToolAttackTag> TagDic = new Dictionary<ushort, ToolAttackTag>();

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
        public string Name = NoNameStr;
        public Dictionary<ushort, HitData_Tool> Attack = new Dictionary<ushort, HitData_Tool>(); //Key : ACoord
    }
    public AttackData_Tool AttackData = new AttackData_Tool();

    //Skill

    void Start()
    {
        var tiles = GameObject.FindGameObjectWithTag("TileRoot").GetComponent<TileRootEditor>().GetComponentsInChildren<H5TileBase>();
        var attackTagPrefab = Resources.Load("Prefab/Widget/AttackTag") as GameObject;
        for (int i = 0; i < tiles.Length; ++i)
        {
            var tile = tiles[i];
            tile.Refresh();

            var attackTagObj = Instantiate(attackTagPrefab);
            var attackTag = attackTagObj.GetComponent<ToolAttackTag>();
            attackTag.InitTag(tile);

            TileDic.Add(LogicHelper.GetCoordinateFromXY(tile.m_Coordinate.x, tile.m_Coordinate.y), tile);
            TagDic.Add(LogicHelper.GetCoordinateFromXY(tile.m_Coordinate.x, tile.m_Coordinate.y), attackTag);
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
                            RefreshTile();
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

    public void RefreshTile()
    {
        var etile = TileDic.GetEnumerator();
        while (etile.MoveNext())
        {
            etile.Current.Value.ClearFlag();
        }

        var etag = TagDic.GetEnumerator();
        while (etag.MoveNext())
        {
            etag.Current.Value.ClearTag();
        }

        var e = AttackData.Attack.GetEnumerator();
        while (e.MoveNext())
        {
            TileDic[e.Current.Key].SetPicked(true);
            TagDic[e.Current.Key].RefreshTag(AttackData.Attack[e.Current.Key]);
        }
        WorldManager.SetBoundEdge(TileDic, new HashSet<ushort>(AttackData.Attack.Keys));
    }
}

[CustomEditor(typeof(SkillEditor))]
public class SkillEditorObject : Editor
{
    static readonly string AttackDataPath = "Assets/Resources/SkillToolData/AttackData";
    static readonly string SkillDataPath = "Assets/Resources/SkillToolData/SkillData";

    public override void OnInspectorGUI()
    {
        var editor = target as SkillEditor;

        editor.EditType = (SkillEditType)EditorGUILayout.EnumPopup("EditType", editor.EditType);

        switch (editor.EditType)
        {
            case SkillEditType.Attack:
                {
                    GUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Save Attack", GUILayout.Width(100));
                    editor.AttackData.Name = EditorGUILayout.TextField(editor.AttackData.Name, GUILayout.Width(150));
                    if (GUILayout.Button("Save"))
                    {
                        SaveAttack(editor);
                    }
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Load Attack", GUILayout.Width(100));
                    var atkFiles = Directory.GetFiles(AttackDataPath, "*.csv");
                    var dropDown = new string[atkFiles.Length + 1];
                    for (int i = 0; i < dropDown.Length; ++i)
                    {
                        dropDown[i] = i == 0 ? SkillEditor.NoNameStr : atkFiles[i - 1].Substring(AttackDataPath.Length + 1).Replace(".csv", "");
                    }
                    var idx = EditorGUILayout.Popup(0, dropDown);
                    if (idx > 0)
                        LoadAttack(editor, dropDown[idx]);
                    //if (GUILayout.Button("Load Attack"))
                    //{
                    //    LoadAttack(editor);
                    //}
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

    public void SaveAttack(SkillEditor editor)
    {
        if (editor.AttackData.Name == SkillEditor.NoNameStr || editor.AttackData.Name == "")
        {
            EditorUtility.DisplayDialog("NoName!", "NoName Cannot be a name of Attack.", "OK");
            return;
        }

        var dest = Directory.GetFiles(AttackDataPath, string.Format("{0}.csv", editor.AttackData.Name));
        if (dest.Length > 0)
        {
            if (EditorUtility.DisplayDialog("Warning!", string.Format("{0} is already exist. Do you want to Override?", editor.AttackData.Name), "OK", "Cancel"))
            {
                File.Delete(dest[0]);
            }
            else
            {
                return;
            }
        }

        using (StreamWriter sw = new StreamWriter(string.Format("{0}/{1}.csv", AttackDataPath, editor.AttackData.Name), false))
        {
            // 상대좌표, HitType, HitDirection, HitValue
            var e = editor.AttackData.Attack.GetEnumerator();
            while (e.MoveNext())
            {
                //상대좌표
                var acoord = new ACoordinate(e.Current.Key);
                var rcoord = acoord - new ACoordinate(5, 5);
                sw.Write(string.Format("{0:X}", rcoord.xy));
                sw.Write(',');

                //Hit Data
                sw.Write(string.Format("{0:X}", e.Current.Value.HitType));
                sw.Write(',');
                sw.Write(string.Format("{0:X}", e.Current.Value.HitDirection));
                sw.Write(',');
                sw.Write(string.Format("{0:X}", e.Current.Value.HitValue));
                sw.Write('\n');
            }
        }
    }

    public void LoadAttack(SkillEditor editor, string atkStr)
    {
        if (atkStr == SkillEditor.NoNameStr)
            return;

        var dest = Directory.GetFiles(AttackDataPath, string.Format("{0}.csv", atkStr));
        if (dest.Length <= 0)
        {
            EditorUtility.DisplayDialog("No Data!", string.Format("There is no Attack Data named {0}.", atkStr), "OK");
            return;
        }

        editor.AttackData = new SkillEditor.AttackData_Tool();
        editor.AttackData.Name = atkStr;

        using (FileStream fs = new FileStream(dest[0], FileMode.Open))
        {
            using (StreamReader sr = new StreamReader(fs, Encoding.UTF8, false))
            {
                string strLineValue = null;

                while ((strLineValue = sr.ReadLine()) != null)
                {
                    // 상대좌표, HitType, HitDirection, HitValue
                    var line = strLineValue.Split(',');

                    var rcoord = new RCoordinate(Convert.ToInt32(line[0], 16));
                    var acoord = new ACoordinate(5, 5) + rcoord;

                    var hitType = (HitType_Tool)(Convert.ToInt32(line[1], 16));
                    var hitDirection = (HitDirection_Tool)(Convert.ToInt32(line[2], 16));
                    var hitValue = Convert.ToByte(line[3], 16);

                    editor.AttackData.Attack.Add(acoord.xy, new SkillEditor.HitData_Tool() { HitType = hitType, HitDirection = hitDirection, HitValue = hitValue });
                }
            }
        }

        editor.RefreshTile();
    }
}
