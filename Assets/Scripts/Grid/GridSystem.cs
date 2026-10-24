using UnityEngine;

public class GridSystem 
{
    private int width;
    private int height;
    private float cellsize;
    private GridObject[,] gridObjectsArray; //2D array to hold the grid objects

    public GridSystem(int width, int height, float cellsize)
    {
        this.width = width;
        this.height = height;
        this.cellsize = cellsize;

        gridObjectsArray = new GridObject[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                //Draw line for debugging purposes. Vector3.Right is adding a small offset to the right to create small dashes
                //100f is duration of the line in seconds
                /*Debug.DrawLine(GetWorldPosition(x, z), GetWorldPosition(x, z)
                    + Vector3.right * .2f, Color.white, 100f);*/

                GridPosition gridPosition = new GridPosition(x, z);
                gridObjectsArray[x, z] = new GridObject(this, gridPosition);
                //put each new grid object into the array
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
    public GridObject GetGridObject(GridPosition gridPosition)
    {
        return gridObjectsArray[gridPosition.x, gridPosition.z];
    }
}
