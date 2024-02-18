using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Placement : MonoBehaviour
{

    [SerializeField]
    private GameObject mouseIndicator, cellIndicator;
    private Renderer cellIndicatorRenderer;

    [SerializeField]
    private InputManager inputManager;
    [SerializeField]
    private Grid grid;

    [SerializeField]
    private ObjectDatabaseSO database;
    private int selectedObjectIndex = -1;

    [SerializeField]
    private GameObject gridVisualisation;

    private GridData gridData;

    private List<GameObject> placedGameObjects = new();

    [SerializeField]
    private GameObject emptyPreviewObject;
    private GameObject previewObject;
    private Renderer[] previewObjectRenderers;

    [SerializeField]
    private Material previewObjectMaterialValid;
    [SerializeField]
    private Material previewObjectMaterialInvalid;

    private int rotation = 0;
    private Vector3Int objectOffset;

    private void Start()
    {
        previewObject = emptyPreviewObject;

        StopPlacement();
        gridData = new();

        cellIndicatorRenderer = cellIndicator.GetComponentInChildren<Renderer>();
    }

    public void StartPlacement(int ID)
    {
        StopPlacement();
        selectedObjectIndex = database.objectsData.FindIndex(data => data.ID == ID);
        if(selectedObjectIndex < 0)
        {
            if(selectedObjectIndex != -1)
                Debug.LogError($"No ID found, {ID}");
            return;
        }
        gridVisualisation.SetActive(true);
        cellIndicator.SetActive(true);

        previewObject.SetActive(true);
        previewObject = Instantiate(database.objectsData[selectedObjectIndex].Prefab);
        previewObjectRenderers = previewObject.GetComponentsInChildren<Renderer>();

        inputManager.OnClicked += PlaceStructure;
        inputManager.OnExit += StopPlacement;
        inputManager.OnRotate += RotateStructure;
    }

    private void RotateStructure()
    {

        if(rotation < 3)
            rotation += 1;
        else
            rotation = 0;

        switch(rotation)
        {
            case 0:
                objectOffset = new Vector3Int(0, 0, 0);
                break;
            case 1:
                objectOffset = new Vector3Int(0, 0, 1);
                break;
            case 2:
                objectOffset = new Vector3Int(1, 0, 1);
                break;
            case 3:
                objectOffset = new Vector3Int(1, 0, 0);
                break;
        }

        previewObject.transform.rotation = Quaternion.Euler(0, rotation * 90, 0);
        previewObject.transform.position += grid.CellToWorld(objectOffset);
        
    }

    private void PlaceStructure()
    {
        if(inputManager.IsPointerOverUI())
            return;
        

        Vector3 mousePosition = inputManager.GetMousePositionOnGrid();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);

        Vector3Int placePosition = gridPosition + objectOffset;
        
        bool placementValidity = gridData.CanPlaceObjectAt(gridPosition, database.objectsData[selectedObjectIndex].Size, rotation);

        if(placementValidity == false)
        {
            Debug.Log("Invalid Pos");
            return;
        }
            

        GameObject newObject = Instantiate(database.objectsData[selectedObjectIndex].Prefab);
        newObject.transform.position = grid.CellToWorld(placePosition);
        newObject.transform.rotation = Quaternion.Euler(0, rotation * 90, 0);
        placedGameObjects.Add(newObject);
        gridData.AddObjectAt(gridPosition,
            rotation,
            database.objectsData[selectedObjectIndex].Size,
            database.objectsData[selectedObjectIndex].ID,
            placedGameObjects.Count -1);

    }

    private void StopPlacement()
    {
        selectedObjectIndex = -1;

        gridVisualisation.SetActive(false);
        cellIndicator.SetActive(false);
        
        if(previewObject.tag != "CannotDestroy")
            Destroy(previewObject);
        previewObject = emptyPreviewObject;
        previewObject.SetActive(false);

        rotation = 0;
        objectOffset = new Vector3Int(0, 0, 0);

        inputManager.OnClicked -= PlaceStructure;
        inputManager.OnExit -= StopPlacement;
        inputManager.OnRotate -= RotateStructure;
    }

    void Update()
    {
        if(selectedObjectIndex < 0)
            return;

        Vector3 mousePosition = inputManager.GetMousePositionOnGrid();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);

        bool placementValidity = gridData.CanPlaceObjectAt(gridPosition, database.objectsData[selectedObjectIndex].Size, rotation);
        cellIndicatorRenderer.material.color = placementValidity ? Color.white : Color.red;

        previewObject.transform.position = grid.CellToWorld(gridPosition + objectOffset);

        Material materialToChangeTo = placementValidity ? previewObjectMaterialValid : previewObjectMaterialInvalid;
        foreach(Renderer renderer in previewObjectRenderers)
        {
            renderer.material = materialToChangeTo;
        }

        mouseIndicator.transform.position = mousePosition;
        cellIndicator.transform.position = grid.CellToWorld(gridPosition);
    }

 
}
