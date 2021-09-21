using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalGameManager : MonoBehaviour
{
    public static GlobalGameManager Instance = null;

    public ProfileManager profileManager;
    public SoundManager soundManager;
    public DatabaseManager databaseManager;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void OnEnable()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Cursor.lockState = CursorLockMode.Locked;
        Application.targetFrameRate = 60;
    }
}
