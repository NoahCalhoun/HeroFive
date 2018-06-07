using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementSystem {
    
    private Transform m_TM;
    private List<H5TileBase> Path;

    private Coordinate m_Coordinate;

    public MovementSystem(Transform _TM, Coordinate _Coordinate)
    {
        this.m_TM = _TM;
        this.m_Coordinate = _Coordinate;
    }

    public void CalcMovePath(byte _x, byte _y)
    {
        Path = MoveManager.Instance.FindPath(WorldManager.Instance.GetTile(m_Coordinate.xy), WorldManager.Instance.GetTile(_x, _y));
    }

    public void CalcMovePath(ushort _xy)
    {
        Path = MoveManager.Instance.FindPath(WorldManager.Instance.GetTile(m_Coordinate.xy), WorldManager.Instance.GetTile(_xy));
    }
}
