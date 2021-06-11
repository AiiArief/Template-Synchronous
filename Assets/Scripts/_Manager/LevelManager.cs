using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGridNode
{
    public LevelGrid parentGrid { get; private set; }
    public int x { get; private set; }
    public int z { get; private set; }
    public Vector3 realWorldPos { get; private set; }

    public int gCost;
    public int hCost;
    public int fCost;

    public bool isWalkable = true;
    public LevelGridNode cameFromNode;

    public LevelGridNode(int x, int z, Vector3 realWorldPos)
    {
        this.x = x;
        this.z = z;
        this.realWorldPos = realWorldPos;

        _GenerateIsWalkable();
        //Debug.Log(x + " " + z + " (" + realWorldPos + ")" + isWalkable);
    }

    public void ResetPathNode()
    {
        gCost = 99999999;
        CalculateFCost();
        cameFromNode = null;
    }

    public void CalculateFCost()
    {
        fCost = gCost + hCost;
    }

    private void _GenerateIsWalkable()
    {
        Collider[] cols = Physics.OverlapBox(realWorldPos + new Vector3(0, 0.15f, 0), new Vector3(0.5f, 0.1f, 0.5f), Quaternion.identity, -1, QueryTriggerInteraction.Ignore);
        foreach (Collider col in cols)
        {
            if(col.gameObject.isStatic)
            {
                isWalkable = false;
                return;
            }
        }

        isWalkable = true;
    }
}

public class LevelGrid
{
    public int width { get; private set; }
    public int depth { get; private set; }
    public Vector3 startPos { get; private set; }
    public LevelGridNode[,] gridNodes;

    public LevelGrid(int width, int depth, Vector3 startPos)
    {
        this.width = width;
        this.depth = depth;
        this.startPos = startPos;

        gridNodes = new LevelGridNode[width, depth];
        for(int i=0; i<width; i++)
        {
            for(int j=0; j<depth; j++)
            {
                gridNodes[i, j] = new LevelGridNode(i, j, new Vector3(startPos.x + i, startPos.y, startPos.z + j));
            }
        }
    }

    public void ResetAllPathNode()
    {
        foreach(LevelGridNode node in gridNodes)
        {
            node.ResetPathNode();
        }
    }

    public LevelGridNode ConvertPosToGrid(Vector3 pos)
    {
        int x = (int)(0 - startPos.x + pos.x);
        int z = (int)(0 - startPos.z + pos.z);

        if (x < 0 || x > width)
            return null;

        if (z < 0 || z > depth)
            return null;

        return gridNodes[x, z];
    }
}

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [SerializeField] Transform m_baseFloorPathfinding;

    public Pathfinding pathfinding { get; private set; }

    public void SetupLevelPathfindingOnLevelStart()
    {
        Vector2 gridSize = new Vector2((int)m_baseFloorPathfinding.localScale.x, (int)m_baseFloorPathfinding.localScale.z);
        Vector3 gridStartPos = new Vector3(m_baseFloorPathfinding.position.x - gridSize.x / 2, 0.0f, m_baseFloorPathfinding.position.z - gridSize.y / 2);
        LevelGrid grid = new LevelGrid((int)gridSize.x, (int)gridSize.y, gridStartPos);
        //pathfinding = new Pathfinding(grid);
        pathfinding = new Pathfinding(new LevelGrid(44, 44, new Vector3(-22, 0, -22)));
    }

    private void Awake()
    {
        Instance = this;
    }
}
