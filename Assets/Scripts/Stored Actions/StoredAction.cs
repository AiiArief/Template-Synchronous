using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StoredAction
{
    public Action action;
    public bool actionHasDone = false;

    protected bool _CheckProcessInputHasOverMinimumTime()
    {
        return PhaseManager.Instance.processInput.currentTimeBeforeNextPhase >= PhaseManager.Instance.processInput.minimumTimeBeforeNextPhase;
    }
}

public class StoredActionSkip : StoredAction
{
    public StoredActionSkip()
    {
        action = () => 
        {
            actionHasDone = true; 
        };
    }
}

public class StoredActionTurn : StoredAction
{
    public StoredActionTurn(Player player, Vector3 m_currentCameraRot)
    {
        action = () =>
        {
            Vector3 faceCameraDir = new Vector3(0.0f, _ConvertTo90Degrees(m_currentCameraRot.y), 0.0f);
            player.transform.rotation = Quaternion.Euler(faceCameraDir);
            player.playerCamera.transform.parent.rotation = Quaternion.Euler(m_currentCameraRot);
            actionHasDone = true;
        };
    }

    private float _ConvertTo90Degrees(float value, bool isRepeat = true)
    {
        if (isRepeat)
        {
            if (value >= 315.0f && value < 45.0f) return 0.0f;
            if (value >= 45.0f && value < 135.0f) return 90.0f;
            if (value >= 135.0f && value < 225.0f) return 180.0f;
            if (value >= 225.0f && value < 315.0f) return 270.0f;
        }
        else
        {
            if (value >= -45.0f && value < 45.0f) return 0.0f;
            if (value >= 45.0f && value < 135.0f) return 90.0f;
            if (value <= -45.0f && value > -135.0f) return -90.0f;
            if (value <= -135.0f || value > 135.0f) return 180.0f;
        }
        return 0.0f;
    }
}

public class StoredActionMove : StoredAction
{
    public StoredActionMove(Player player)
    {
        Vector3 m_posBeforeMove = player.transform.position;
        float gravitySpeed = _CalcGravitySpeed(player.gravityPerTurn);

        action = () =>
        {
            player.characterController.Move(Vector3.down * gravitySpeed * Time.deltaTime);
            actionHasDone = _CheckProcessInputHasOverMinimumTime();
        };
    }

    public StoredActionMove(Player player, Vector3 direction, float range = 1)
    {
        Vector3 posBeforeMove = player.characterController.transform.position;
        float moveSpeed = 1.0f / PhaseManager.Instance.processInput.minimumTimeBeforeNextPhase;
        float gravitySpeed = _CalcGravitySpeed(player.gravityPerTurn);
        bool hasCollided = false;

        action = () =>
        {
            Transform transform = player.transform;
            Vector3 localTarget = transform.right * direction.x * range + transform.forward * direction.z * range;
            Vector3 target = new Vector3(posBeforeMove.x + localTarget.x, transform.position.y, posBeforeMove.z + localTarget.z);

            if (!_CheckCollisionInDirection(player.collisionChecker, direction) && !hasCollided)
            {
                if (Vector3.Distance(transform.position, target) > 0.1f && !_CheckProcessInputHasOverMinimumTime())
                {
                    //if (player.playerId == 0) Debug.Log(Time.deltaTime);
                    player.characterController.Move(localTarget * moveSpeed * Time.deltaTime + Vector3.down * gravitySpeed * Time.deltaTime);
                }
                else
                {
                    transform.position = target;
                    actionHasDone = true;
                }
            } else
            {
                hasCollided = true;
                Vector3 newPosBeforeMove = new Vector3(posBeforeMove.x, transform.position.y, posBeforeMove.z);
                if (Vector3.Distance(transform.position, newPosBeforeMove) > 0.1f)
                {
                    player.characterController.Move(-localTarget * moveSpeed * Time.deltaTime + Vector3.down * gravitySpeed * Time.deltaTime);
                }
                else
                {
                    transform.position = newPosBeforeMove;
                    actionHasDone = true;
                }
            }
        };
    }

    private bool _CheckCollisionInDirection(CollisionChecker collisionChecker, Vector3 direction)
    {
        if (direction.z > 0.0f)
            return collisionChecker.frontCollider.GetColliders().Count > 0.0f;

        if (direction.z < 0.0f)
            return collisionChecker.backCollider.GetColliders().Count > 0.0f;

        if (direction.x > 0.0f)
            return collisionChecker.rightCollider.GetColliders().Count > 0.0f;

        if (direction.x < 0.0f)
            return collisionChecker.leftCollider.GetColliders().Count > 0.0f;

        return false;
    }

    private float _CalcGravitySpeed(int gravityPerTurn)
    {
        return gravityPerTurn / PhaseManager.Instance.processInput.minimumTimeBeforeNextPhase;
    }
}