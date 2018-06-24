using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ForceType
{
    None = -1,
    KnockBack,
    Smack,
}

public class SkillData
{
    // Status 관련
    public int Damage;

    // 이동 관련
    public ForceType ForceType;
    public H5Direction ForceDir;
    public byte Force;

    // 스킬 범위 (추가해야함
}

public class SkillSystem : H5SystemBase
{
    public bool DoSkillActive(SkillData skillData, Coordinate targetPos)
    {
        // 임시로 범위는 1로 가정
        // 1. targetPos 기준에서 스킬의 범위 안에 있는 모든 타일을 가져온다.
        H5TileBase tile = WorldManager.Instance.GetTile(targetPos.xy);
        // 2. 타일 위에 있는 오브젝트를 가져온다
        H5CharacterBase obj = (H5CharacterBase)tile.ObjectOnTile;
        if (obj == null) return false;
        // 3. 해당 오브젝트를 움직인다.
        switch(skillData.ForceType)
        {
            case ForceType.KnockBack: // 밀치기
                obj.KnockBackTo(skillData.ForceDir, skillData.Force);
                break;
            case ForceType.Smack: // 날리기
                break;
            default:
                break;
        }

        return true;
    }

    public static MovementSystem.MoveState ConverterForceTypeToMoveState(ForceType forceType)
    {
        switch (forceType)
        {
            case ForceType.KnockBack: return MovementSystem.MoveState.KnockBack;
            default: break;
        }

        return MovementSystem.MoveState.None;
    }
}
