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
            NextState = MoveState.None;
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
        m_Speed = 10.1f;
        m_State = new MoveStateMachine();
    }

    public void SetWalk(byte _x, byte _y)
    {
        if (m_State != null) m_State.SetState(MoveState.Walk);
        CalcMovePath(LogicHelper.GetCoordinateFromXY(_x, _y));
    }

    public void SetKnockBack(TILE_DIR dir, byte count)
    {
        if (m_State != null) m_State.SetState(MoveState.KnockBack);
        CurTargetTile = MoveManager.Instance.FindStraight(WorldManager.Instance.GetTile(m_TM), dir, count);
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
                m_State.NextStep();
                break;
            case MoveStep.In:
                m_State.NextStep();
                break;
            case MoveStep.End:
                m_State.NextStep();
                break;
        }
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

        MoveToTarget(m_Speed);

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
        return true;
    }

    private bool InStateKnockBack()
    {
        if (CurTargetTile == null) return true;

        float KnockBackSpeed = (CurTargetTile.TM.position - m_TM.position).magnitude * 7.0f;

        MoveToTarget(Mathf.Clamp(KnockBackSpeed, 1, 100));

        return false;
    }

    private void CalcMovePath(ushort _xy)
    {
        H5TileBase Start = CurTargetTile ?? WorldManager.Instance.GetTile(m_TM);

        Path = MoveManager.Instance.FindPath(Start, WorldManager.Instance.GetTile(_xy));
    }

    private void MoveToTarget(float speed)
    {
        Vector3 Dir = CurTargetTile.TM.position - m_TM.position;
        Dir.y = 0;
        Vector3 Move = Dir.normalized * speed * Time.deltaTime;

        if (Move.magnitude >= Dir.magnitude)
        {
            Move = Dir;
            CurTargetTile = null;
        }

        m_TM.Translate(Move);
    }
}
