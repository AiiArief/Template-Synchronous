using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityPlayer : Entity
{
    public int playerId { get { return transform.GetSiblingIndex(); } }

    [SerializeField] CameraLook m_playerCameraLook;
    public CameraLook playerCameraLook { get { return m_playerCameraLook; } }

    public bool isDead = false; // temp

    public override void WaitInput()
    {
        if(!isDead) // temp
        {
            float moveH = Input.GetAxisRaw("Horizontal" + " #" + playerId);
            float moveV = Input.GetAxisRaw("Vertical" + " #" + playerId);
            float camH = Input.GetAxis("Camera X" + " #" + playerId);
            float camV = Input.GetAxis("Camera Y" + " #" + playerId);
            bool moveMod = Input.GetButton("Move Modifier" + " #" + playerId);
            bool camMod = Input.GetButton("Camera Modifier 1" + " #" + playerId) || Input.GetButton("Camera Modifier 2" + " #" + playerId);
            bool skipTurn = _CheckDoubleInput("Move Modifier" + " #" + playerId, 0.5f);

            Vector2 camRot = (camMod) ? new Vector2(moveH, moveV) : new Vector2(camH, camV);
            playerCameraLook.HandleCameraWaitInput(camRot.x, camRot.y);

            if (skipTurn)
            {
                storedActions.Add(new StoredActionMove(this));
                storedActions.Add(new StoredActionSkip());
                storedActions.Add(new StoredActionCameraLook(this, m_playerCameraLook));
                return;
            }

            bool isMoving = Mathf.Abs(moveH) > 0.0f || Mathf.Abs(moveV) > 0.0f;
            if (isMoving && !camMod)
            {
                float moveRange = moveMod ? 2 : 1;
                Vector3 moveDir = (Mathf.Abs(_FCInput(moveH) + _FCInput(moveV)) != 1.0f) ? new Vector3(0.0f, 0.0f, _FCInput(moveV)) : new Vector3(_FCInput(moveH), 0.0f, _FCInput(moveV));

                storedActions.Add(new StoredActionTurn(this, m_playerCameraLook.currentCameraRot));
                storedActions.Add(new StoredActionMove(this, moveDir, moveRange));
                storedActions.Add(new StoredActionCameraLook(this, m_playerCameraLook));
                return;
            }
        }
    }

    private float _FCInput(float input)
    {
        return (input < 0.0f) ? Mathf.Floor(input) : Mathf.Ceil(input);
    }

    [HideInInspector] float m_buttonDownCount = 0;
    [HideInInspector] float m_buttonDownTime = 0;
    private bool _CheckDoubleInput(string inputName, float buttonDownDelay)
    {
        if (Input.GetButtonDown(inputName))
        {
            m_buttonDownCount++;
            if (m_buttonDownCount == 1) m_buttonDownTime = Time.time;
        }
        if (m_buttonDownCount > 1 && Time.time - m_buttonDownTime < buttonDownDelay)
        {
            m_buttonDownCount = 0;
            m_buttonDownTime = 0;
            return true;
        }
        else if (m_buttonDownCount > 2 || Time.time - m_buttonDownTime > 1) m_buttonDownCount = 0;
        return false;
    }
}
