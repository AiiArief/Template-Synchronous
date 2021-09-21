using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI : MonoBehaviour
{
    [SerializeField] UIHUDDialogue m_hudDialogue;
    public UIHUDDialogue HUDDialogue { get { return m_hudDialogue; } }

    public virtual void AddDialogue(DialogueString dialogueString)
    {
        HUDDialogue.dialogueStrings.Add(dialogueString);
    }
}
