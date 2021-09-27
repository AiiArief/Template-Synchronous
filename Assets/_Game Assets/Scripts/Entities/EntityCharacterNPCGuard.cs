using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityCharacterNPCGuard : EntityCharacterNPC
{
    Vector3 m_guardedArea;

    public override void SetupWaitInput()
    {
        base.SetupWaitInput();
    
        if(m_guardedArea == null)
            m_guardedArea = transform.position;
    }

    public override void WaitInput()
    {
        _DoIdle();
    }

    public void SetGuardArea(Transform waypoint)
    {
        m_guardedArea = waypoint.position;
        _NextIdle();
    }

    protected override void _DoIdle()
    {
        if (!_CheckHasArrivedAtPoint(m_guardedArea, false))
        {
            _MoveToPointEntity(m_guardedArea);
        }
        else
        {
            base._DoIdle();
        }
    }

    private bool _CheckHasArrivedAtPoint(Vector3 point, bool isUnpassable)
    {
        return Vector3.Distance(transform.position, point) < (isUnpassable ? 1.1f : 0.1f);
    }

    private void _MoveToPointEntity(Vector3 point)
    {
        LevelPathfinding pathfinding = new LevelPathfinding();
        List<LevelGridNode> nodes = pathfinding.FindPath(transform.position, point);
        if (nodes == null || nodes.Count <= 1)
        {
            SkipTurnNPC();
            return;
        }

        LevelGridNode nextNode = nodes[1]; // kalo pointnya masih sama kayak dulu ga usah find path
        Vector3 dir = (nextNode.realWorldPos - transform.position).normalized;
        storedActions.Add(new StoredActionMove(this, dir));
        
        //float angle = Mathf.Atan2(nextNode.realWorldPos.x - transform.position.x, nextNode.realWorldPos.z - transform.position.z) * Mathf.Rad2Deg;
        //storedActions.Add(new StoredActionTurn(this, angle, false));        
    }
}
