using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementSystem {
    
    private Transform m_TM;
    private List<H5TileBase> Path;
    private H5TileBase CurTarget;

    private float m_Speed;

    public MovementSystem(Transform _TM)
    {
        this.m_TM = _TM;
        m_Speed = 1.1f;
    }

    public void CalcMovePath(byte _x, byte _y)
    {
        H5TileBase Start = CurTarget == null ? WorldManager.Instance.GetTile(m_TM) : CurTarget;
        
        Path = MoveManager.Instance.FindPath(Start, WorldManager.Instance.GetTile(_x, _y));
    }

    public void CalcMovePath(ushort _xy)
    {
        H5TileBase Start = CurTarget == null ? WorldManager.Instance.GetTile(m_TM) : CurTarget;

        Path = MoveManager.Instance.FindPath(Start, WorldManager.Instance.GetTile(_xy));
    }

    public void Update()
    {
        MoveToTarget();
    }

    private void MoveToTarget()
    {
        if ((Path == null || Path.Count <= 0) && CurTarget == null) return;

        if (CurTarget == null)
        {
            CurTarget = Path[0];
            Path.RemoveAt(0);
        }

        Vector3 Dir = CurTarget.TM.position - m_TM.position;
        Dir.y = 0;
        Vector3 Move = Dir.normalized * m_Speed * Time.deltaTime;

        if (Move.magnitude >= Dir.magnitude)
        {
            Move = Dir;
            CurTarget = null;
        }

        m_TM.Translate(Move);
    }
}
