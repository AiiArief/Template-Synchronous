using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StoredStatusEffect
{
    // spell id buat cek ke database
    public Action effectAction;
    public Action effectRemovalAction;
}

public class StoredStatusEffectCustom : StoredStatusEffect
{
    public StoredStatusEffectCustom(Action customEffectAction, Action customEffectRemovalAction)
    {
        effectAction = customEffectAction;
        effectRemovalAction = customEffectRemovalAction;
    }
}

public class StoredStatusEffectAutoSkip : StoredStatusEffect
{
    public StoredStatusEffectAutoSkip(EntityCharacter target, float autoTime = 0.0f, int turnCount = -1)
    {
        float currentAutoTime = autoTime;
        effectAction = () =>
        {
            currentAutoTime = Mathf.Max(0.0f, currentAutoTime - Time.deltaTime);
            if (currentAutoTime <= 0.0f)
            {
                target.StoredActionSkipTurn();
            }
        };

        effectRemovalAction = () =>
        {
            turnCount = (turnCount < 0) ? turnCount : Mathf.Max(0, turnCount - 1);
            currentAutoTime = autoTime;

            if(turnCount == 0)
            {
                target.storedStatusEffects.Remove(this);
            }
        };
    }
}

public class StoredStatusEffectPlayerDisableInput : StoredStatusEffect
{
    public StoredStatusEffectPlayerDisableInput(EntityCharacterPlayer player, int turnCount = -1)
    {
        effectAction = () =>
        {
            player.enableMoveInput = false;
            player.playerCameraLook.enableCameraLook = false;
        };

        // belom di handle kalo ilangnya bukan karena turn countnya abis
        effectRemovalAction = () =>
        {
            turnCount = (turnCount < 0) ? turnCount : Mathf.Max(0, turnCount - 1);
            if (turnCount == 0)
            {
                player.enableMoveInput = true;
                player.playerCameraLook.enableCameraLook = true;
                player.storedStatusEffects.Remove(this);
            }
        };
    }
}
