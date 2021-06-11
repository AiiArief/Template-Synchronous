using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConeOfVisionView : MonoBehaviour
{
    [SerializeField] EntityEnemy m_enemyPatrol;

    Light m_visionLight;

    private void Awake()
    {
        m_visionLight =  GetComponentInChildren<Light>();
    }

    private void Update()
    {
        switch(m_enemyPatrol.patrolState)
        {
            case PatrolStateEnum.Idle:
                m_visionLight.color = Color.white;
                break;
            case PatrolStateEnum.Suspicious:
                m_visionLight.color = Color.yellow;
                break;
            case PatrolStateEnum.Alert:
                m_visionLight.color = Color.red;
                break;
            case PatrolStateEnum.SearchSuspicious:
                m_visionLight.color = Color.yellow * new Color(1, 1, 1, 0.5f);
                break;
            case PatrolStateEnum.SearchAlert:
                m_visionLight.color = Color.red * new Color(1, 1, 1, 0.5f);
                break;
        }
    }
}
