using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityPlayer : Entity
{
    public bool isPlayable { get { return gameObject.activeSelf; } }
    public void SetIsPlayable(bool newIsPlayable) { gameObject.SetActive(newIsPlayable); }
    public int playerId { get { return transform.GetSiblingIndex(); } }

    [SerializeField] CameraLook m_playerCameraLook;
    public CameraLook playerCameraLook { get { return m_playerCameraLook; } }

    public bool isDead = false;

    public override void WaitInput()
    {
        if(!isDead)
        {
            float moveH = Input.GetAxisRaw("Horizontal" + " #" + playerId);
            float moveV = Input.GetAxisRaw("Vertical" + " #" + playerId);
            float camH = Input.GetAxis("Camera X" + " #" + playerId);
            float camV = Input.GetAxis("Camera Y" + " #" + playerId);
            bool moveMod = Input.GetButton("Move Modifier" + " #" + playerId);
            bool camMod = Input.GetButton("Camera Modifier 1" + " #" + playerId) || Input.GetButton("Camera Modifier 2" + " #" + playerId);

            bool isMoving = Mathf.Abs(moveH) > 0.0f || Mathf.Abs(moveV) > 0.0f;
            if (isMoving && !camMod)
            {
                float moveRange = 1;
                Vector3 moveDir = (Mathf.Abs(_FCInput(moveH) + _FCInput(moveV)) != 1.0f) ? new Vector3(0.0f, 0.0f, _FCInput(moveV)) : new Vector3(_FCInput(moveH), 0.0f, _FCInput(moveV));
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

                storedActions.Add(new StoredActionTurn(this, m_playerCameraLook.currentCameraRot));
                storedActions.Add(new StoredActionMove(this, moveDir, moveRange));

            }
        }
    }

    protected override void Awake()
    {
        base.Awake();
        m_playerCameraLook.SetupCameraLook(this);
    }

    private float _FCInput(float input)
    {
        return (input < 0.0f) ? Mathf.Floor(input) : Mathf.Ceil(input);
    }
}
