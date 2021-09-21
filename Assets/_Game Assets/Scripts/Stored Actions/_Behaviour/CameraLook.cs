using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLook : MonoBehaviour
{
    public bool enableCameraLook = true;

    private bool m_hasBeenSetup = false;

    private Vector3 m_currentCameraRot;
    public Vector3 currentCameraRot { get { return m_currentCameraRot; } }

    [SerializeField] Camera m_playerCamera;
    public Camera playerCamera { get { return m_playerCamera; } }

    [SerializeField] Vector2 m_xRotClamp = new Vector2(-60.0f, 90.0f);

    public void SetupCameraWaitInput()
    {
        if(!m_hasBeenSetup)
        {
            m_currentCameraRot = transform.rotation.eulerAngles;
            m_hasBeenSetup = true;
        }
    }

    public void HandleCameraWaitInput(float camH, float camV)
    {
        if (!enableCameraLook)
            return;

        m_currentCameraRot.y = Mathf.Repeat(m_currentCameraRot.y + camH, 360);
        m_currentCameraRot.x = Mathf.Clamp(m_currentCameraRot.x - camV, m_xRotClamp.x, m_xRotClamp.y);

        transform.rotation = Quaternion.Euler(m_currentCameraRot.x, m_currentCameraRot.y, 0.0f);
    }

    public void CloneCameraWaitInput(CameraLook copyCameraLook)
    {
        if (!enableCameraLook)
            return;

        m_currentCameraRot = copyCameraLook.currentCameraRot;

        transform.rotation = Quaternion.Euler(m_currentCameraRot.x, m_currentCameraRot.y, 0.0f);
    }
}
