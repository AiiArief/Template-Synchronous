using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGridNode
{
    public LevelGrid parentGrid { get; private set; }
    public int x { get; private set; }
    public int y { get; private set; }
    public Vector3 realWorldPos { get; private set; }

    public bool isStaticNode { get; private set; } = false;
    public List<Entity> entityListOnThisNode { get; private set; } = new List<Entity>();

    public LevelGridNode(int x, int y, Vector3 realWorldPos)
    {
        this.x = x;
        this.y = y;
        this.realWorldPos = realWorldPos;

        _GenerateIsStaticNode();
    }

    public bool CheckIsWalkable()
    {
        if (isStaticNode)
            return false;

        foreach(Entity entity in entityListOnThisNode)
        {
            if (entity.gameObject.activeSelf && entity.GetComponent<TagEntityUnpassable>())
                return false;
        }

        return true;
    }

    private void _GenerateIsStaticNode()
    {
        Collider[] cols = Physics.OverlapBox(realWorldPos + new Vector3(0, 0.15f, 0), new Vector3(0.45f, 0.1f, 0.45f), Quaternion.identity, -1, QueryTriggerInteraction.Ignore);
        foreach (Collider col in cols)
        {
            if (col.gameObject.isStatic)
            {
                isStaticNode = true;
                return;
            }
        }

        isStaticNode = false;
    }
}

public class LevelGrid
{
    public int width { get; private set; }
    public int depth { get; private set; }
    public Vector3 startPos { get; private set; }
    public LevelGridNode[,] gridNodes { get; private set; }

    public LevelGrid(int newWidth, int newDepth, Vector3 newStartPos)
    {
        width = newWidth;
        depth = newDepth;
        startPos = newStartPos;

        gridNodes = new LevelGridNode[width, depth];
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < depth; j++)
            {
                gridNodes[i, j] = new LevelGridNode(i, j, new Vector3(startPos.x + i, startPos.y, startPos.z + j));
            }
        }
    }

    public LevelGridNode ConvertPosToGrid(Vector3 pos)
    {
        int x = (int)(0 - startPos.x + pos.x);
        int z = (int)(0 - startPos.z + pos.z);

        if (!CheckNodeIsExist(new Vector2(x, z)))
            return null;

        return gridNodes[x, z];
    }

    public bool CheckNodeIsExist(Vector2 pos)
    {
        if (pos.x < 0 || pos.x >= width)
            return false;

        if (pos.y < 0 || pos.y >= depth)
            return false;

        return true;
    }
}

public class LevelManager : MonoBehaviour
{
    [SerializeField] Transform m_gridMesh;

    public LevelGrid grid { get; private set; }

    public void SetupLevelOnLevelStart()
    {
        Vector2 gridSize = new Vector2((int)m_gridMesh.localScale.x, (int)m_gridMesh.localScale.y);
        Vector3 gridStartPos = new Vector3(m_gridMesh.position.x - gridSize.x / 2, 0.0f, m_gridMesh.position.z - gridSize.y / 2);

        grid = new LevelGrid((int)gridSize.x, (int)gridSize.y, gridStartPos);
    }

    public LevelGridNode AssignToGridFromRealWorldPos(Entity entity)
    {
        LevelGridNode nodeFromRealWorldPos = grid.ConvertPosToGrid(entity.transform.position);

        return nodeFromRealWorldPos;
    }
}
