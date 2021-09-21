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

        foreach (Entity entity in entityListOnThisNode)
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

[System.Serializable]
public class LevelGrid
{
    [SerializeField] Transform m_gridMesh;

    public Vector2 size { get; private set; }
    public Vector3 startPos { get; private set; }
    public LevelGridNode[,] gridNodes { get; private set; }

    public void SetupGrid()
    {
        size = new Vector2(Mathf.RoundToInt(m_gridMesh.localScale.x), Mathf.RoundToInt(m_gridMesh.localScale.y));
        startPos = new Vector3(m_gridMesh.position.x - size.x / 2, m_gridMesh.position.y, m_gridMesh.position.z - size.y / 2);

        gridNodes = new LevelGridNode[(int)size.x, (int)size.y];
        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                gridNodes[i, j] = new LevelGridNode(i, j, new Vector3(startPos.x + i, startPos.y, startPos.z + j));
            }
        }
    }

    public LevelGridNode ConvertPosToNode(Vector3 pos)
    {
        int x = Mathf.RoundToInt(0 - startPos.x + pos.x);
        int z = Mathf.RoundToInt(0 - startPos.z + pos.z);

        if (!_CheckNodeIsExist(new Vector2(x, z)))
            return null;

        return gridNodes[x, z];
    }

    private bool _CheckNodeIsExist(Vector2 pos)
    {
        if (pos.x < 0 || pos.x >= size.x)
            return false;

        if (pos.y < 0 || pos.y >= size.y)
            return false;

        return true;
    }
}

public class LevelManager : MonoBehaviour
{
    [SerializeField] int minYPosToleration = -1;
    [SerializeField] LevelGrid[] m_grids;
    public LevelGrid[] grids { get { return m_grids; } }

    public void SetupAllGridsOnLevelStart()
    {
        foreach (LevelGrid grid in m_grids)
        {
            grid.SetupGrid();
        }
    }

    public LevelGrid GetClosestGridFromPosition(Vector3 pos)
    {
        for (int i = 0; i < m_grids.Length; i++)
        {
            bool isYBetweenGrid =
                (m_grids.Length == 1) ? true :
                (i == 0) ? pos.y < m_grids[i + 1].startPos.y + minYPosToleration :
                (i == m_grids.Length - 1) ? pos.y >= m_grids[i].startPos.y + minYPosToleration :
                pos.y >= m_grids[i].startPos.y + minYPosToleration && pos.y < m_grids[i + 1].startPos.y + minYPosToleration;

            if (isYBetweenGrid)
            {
                //Vector2 pos2D = new Vector2(pos.x, pos.z);
                //bool isXBetweenGrid = pos2D.x >= m_grids[i].startPos.x && pos2D.x < m_grids[i].startPos.x + m_grids[i].size.x;
                //bool isZBetweenGrid = pos2D.y >= m_grids[i].startPos.y && pos2D.y < m_grids[i].startPos.y + m_grids[i].size.y;
                //if (isXBetweenGrid && isZBetweenGrid)
                    return m_grids[i];
            }
        }

        return null;
    }
}
