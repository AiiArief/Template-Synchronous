using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
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
    public PatrolStateEnum patrolState { get; protected set; } = PatrolStateEnum.Idle;

    [SerializeField] protected UnityEvent[] m_patrolWaitInputs; // bikin class baru ??
    protected int currentPatrol = 0;

    public int currentAlertLevel { get; protected set; } = 0;

    public override void WaitInput()
    {
        _DoPatrol();
    }

    public override void AfterInput()
    {
        afterActionHasDone = true;
    }

    protected int m_skipTurnCount = 0;
    public virtual void SkipTurnEntity(int turns = 1)
    {
        storedActions.Add(new StoredActionSkip());
        if (patrolState == PatrolStateEnum.Idle)
        {
            if (m_skipTurnCount == 0)
                m_skipTurnCount = turns;
            else
                m_skipTurnCount = Mathf.Max(m_skipTurnCount - 1, 0);

            if (m_skipTurnCount == 0)
                _NextPatrol();
        }
    }

    public virtual void TurnToDegreeEntity(float degree)
    {
        storedActions.Add(new StoredActionTurn(this, degree));

        if (patrolState == PatrolStateEnum.Idle)
            _NextPatrol();
    }

    protected virtual void _DoPatrol()
    {
        if (m_patrolWaitInputs.Length == 0)
        {
            SkipTurnEntity();
            return;
        }

        m_patrolWaitInputs[currentPatrol].Invoke();
    }

    protected virtual void _NextPatrol()
    {
        currentPatrol = (int)Mathf.Repeat(currentPatrol + 1, m_patrolWaitInputs.Length);
    }

    protected virtual EntityPlayer _CheckCanCatchPlayer()
    {
        foreach (EntityPlayer player in PlayerManager.Instance.players)
        {
            if (!player.isPlayable)
                continue;

            bool isInFront = Vector3.Distance(collisionEntityChecker.frontCollider2.transform.position, player.transform.position) < 1.0f;
            if (isInFront) return player;
        }

        return null;
    }
}
