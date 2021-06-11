using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EntityEnemyLaser : EntityEnemy
{
    [SerializeField] CollisionChecker m_laserTrigger;

    LineRenderer m_laserLine;
    Vector3 m_laserLineDefaultEndPos;

    public override void WaitInput()
    {
        switch (patrolState)
        {
            case PatrolStateEnum.Idle:
                _DoPatrol();
                break;
            case PatrolStateEnum.Alert:
                SkipTurnEntity();
                print("PLEASE MOVE AWAY FROM MY LASER");
                break;
        }
    }

    public override void AfterInput()
    {
        var colliders = m_laserTrigger.triggerCollider.GetCollidersWithFilter<TagEntityUnpassable>();
        if (colliders.Count > 0)
        {
            float closestDistance = Mathf.Infinity;
            TagEntityUnpassable closestEntity = null;
            foreach (TagEntityUnpassable entity in colliders)
            {
                float newDistance = Vector3.Distance(m_laserTrigger.transform.position, entity.transform.position);
                if (newDistance < closestDistance)
                {
                    closestDistance = newDistance;
                    closestEntity = entity;
                }
            }

            m_laserLine.SetPosition(1, new Vector3(closestEntity.transform.position.x, m_laserLineDefaultEndPos.y, closestEntity.transform.position.z));
            if (closestEntity && closestEntity.GetComponent<TagEntityLaserTriggerable>())
            {
                patrolState = PatrolStateEnum.Alert;
            }
        }
        else
        {
            patrolState = PatrolStateEnum.Idle;
            m_laserLine.SetPosition(1, m_laserLineDefaultEndPos);
        }

        afterActionHasDone = true;
    }

    protected override void Awake()
    {
        base.Awake();
        m_laserLine = m_laserTrigger.GetComponent<LineRenderer>();
        m_laserLineDefaultEndPos = m_laserLine.GetPosition(1);
    }
}
