using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityCharacter : Entity
{
    public CharacterController characterController { get; private set; }
    [SerializeField] bool m_detectCollisionOnStart = true;

    public bool enableMoveInput = true;
    public float gravityPerTurn = 3.0f;

    public LevelGridNode currentNode { get; private set; }

    public List<StoredStatusEffect> storedStatusEffects { get; private set; } = new List<StoredStatusEffect>();

    public override void WaitInput()
    {
        foreach(StoredStatusEffect effect in storedStatusEffects)
        {
            effect.effectAction.Invoke();
        }

        // update ui disini kah?
    }

    public override void AfterInput()
    {
        foreach (StoredStatusEffect effect in storedStatusEffects.ToArray())
        {
            effect.effectRemovalAction.Invoke();
        }
    }

    public override void SetIsUpdateAble(bool isUpdateAble)
    {
        base.SetIsUpdateAble(isUpdateAble);

        if (isUpdateAble)
            _AssignComponent();
    }

    public virtual void StoredActionSkipTurn()
    {
        storedActions.Add(new StoredActionSkip());
        storedActions.Add(new StoredActionMove(this));
    }

    public void AssignToLevelGrid(LevelGridNode node = null)
    {
        currentNode = node;
        if (node == null)
        {
            LevelGrid tempGrid = GameManager.Instance.levelManager.GetClosestGridFromPosition(transform.position);
            LevelGridNode tempGridNode = tempGrid.ConvertPosToNode(transform.position);
            currentNode = tempGridNode;
        }

        currentNode.entityListOnThisNode.Add(this);
    }

    protected virtual void _AssignComponent()
    {
        if (!characterController)
        {
            characterController = GetComponent<CharacterController>();
            characterController.detectCollisions = m_detectCollisionOnStart;
        }
    }
}
