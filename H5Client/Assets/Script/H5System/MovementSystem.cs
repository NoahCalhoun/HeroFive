using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementSystem : H5SystemBase
{
    public enum MoveState
    {
        None = -1,
        Stand,
        Walk,
        KnockBack,
    }

    public enum MoveStep
    {
        None = -1,
        On,     // 입장
        In,     // 중
        End,    // 퇴장
    }

    public class MoveStateMachine
    {
        public MoveState State { get; private set; }
        public MoveStep Step { get; private set; }

        public MoveState NextState { get; private set; }

        public MoveStateMachine()
        {
            State = MoveState.None;
            Step = MoveStep.None;
            NextState = MoveState.None;
        }

        public void SetState(MoveState state, MoveStep step = MoveStep.On)
        {
            State = state;
            Step = step;
            NextState = MoveState.Stand;
        }

        public void SetNextState(MoveState state)
        {
            NextState = state;
        }

        public void NextStep()
        {
            switch (Step)
            {
                case MoveStep.On:
                    Step = MoveStep.In;
                    break;
                case MoveStep.In:
                    Step = MoveStep.End;
                    break;
                case MoveStep.End:
                    Step = MoveStep.On;
                    State = NextState;
                    break;
            }
        }
    }

    private Transform m_TM;
    private List<H5TileBase> Path;
    private H5TileBase CurTargetTile;

    private float m_Speed;

    private MoveStateMachine m_State;

    public override void InitSystem(H5CharacterBase owner)
    {
        base.InitSystem(owner);
        m_TM = owner.TM;
        m_Speed = 8f;
        m_State = new MoveStateMachine();
    }

    // Walk set
    public bool SetWalk(byte _x, byte _y)
    {
        if (m_State == null || !IsMovable_Walk()) return false;

        if (!CalcMovePath(LogicHelper.GetCoordinateFromXY(_x, _y)))
            return false;

        if (Path.Count <= 0) return false;

        if (!OwnerOnTileSwitchTo(Path[Path.Count - 1])) return false;

        m_State.SetState(MoveState.Walk);
        return true;
    }

    public bool IsMovable_Walk()
    {
        if (m_State == null) return false;

        switch (m_State.State)
        {
            case MoveState.KnockBack:
                return false;
        }

        return true;
    }

    // KnockBack set
    public bool SetKnockBack(H5Direction dir, byte count)
    {
        if (m_State == null) return false;

        CurTargetTile = MoveManager.Instance.FindStraight(WorldManager.Instance.GetTile(m_TM), dir, count);
        if (!OwnerOnTileSwitchTo(CurTargetTile)) return false;

        m_State.SetState(MoveState.KnockBack);
        Owner.Direction = dir.Reverse();
        return true;
    }

    // 이동은 set 하는 순간 '순간이동' 처럼 목적지로 이동이 결정되고, update는 보여주는 과정일 뿐이다.
    // 따라서 움직이기 전 타일을 놓아주고 이동 후 타일을 잡는다.
    // Owner.OwnTile == null을 허용하지 않는 것은 Respawn 때 타일을 넣어주어 해결하자
    private bool OwnerOnTileSwitchTo(H5TileBase tile)
    {
        if (Owner.OwnTile == null || tile == null) return false;

        if (!Owner.OwnTile.LeaveTile(Owner)) return false;

        if (!tile.OnTile(Owner)) return false;
        Owner.OwnTile = tile;

        return true;
    }

    public void Update()
    {
        if (m_State == null) return;

        switch(m_State.State)
        {
            case MoveState.None:
                // None 상태의 On에서 캐싱 데이터를 전부 날릴까...?
                break;
            case MoveState.Stand:
                ProcessStateStand();
                break;
            case MoveState.Walk:
                ProcessStateWalk();
                break;
            case MoveState.KnockBack:
                ProcessStateKnockBack();
                break;
        }
    }

    private void ProcessStateStand()
    {
        if (m_State == null) return;

        switch (m_State.Step)
        {
            case MoveStep.On:
                if (OnStateStand()) m_State.NextStep();
                break;
            case MoveStep.In:
                if (InStateStand()) m_State.NextStep();
                break;
            case MoveStep.End:
                m_State.NextStep();
                break;
        }
    }

    private bool OnStateStand()
    {
        Owner.SpriteSystem.State = ActionState.Idle;
        return true;
    }

    private bool InStateStand()
    {
        return false;
    }

    private void ProcessStateWalk()
    {
        if (m_State == null) return;

        switch (m_State.Step)
        {
            case MoveStep.On:
                if (OnStateWalk()) m_State.NextStep();
                break;
            case MoveStep.In:
                if (InStateWalk()) m_State.NextStep();
                break;
            case MoveStep.End:
                m_State.NextStep();
                break;
        }
    }

    private bool OnStateWalk()
    {
        Owner.SpriteSystem.State = ActionState.Move;
        return true;
    }

    private bool InStateWalk()
    {
        if ((Path == null || Path.Count <= 0) && CurTargetTile == null) return true;

        if (CurTargetTile == null)
        {
            CurTargetTile = Path[0];
            Path.RemoveAt(0);
        }

        Owner.Direction = MoveToTarget(m_Speed);

        return false;
    }

    private void ProcessStateKnockBack()
    {
        if (m_State == null) return;

        switch (m_State.Step)
        {
            case MoveStep.On:
                if (OnStateKnockBack()) m_State.NextStep();
                break;
            case MoveStep.In:
                if (InStateKnockBack()) m_State.NextStep();
                break;
            case MoveStep.End:
                m_State.NextStep();
                break;
        }
    }

    private bool OnStateKnockBack()
    {
        Owner.SpriteSystem.State = ActionState.Hit;
        return true;
    }

    private bool InStateKnockBack()
    {
        if (CurTargetTile == null) return true;

        float KnockBackSpeed = (CurTargetTile.TM.position - m_TM.position).magnitude * 7.0f;

        MoveToTarget(Mathf.Clamp(KnockBackSpeed, 1, 100));

        return false;
    }

    private bool CalcMovePath(ushort _xy)
    {
        H5TileBase Start = CurTargetTile ?? Owner.OwnTile;
        if (Start == null) return false;
        
        Path = MoveManager.Instance.FindPath(Start, WorldManager.Instance.GetTile(_xy));
        return true;
    }

    private H5Direction MoveToTarget(float speed)
    {
        Vector3 Dir = CurTargetTile.TM.position - m_TM.position;
        Dir.y = 0;
        Vector3 Move = Dir.normalized * speed * Time.deltaTime;

        if (Move.sqrMagnitude >= Dir.sqrMagnitude)
        {
            Move = Dir;
            CurTargetTile = null;
        }

        m_TM.Translate(Move);
        return Move.VectorToDirection();
    }
}
