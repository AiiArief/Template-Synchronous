using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityManagerNPC : EntityManager
{
    public override void SetupEntitiesOnLevelStart()
    {
        base.SetupEntitiesOnLevelStart();
        _AssignNPCsToGrid();
    }

    private void _AssignNPCsToGrid()
    {
        foreach(EntityCharacterNPC npc in entities)
        {
            npc.AssignToLevelGrid();
        }
    }
}
