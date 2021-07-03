using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityManager : MonoBehaviour
{
    protected List<Entity> entities { get; private set; } = new List<Entity>();

    public virtual void SetupEntitiesOnLevelStart()
    {
        _AssignEntities();
    }

    public virtual void SetupEntitiesOnWaitInputStart()
    {
        foreach (Entity entity in entities)
        {
            if (entity.isUpdateAble)
            {
                entity.SetupWaitInput();
            }
        }
    }

    public virtual bool CheckEntitiesHasDoneWaitInput()
    {
        foreach (Entity entity in entities)
        {
            if (entity.isUpdateAble && entity.storedActions.Count == 0)
            {
                entity.WaitInput();
            }
        }

        foreach (Entity entity in entities)
        {
            if (entity.isUpdateAble && entity.storedActions.Count == 0)
            {
                return false;
            }
        }

        return true;
    }

    public virtual void SetupEntitiesOnProcessInputStart()
    {
        foreach (Entity entity in entities)
        {
            if(entity.isUpdateAble)
            {
                entity.SetupProcessInput();
            }
        }
    }

    public virtual bool CheckEntitiesHasDoneProcessInput()
    {
        foreach (Entity entity in entities)
        {
            if (entity.isUpdateAble && !entity.CheckAllActionHasDone())
            {
                foreach (StoredAction storedAction in entity.storedActions)
                {
                    if (!storedAction.actionHasDone)
                    {
                        storedAction.action.Invoke();
                    }
                }
            }
        }

        foreach (Entity entity in entities)
        {
            if (entity.isUpdateAble && !entity.CheckAllActionHasDone())
            {
                return false;
            }
        }

        return true;
    }

    public virtual void SetupEntitiesOnAfterInputStart()
    {
        foreach (Entity entity in entities)
        {
            if(entity.isUpdateAble)
            {
                entity.SetupAfterInput();
            }
        }
    }

    public virtual bool CheckEntitiesHasDoneAfterInput()
    {
        foreach (Entity entity in entities)
        {
            if (entity.isUpdateAble)
            {
                entity.AfterInput();
            }
        }

        foreach (Entity entity in entities)
        {
            if (entity.isUpdateAble && !entity.afterActionHasDone)
            {
                return false;
            }
        }

        return true;
    }

    protected virtual void _AssignEntities()
    {
        foreach (Transform child in transform)
        {
            Entity entity = child.GetComponent<Entity>();
            if (entity)
            {
                entities.Add(entity);
                entity.AssignToLevelGrid();
            }
        }
    }
}
