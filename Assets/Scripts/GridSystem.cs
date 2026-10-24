using UnityEngine;

public class GridSystem 
{
    private int width;
    private int height;
    private float cellsize;

    public GridSystem(int width, int height, float cellsize)
    {
        this.width = width;
        this.height = height;
        this.cellsize = cellsize;

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                //Draw line for debugging purposes. Vector3.Right is adding a small offset to the right to create small dashes
                //100f is duration of the line in seconds
                Debug.DrawLine(GetWorldPosition(x, z), GetWorldPosition(x, z)
                    + Vector3.right * .2f, Color.white, 100f);
            }
        }
    }

    public Vector3 GetWorldPosition(int x, int z)
    {
        return new Vector3(x, 0, z) * cellsize;
    }

    public GridPosition GetGridPosition(Vector3 worldPosition)
    {
        //Convert to int and round to nearest whole number to get the grid position
        return new GridPosition(
            Mathf.RoundToInt(worldPosition.x / cellsize),
            Mathf.RoundToInt(worldPosition.z / cellsize));
    }
}
