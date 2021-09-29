using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelPathfinding
{
    private const int MOVE_STRAIGHT_COST = 10;

    List<LevelGridNode> m_openList;
    List<LevelGridNode> m_closedList;

    // trik2 optimisasi :
    // jangan jauh2 pathfindingnya
    // kalo emang ga ada objek antara dua point, simple move to aja
    // kalo tujuan udah ada entity, jangan ngesearch semuanya
    // kalo pathfindingnya masih sama ga usah find path
    public List<LevelGridNode> FindPath(Vector3 startWorldPos, Vector3 endWorldPos, float maxDistance = 20.0f)
    {
        LevelGrid startNodeGrid = GameManager.Instance.levelManager.GetClosestGridFromPosition(startWorldPos);
        LevelGrid endNodeGrid = GameManager.Instance.levelManager.GetClosestGridFromPosition(endWorldPos);

        if (startNodeGrid != endNodeGrid)
        {
            Debug.Log("Different Grid");
            return null;
        }

        // kalo lebih dari jarak mungkin ubah endworldpos ke terdeket?
        LevelGridNode startNode = startNodeGrid.ConvertPosToNode(startWorldPos);
        LevelGridNode endNode = endNodeGrid.ConvertPosToNode(endWorldPos);

        if (startNode == null || endNode == null)
        {
            Debug.Log("Invalid Node :\nStart Node : " + startNode + "\nEnd Node : " + endNode);
            return null;
        }

        m_openList = new List<LevelGridNode> { startNode };
        m_closedList = new List<LevelGridNode>();

        LevelGrid grid = startNodeGrid = endNodeGrid;
        grid.ResetAllPathNode();

        startNode.gCost = 0;
        startNode.hCost = _CalculateDistanceCost(startNode, endNode);
        startNode.CalculateFCost();

        // this shit is costlyyyyy apalgi kalo jaoh
        while (m_openList.Count > 0)
        {
            LevelGridNode currentNode = _CalculateLowestFCostNode(m_openList);

            if (currentNode == endNode)
            {
                return _CalculatePath(endNode);
            }

            m_openList.Remove(currentNode);
            m_closedList.Add(currentNode);

            foreach (LevelGridNode neighbourNode in _GenerateNeighbourList(currentNode))
            {
                if (m_closedList.Contains(neighbourNode)) continue;
                if (neighbourNode.isStaticNode) //|| !neighbourNode.CheckListEntityIsPassable()) // ini tuh kalo ada entity di tujuan bakalan false
                {
                    m_closedList.Add(neighbourNode);
                    continue;
                }

                int tentativeGCost = currentNode.gCost + _CalculateDistanceCost(currentNode, neighbourNode);
                if (tentativeGCost < neighbourNode.gCost)
                {
                    neighbourNode.cameFromNode = currentNode;
                    neighbourNode.gCost = tentativeGCost;
                    neighbourNode.hCost = _CalculateDistanceCost(neighbourNode, endNode);
                    neighbourNode.CalculateFCost();

                    if (!m_openList.Contains(neighbourNode))
                    {
                        m_openList.Add(neighbourNode);
                    }
                }
            }
        }

        return null;
    }

    private List<LevelGridNode> _GenerateNeighbourList(LevelGridNode currentNode)
    {
        List<LevelGridNode> neighbourList = new List<LevelGridNode>();
        LevelGrid grid = currentNode.parentGrid;

        if (currentNode.gridPos.x - 1 >= 0)
            neighbourList.Add(grid.gridNodes[(int)currentNode.gridPos.x - 1, (int)currentNode.gridPos.y]);

        if (currentNode.gridPos.x + 1 < grid.size.x)
            neighbourList.Add(grid.gridNodes[(int)currentNode.gridPos.x + 1, (int)currentNode.gridPos.y]);

        if (currentNode.gridPos.y - 1 >= 0)
            neighbourList.Add(grid.gridNodes[(int)currentNode.gridPos.x, (int)currentNode.gridPos.y - 1]);

        if (currentNode.gridPos.y + 1 < grid.size.y)
            neighbourList.Add(grid.gridNodes[(int)currentNode.gridPos.x, (int)currentNode.gridPos.y + 1]);

        return neighbourList;
    }

    private int _CalculateDistanceCost(LevelGridNode a, LevelGridNode b)
    {
        int xDistance = Mathf.Abs((int)a.gridPos.x - (int)b.gridPos.x);
        int yDistance = Mathf.Abs((int)a.gridPos.y - (int)b.gridPos.y);
        int total = Mathf.Abs(xDistance + yDistance);
        return MOVE_STRAIGHT_COST * total;
    }

    private LevelGridNode _CalculateLowestFCostNode(List<LevelGridNode> pathNodeList)
    {
        LevelGridNode lowestFCostNode = pathNodeList[0];
        for (int i = 1; i < pathNodeList.Count; i++)
        {
            if (pathNodeList[i].fCost < lowestFCostNode.fCost)
            {
                lowestFCostNode = pathNodeList[i];
            }
        }
        return lowestFCostNode;
    }

    private List<LevelGridNode> _CalculatePath(LevelGridNode endNode)
    {
        List<LevelGridNode> path = new List<LevelGridNode>();
        path.Add(endNode);
        LevelGridNode currentNode = endNode;
        while (currentNode.cameFromNode != null)
        {
            path.Add(currentNode.cameFromNode);
            currentNode = currentNode.cameFromNode;
        }
        path.Reverse();
        return path;
    }
}
