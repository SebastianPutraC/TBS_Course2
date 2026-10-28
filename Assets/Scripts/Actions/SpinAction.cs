using UnityEngine;
using System;
using System.Collections.Generic;

public class SpinAction : BaseAction
{
    //public delegate void SpinCompleteDelegate();
    private float totalSpinAmount;

    private void Update()
    {
        if (!isActive)
        {
            return;
        }
        float spinAddAmount = 360f * Time.deltaTime;
        transform.eulerAngles += new Vector3(0, spinAddAmount, 0);

        totalSpinAmount += spinAddAmount;
        if (totalSpinAmount >= 360f)
        {
            //Fire delegate function once the spin is finished
            //onActionComplete();

            ActionComplete();
        }
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        //Set delegate parameter to onActionComplete that store ClearBusy function form UnitActionSystem
        //this.onActionComplete = onSpinComplete;

        totalSpinAmount = 0f;

        ActionStart(onActionComplete);
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        //Only valid action is on the same tile as the unit
        GridPosition unitGridPosition = unit.GetGridPosition();

        return new List<GridPosition>
        { unitGridPosition};
    }

    public override string GetActionName()
    {
        return "Spin";
    }

    public override int GetActionPointsCost()
    {
        return 1; //Custom Action cost
    }
}
