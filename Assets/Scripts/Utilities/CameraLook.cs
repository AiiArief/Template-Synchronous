using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLook : MonoBehaviour
{
    private EntityPlayer m_player;

    private Vector3 m_currentCameraRot;
    public Vector3 currentCameraRot { get { return m_currentCameraRot; } }

    public Camera playerCamera { get { return transform.GetComponentInChildren<Camera>(); } }

    public void SetupCameraLook(EntityPlayer player)
    {
        // setup player id, setup string input
        m_player = player;
    }

    private void Update()
    {
        if (!m_player)
            return; 

        float moveH = Input.GetAxisRaw("Horizontal" + " #" + m_player.playerId);
        float moveV = Input.GetAxisRaw("Vertical" + " #" + m_player.playerId);
        float camH = Input.GetAxis("Camera X" + " #" + m_player.playerId);
        float camV = Input.GetAxis("Camera Y" + " #" + m_player.playerId);
        bool moveMod = Input.GetButton("Move Modifier" + " #" + m_player.playerId);
        bool camMod = Input.GetButton("Camera Modifier 1" + " #" + m_player.playerId) || Input.GetButton("Camera Modifier 2" + " #" + m_player.playerId);

        Vector2 camRot = (camMod) ? new Vector2(moveH, moveV) : new Vector2(camH, camV);
        _HandleCameraWaitInput(camRot.x, camRot.y, moveMod);
    }
    private void _HandleCameraWaitInput(float camH, float camV, bool moveMod)
    {
        m_currentCameraRot.y = Mathf.Repeat(m_currentCameraRot.y + camH, 360);
        m_currentCameraRot.x = Mathf.Clamp(m_currentCameraRot.x - camV, -60.0f, 90.0f);

        transform.rotation = Quaternion.Euler(m_currentCameraRot.x, m_currentCameraRot.y, 0.0f);
    }
}
