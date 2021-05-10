using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhaseAfterInput : MonoBehaviour
{
    public void UpdateAfterInput()
    {
        // get after input from all player, enemy, and level
        // check below entities & forward entities
        // store it what will happened?
        // if all after input has done, set to wait input

        bool playerManagerHasDoneAfterInput = PlayerManager.Instance.CheckAllPlayersHasDoneAfterInput();
        bool enemyManagerHasDoneAfterInput = EnemyManager.Instance.CheckAllEnemiesHasDoneAfterInput();
        bool levelManagerHasDoneAfterInput = true;

        if (playerManagerHasDoneAfterInput && enemyManagerHasDoneAfterInput && levelManagerHasDoneAfterInput)
        {
            PhaseManager.Instance.SetPhase(PhaseEnum.WaitInput);
            // reset semuanya
        }
    }

    private void OnEnable()
    {
        // afteraction semuanya false
        PlayerManager.Instance.SetupAllPlayersAfterInput();
        EnemyManager.Instance.SetupAllEnemiesAfterInput();
    }
}
