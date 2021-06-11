using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EntityEnemyCamera : EntityEnemy
{
    [SerializeField] ConeOfVision m_coneOfVision;

    public override void WaitInput()
    {
        switch (patrolState)
        {
            case PatrolStateEnum.Idle:
                _DoPatrol();
                break;
            case PatrolStateEnum.Alert:
                SkipTurnEntity();
                // selalu liat targetnya kah?
                print("PLEASE. SOMEONE. DO. SOMETHING.");
                break;
        }
    }

    public override void AfterInput()
    {
        m_coneOfVision.UpdateIsInCone();
        m_coneOfVision.UpdateAlertTargetEntity();
        switch (patrolState)
        {
            case PatrolStateEnum.Idle:
                currentAlertLevel = 0;

                if (m_coneOfVision.isInConeList.ContainsValue(IsInConeArea.AlertArea))
                {
                    patrolState = PatrolStateEnum.Alert;
                    print("INTRUDER. ALERT.");
                }
                afterActionHasDone = true;
                break;
            case PatrolStateEnum.Alert:
                currentAlertLevel = EnemyManager.Instance.maxAlertLevel;

                if (m_coneOfVision.CheckAllIsInConeAreTheSame(IsInConeArea.OutOfArea))
                {
                    patrolState = PatrolStateEnum.Idle;
                    afterActionHasDone = true;
                    print("INTRUDER. IS. MISSING.");
                    break;
                }

                afterActionHasDone = true;
                break;
        }
    }

    protected override void Awake()
    {
        base.Awake();
        m_coneOfVision.SetupConeOfVision(this);
    }
}
