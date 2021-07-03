using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoredActionMove : StoredAction
{
    public StoredActionMove(Entity entity)
    {
        Vector3 m_posBeforeMove = entity.transform.position;
        float gravitySpeed = _CalcGravitySpeed(entity.gravityPerTurn);

        action = () =>
        {
            entity.characterController.Move(Vector3.down * gravitySpeed * Time.deltaTime);
            actionHasDone = _CheckProcessInputHasOverMinimumTime();
        };
    }

    public StoredActionMove(Entity entity, Vector3 direction, float range = 1)
    {
        Transform transform = entity.transform;
        LevelGrid currentGrid = GameManager.Instance.levelManager.grid;
        LevelGridNode currentNode = entity.currentNode;

        Vector3 localTarget = transform.right * direction.x + transform.forward * direction.z;
        float moveSpeed = 1.0f / GameManager.Instance.phaseManager.processInput.minimumTimeBeforeNextPhase;
        float gravitySpeed = _CalcGravitySpeed(entity.gravityPerTurn);
        
        currentNode.entityListOnThisNode.Remove(entity);
        for(int i=0; i<range; i++)
        {
            Vector2 tempNodePos = new Vector2(currentNode.x + Mathf.RoundToInt(localTarget.x), currentNode.y + Mathf.RoundToInt(localTarget.z));

            if (!currentGrid.CheckNodeIsExist(tempNodePos))
                break;

            if (!currentGrid.gridNodes[(int)tempNodePos.x, (int)tempNodePos.y].CheckIsWalkable())
                break;

            currentNode = currentGrid.gridNodes[(int)tempNodePos.x, (int)tempNodePos.y];
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
