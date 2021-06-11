using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionEntityChecker: MonoBehaviour
{
    [SerializeField]
    CollisionCheckerChild
        m_frontCollider,
        m_frontCollider2,
        m_backCollider,
        m_backCollider2,
        m_rightCollider,
        m_rightCollider2,
        m_leftCollider,
        m_leftCollider2,
        m_upCollider,
        m_downCollider;

    public CollisionCheckerChild frontCollider { get { return m_frontCollider; } }
    public CollisionCheckerChild frontCollider2 { get { return m_frontCollider2; } }
    public CollisionCheckerChild backCollider { get { return m_backCollider; } }
    public CollisionCheckerChild backCollider2 { get { return m_backCollider2; } }
    public CollisionCheckerChild rightCollider { get { return m_rightCollider; } }
    public CollisionCheckerChild rightCollider2 { get { return m_rightCollider2; } }
    public CollisionCheckerChild leftCollider { get { return m_leftCollider; } }
    public CollisionCheckerChild leftCollider2 { get { return m_leftCollider2; } }
    public CollisionCheckerChild upCollider { get { return m_upCollider; } }
    public CollisionCheckerChild downCollider { get { return m_downCollider; } }
}
