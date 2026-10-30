using NUnit.Framework;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MoveAction : BaseAction
{
    private List<Vector3> positionList;
    private int currentPositionIndex;

    //Previously used for getting the targetPosition, now changed to List<Vector3> positionList
    //private Vector3 targetPosition;

    [SerializeField] private int maxMoveDistance = 4;
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float rotateSpeed = 10f;

    public event EventHandler OnStartMoving;
    public event EventHandler OnStopMoving;


    /*protected override void Awake()
    {
        base.Awake();

        //Set unit's starting target as its starting position to stop it from moving to 0,0,0
        targetPosition = transform.position;
    }*/

    private void Update()
    {
        if (!isActive)
        {
            return;
        }

        Vector3 targetPosition = positionList[currentPositionIndex];

        // Why move this?
        Vector3 moveDirection = (targetPosition - transform.position).normalized;

        // Rotate the unit to face the movement direction
        transform.forward = Vector3.Lerp(transform.forward, moveDirection, Time.deltaTime * rotateSpeed);

        float stoppingDistance = 0.1f;

        //Stops the unit from moving if it is within the stopping distance of the target position
        if (Vector3.Distance(transform.position, targetPosition) > stoppingDistance)
        {
            //From Here?
            transform.position += moveDirection * moveSpeed * Time.deltaTime;
        }
        else
        {
            currentPositionIndex++;

            //At the target / end of the list
            if (currentPositionIndex >= positionList.Count)
            {
                OnStopMoving?.Invoke(this, EventArgs.Empty);
                ActionComplete();
            }
        }
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        List<GridPosition> pathGridPositionList = 
            Pathfinding.Instance.FindPath(unit.GetGridPosition(), gridPosition, out int pathLength);

        currentPositionIndex = 0;

        positionList = new List<Vector3>();

        foreach(GridPosition pathGridPosition in pathGridPositionList)
        {
            positionList.Add(LevelGrid.Instance.GetWorldPosition(pathGridPosition));
        }

        //targetPosition = LevelGrid.Instance.GetWorldPosition(gridPosition);

        OnStartMoving?.Invoke(this, EventArgs.Empty);

        ActionStart(onActionComplete);
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        List<GridPosition> validGridPositions = new List<GridPosition>();
        
        GridPosition unitGridPosition = unit.GetGridPosition();

        //Get each grid position within the max move distance and check if it is valid
        for (int x = -maxMoveDistance; x <= maxMoveDistance; x++)
        {
            for (int z = -maxMoveDistance; z <= maxMoveDistance; z++)
            {
                GridPosition offsetGridPostion = new GridPosition(x, z);
                GridPosition testGridPosition = unitGridPosition + offsetGridPostion;

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                {
                    //Invalid Grid Position, skip this
                    continue;
                }
                if (unitGridPosition == testGridPosition)
                {
                    //Same Unit Position, skip this
                    continue;
                }
                if (LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition))
                {
                    //Grid Position already occupied by another unit, skip this
                    continue;
                }

                if (!Pathfinding.Instance.IsWalkableGridPosition(testGridPosition))
                {   //From the pathfinding, check if the grid has an obstacle or not
                    continue;
                }

                if (!Pathfinding.Instance.HasPath(unitGridPosition, testGridPosition))
                {
                    //Available grid is blocked by obstacles, skip it
                    continue;
                }
                int pathfindingDistanceMultipler = 10;

                if (Pathfinding.Instance.GetPathLength(unitGridPosition, testGridPosition) 
                    > maxMoveDistance * pathfindingDistanceMultipler)
                {
                    //Path length is too long after calculating by maxMoveDistance
                    continue;
                }


                validGridPositions.Add(testGridPosition);
            }
        }

        return validGridPositions;
    }

    public override string GetActionName()
    {
        return "Move";
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        int targetCountAtGridPosition = 
            unit.GetAction<ShootAction>().GetTargetCountAtPosition(gridPosition);

        //Prioritize a grid position where this unit can shoot more enemies
        return new EnemyAIAction
        {
            gridPosition = gridPosition,
            actionValue = targetCountAtGridPosition * 10
        };
    }
}
