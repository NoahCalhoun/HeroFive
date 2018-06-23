using System.Collections.Generic;

class AStarNode
{
    public H5TileBase This;
    public AStarNode Parent;
    public long F; // 총 비용 F = G + H
    public long H; // 목적지와의 거리
    public long G; // 시작점과의 거리
    
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

    List<H5TileBase> PathList = new List<H5TileBase>();
    Dictionary<long, AStarNode> OpenDic = new Dictionary<long, AStarNode>();
    HashSet<ushort> CloseSet = new HashSet<ushort>();
    SortedDictionary<long, List<AStarNode>> SortedDic = new SortedDictionary<long, List<AStarNode>>();

    public H5TileBase FindStraight(H5TileBase _start, H5Direction _dir, byte _count)
    {
        H5TileBase result = null;
        H5TileBase neighbor = _start;

        for (byte i = 0; i < _count; ++i)
        {
            neighbor = neighbor.GetNeighbor(_dir);

            if (neighbor == null || !neighbor.IsWalkable() ) break;

            result = neighbor;
        }

        return result;
    }

    // List의 끝이 타겟 타일이다.
    public List<H5TileBase> FindPath(H5TileBase _start, H5TileBase _target)
    {
        PathList.Clear();
        OpenDic.Clear();
        CloseSet.Clear();
        SortedDic.Clear();

        AStarNode CurNode = new AStarNode(_start, null, 0, _target);

        while(true)
        {
            CloseSet.Add(CurNode.This.m_Coordinate.xy);

            foreach (H5Direction direction in System.Enum.GetValues(typeof(H5Direction)))
            {
                if (direction == H5Direction.Max) break;
                AddOpenDic(CurNode, direction, _target);
            }

            if (OpenDic.Count <= 0 || SortedDic.Count <= 0) break;
            
            var e = SortedDic.GetEnumerator();
            while (e.MoveNext())
            {
                if (e.Current.Value.Count > 0)
                    break;
            }
            CurNode = e.Current.Value[0];
            OpenDic.Remove(CurNode.This.m_Coordinate.xy);
            SortedDic[CurNode.F].RemoveAt(0);

            if (CurNode.GetCoordinate() == _target.m_Coordinate.xy)
            {
                while (true)
                {
                    PathList.Insert(0, CurNode.This);
                    CurNode = CurNode.Parent;

                    if (CurNode.GetCoordinate() == _start.m_Coordinate.xy)
                        break;
                }
                break;
            }
        }
        
        return new List<H5TileBase>(PathList);
    }
    
    void AddOpenDic(AStarNode _node, H5Direction _direction, H5TileBase _target)
    {
        H5TileBase NeighborTile = _node.This.GetNeighbor(_direction);
        if (NeighborTile == null || !NeighborTile.IsWalkable()) return;

        ushort Coordinate = NeighborTile.m_Coordinate.xy;
        if (CloseSet.Contains(Coordinate)) return;

        long NeighborG = _node.G + 1;

        AStarNode inOpen;
        if (OpenDic.TryGetValue(Coordinate, out inOpen))
        {
            if (inOpen.G < NeighborG)
            {
                return;
            }
            else
            {
                long f = inOpen.F;
                inOpen.G = NeighborG;
                inOpen.F = inOpen.G + inOpen.H;
                inOpen.Parent = _node;
                SortedDic[f].Remove(inOpen);
                if (SortedDic.ContainsKey(inOpen.F) == false)
                    SortedDic.Add(inOpen.F, new List<AStarNode>());
                SortedDic[inOpen.F].Add(inOpen);
                return;
            }
        }

        AStarNode NeighborNode = new AStarNode(NeighborTile, _node, NeighborG, _target);
        OpenDic.Add(Coordinate, NeighborNode);
        if (SortedDic.ContainsKey(NeighborNode.F) == false)
            SortedDic.Add(NeighborNode.F, new List<AStarNode>());
        SortedDic[NeighborNode.F].Add(NeighborNode);
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
