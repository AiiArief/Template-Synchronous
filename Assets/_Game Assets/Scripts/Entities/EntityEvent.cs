using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EntityEvent: Entity
{
    protected EntityManagerEvent eventManager;
    protected EntityCharacterPlayer mainPlayer;

    public virtual void EventOnLoadLevel()
    {
        eventManager = GameManager.Instance.eventManager;
        mainPlayer = GameManager.Instance.playerManager.GetPlayerPlayableList()[0];
        // teleport player?
        // setup semua player pref?
    }
}
