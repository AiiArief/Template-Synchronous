using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;

    public List<EntityPlayer> players { get; private set; } = new List<EntityPlayer>();

    public void SetupPlayersOnLevelStart(int onLevelStartPlayerCount)
    {
        _AssignAllPlayers();
        _SetPlayersIsActive(onLevelStartPlayerCount);
    }

    public void SetupAllPlayersWaitInput()
    {
        foreach (EntityPlayer player in players)
        {
            if (player.isPlayable)
                player.SetupWaitInput();
        }
    }

    public bool CheckAllPlayersHasDoneInput()
    {
        foreach (EntityPlayer player in players)
        {
            if (player.isPlayable && player.storedActions.Count == 0)
                player.WaitInput();
        }

        foreach (EntityPlayer player in players)
        {
            if (player.isPlayable && player.storedActions.Count == 0)
                return false;
        }

        return true;
    }

    public void SetupAllPlayersProcessInput()
    {
        foreach (EntityPlayer player in players)
        {
            player.SetupProcessInput();
        }
    }

    public bool CheckAllPlayersHasDoneProcess()
    {
        foreach (EntityPlayer player in players)
        {
            if (player.isPlayable && !player.CheckAllActionHasDone())
            {
                foreach (StoredAction storedAction in player.storedActions)
                    if (!storedAction.actionHasDone)
                        storedAction.action.Invoke();
            }
        }

        foreach (EntityPlayer player in players)
        {
            if (player.isPlayable && !player.CheckAllActionHasDone())
                return false;
        }

        return true;
    }

    public void SetupAllPlayersAfterInput()
    {
        foreach (EntityPlayer player in players)
        {
            player.SetupAfterInput();
        }
    }

    public bool CheckAllPlayersHasDoneAfterInput()
    {
        foreach (EntityPlayer player in players)
        {
            if (player.isPlayable)
                player.AfterInput();
        }

        foreach (EntityPlayer player in players)
        {
            if (player.isPlayable && !player.afterActionHasDone)
                return false;
        }

        return true;
    }

    public List<EntityPlayer> GetPlayerPlayableList()
    {
        var playablePlayers = new List<EntityPlayer>();
        foreach (EntityPlayer player in players)
        {
            if (player.isPlayable)
                playablePlayers.Add(player);
        }

        return playablePlayers;
    }

    public void SetPlayerPlayable(int playerId, bool set)
    {
        players[playerId].SetIsPlayable(set);
        _SetupAllPlayersCamera(GetPlayerPlayableList());
    }

    private void Awake()
    {
        Instance = this;
    }

    private void _AssignAllPlayers()
    {
        foreach (Transform child in transform)
        {
            EntityPlayer player = child.GetComponent<EntityPlayer>();
            if (player)
            {
                players.Add(player);
                player.AssignToLevelGrid();
            }
        }
    }

    private void _SetPlayersIsActive(int playerCount)
    {
        for (int i = 0; i < playerCount; i++)
            players[i].SetIsPlayable(true);

        for (int i = playerCount; i < players.Count; i++)
            players[i].SetIsPlayable(false);

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
