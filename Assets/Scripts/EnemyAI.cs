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
        SpinAction spinAction = enemyUnit.GetSpinAction();

        //Will be refactored later
        GridPosition actionGridPosition = enemyUnit.GetGridPosition();

        if (!spinAction.IsValidActionGridPosition(actionGridPosition))
        {
            return false;
        }
        if (!enemyUnit.TrySpendActionPointsToTakeAction(spinAction))
        {
            return false;
        }

        spinAction.TakeAction(actionGridPosition, onEnemyAiActionComplete);
        return true;
    }
}
