using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.HID;

public class CursorManager : MonoBehaviour
{
    [SerializeField] Texture2D basicCursor;
    [SerializeField] Texture2D clickedCursor;
    [SerializeField] Texture2D resourceCursor;
    [SerializeField] Texture2D FightCursor;
    Texture2D currentCursor;
    CursorControllers controller;
    UnitsController unitsController;
    [SerializeField] Camera cam;
    [SerializeField] XSpot goToMark;

    private void OnEnable()
    {
        controller.Enable();
    }
    private void OnDisable()
    {
        controller.Disable();
    } 
    private void Awake()
    {
        controller = new CursorControllers();
        ChangeCursor(basicCursor);
        Cursor.lockState = CursorLockMode.Confined;
    }
    private void Start()
    {
        unitsController = GameObject.FindAnyObjectByType<UnitsController>();
        currentCursor = basicCursor;
        controller.Cursor.Click.started += _ => StartedClick();
        controller.Cursor.Click.performed += _ => EndedClick();

    }
    private void FixedUpdate()
    {
        if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 5000.0f,
            LayerMask.GetMask("Wood", "Food", "Units", "Iron", "Gold")))
        {
            if (unitsController.ConstainsAllyUnitType(UnitType.Soldier))
            {
                if (hit.transform.gameObject.tag == "Enemy" && hit.transform.gameObject.tag != "Ally")
                {
                    ChangeCursor(FightCursor);
                }
            }
            if (unitsController.ConstainsUnitType(UnitType.Villager))
            {
                if (hit.transform.gameObject.layer == 7 ||
                    hit.transform.gameObject.layer == 9 ||
                    hit.transform.gameObject.layer == 11 ||
                    hit.transform.gameObject.layer == 12)
                {
                    ChangeCursor(resourceCursor);
                }
            }
        }
        else
        {
            ChangeCursor(currentCursor);
        }
    }
    private void StartedClick()
    {
        if (unitsController.checkIfSelectIsUnit())
        {
            ChangeCursor(clickedCursor);
            currentCursor = clickedCursor;
        }
        goToMark.showX();
    }
    private void EndedClick()
    {
        ChangeCursor(basicCursor);
        currentCursor = basicCursor;
        goToMark.hideX();
    }
    private void ChangeCursor(Texture2D cursorType)
    {
        Vector2 hotspot = new Vector2(cursorType.width / 4, cursorType.height / 4);
        Cursor.SetCursor(cursorType, hotspot, CursorMode.Auto);
    }
}
