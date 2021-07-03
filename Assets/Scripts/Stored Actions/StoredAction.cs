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
        return GameManager.Instance.phaseManager.processInput.currentTimeBeforeNextPhase >= GameManager.Instance.phaseManager.processInput.minimumTimeBeforeNextPhase;
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