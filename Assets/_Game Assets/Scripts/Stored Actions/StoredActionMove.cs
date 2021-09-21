using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoredActionMove : StoredAction
{
    public StoredActionMove(EntityCharacter entity)
    {
        Vector3 m_posBeforeMove = entity.transform.position;
        float gravitySpeed = _CalcGravitySpeed(entity.gravityPerTurn);

        action = () =>
        {
            entity.characterController.Move(Vector3.down * gravitySpeed * Time.deltaTime);
            actionHasDone = _CheckProcessInputHasOverMinimumTime();
        };
    }

    public StoredActionMove(EntityCharacter entity, Vector3 direction, float range = 1)
    {
        Transform transform = entity.transform;
        LevelManager levelManager = GameManager.Instance.levelManager;
        LevelGridNode currentNode = entity.currentNode;

        Vector3 localTarget = transform.right * direction.x + transform.forward * direction.z;
        float moveSpeed = 1.0f / GameManager.Instance.phaseManager.processInput.minimumTimeBeforeNextPhase;
        float gravitySpeed = _CalcGravitySpeed(entity.gravityPerTurn);
        
        currentNode.entityListOnThisNode.Remove(entity);
        for(int i=0; i<range; i++)
        {
            // struggle = true kalo nabrak?
            if (!entity.enableMoveInput)
                break;

            Vector3 nextPos = new Vector3(currentNode.realWorldPos.x + Mathf.RoundToInt(localTarget.x), transform.position.y, currentNode.realWorldPos.z + Mathf.RoundToInt(localTarget.z));
            LevelGrid tempGrid = levelManager.GetClosestGridFromPosition(nextPos);
            if (tempGrid == null)
                break;

            LevelGridNode tempGridNode = tempGrid.ConvertPosToNode(nextPos);
            if (tempGridNode == null)
                break;

            if (!tempGridNode.CheckIsWalkable())
                break;

            currentNode = tempGridNode;
        }

        entity.AssignToLevelGrid(currentNode);

        action = () =>
        {
            Vector3 target = new Vector3(currentNode.realWorldPos.x, transform.position.y, currentNode.realWorldPos.z);
            if (Vector3.Distance(transform.position, target) > 0.1f && !_CheckProcessInputHasOverMinimumTime())
            {
                entity.characterController.Move(localTarget * range * moveSpeed * Time.deltaTime + Vector3.down * gravitySpeed * Time.deltaTime);
            }
            else
            {
                transform.position = target;
                actionHasDone = true;
            }
        };
    }

    private float _CalcGravitySpeed(float gravityPerTurn)
    {
        return gravityPerTurn / GameManager.Instance.phaseManager.processInput.minimumTimeBeforeNextPhase;
    }
}
