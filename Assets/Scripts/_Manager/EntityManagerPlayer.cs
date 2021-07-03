using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityManagerPlayer : EntityManager
{
    [SerializeField] int playerCountOnLevelStart = 1;

    public override void SetupEntitiesOnLevelStart()
    {
        base.SetupEntitiesOnLevelStart();
        _SetPlayersIsActive();
    }

    public List<EntityPlayer> GetPlayerPlayableList()
    {
        var playablePlayers = new List<EntityPlayer>();
        foreach (EntityPlayer player in entities)
        {
            if (player.isUpdateAble)
            {
                playablePlayers.Add(player);
            }
        }

        return playablePlayers;
    }

    public void SetPlayerPlayable(int playerId, bool set)
    {
        entities[playerId].SetIsUpdateAble(set);
        _SetupAllPlayersCamera(GetPlayerPlayableList());
    }

    private void _SetPlayersIsActive()
    {
        for (int i = 0; i < playerCountOnLevelStart; i++)
            entities[i].SetIsUpdateAble(true);

        for (int i = playerCountOnLevelStart; i < entities.Count; i++)
            entities[i].SetIsUpdateAble(false);

        _SetupAllPlayersCamera(GetPlayerPlayableList());
    }

    private void _SetupAllPlayersCamera(List<EntityPlayer> playablePlayers)
    {
        int playerCount = playablePlayers.Count;
        switch (playerCount)
        {
            case 1:
                playablePlayers[0].playerCameraLook.playerCamera.rect = new Rect(0, 0, 1, 1);
                break;
            case 2:
                playablePlayers[0].playerCameraLook.playerCamera.rect = new Rect(0, 0, 0.5f, 1);
                playablePlayers[1].playerCameraLook.playerCamera.rect = new Rect(0.5f, 0, 0.5f, 1);
                break;
            case 3:
                playablePlayers[0].playerCameraLook.playerCamera.rect = new Rect(0, 0.5f, 1, 0.5f);
                playablePlayers[1].playerCameraLook.playerCamera.rect = new Rect(0, 0, 0.5f, 0.5f);
                playablePlayers[2].playerCameraLook.playerCamera.rect = new Rect(0.5f, 0, 0.5f, 0.5f);
                break;
            case 4:
                playablePlayers[0].playerCameraLook.playerCamera.rect = new Rect(0, 0.5f, 0.5f, 0.5f);
                playablePlayers[1].playerCameraLook.playerCamera.rect = new Rect(0.5f, 0.5f, 0.5f, 0.5f);
                playablePlayers[2].playerCameraLook.playerCamera.rect = new Rect(0, 0, 0.5f, 0.5f);
                playablePlayers[3].playerCameraLook.playerCamera.rect = new Rect(0.5f, 0, 0.5f, 0.5f);
                break;
        }
    }
}
