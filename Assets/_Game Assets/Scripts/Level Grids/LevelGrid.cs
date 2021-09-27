using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGrid : MonoBehaviour
{
    [SerializeField] LevelGridNode m_gridNodePrefab;

    [SerializeField] Vector2 m_size;
    public Vector2 size { get { return m_size; } }

    public Vector3 startPos { get; private set; }
    public LevelGridNode[,] gridNodes { get; private set; }

    public void EditorGenerateAllGridNodes()
    {
        EditorDestroyAllNodes();

        Vector3 tempStartPos = _CalculateStartPos();
        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                LevelGridNode node = Instantiate(m_gridNodePrefab, transform, false);
                node.EditorGenerateGridNode(new Vector2(i, j), new Vector3(tempStartPos.x + i, tempStartPos.y, tempStartPos.z + j));
            }
        }
    }

    public void EditorDestroyAllNodes()
    {
        int childCountBefore = transform.childCount;
        for (int i = 0; i < childCountBefore; i++)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }
    }

    public void SetupGridOnLevelStart()
    {
        // masukin ke grid node
        startPos = _CalculateStartPos();
        gridNodes = new LevelGridNode[(int)size.x, (int)size.y];

        int index = 0;
        for(int i=0; i<size.x; i++ )
        {
            for(int j=0; j<size.y; j++)
            {
                gridNodes[i, j] = transform.GetChild(index).GetComponent<LevelGridNode>();
                index++;
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

    public void ResetAllPathNode()
    {
        foreach (LevelGridNode node in gridNodes)
        {
            node.ResetPathNode();
        }
    }

    private Vector3 _CalculateStartPos()
    {
        return new Vector3(transform.position.x - size.x / 2, transform.position.y, transform.position.z - size.y / 2);
    }

    private bool _CheckNodeIsExist(Vector2 pos)
    {
        if (pos.x < 0 || pos.x >= m_size.x)
            return false;

        if (pos.y < 0 || pos.y >= m_size.y)
            return false;

        return true;
    }
}
