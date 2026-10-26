using NUnit.Framework;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MoveAction : BaseAction
{
    private Vector3 targetPosition;
    [SerializeField] private Animator unitAnimator;
    [SerializeField] private int maxMoveDistance = 4;
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float rotateSpeed = 10f;

    protected override void Awake()
    {
        base.Awake();

        //Set unit's starting target as its starting position to stop it from moving to 0,0,0
        targetPosition = transform.position;
    }

    private void Update()
    {
        if (!isActive)
        {
            return;
        }
        // Why move this?
        Vector3 moveDirection = (targetPosition - transform.position).normalized;
        float stoppingDistance = 0.1f;

        //Stops the unit from moving if it is within the stopping distance of the target position
        if (Vector3.Distance(transform.position, targetPosition) > stoppingDistance)
        {
            //From Here?
            transform.position += moveDirection * moveSpeed * Time.deltaTime;

            unitAnimator.SetBool("IsWalking", true);
        }
        else
        {
            unitAnimator.SetBool("IsWalking", false);
            ActionComplete();
        }
        // Why move this?
        // Rotate the unit to face the movement direction
        transform.forward = Vector3.Lerp(transform.forward, moveDirection, Time.deltaTime * rotateSpeed);
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        ActionStart(onActionComplete);

        targetPosition = LevelGrid.Instance.GetWorldPosition(gridPosition);
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

                validGridPositions.Add(testGridPosition);
            }
        }

        return validGridPositions;
    }

    public override string GetActionName()
    {
        return "Move";
    }
}
