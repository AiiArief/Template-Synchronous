using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EntityEnemyGuard : EntityEnemy
{
    [SerializeField] ConeOfVision m_coneOfVision;

    Vector3 m_guardedArea;

    public override void WaitInput()
    {
        switch (patrolState)
        {
            case PatrolStateEnum.Idle:
                _DoPatrol();
                break;
            case PatrolStateEnum.Suspicious:
                _MoveToPointEntity(m_coneOfVision.alertTargetEntity.transform.position);
                break;
            case PatrolStateEnum.Alert:
                _MoveToPointEntity(m_coneOfVision.alertTargetEntity.transform.position);
                break;
            case PatrolStateEnum.SearchAlert:
                _MoveToPointEntity(m_coneOfVision.lastTargetPos);
                // kalo udah sampe liat2 sekitar
                break;
            case PatrolStateEnum.SearchSuspicious:
                _MoveToPointEntity(m_coneOfVision.lastTargetPos);
                // kalo udah sampe liat2 sekitar
                break;
        }
    }

    // holy shit, this is unoptimized as hell lmaooo
    // even hell is more optimized than this
    public override void AfterInput()
    {
        m_coneOfVision.UpdateIsInCone();
        m_coneOfVision.UpdateAlertTargetEntity();
        switch (patrolState)
        {
            case PatrolStateEnum.Idle:
                if (m_coneOfVision.isInConeList.ContainsValue(IsInConeArea.SuspiciousArea))
                {
                    patrolState = PatrolStateEnum.Suspicious;
                    // puter cone of vision
                    print("is someone there?");
                }

                if (m_coneOfVision.isInConeList.ContainsValue(IsInConeArea.AlertArea))
                {
                    patrolState = PatrolStateEnum.Alert;
                    print("someone is here!!!");
                }
                afterActionHasDone = true;
                break;
            case PatrolStateEnum.Suspicious:
                currentAlertLevel = Mathf.Min(currentAlertLevel + m_coneOfVision.susAlertLevelInc, EnemyManager.Instance.maxAlertLevel);

                if (currentAlertLevel >= EnemyManager.Instance.maxAlertLevel || m_coneOfVision.isInConeList.ContainsValue(IsInConeArea.AlertArea))
                {
                    patrolState = PatrolStateEnum.Alert;
                    print("someone is here!!");
                }

                if (m_coneOfVision.CheckAllIsInConeAreTheSame(IsInConeArea.OutOfArea))
                {
                    patrolState = PatrolStateEnum.SearchSuspicious;
                    print("must be a wind?");
                }

                afterActionHasDone = true;
                break;
            case PatrolStateEnum.Alert:
                currentAlertLevel = EnemyManager.Instance.maxAlertLevel;

                if (m_coneOfVision.CheckAllIsInConeAreTheSame(IsInConeArea.OutOfArea))
                {
                    patrolState = PatrolStateEnum.SearchAlert;
                    afterActionHasDone = true;
                    print("where aree yoouuuu");
                    break;
                }

                EntityPlayer caughtPlayer = _CheckCanCatchPlayer();
                if (caughtPlayer)
                {
                    caughtPlayer.isDead = true; // temp
                    afterActionHasDone = true;
                    break;
                }
                else
                {
                    afterActionHasDone = true;
                }
                break;
            case PatrolStateEnum.SearchAlert:
                currentAlertLevel = Mathf.Max(currentAlertLevel - 1, 0);

                if (currentAlertLevel <= 0)
                {
                    patrolState = PatrolStateEnum.Idle;
                    print("damn he's already escape");
                }

                if (!m_coneOfVision.isInConeList.ContainsValue(IsInConeArea.OutOfArea))
                {
                    patrolState = PatrolStateEnum.Alert;
                    print("there you are!");
                }

                afterActionHasDone = true;
                break;
            case PatrolStateEnum.SearchSuspicious:
                currentAlertLevel = Mathf.Max(currentAlertLevel - 1, 0);

                if (currentAlertLevel <= 0)
                {
                    patrolState = PatrolStateEnum.Idle;
                    print("yea probably just my imagination");
                }

                if (m_coneOfVision.isInConeList.ContainsValue(IsInConeArea.SuspiciousArea))
                {
                    patrolState = PatrolStateEnum.Suspicious;
                    print("there is something ...");
                }

                if (m_coneOfVision.isInConeList.ContainsValue(IsInConeArea.AlertArea))
                {
                    patrolState = PatrolStateEnum.Alert;
                    print("!!!!");
                }

                afterActionHasDone = true;
                break;
        }
    }

    public override void SkipTurnEntity(int turns = 1)
    {
        storedActions.Add(new StoredActionMove(this));
        base.SkipTurnEntity(turns);
    }

    public override void TurnToDegreeEntity(float degree)
    {
        storedActions.Add(new StoredActionMove(this));
        base.TurnToDegreeEntity(degree);
    }

    public void SetGuardArea(Transform waypoint)
    {
        m_guardedArea = waypoint.position;
        _NextPatrol();
    }

    protected override void Awake()
    {
        base.Awake();
        m_coneOfVision.SetupConeOfVision(this);
        m_guardedArea = transform.position;
    }

    protected override void _DoPatrol()
    {
        if (!_CheckHasArrivedAtPoint(m_guardedArea, false))
        {
            _MoveToPointEntity(m_guardedArea);
        }
        else
        {
            base._DoPatrol();
        }
    }

    private bool _CheckHasArrivedAtPoint(Vector3 point, bool isUnpassable)
    {
        return Vector3.Distance(transform.position, point) < (isUnpassable ? 1.1f : 0.1f);
    }

    private void _MoveToPointEntity(Vector3 point)
    {
        List<LevelGridNode> nodes = LevelManager.Instance.pathfinding.FindPath(transform.position, point);
        if (nodes == null || nodes.Count <= 1)
        {
            SkipTurnEntity();
            return;
        }

        LevelGridNode nextNode = nodes[1]; // kalo pointnya masih sama kayak dulu ga usah find path
        float angle = Mathf.Atan2(nextNode.realWorldPos.x - transform.position.x, nextNode.realWorldPos.z - transform.position.z) * Mathf.Rad2Deg;

        storedActions.Add(new StoredActionTurn(this, angle, false));
        storedActions.Add(new StoredActionMove(this, Vector3.forward));
    }
}
