using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PhaseEnum
{
    None,
    WaitInput,
    ProcessInput,
    AfterInput
}

public class PhaseManager : MonoBehaviour
{
    public static PhaseManager Instance;

    public PhaseWaitInput waitInput { get; private set; }
    public PhaseProcessInput processInput { get; private set; }
    public PhaseAfterInput afterInput { get; private set; }

    public PhaseEnum currentPhase { get; private set; } = PhaseEnum.None;

    public void SetPhase(PhaseEnum phase)
    {
        currentPhase = phase;

        _DisableAllPhase();
        _ActivateCurrentPhase();
    }

    public void UpdateCurrentPhase()
    {
        switch(currentPhase)
        {
            case PhaseEnum.WaitInput:
                waitInput.UpdateWaitInput();
                break;
            case PhaseEnum.ProcessInput:
                processInput.UpdateProcessInput();
                break;
            case PhaseEnum.AfterInput:
                afterInput.UpdateAfterInput();
                break;
        }
    }

    private void Awake()
    {
        Instance = this;

        waitInput = GetComponentInChildren<PhaseWaitInput>();
        processInput = GetComponentInChildren<PhaseProcessInput>();
        afterInput = GetComponentInChildren<PhaseAfterInput>();
    }

    private void _DisableAllPhase()
    {
        waitInput.gameObject.SetActive(false);
        processInput.gameObject.SetActive(false);
        afterInput.gameObject.SetActive(false);
    }

    private void _ActivateCurrentPhase()
    {
        switch(currentPhase)
        {
            case PhaseEnum.WaitInput: waitInput.gameObject.SetActive(true); break;
            case PhaseEnum.ProcessInput: processInput.gameObject.SetActive(true); break;
            case PhaseEnum.AfterInput: afterInput.gameObject.SetActive(true); break;
        }
    }
}
