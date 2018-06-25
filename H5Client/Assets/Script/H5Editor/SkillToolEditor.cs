using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

public class Boundary
{
    public HashSet<RCoordinate> RelativeBound;

    public HashSet<ACoordinate> GetAbsoluteBound(ACoordinate pos)
    {
        if (RelativeBound == null || RelativeBound.Count <= 0)
            return null;

        var returnSet = new HashSet<ACoordinate>();
        var e = RelativeBound.GetEnumerator();
        while(e.MoveNext())
        {
            var aPos = pos + e.Current;
            if (aPos.IsValid)
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

public class AttackFlag
{
    //only : 유일값
    //flag : 중복 가능 값

    //데미지 관련
    //1. (only)물리적인 데미지 값
    //2. (flag)나중에 타입? 속성? 같은게 들어갈 것 같은데...
    //3. (flag)상태이상, 도 당연히 들어가겠지 뭐 (확률이 필요할런지는 미지수)

    //이동 관련
    //1. (only)밀기 or 당기기
    //2. (only)방향 (8방향 + 랜덤 + 안밀림)
    //3. (only)몇 칸?
}

public class AttackBaseData
{
    public Dictionary<RCoordinate, AttackFlag> Attack;

    public Boundary GetBoundary { get { return new Boundary() { RelativeBound = new HashSet<RCoordinate>(Attack.Keys) }; } }
}

public class SkillTool : MonoBehaviour
{

}

[CustomEditor(typeof(SkillTool))]
public class SkillToolEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var editor = target as SkillTool;
    }
}
