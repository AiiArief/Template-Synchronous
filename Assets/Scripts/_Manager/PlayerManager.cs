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
        //panggil semua player buat liat input
        foreach (EntityPlayer player in players)
        {
            if (player.isPlayable && player.storedActions.Count == 0)
                player.WaitInput();
        }

        // kalo semua udah ada stored action return true
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
        // panggil semua player process
        foreach (EntityPlayer player in players)
        {
            if (player.isPlayable && !player.CheckAllActionHasDone())
            {
                foreach (StoredAction storedAction in player.storedActions)
                    if (!storedAction.actionHasDone)
                        storedAction.action.Invoke();
            }
        }

        // return true kalo semua udah selesai process
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
        // panggil semua player after input
        foreach (EntityPlayer player in players)
        {
            if (player.isPlayable)
                player.AfterInput();
        }

        // return true kalo semua udah selesai after input
        foreach (EntityPlayer player in players)
        {
            if (player.isPlayable && !player.afterActionHasDone)
                return false;
        }

        return true;
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
            if (player) players.Add(player);
        }
    }

    // temp, masih ada kemungkinan ngebug kalo tiba2 player2 disconnect pas main ber4
    private void _SetPlayersIsActive(int playerCount)
    {
        for (int i = 0; i < playerCount; i++)
            players[i].SetIsPlayable(true);

        for (int i = playerCount; i < players.Count; i++)
            players[i].SetIsPlayable(false);

        _SetupAllPlayersCamera(playerCount);
    }

    private void _SetupAllPlayersCamera(int playerCount)
    {
        switch(playerCount)
        {
            case 1:
                players[0].playerCameraLook.playerCamera.rect = new Rect(0, 0, 1, 1);
                break;
            case 2:
                players[0].playerCameraLook.playerCamera.rect = new Rect(0, 0, 0.5f, 1);
                players[1].playerCameraLook.playerCamera.rect = new Rect(0.5f, 0, 0.5f, 1);
                break;
            case 3:
                players[0].playerCameraLook.playerCamera.rect = new Rect(0, 0.5f, 1, 0.5f);
                players[1].playerCameraLook.playerCamera.rect = new Rect(0, 0, 0.5f, 0.5f);
                players[2].playerCameraLook.playerCamera.rect = new Rect(0.5f, 0, 0.5f, 0.5f);
                break;
            case 4:
                players[0].playerCameraLook.playerCamera.rect = new Rect(0, 0.5f, 0.5f, 0.5f);
                players[1].playerCameraLook.playerCamera.rect = new Rect(0.5f, 0.5f, 0.5f, 0.5f);
                players[2].playerCameraLook.playerCamera.rect = new Rect(0, 0, 0.5f, 0.5f);
                players[3].playerCameraLook.playerCamera.rect = new Rect(0.5f, 0, 0.5f, 0.5f);
                break;
        }
    }
}
