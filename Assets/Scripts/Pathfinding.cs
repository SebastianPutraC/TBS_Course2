using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{

    public static Pathfinding Instance { get; private set; }


    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14;

    [SerializeField] private Transform gridDebugObjectPrefab;
    [SerializeField] private LayerMask obstaclesLayerMask;

    private int width;
    private int height;
    private float cellSize;
    private GridSystem<PathNode> gridSystem;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There's more than one Pathfinding! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void Setup(int width, int height, float cellSize)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;

        gridSystem = new GridSystem<PathNode>(width, height, cellSize,
    (GridSystem<PathNode> g, GridPosition gridPosition) => new PathNode(gridPosition));
        gridSystem.CreateDebugObjects(gridDebugObjectPrefab);

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                GridPosition gridPosition = new GridPosition(x, z);
                Vector3 worldPosition = LevelGrid.Instance.GetWorldPosition(gridPosition);

                //Below the floor so it can detect the obstacle colldier
                float raycastOffsetDistance = 5f;

                //The vector3.up is *2 from the raycastOffsetDistance to make sure it hits the obstacle
                if (Physics.Raycast(worldPosition + Vector3.down * raycastOffsetDistance,
                    Vector3.up, raycastOffsetDistance * 2, obstaclesLayerMask))
                {
                    GetNode(x, z).SetIsWalkable(false);
                }
            }
        }
    }

    public List<GridPosition> FindPath(GridPosition startGridPosition, GridPosition endGridPosition)
    {
        // Add the node to the openList, check the loop of openList and closedList
        List<PathNode> openList = new List<PathNode>(); // All the nodes queued up for searching to be evaluated
        List<PathNode> closedList = new List<PathNode>(); // All the nodes already been searched or evaluated

        PathNode startNode = gridSystem.GetGridObject(startGridPosition);
        PathNode endNode = gridSystem.GetGridObject(endGridPosition);
        openList.Add(startNode); //Initialize where the grid starts

        // Get the map of the entire grid for PathNode, and also setup for each G,H, and F cost
        for (int x = 0; x < gridSystem.GetWidth(); x++)
        {
            for (int z = 0; z < gridSystem.GetHeight(); z++)
            {
                GridPosition gridPosition = new GridPosition(x, z);
                PathNode pathNode = gridSystem.GetGridObject(gridPosition);

                pathNode.SetGCost(int.MaxValue); //The default value is infinite since there's no target yet
                pathNode.SetHCost(0); // Setup so the Unit already reached the target (the grid its standing on)
                pathNode.CalculateFCost(); // Calculate initial f cost
                pathNode.ResetCameFromPathNode(); //not moving yet, set the previous path node to null
            }
        }

        startNode.SetGCost(0); //Starting g cost is 0 because we're already there

        //Substracting from beginning till end
        startNode.SetHCost(CalculateDistance(startGridPosition, endGridPosition));
        startNode.CalculateFCost();

        while (openList.Count > 0)
        {
            PathNode currentNode = GetLowestFCostPathNode(openList);

            /*Debug.Log("---");
            foreach (var x in openList)
            {
                Debug.Log(x.ToString());
            }
            Debug.Log("---");
            Debug.Log("+++");
            foreach (var x in closedList)
            {
                Debug.Log(x.ToString());
            }
            Debug.Log("Current : " + currentNode.ToString());
            Debug.Log("+++");*/

            if (currentNode == endNode)
            {
                // Reached final node, the calculate path will be used for movement
                return CalculatePath(endNode);
            }

            //Current node has been searched by the GetLowestFCostPathNode
            openList.Remove(currentNode);
            closedList.Add(currentNode);

            //Get each neighboring nodes 
            foreach (PathNode neighbourNode in GetNeighbourList(currentNode))
            {
                //Checks if the node is already being searched
                if (closedList.Contains(neighbourNode))
                {
                    continue;
                }
                //Skip obstacle node
                if (!neighbourNode.IsWalkable())
                {
                    closedList.Add(neighbourNode);
                    continue;
                }

                //Cost to move from current node to the neighbor node
                int tentativeGCost =
                    currentNode.GetGCost() + CalculateDistance(currentNode.GetGridPosition(), neighbourNode.GetGridPosition());

                //If the neighborNode has lower gCost compared to the tentativeGCost, pick this neighborNode
                if (tentativeGCost < neighbourNode.GetGCost())
                {
                    neighbourNode.SetCameFromPathNode(currentNode); // Used in the CalculatePath function
                    neighbourNode.SetGCost(tentativeGCost); //Update the GCost, this will be tested again with another neighbor node

                    //Substracting the chosen lower neighbor node to the target/end grid position
                    neighbourNode.SetHCost(CalculateDistance(neighbourNode.GetGridPosition(), endGridPosition));
                    neighbourNode.CalculateFCost();

                    //This node will be used again to check for GetLowestFCostPathNode that accept the openList as the parameter
                    if (!openList.Contains(neighbourNode))
                    {
                        openList.Add(neighbourNode);
                    }
                }
            }
        }

        // No path found
        return null;
    }

    //Distance calculated by simply subtracting the x and z from A to B, then multiply it by the
    //default cost for diagonal(14) and straight(10)
    public int CalculateDistance(GridPosition gridPositionA, GridPosition gridPositionB)
    {
        GridPosition gridPositionDistance = gridPositionA - gridPositionB;
        int xDistance = Mathf.Abs(gridPositionDistance.x);
        int zDistance = Mathf.Abs(gridPositionDistance.z);
        int remaining = Mathf.Abs(xDistance - zDistance);
        return MOVE_DIAGONAL_COST * Mathf.Min(xDistance, zDistance) + MOVE_STRAIGHT_COST * remaining;
    }

    private PathNode GetLowestFCostPathNode(List<PathNode> pathNodeList)
    {
        PathNode lowestFCostPathNode = pathNodeList[0];
        for (int i = 0; i < pathNodeList.Count; i++)
        {
            if (pathNodeList[i].GetFCost() < lowestFCostPathNode.GetFCost())
            {
                lowestFCostPathNode = pathNodeList[i];
            }
        }
        return lowestFCostPathNode;
    }

    private PathNode GetNode(int x, int z)
    {
        return gridSystem.GetGridObject(new GridPosition(x, z));
    }

    private List<PathNode> GetNeighbourList(PathNode currentNode)
    {
        List<PathNode> neighbourList = new List<PathNode>();

        GridPosition gridPosition = currentNode.GetGridPosition();

        //All the if statement is checking if the grid that are now being checked is higher than grid limit or lower than 0
        if (gridPosition.x - 1 >= 0)
        {
            // Left
            neighbourList.Add(GetNode(gridPosition.x - 1, gridPosition.z + 0));
            if (gridPosition.z - 1 >= 0)
            {
                // Left Down
                neighbourList.Add(GetNode(gridPosition.x - 1, gridPosition.z - 1));
            }

            if (gridPosition.z + 1 < gridSystem.GetHeight())
            {
                // Left Up
                neighbourList.Add(GetNode(gridPosition.x - 1, gridPosition.z + 1));
            }
        }

        if (gridPosition.x + 1 < gridSystem.GetWidth())
        {
            // Right
            neighbourList.Add(GetNode(gridPosition.x + 1, gridPosition.z + 0));
            if (gridPosition.z - 1 >= 0)
            {
                // Right Down
                neighbourList.Add(GetNode(gridPosition.x + 1, gridPosition.z - 1));
            }
            if (gridPosition.z + 1 < gridSystem.GetHeight())
            {
                // Right Up
                neighbourList.Add(GetNode(gridPosition.x + 1, gridPosition.z + 1));
            }
        }

        if (gridPosition.z - 1 >= 0)
        {
            // Down
            neighbourList.Add(GetNode(gridPosition.x + 0, gridPosition.z - 1));
        }
        if (gridPosition.z + 1 < gridSystem.GetHeight())
        {
            // Up
            neighbourList.Add(GetNode(gridPosition.x + 0, gridPosition.z + 1));
        }

        return neighbourList;
    }

    //Walk backward from the end using the GetCameFromPathNode stored after getting the lowest neighbor node
    private List<GridPosition> CalculatePath(PathNode endNode)
    {
        List<PathNode> pathNodeList = new List<PathNode>();
        pathNodeList.Add(endNode);
        PathNode currentNode = endNode;

        //Keep cycling backward and find out if each node chosen has a cameFromPathNode and add it to the list
        while (currentNode.GetCameFromPathNode() != null)
        {
            pathNodeList.Add(currentNode.GetCameFromPathNode());
            currentNode = currentNode.GetCameFromPathNode();
        }

        //Reverse this so the path is started from the beginning
        pathNodeList.Reverse();

        List<GridPosition> gridPositionList = new List<GridPosition>();
        foreach (PathNode pathNode in pathNodeList)
        {
            gridPositionList.Add(pathNode.GetGridPosition());
        }

        //This is used as a final movement path
        return gridPositionList;
    }

}
