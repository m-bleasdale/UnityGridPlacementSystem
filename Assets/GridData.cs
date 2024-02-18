using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridData
{
    Dictionary<Vector3Int, PlacementData> placedObjects = new();

    public void AddObjectAt(Vector3Int gridPosition, int rotation, Vector2Int objectSize, int ID, int placedObjectIndex)
    {
        List<Vector3Int> positionToOccupy = CalculatePositions(gridPosition, objectSize, rotation);
        PlacementData data = new PlacementData(positionToOccupy, ID, placedObjectIndex); //Add rotation property to store rotation to delet appropriate cells when removing object
        foreach (var pos in positionToOccupy)
        {
            if(placedObjects.ContainsKey(pos))
            {
                Debug.Log($"ERROR: Dictionary already contains this cell position {pos}");
                return;
            }
            placedObjects[pos] = data;
        }
    }

    private List<Vector3Int> CalculatePositions(Vector3Int gridPosition, Vector2Int objectSize, int rotation)
    {
        List<Vector3Int> returnVal = new();

        //rotation = 0
        if(rotation == 0){
            for (int x = 0; x < objectSize.x; x++)
            {
                for (int y = 0; y < objectSize.y; y++)
                {
                    returnVal.Add(gridPosition + new Vector3Int(x, 0, y));
                }
            }
        }
        
        //rotation = 1
        if(rotation == 1)
        {
            for (int y = -(objectSize.x - 1); y <= 0; y++)
            {
                for (int x = 0; x < objectSize.y; x++)
                {
                    returnVal.Add(gridPosition + new Vector3Int(x, 0, y));
                }
            }
        }

        //rotation = 2
        if(rotation == 2){
            for (int x = -(objectSize.x - 1); x <= 0; x++)
            {
                for (int y = -(objectSize.y - 1); y <= 0; y++)
                {
                    returnVal.Add(gridPosition + new Vector3Int(x, 0, y));
                }
            }
        }

        //rotation 3
        if(rotation == 3){
            for (int y = 0; y < objectSize.x; y++)
            {
                for (int x = -(objectSize.y - 1); x <= 0; x++)
                {
                    returnVal.Add(gridPosition + new Vector3Int(x, 0, y));
                }
            }
        }

        return returnVal;
    }

    public bool CanPlaceObjectAt(Vector3Int gridPosition, Vector2Int objectSize, int rotation)
    {
        List<Vector3Int> positionToOccupy = CalculatePositions(gridPosition, objectSize, rotation);
        foreach (var pos in positionToOccupy)
        {
            if (placedObjects.ContainsKey(pos))
                return false;
        }
        return true;
    }
}

public class PlacementData
{
    public List<Vector3Int> occupiedPositions;
    public int ID { get; private set; }
    public int PlacedObjectIndex { get; private set; }

    public PlacementData(List<Vector3Int> occupiedPositions, int iD, int placedObjectIndex)
    {
        this.occupiedPositions = occupiedPositions;
        ID = iD;
        PlacedObjectIndex = placedObjectIndex;
    }
}
