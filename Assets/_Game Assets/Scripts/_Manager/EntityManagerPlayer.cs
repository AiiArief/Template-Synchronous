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
        _AssignPlayersToGrid();
        _SetPlayersIsActive();
    }

    public List<EntityCharacterPlayer> GetPlayerPlayableList()
    {
        var playablePlayers = new List<EntityCharacterPlayer>();
        foreach (EntityCharacterPlayer player in entities)
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

    private void _AssignPlayersToGrid()
    {
        foreach (EntityCharacterPlayer player in entities)
        {
            player.AssignToLevelGrid();
        }
    }

    private void _SetPlayersIsActive()
    {
        for (int i = 0; i < playerCountOnLevelStart; i++)
            entities[i].SetIsUpdateAble(true);

        for (int i = playerCountOnLevelStart; i < entities.Count; i++)
            entities[i].SetIsUpdateAble(false);

        _SetupAllPlayersCamera(GetPlayerPlayableList());
    }

    private void _SetupAllPlayersCamera(List<EntityCharacterPlayer> playablePlayers)
    {
        int playerCount = playablePlayers.Count;
        switch (playerCount)
        {
            case 1:
                playablePlayers[0].ResizeCamera(new Rect(0, 0, 1, 1));
                break;
            case 2:
                playablePlayers[0].ResizeCamera(new Rect(0, 0, 0.5f, 1));
                playablePlayers[1].ResizeCamera(new Rect(0.5f, 0, 0.5f, 1));
                break;
            case 3:
                playablePlayers[0].ResizeCamera(new Rect(0, 0.5f, 1, 0.5f));
                playablePlayers[1].ResizeCamera(new Rect(0, 0, 0.5f, 0.5f));
                playablePlayers[2].ResizeCamera(new Rect(0.5f, 0, 0.5f, 0.5f));
                break;
            case 4:
                playablePlayers[0].ResizeCamera(new Rect(0, 0.5f, 0.5f, 0.5f));
                playablePlayers[1].ResizeCamera(new Rect(0.5f, 0.5f, 0.5f, 0.5f));
                playablePlayers[2].ResizeCamera(new Rect(0, 0, 0.5f, 0.5f));
                playablePlayers[3].ResizeCamera(new Rect(0.5f, 0, 0.5f, 0.5f));
                break;
        }
    }
}
