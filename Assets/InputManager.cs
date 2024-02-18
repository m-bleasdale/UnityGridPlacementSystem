using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour
{

    [SerializeField]
    private LayerMask placementLayermask;
    [SerializeField]
    private LayerMask destructionLayermask;

    [SerializeField]
    private GameObject nullObject;

    private Vector3 lastMousePosition;

    public event Action OnClicked, OnExit, OnRotate;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0)){
            OnClicked?.Invoke();
        }
        if(Input.GetKeyDown(KeyCode.Escape)){
            OnExit?.Invoke();
        }
        if(Input.GetKeyDown(KeyCode.R)){
            OnRotate?.Invoke();
        }
    }

    public bool IsPointerOverUI()
    => EventSystem.current.IsPointerOverGameObject();

    public Vector3 GetMousePositionOnGrid()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, placementLayermask)){
            lastMousePosition = hit.point;
        }

        return lastMousePosition;
    }

    public GameObject GetGameObjectMouseOver()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, destructionLayermask)){
            return hit.transform.root.gameObject;
        }

        return nullObject;
    }
}
