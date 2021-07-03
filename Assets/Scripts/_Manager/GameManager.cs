using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] PhaseManager m_phaseManager;
    public PhaseManager phaseManager { get { return m_phaseManager; } }

    [SerializeField] EntityManagerPlayer m_playerManager;
    public EntityManagerPlayer playerManager { get { return m_playerManager; } }

    [SerializeField] EntityManagerEnemy m_enemyManager;
    public EntityManagerEnemy enemyManager { get { return m_enemyManager; } }

    [SerializeField] LevelManager m_levelManager;
    public LevelManager levelManager { get { return m_levelManager; } }

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Application.targetFrameRate = 60;

        m_levelManager.SetupLevelOnLevelStart();
        m_playerManager.SetupEntitiesOnLevelStart();
        m_enemyManager.SetupEntitiesOnLevelStart();
        m_phaseManager.SetPhase(PhaseEnum.WaitInput);
    }

    private void Update()
    {
        if (m_phaseManager.currentPhase != PhaseEnum.None)
            m_phaseManager.UpdateCurrentPhase();
    }
}
