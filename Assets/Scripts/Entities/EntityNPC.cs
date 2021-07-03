﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

public enum AlertStateEnum
{
    Idle,
    Suspicious,
    Alert,
    SearchSuspicious,
    SearchAlert
}

public class EntityNPC : Entity
{
    public AlertStateEnum alertState { get; protected set; } = AlertStateEnum.Idle;

    [SerializeField] protected UnityEvent[] m_idleWaitInputs;
    protected int currentIdle = 0;

    public int currentAlertLevel { get; protected set; } = 0;

    public override void WaitInput()
    {
        _DoIdle();
    }

    protected int m_skipTurnCount = 0;
    public virtual void SkipTurnEntity(int turns = 1)
    {
        storedActions.Add(new StoredActionSkip());
        if (alertState == AlertStateEnum.Idle)
        {
            if (m_skipTurnCount == 0)
                m_skipTurnCount = turns;
            else
                m_skipTurnCount = Mathf.Max(m_skipTurnCount - 1, 0);

            if (m_skipTurnCount == 0)
                _NextIdle();
        }
    }

    public virtual void TurnToDegreeEntity(float degree)
    {
        storedActions.Add(new StoredActionTurn(this, degree));

        if (alertState == AlertStateEnum.Idle)
            _NextIdle();
    }

    protected virtual void _DoIdle()
    {
        if (m_idleWaitInputs.Length == 0)
        {
            SkipTurnEntity();
            return;
        }

        m_idleWaitInputs[currentIdle].Invoke();
    }

    protected virtual void _NextIdle()
    {
        currentIdle = (int)Mathf.Repeat(currentIdle + 1, m_idleWaitInputs.Length);
    }
}
