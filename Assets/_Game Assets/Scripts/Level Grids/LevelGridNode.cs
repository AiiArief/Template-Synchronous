using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevelGridNode
{
    public LevelGrid parentGrid { get; private set; }

    [SerializeField] Vector2 m_gridPos;
    public Vector2 gridPos { get { return m_gridPos; } }

    [SerializeField] Vector3 m_realWorldPos;
    public Vector3 realWorldPos { get { return m_realWorldPos; } }

    [SerializeField] bool m_isStaticNode;
    public bool isStaticNode { get { return m_isStaticNode; } }
    public List<Entity> entityListOnThisNode { get; private set; } = new List<Entity>();

    [HideInInspector] public int gCost;
    [HideInInspector] public int hCost;
    [HideInInspector] public int fCost;
    [HideInInspector] public LevelGridNode cameFromNode;

    public LevelGridNode(LevelGridNode scriptableNode = null, LevelGrid parentGrid = null)
    {
        if(scriptableNode != null)
        {
            this.parentGrid = parentGrid;
            m_gridPos = scriptableNode.m_gridPos;
            m_realWorldPos = scriptableNode.m_realWorldPos;
            m_isStaticNode = scriptableNode.m_isStaticNode;
        }
    }

    public void EditorGenerateGridNode(Vector2 gridPos, Vector3 realWorldPos)
    {
        m_gridPos = gridPos;
        m_realWorldPos = realWorldPos;
        _GenerateIsStaticNode();
    }

    public bool CheckListEntityIsPassable()
    {
        foreach (Entity entity in entityListOnThisNode)
        {
            if (entity.isUpdateAble && entity.GetComponent<TagEntityUnpassable>())
                return false;
        }

        return true;
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

    private void _GenerateIsStaticNode()
    {
        Collider[] cols = Physics.OverlapBox(realWorldPos + new Vector3(0, 0.15f, 0), new Vector3(0.45f, 0.1f, 0.45f), Quaternion.identity, -1, QueryTriggerInteraction.Ignore);
        foreach (Collider col in cols)
        {
            if (col.gameObject.GetComponent<TagGridCollider>())
            {
                m_isStaticNode = true;
                return;
            }
        }

        m_isStaticNode = false;
    }
}
