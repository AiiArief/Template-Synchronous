using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueString
{
    public string name;
    public string dialogue;

    public DialogueString(string name, string dialogue)
    {
        this.name = name;
        this.dialogue = dialogue;
    }
}

public class UIHUDDialogue : MonoBehaviour
{
    [SerializeField] Text m_dialogueNameText;
    public Text dialogueNameText { get { return m_dialogueNameText; } }

    [SerializeField] Text m_dialogueText;
    public Text dialogueText { get { return m_dialogueText; } }

    [SerializeField] float m_textSpeed = 0.25f;
    public float textSpeed { get { return m_textSpeed; } }

    public List<DialogueString> dialogueStrings { get; private set; } = new List<DialogueString>();

    public bool CheckDialogueAvailable()
    {
        return dialogueStrings.Count > 0;
    }

    public void ShowHideDialogue(bool show)
    {
        gameObject.SetActive(show);
    }
}