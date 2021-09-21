using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// todo : ubah ke jeson
public class ProfileManager : MonoBehaviour
{
    public const string PLAYERPREFS_ISFIRSTTIMESAVE = "isFirstTimeSave";
    public const string PLAYERPREFS_ISMUTE = "isMute";

    private void Awake()
    {
        _SetupFirstTimeSave();
    }

    private void _SetupFirstTimeSave()
    {
        if (PlayerPrefs.GetString(PLAYERPREFS_ISFIRSTTIMESAVE, true.ToString()) == true.ToString())
        {
            PlayerPrefs.SetString(PLAYERPREFS_ISMUTE, false.ToString());
        }
    }
}
