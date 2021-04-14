using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhaseWaitInput : MonoBehaviour
{
    public void UpdateWaitInput()
    {
        // wait all input from player, enemy, and level
        // store all input to each own object
        // check has stored from player, enemy, and level --> change phase

        bool playerManagerHasDoneInput = PlayerManager.Instance.CheckAllPlayersHasDoneInput();
        bool enemyManagerHasDoneInput = true;
        bool levelManagerHasDoneInput = true;

        if(playerManagerHasDoneInput && enemyManagerHasDoneInput && levelManagerHasDoneInput)
        {
            PhaseManager.Instance.SetPhase(PhaseEnum.ProcessInput);
        }
    }

    private void OnEnable()
    {
        // stored action semuanya null
        PlayerManager.Instance.SetupAllPlayersWaitInput();

    }
}
