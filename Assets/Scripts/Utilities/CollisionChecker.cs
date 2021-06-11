using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionChecker : MonoBehaviour
{
    [SerializeField]
    CollisionCheckerChild m_trigger;

    public CollisionCheckerChild triggerCollider { get { return m_trigger; } }
}
