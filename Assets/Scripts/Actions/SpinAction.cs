using UnityEngine;
using System;

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
            isActive = false;
            //Fire delegate function once the spin is finished
            onActionComplete();
        }
    }

    public void Spin(Action onSpinComplete)
    {
        //Set delegate parameter to onActionComplete that store ClearBusy function form UnitActionSystem
        this.onActionComplete = onSpinComplete;
        totalSpinAmount = 0f;
        isActive = true;
    }
}
