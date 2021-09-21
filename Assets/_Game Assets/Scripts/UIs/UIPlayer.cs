using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPlayer : UI
{
    [SerializeField] UIHUDGameplay m_hudGameplay;
    public UIHUDGameplay HUDGameplay { get { return m_hudGameplay; } }

    [SerializeField] Camera m_hudCamera;
    public Camera HUDCamera { get { return m_hudCamera; } }
}
