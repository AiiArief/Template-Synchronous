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
        Vector3 posBeforeMove = entity.characterController.transform.position;
        float moveSpeed = 1.0f / PhaseManager.Instance.processInput.minimumTimeBeforeNextPhase;
        float gravitySpeed = _CalcGravitySpeed(entity.gravityPerTurn);
        bool hasCollided = false;

        action = () =>
        {
            Transform transform = entity.transform;
            Vector3 localTarget = transform.right * direction.x * range + transform.forward * direction.z * range;
            Vector3 target = new Vector3(posBeforeMove.x + localTarget.x, transform.position.y, posBeforeMove.z + localTarget.z);

            if (!_CheckCollisionInDirection(entity.collisionEntityChecker, direction) && !hasCollided)
            {
                if (Vector3.Distance(transform.position, target) > 0.1f && !_CheckProcessInputHasOverMinimumTime())
                {
                    entity.characterController.Move(localTarget * moveSpeed * Time.deltaTime + Vector3.down * gravitySpeed * Time.deltaTime);
                }
                else
                {
                    transform.position = target;
                    actionHasDone = true;
                }
            }
            else
            {
                hasCollided = true;
                Vector3 newPosBeforeMove = new Vector3(posBeforeMove.x, transform.position.y, posBeforeMove.z);
                if (Vector3.Distance(transform.position, newPosBeforeMove) > 0.1f)
                {
                    entity.characterController.Move(-localTarget * moveSpeed * Time.deltaTime + Vector3.down * gravitySpeed * Time.deltaTime);
                }
                else
                {
                    transform.position = newPosBeforeMove;
                    actionHasDone = true;
                }
            }
        };
    }

    private bool _CheckCollisionInDirection(CollisionEntityChecker collisionEntityChecker, Vector3 direction)
    {
        if (direction.z > 0.0f)
            return collisionEntityChecker.frontCollider.CheckColliderHaveEntityTag<TagEntityUnpassable>();

        if (direction.z < 0.0f)
            return collisionEntityChecker.backCollider.CheckColliderHaveEntityTag<TagEntityUnpassable>();

        if (direction.x > 0.0f)
            return collisionEntityChecker.rightCollider.CheckColliderHaveEntityTag<TagEntityUnpassable>();

        if (direction.x < 0.0f)
            return collisionEntityChecker.leftCollider.CheckColliderHaveEntityTag<TagEntityUnpassable>();

        return false;
    }

    private float _CalcGravitySpeed(int gravityPerTurn)
    {
        return gravityPerTurn / PhaseManager.Instance.processInput.minimumTimeBeforeNextPhase;
    }
}
