using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionCheckerChild : MonoBehaviour
{
    private HashSet<Collider> m_colliders = new HashSet<Collider>();
    public HashSet<Collider> GetColliders() { return m_colliders; }

    public HashSet<T> GetCollidersWithFilter<T>()
    {
        HashSet<T> hashSet = new HashSet<T>();
        foreach(Collider collider in m_colliders)
        {
            T t = collider.GetComponent<T>();
            if(t != null)
            {
                hashSet.Add(t);
            }
        }
        return hashSet;
    }

    public bool CheckColliderHaveEntityTag<T>()
    {
        foreach(Collider collider in m_colliders)
        {
            if (!collider.isTrigger && collider.gameObject.isStatic)
                return true;
            if (collider.GetComponent<T>() != null)
                return true;
        }

        return false;
    }

    private void OnTriggerEnter(Collider other)
    {
        m_colliders.Add(other);
    }

    private void OnTriggerExit(Collider other)
    {
        m_colliders.Remove(other);
    }
}
