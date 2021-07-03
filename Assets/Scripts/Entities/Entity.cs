using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public bool isUpdateAble { get { return gameObject.activeSelf; } }

    public List<StoredAction> storedActions { get; private set; } = new List<StoredAction>();
    public bool afterActionHasDone { get; protected set; } = false;

    public CharacterController characterController { get; private set; }
    [SerializeField] float m_gravityPerTurn = 3.0f;
    public float gravityPerTurn { get { return m_gravityPerTurn; } }

    public LevelGridNode currentNode { get; private set; }

    public void AssignToLevelGrid(LevelGridNode node = null)
    {
        currentNode = node;
        if (node == null)
            currentNode = GameManager.Instance.levelManager.AssignToGridFromRealWorldPos(this);

        currentNode.entityListOnThisNode.Add(this);
    }

    public virtual void SetupWaitInput()
    {
        storedActions.Clear();
    }

    public virtual void WaitInput()
    {
        storedActions.Add(new StoredActionSkip());
    }

    public virtual void SetupProcessInput()
    {
        foreach (StoredAction storedAction in storedActions)
            storedAction.actionHasDone = false;
    }

    public virtual bool CheckAllActionHasDone()
    {
        foreach (StoredAction storedAction in storedActions)
        {
            if (!storedAction.actionHasDone) return false;
        }

        return true;
    }

    public virtual void SetupAfterInput()
    {
        afterActionHasDone = false;
    }

    public virtual void AfterInput()
    {
        afterActionHasDone = true;
    }

    public void SetIsUpdateAble(bool isUpdateAble)
    {
        gameObject.SetActive(isUpdateAble);
        storedActions.Clear();

        if (isUpdateAble)
            _AssignComponent();
    }

    protected virtual void _AssignComponent()
    {
        if(!characterController)
            characterController = GetComponent<CharacterController>();
    }
}
