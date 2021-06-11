using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding
{
    private const int MOVE_STRAIGHT_COST = 10;

    LevelGrid m_grid;
    List<LevelGridNode> m_openList;
    List<LevelGridNode> m_closedList;

    public Pathfinding(LevelGrid grid)
    {
        m_grid = grid;
    }

    public List<LevelGridNode> FindPath(Vector3 startWorldPos, Vector3 endWorldPos)
    {
        LevelGridNode startNode = m_grid.ConvertPosToGrid(startWorldPos);
        LevelGridNode endNode = m_grid.ConvertPosToGrid(endWorldPos);

        if (startNode == null || endNode == null)
        {
            Debug.Log("Invalid Node :\nStart Node : " + startNode + "\nEnd Node : " + endNode);
            return null;
        }

        m_openList = new List<LevelGridNode> { startNode };
        m_closedList = new List<LevelGridNode>();

        m_grid.ResetAllPathNode();

        startNode.gCost = 0;
        startNode.hCost = _CalculateDistanceCost(startNode, endNode);
        startNode.CalculateFCost();

        while (m_openList.Count > 0)
        {
            LevelGridNode currentNode = _CalculateLowestFCostNode(m_openList);

            if (currentNode == endNode)
            {
                return _CalculatePath(endNode);
            }

            m_openList.Remove(currentNode);
            m_closedList.Add(currentNode);

            foreach(LevelGridNode neighbourNode in _GenerateNeighbourList(currentNode))
            {
                if (m_closedList.Contains(neighbourNode)) continue;
                if (!neighbourNode.isWalkable)
                {
                    m_closedList.Add(neighbourNode);
                    continue;
                }

                int tentativeGCost = currentNode.gCost + _CalculateDistanceCost(currentNode, neighbourNode);
                if(tentativeGCost < neighbourNode.gCost)
                {
                    neighbourNode.cameFromNode = currentNode;
                    neighbourNode.gCost = tentativeGCost;
                    neighbourNode.hCost = _CalculateDistanceCost(neighbourNode, endNode);
                    neighbourNode.CalculateFCost();

                    if(!m_openList.Contains(neighbourNode))
                    {
                        m_openList.Add(neighbourNode);
                    }
                }
            }
        }

        // Out of nodes on the openList
        return null;
    }

    private List<LevelGridNode> _GenerateNeighbourList(LevelGridNode currentNode)
    {
        List<LevelGridNode> neighbourList = new List<LevelGridNode>();

        if(currentNode.x - 1 >= 0)
            neighbourList.Add(m_grid.gridNodes[currentNode.x - 1, currentNode.z]);

        if (currentNode.x + 1 < m_grid.width)
            neighbourList.Add(m_grid.gridNodes[currentNode.x + 1, currentNode.z]);

        if (currentNode.z - 1 >= 0)
            neighbourList.Add(m_grid.gridNodes[currentNode.x, currentNode.z - 1]);

        if (currentNode.z + 1 < m_grid.depth)
            neighbourList.Add(m_grid.gridNodes[currentNode.x, currentNode.z + 1]);

        return neighbourList;
    }

    private int _CalculateDistanceCost(LevelGridNode a, LevelGridNode b)
    {
        int xDistance = Mathf.Abs(a.x - b.x);
        int yDistance = Mathf.Abs(a.z - b.z);
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
