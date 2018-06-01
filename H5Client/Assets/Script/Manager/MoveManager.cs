using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class AStarNode
{
    public H5TileBase This;
    public AStarNode Parent;
    public long F; // 총 비용 F = G + H
    public long H; // 목적지와의 거리
    public long G; // 시작점과의 거리

    public AStarNode(H5TileBase _This, AStarNode _Parent, H5TileBase _Start, H5TileBase _Target)
    public AStarNode(H5TileBase _This, AStarNode _Parent, long _G, H5TileBase _Target)
    {
        This = _This;
        Parent = _Parent;
        H = MoveManager.GetDistance(_This, _Target);
        G = _G;
        F = H + G;
    }
     
    public ushort GetCoordinate()
    {
        return This ? This.m_Coordinate.xy : ushort.MaxValue;
    }
}

public class MoveManager
{
    private static MoveManager mInstance;
    public static MoveManager Instance { get { if (mInstance == null) mInstance = new MoveManager(); return mInstance; } }

    public List<H5TileBase> FindPath(H5TileBase _start, H5TileBase _target)
    {
        List<H5TileBase> pathList = new List<H5TileBase>();
        
        Dictionary<long, AStarNode> openDic = new Dictionary<long, AStarNode>();
        HashSet<ushort> closeSet = new HashSet<ushort>();

        AStarNode CurNode = new AStarNode(_start, null, 0, _target);
        AStarNode NextNode = null;

        while(true)
        {
            closeSet.Add(CurNode.This.m_Coordinate.xy);

            foreach (TILE_NEIGHBOR direction in System.Enum.GetValues(typeof(TILE_NEIGHBOR)))
            {
                if (direction == TILE_NEIGHBOR.Max) break;
                AddOpenDic(CurNode, direction, _target, openDic, closeSet, ref NextNode);
            }

            if (openDic.Count == 0) break;

            CurNode = NextNode;
            NextNode = null;

            openDic.Remove(CurNode.This.m_Coordinate.xy);

            if (CurNode.GetCoordinate() == _target.m_Coordinate.xy)
            {
                while (true)
                {
                    pathList.Insert(0, CurNode.This);
                    CurNode = CurNode.Parent;

                    if (CurNode.GetCoordinate() == _start.m_Coordinate.xy)
                        break;
                }
                break;
            }
        }
        
        return pathList;
    }
    
    void AddOpenDic(AStarNode _node, TILE_NEIGHBOR _direction, H5TileBase _target, Dictionary<long, AStarNode> _openDic, HashSet<ushort> _closeSet, ref AStarNode _NextNode)
    {
        H5TileBase NeighborTile = _node.This.GetNeighbor(_direction);
        if (NeighborTile == null || !NeighborTile.IsWalkable) return;

        ushort Coordinate = NeighborTile.m_Coordinate.xy;
        if (_closeSet.Contains(Coordinate)) return;

        long NeighborG = _node.G + 1;

        AStarNode inOpen;
        if (_openDic.TryGetValue(Coordinate, out inOpen))
        {
            if (inOpen.G < NeighborG)
            {
                return;
            }
            else
            {
                inOpen.G = NeighborG;
                inOpen.F = inOpen.G + inOpen.H;
                inOpen.Parent = _node;
                if (_NextNode == null || _NextNode.F > inOpen.F) _NextNode = inOpen;
                return;
            }
        }

        AStarNode NeighborNode = new AStarNode(NeighborTile, _node, NeighborG, _target);
        _openDic.Add(Coordinate, NeighborNode);

        if (_NextNode == null || _NextNode.F > NeighborNode.F) _NextNode = NeighborNode;
    }

    public static long GetDistance(H5TileBase tile1, H5TileBase tile2)
    {
        if (tile1 == null || tile2 == null)
            return -1;

        long DistX = tile1.m_Coordinate.x > tile2.m_Coordinate.x ? tile1.m_Coordinate.x - tile2.m_Coordinate.x : tile2.m_Coordinate.x - tile1.m_Coordinate.x;
        long DistY = tile1.m_Coordinate.y > tile2.m_Coordinate.y ? tile1.m_Coordinate.y - tile2.m_Coordinate.y : tile2.m_Coordinate.y - tile1.m_Coordinate.y;
        return DistX + DistY;
    }
}
