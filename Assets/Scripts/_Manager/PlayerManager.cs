using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;

    public List<Player> players { get; private set; } = new List<Player>();

    public void SetupPlayersOnLevelStart(int onLevelStartPlayerCount)
    {
        _AssignAllPlayers();
        _SetPlayersIsActive(onLevelStartPlayerCount);
    }

    public void SetupAllPlayersWaitInput()
    {
        foreach(Player player in players)
        {
            if(player.isPlayable)
                player.SetupWaitInput();
        }
    }

    public bool CheckAllPlayersHasDoneInput()
    {
        //panggil semua player buat liat input
        foreach(Player player in players)
        {
            if(player.isPlayable)
                player.WaitInput();
        }

        // kalo semua udah ada stored action return true
        foreach(Player player in players)
        {
            if (player.isPlayable && player.storedActions.Count == 0) 
                return false;
        }

        return true;
    }

    public void SetupAllPlayersProcessInput()
    {
        foreach (Player player in players)
        {
            player.SetupProcessInput();
        }
    }

    public bool CheckAllPlayersHasDoneProcess()
    {
        // panggil semua player process
        foreach (Player player in players)
        {
            if (player.isPlayable && !player.CheckAllActionHasDone())
            {
                foreach(StoredAction storedAction in player.storedActions)
                   if(!storedAction.actionHasDone) 
                        storedAction.action.Invoke();
            }
        }

        // return true kalo semua udah selesai process
        foreach(Player player in players)
        {
            if (player.isPlayable && !player.CheckAllActionHasDone()) 
                return false;
        }

        return true;
    }

    public void SetupAllPlayersAfterInput()
    {
        foreach (Player player in players)
        {
            player.SetupAfterInput();
        }
    }

    public bool CheckAllPlayersHasDoneAfterInput()
    {
        // panggil semua player after input
        foreach (Player player in players)
        {
            if (player.isPlayable)
                player.AfterInput();
        }

        // return true kalo semua udah selesai after input
        foreach (Player player in players)
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
            Player player = child.GetComponent<Player>();
            if (player) players.Add(player);
        }
    }

    // temp, masih ada kemungkinan ngebug kalo tiba2 player2 disconnect pas main ber4
    private void _SetPlayersIsActive(int playerCount)
    {
        for (int i = 0; i < playerCount; i++)
            players[i].isPlayable = true;

        for (int i = playerCount; i < players.Count; i++)
            players[i].isPlayable = false;
    }
}
