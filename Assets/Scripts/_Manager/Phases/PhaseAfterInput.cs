using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhaseAfterInput : MonoBehaviour
{
    public void ActivateAfterInput()
    {
        gameObject.SetActive(true);

        GameManager.Instance.playerManager.SetupEntitiesOnAfterInputStart();
        GameManager.Instance.enemyManager.SetupEntitiesOnAfterInputStart();
    }

    public void UpdateAfterInput()
    {
        bool playerManagerHasDoneAfterInput = GameManager.Instance.playerManager.CheckEntitiesHasDoneAfterInput();
        bool enemyManagerHasDoneAfterInput = GameManager.Instance.enemyManager.CheckEntitiesHasDoneAfterInput();

        if (playerManagerHasDoneAfterInput && enemyManagerHasDoneAfterInput)
        {
            GameManager.Instance.phaseManager.SetPhase(PhaseEnum.WaitInput);
        }
    }
}
