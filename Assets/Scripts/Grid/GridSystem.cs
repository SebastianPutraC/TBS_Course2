using UnityEngine;
using System;

public class GridSystem <TGridObject>
{
    private int width;
    private int height;
    private float cellsize;
    private TGridObject[,] gridObjectsArray; //2D array to hold the grid objects

    public GridSystem(int width, int height, float cellsize, 
        Func<
            GridSystem<TGridObject>, // First Input
            GridPosition, //Second Input
            TGridObject> //Return Value
            createGridObject) // Function name
    {
        this.width = width;
        this.height = height;
        this.cellsize = cellsize;

        gridObjectsArray = new TGridObject[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                //Draw line for debugging purposes. Vector3.Right is adding a small offset to the right to create small dashes
                //100f is duration of the line in seconds
                /*Debug.DrawLine(GetWorldPosition(x, z), GetWorldPosition(x, z)
                    + Vector3.right * .2f, Color.white, 100f);*/


                GridPosition gridPosition = new GridPosition(x, z);
                //This is where the Func function is used
                gridObjectsArray[x, z] = createGridObject(this, gridPosition); 
                //Put each new grid object into the array
            }
        }
    }

    public Vector3 GetWorldPosition(GridPosition gridPosition)
    {
        return new Vector3(gridPosition.x, 0, gridPosition.z) * cellsize;
    }

    public GridPosition GetGridPosition(Vector3 worldPosition)
    {
        // Convert to int and round to nearest whole number to get the grid position
        // This is because mouse position can be in between grid cells, so we need to round to the nearest cell
        return new GridPosition(
            Mathf.RoundToInt(worldPosition.x / cellsize),
            Mathf.RoundToInt(worldPosition.z / cellsize));
    }


    public void CreateDebugObjects(Transform debugPrefab)
    {
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                //Duplicate gridPosition from the constructor to put the data on the GridDebugObject
                GridPosition gridPosition = new GridPosition(x, z);

                //Show debug object at the world position of each grid cell
                //No rotation is needed, so we use Quaternion.identity
                Transform debugTransform = GameObject.Instantiate(debugPrefab, GetWorldPosition(gridPosition), Quaternion.identity);
                GridDebugObject gridDebugObject = debugTransform.GetComponent<GridDebugObject>();
                gridDebugObject.SetGridObject(GetGridObject(gridPosition));
            }
        }
    }
    public TGridObject GetGridObject(GridPosition gridPosition)
    {
        return gridObjectsArray[gridPosition.x, gridPosition.z];
    }

    public bool IsValidGridPosition(GridPosition gridPosition)
    {
        //Check if the grid position is within the bounds of the grid
        return gridPosition.x >= 0 && 
            gridPosition.z >= 0 && 
            gridPosition.x < width && 
            gridPosition.z < height;
    }

    public int GetWidth()
    {
        return width;
    }
    public int GetHeight()
    {
        return height;
    }
}
