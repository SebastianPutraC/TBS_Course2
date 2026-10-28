using System;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    private enum State
    {
        WAITINGFORENEMYTURN,
        TAKINGTURN,
        BUSY,
    }
    [SerializeField] private State state;

    private float timer;

    private void Awake()
    {
        state = State.WAITINGFORENEMYTURN;
    }

    private void Start()
    {
        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
    }

    private void Update()
    {
        if (TurnSystem.Instance.IsPlayerTurn())
        {
            return;
        }

        switch (state)
        {
            case State.WAITINGFORENEMYTURN:
                break;
            case State.TAKINGTURN:
                timer -= Time.deltaTime;
                if (timer <= 0f)
                {
                    if (TryTakeEnemyAIAction(SetStateTakingTurn))
                    {
                        state = State.BUSY;
                    }
                    //End turn because no enemy unit can take any action
                    else
                    {
                        TurnSystem.Instance.NextTurn();
                    }
                }
                break;
            case State.BUSY:
                break;
        }
    }

    //SetStateTakingTurn is called after each ai unit is done with their turn
    //Then reset the timer to run another TakeEnemyAIAction that will call this function again
    private void SetStateTakingTurn()
    {
        timer = 0.5f;
        state = State.TAKINGTURN;
    }

    private void TurnSystem_OnTurnChanged(object sender, System.EventArgs e)
    {
        if (!TurnSystem.Instance.IsPlayerTurn())
        {
            state = State.TAKINGTURN;
            timer = 2f;
        }
    }
    private bool TryTakeEnemyAIAction(Action onEnemyAiActionComplete)
    {
        foreach(Unit enemyUnit in UnitManager.Instance.GetEnemyUnitList())
        {
            if (TryPerformEnemyAIAction(enemyUnit, onEnemyAiActionComplete))
            {
                return true;
            }
        }

        return false;
    }
    private bool TryPerformEnemyAIAction(Unit enemyUnit, Action onEnemyAiActionComplete)
    {
        EnemyAIAction bestEnemyAIAction = null;
        BaseAction bestBaseAction = null;

        //Review every action on array, will get the best enemy action from the value of each action in the array
        foreach (BaseAction baseAction in enemyUnit.GetBaseActionArray())
        {
            // Maybe the best action but, the Enemy cannot afford this action, skip it
            if (!enemyUnit.CanSpendActionPointsToTakeAction(baseAction))
            {
                continue;
            }

            //No current bestEnemyAIAction yet
            if (bestEnemyAIAction == null)
            {
                bestEnemyAIAction = baseAction.GetBestEnemyAIAction();
                bestBaseAction = baseAction;
            }
            else
            {
                //Check if this action is better than the previous one
                EnemyAIAction testEnemyAIAction = baseAction.GetBestEnemyAIAction();
                if (testEnemyAIAction != null && testEnemyAIAction.actionValue > bestEnemyAIAction.actionValue)
                {
                    bestEnemyAIAction = testEnemyAIAction;
                    bestBaseAction = baseAction;
                }
            }

        }

        //After having the bestEnemyAIAction and the points to do it, Perform the action
        if (bestEnemyAIAction != null && enemyUnit.TrySpendActionPointsToTakeAction(bestBaseAction))
        {
            bestBaseAction.TakeAction(bestEnemyAIAction.gridPosition, onEnemyAiActionComplete);
            return true;
        }
        else
        {
            //No action the enemy could take because of lack of points or no target at move then shoot range
            return false;
        }
    }
}
