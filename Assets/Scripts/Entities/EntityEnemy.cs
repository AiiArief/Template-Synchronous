using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum PatrolStateEnum
{
    Idle,
    Suspicious,
    Alert,
    SearchSuspicious,
    SearchAlert
}

public class EntityEnemy : Entity
{
    [SerializeField] ConeOfVision m_coneOfVision;

    PatrolStateEnum m_patrolState = PatrolStateEnum.Idle;

    [SerializeField] int m_maxAlertLevel = 15;
    [SerializeField] int m_susAlertLevelInc = 4;
    int m_currentAlertLevel = 0;

    [SerializeField] UnityEvent[] m_patrolWaitInputs; // bikin class baru ??
    int currentPatrol = 0;

    Entity m_alertTargetEntity;
    Vector3 m_lastTargetPos;

    public override void WaitInput()
    {
        switch (m_patrolState)
        {
            case PatrolStateEnum.Idle:
                if (m_patrolWaitInputs.Length == 0)
                {
                    base.WaitInput();
                    return;
                }

                m_patrolWaitInputs[currentPatrol].Invoke();
                break;
            case PatrolStateEnum.Suspicious:
                MoveToPointEntity(m_alertTargetEntity.transform);
                break;
            case PatrolStateEnum.Alert:
                MoveToPointEntity(m_alertTargetEntity.transform);
                break;
            case PatrolStateEnum.SearchAlert:
                MoveToPointEntity(m_lastTargetPos);
                // kalo udah sampe liat2 sekitar
                break;
            case PatrolStateEnum.SearchSuspicious:
                MoveToPointEntity(m_lastTargetPos);
                // kalo udah sampe liat2 sekitar
                break;
        }
    }

    // holy shit, this is unoptimized as hell lmaooo
    // even hell is more optimized than this
    public override void AfterInput()
    {
        m_coneOfVision.UpdateIsInCone();
        _UpdateAlertTargetEntity();
        switch (m_patrolState)
        {
            case PatrolStateEnum.Idle:
                if (m_coneOfVision.isInConeList.ContainsValue(IsInConeArea.SuspiciousArea))
                {
                    m_patrolState = PatrolStateEnum.Suspicious;
                    print("what is that nice?");
                }

                if (m_coneOfVision.isInConeList.ContainsValue(IsInConeArea.AlertArea))
                {
                    m_patrolState = PatrolStateEnum.Alert; 
                    print("someone is here!!!");
                }
                afterActionHasDone = true;
                break;
            case PatrolStateEnum.Suspicious:
                m_currentAlertLevel = Mathf.Min(m_currentAlertLevel + m_susAlertLevelInc, m_maxAlertLevel);

                if (m_currentAlertLevel >= m_maxAlertLevel || m_coneOfVision.isInConeList.ContainsValue(IsInConeArea.AlertArea))
                {
                    m_patrolState = PatrolStateEnum.Alert;
                    print("someone is here!!");
                }

                if (m_coneOfVision.CheckAllIsInConeAreTheSame(IsInConeArea.OutOfArea))
                {
                    m_patrolState = PatrolStateEnum.SearchSuspicious;
                    print("must be a wind?");
                }

                afterActionHasDone = true;
                break;
            case PatrolStateEnum.Alert:
                m_currentAlertLevel = m_maxAlertLevel;

                if (m_coneOfVision.CheckAllIsInConeAreTheSame(IsInConeArea.OutOfArea))
                {
                    m_patrolState = PatrolStateEnum.SearchAlert;
                    afterActionHasDone = true;
                    print("where aree yoouuuu");
                    break;
                }

                EntityPlayer coughtPlayer = _CheckCanCatchPlayer();
                if (coughtPlayer)
                {
                    coughtPlayer.isDead = true; // temp
                    afterActionHasDone = true;
                    break;
                } else
                {
                    afterActionHasDone = true;
                }
                break;
            case PatrolStateEnum.SearchAlert:
                m_currentAlertLevel = Mathf.Max(m_currentAlertLevel - 1, 0);

                if (m_currentAlertLevel <= 0)
                {
                    m_patrolState = PatrolStateEnum.Idle;
                    print("damn he's already escaped");
                }

                if (!m_coneOfVision.isInConeList.ContainsValue(IsInConeArea.OutOfArea))
                {
                    m_patrolState = PatrolStateEnum.Alert;
                    print("there you are!");
                }

                afterActionHasDone = true;
                break;
            case PatrolStateEnum.SearchSuspicious:
                m_currentAlertLevel = Mathf.Max(m_currentAlertLevel - 1, 0);

                if (m_currentAlertLevel <= 0)
                {
                    m_patrolState = PatrolStateEnum.Idle;
                    print("yea probably just my imagination");
                }

                if (m_coneOfVision.isInConeList.ContainsValue(IsInConeArea.SuspiciousArea))
                {
                    m_patrolState = PatrolStateEnum.Suspicious;
                    print("there is something ...");
                }

                if (m_coneOfVision.isInConeList.ContainsValue(IsInConeArea.AlertArea))
                {
                    m_patrolState = PatrolStateEnum.Alert;
                    print("!!!!");
                }

                afterActionHasDone = true;
                break;
        }
    }

