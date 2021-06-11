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