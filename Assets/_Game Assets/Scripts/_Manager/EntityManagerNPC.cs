using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityManagerNPC : EntityManager
{
    public override void SetupEntitiesOnLevelStart()
    {
        base.SetupEntitiesOnLevelStart();
        _AssignNPCsToGrid();
        _SetNPCsIsActive();
    }

    private void _AssignNPCsToGrid()
    {
        foreach(EntityCharacterNPC npc in entities)
        {
            npc.AssignToLevelGrid();
        }
    }

    private void _SetNPCsIsActive()
    {
        foreach(EntityCharacterNPC npc in entities)
        {
            npc.SetIsUpdateAble(true);
        }
    }
}