    public void SkipTurnEntity()
    {
        storedActions.Add(new StoredActionMove(this));
        storedActions.Add(new StoredActionSkip());

        if (m_patrolState == PatrolStateEnum.Idle)
            _NextPatrol();
    }

    public void TurnToDegreeEntity(float degree)
    {
        storedActions.Add(new StoredActionMove(this));
        storedActions.Add(new StoredActionTurn(this, degree));

        if (m_patrolState == PatrolStateEnum.Idle)
            _NextPatrol();
    }

    public void MoveToPointEntity(Transform waypoint) { MoveToPointEntity(waypoint.position, waypoint.GetComponent<TagEntityUnpassable>()); }
    public bool MoveToPointEntity(Vector3 point, bool isUnpassable = false)
    {
        if(Vector3.Distance(transform.position, point) > 1.0f)
        {
            float angle = Mathf.Atan2(point.x - transform.position.x, point.z - transform.position.z) * Mathf.Rad2Deg;

            storedActions.Add(new StoredActionTurn(this, angle, false));
            storedActions.Add(new StoredActionMove(this, Vector3.forward));
        } else
        {
            SkipTurnEntity();
            return true;
        }

        if (m_patrolState == PatrolStateEnum.Idle)
            if (Vector3.Distance(transform.position, point) < (!isUnpassable ? 1.1f : 2.1f))
                _NextPatrol();

        return false;
    }

    protected override void Awake()
    {
        base.Awake();
        m_coneOfVision.SetupConeOfVision(this);
    }

    private void _NextPatrol()
    {
        currentPatrol = (int)Mathf.Repeat(currentPatrol + 1, m_patrolWaitInputs.Length);
    }

    private void _UpdateAlertTargetEntity()
    {
        m_alertTargetEntity = null;
        foreach(KeyValuePair<Entity, IsInConeArea> entry in m_coneOfVision.isInConeList)
        {
            if(entry.Value == IsInConeArea.SuspiciousArea || entry.Value == IsInConeArea.AlertArea)
            {
                if (m_alertTargetEntity == null)
                    m_alertTargetEntity = entry.Key;
                else
                    m_alertTargetEntity = (Vector3.Distance(entry.Key.transform.position, transform.position) < Vector3.Distance(m_alertTargetEntity.transform.position, transform.position)) ? entry.Key : m_alertTargetEntity;

                m_lastTargetPos = m_alertTargetEntity.transform.position;
            }
        }
    }

    private EntityPlayer _CheckCanCatchPlayer()
    {
        foreach (EntityPlayer player in PlayerManager.Instance.players)
        {
            if (!player.isPlayable)
                continue;

            bool isInFront = Vector3.Distance(collisionChecker.frontCollider2.transform.position, player.transform.position) < 1.0f;
            if (isInFront) return player;
        }

        return null;
    }
}
