using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public List<StoredAction> storedActions { get; private set; } = new List<StoredAction>();
    public bool afterActionHasDone { get; private set; } = false; // temp
    public int gravityPerTurn { get; private set; } = 3;


    public bool isPlayable { get { return gameObject.activeSelf; } set { gameObject.SetActive(isPlayable); } }
    public int playerId { get { return transform.GetSiblingIndex(); } }

    public CharacterController characterController { get; private set; }

    [SerializeField] CollisionChecker m_collisionChecker;
    public CollisionChecker collisionChecker { get { return m_collisionChecker; } }

    [SerializeField] Camera m_playerCamera;
    public Camera playerCamera { get { return m_playerCamera; } }
    private Vector3 m_currentCameraRot;

    public void SetupWaitInput()
    {
        storedActions.Clear();
    }

    public void WaitInput()
    {
        float moveH = Input.GetAxisRaw("Horizontal");
        float moveV = Input.GetAxisRaw("Vertical");
        float camH = Input.GetAxis("Mouse X");
        float camV = Input.GetAxis("Mouse Y");
        bool moveMod = Input.GetButton("Move Modifier");
        bool camMod = Input.GetButton("Camera Modifier 1") || Input.GetButton("Camera Modifier 2");

        Vector2 camRot = (camMod) ? new Vector2(moveH, moveV) : new Vector2(camH, camV);
        _HandleCameraWaitInput(camRot.x, camRot.y, moveMod);

        bool isMoving = Mathf.Abs(moveH) > 0.0f || Mathf.Abs(moveV) > 0.0f;
        if (isMoving && !camMod)
        {
            float moveRange = 1;
            Vector3 moveDir = (Mathf.Abs(Mathf.Round(moveH) + Mathf.Round(moveV)) != 1.0f) ? new Vector3(0.0f, 0.0f, Mathf.Round(moveV)) : new Vector3(Mathf.Round(moveH), 0.0f, Mathf.Round(moveV));
            print(moveDir);
            if (moveMod)
            {
                if (moveV < 0.0f)
                {
                    storedActions.Add(new StoredActionMove(this));
                    storedActions.Add(new StoredActionSkip());
                    return;
                }

                moveRange = 2;
            }

            storedActions.Add(new StoredActionTurn(this, m_currentCameraRot));
            storedActions.Add(new StoredActionMove(this, moveDir, moveRange));

        }
    }

    public void SetupProcessInput()
    {
        foreach (StoredAction storedAction in storedActions)
            storedAction.actionHasDone = false;
    }

    public bool CheckAllActionHasDone()
    {
        foreach (StoredAction storedAction in storedActions)
        {
            if (!storedAction.actionHasDone) return false;
        }

        return true;
    }

    public void SetupAfterInput()
    {
        afterActionHasDone = false;
    }

    public void AfterInput()
    {
        Debug.Log("done move");
        afterActionHasDone = true;
    }

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    private void _HandleCameraWaitInput(float camH, float camV, bool moveMod)
    {
        m_currentCameraRot.y = Mathf.Repeat(m_currentCameraRot.y + camH, 360);
        m_currentCameraRot.x = Mathf.Clamp(m_currentCameraRot.x - camV, -60.0f, 90.0f);

        m_playerCamera.transform.parent.rotation = Quaternion.Euler(m_currentCameraRot.x, m_currentCameraRot.y, 0.0f);
    }
}
