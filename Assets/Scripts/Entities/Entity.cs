using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public List<StoredAction> storedActions { get; private set; } = new List<StoredAction>();
    public bool afterActionHasDone { get; protected set; } = false; // temp
    public int gravityPerTurn { get; private set; } = 3;

    public CharacterController characterController { get; private set; }

    [SerializeField] CollisionChecker m_collisionChecker;
    public CollisionChecker collisionChecker { get { return m_collisionChecker; } }

    public virtual void SetupWaitInput()
    {
        storedActions.Clear();
    }

    public virtual void WaitInput()
    {
        storedActions.Add(new StoredActionMove(this));
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

    protected virtual void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }
}
