using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityManagerEvent: EntityManager
{
    [SerializeField] EntityEvent m_onLoadLevelEvent;

    // tambah action gitu disini? atau di entity event process input matiin tapi nyalain pas after input aja?
    public List<Action> afterInputActionList { get; private set; } = new List<Action>();

    public override void SetupEntitiesOnLevelStart()
    {
        base.SetupEntitiesOnLevelStart();

        m_onLoadLevelEvent.EventOnLoadLevel();
        _ExecuteFirstAction();
    }

    public override void SetupEntitiesOnAfterInputStart()
    {
        base.SetupEntitiesOnAfterInputStart();
        _ExecuteFirstAction();
    }

    private void _ExecuteFirstAction()
    {
        if(afterInputActionList.Count > 0)
        {
            afterInputActionList[0].Invoke();
            afterInputActionList.RemoveAt(0);
        }
    }
}
