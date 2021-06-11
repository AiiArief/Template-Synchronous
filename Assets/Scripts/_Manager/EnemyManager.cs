using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance;
    public List<EntityEnemy> enemies { get; private set; } = new List<EntityEnemy>();

    public int maxAlertLevel = 15;

    public void SetupEnemiesOnLevelStart()
    {
        _AssignAllEnemies();
    }

    public void SetupAllEnemiesWaitInput()
    {
        foreach (EntityEnemy enemy in enemies)
        {
           enemy.SetupWaitInput();
        }
    }

    public bool CheckAllEnemiesHasDoneInput()
    {
        foreach (EntityEnemy enemy in enemies)
        {
            if (enemy.storedActions.Count == 0)
                enemy.WaitInput();
        }

        // kalo semua udah ada stored action return true
        foreach (EntityEnemy enemy in enemies)
        {
            if (enemy.storedActions.Count == 0)
                return false;
        }

        return true;
    }

    public void SetupAllEnemiesProcessInput()
    {
        foreach (EntityEnemy enemy in enemies)
        {
            enemy.SetupProcessInput();
        }
    }

    public bool CheckAllEnemiesHasDoneProcess()
    {
        foreach (EntityEnemy enemy in enemies)
        {
            if (!enemy.CheckAllActionHasDone())
            {
                foreach (StoredAction storedAction in enemy.storedActions)
                {
                    if (!storedAction.actionHasDone)
                        storedAction.action.Invoke();
                }
            }
        }

        foreach (EntityEnemy enemy in enemies)
        {
            if (!enemy.CheckAllActionHasDone())
                return false;
        }

        return true;
    }

    public void SetupAllEnemiesAfterInput()
    {
        foreach (EntityEnemy enemy in enemies)
        {
            enemy.SetupAfterInput();
        }
    }

    public bool CheckAllEnemiesHasDoneAfterInput()
    {
        // panggil semua player after input
        foreach (EntityEnemy enemy in enemies)
        {
               enemy.AfterInput();
        }

        // return true kalo semua udah selesai after input
        foreach (EntityEnemy enemy in enemies)
        {
            if (!enemy.afterActionHasDone)
                return false;
        }

        return true;
    }

    private void Awake()
    {
        Instance = this;
    }

    private void _AssignAllEnemies()
    {
        foreach (Transform child in transform)
        {
            EntityEnemy enemy = child.GetComponent<EntityEnemy>();
            if (enemy) enemies.Add(enemy);
        }
    }
}
