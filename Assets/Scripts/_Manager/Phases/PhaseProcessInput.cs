using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhaseProcessInput : MonoBehaviour
{
    // kasih waktu 0.25f
    [SerializeField] float m_minimumTimeBeforeNextPhase = 0.25f;
    public float minimumTimeBeforeNextPhase { get { return m_minimumTimeBeforeNextPhase; } }
    public float currentTimeBeforeNextPhase { get; private set; } = 0.0f;

    public void UpdateProcessInput()
    {
        // process all input from player, enemy, & level
        // before processing, check if able to process or not

        bool playerManagerHasDoneProcess = PlayerManager.Instance.CheckAllPlayersHasDoneProcess();
        bool enemyManagerHasDoneProcess = EnemyManager.Instance.CheckAllEnemiesHasDoneProcess();
        bool levelManagerHasDoneProcess = true;

        currentTimeBeforeNextPhase += Time.deltaTime;
        bool timeHasPassed = currentTimeBeforeNextPhase >= m_minimumTimeBeforeNextPhase;

        if (timeHasPassed && playerManagerHasDoneProcess && enemyManagerHasDoneProcess && levelManagerHasDoneProcess)
        {
            PhaseManager.Instance.SetPhase(PhaseEnum.AfterInput);
        }
    }

    private void OnEnable()
    {
        currentTimeBeforeNextPhase = 0.0f;
        // hasdoneaction semua nya false
        PlayerManager.Instance.SetupAllPlayersProcessInput();
        EnemyManager.Instance.SetupAllEnemiesProcessInput();
    }
}
