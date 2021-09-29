using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityCharacterNPCGuard : EntityCharacterNPC
{
    Vector3 m_guardedArea; 
    bool m_guardedAreaHasBeenSetup = false;

    public override void SetupWaitInput()
    {
        base.SetupWaitInput();
    
        if(!m_guardedAreaHasBeenSetup)
        {
            m_guardedAreaHasBeenSetup = true;
            m_guardedArea = transform.position;
        }
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

    // kalo pathfindingnya masih sama gausah find path
    private void _MoveToPointEntity(Vector3 point)
    {
        LevelPathfinding pathfinding = new LevelPathfinding();
        List<LevelGridNode> nodes = pathfinding.FindPath(transform.position, point);
        if (nodes == null || nodes.Count <= 1)
        {
            SkipTurnNPC();
            return;
        }

        LevelGridNode nextNode = nodes[1];

        float angle = Mathf.Atan2(nextNode.realWorldPos.x - transform.position.x, nextNode.realWorldPos.z - transform.position.z) * Mathf.Rad2Deg;
        storedActions.Add(new StoredActionTurn(this, angle, false));

        Vector3 dir = (nextNode.realWorldPos - transform.position).normalized;
        storedActions.Add(new StoredActionMove(this, dir, true));
    }
}
