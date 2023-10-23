using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitsController : MonoBehaviour
{
    Camera cam;
    int layerMask;
    Dictionary<int, GameObject> selectedUnits;
    GameManager gameManager;

    void Start()
    {
        layerMask = LayerMask.GetMask("Ground");

        cam = GameObject.FindAnyObjectByType<Camera>();

        selectedUnits = GameObject.FindAnyObjectByType<Selection_Dictonary>().selectedTable;
        gameManager = GameObject.FindAnyObjectByType<GameManager>();  
        
    }

    public List<GameObject> getAllUnitsFromType(UnitType type)
    {
        List<GameObject> units = new List<GameObject>();

        foreach (var unit in selectedUnits)
        {
            if (unit.Value.GetComponent<Unit>() != null)
            {
                if (unit.Value.GetComponent<Unit>().Type == type)
                {
                    units.Add(unit.Value);
                }
            }
        }
        return units;
    }
    public List<GameObject> getAllUnitsAllyFromType(UnitType type)
    {
        List<GameObject> units = new List<GameObject>();

        foreach (var unit in selectedUnits)
        {
            if (unit.Value.GetComponent<Unit>() != null)
            {
                if (unit.Value.GetComponent<Unit>().Type == type && unit.Value.transform.tag == "Ally")
                {
                    units.Add(unit.Value);
                }
            }
        }
        return units;
    }
    public bool ConstainsAllyUnitType(UnitType type)
    {
        if (selectedUnits != null)
        {
            foreach (var unit in selectedUnits.ToList())
            {
                if (unit.Value.GetComponent<Unit>() != null)
                {
                    if (unit.Value.GetComponent<Unit>().Type == type && unit.Value.gameObject.tag == "Ally")
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }
    public bool ConstainsUnitType(UnitType type)
    {
        foreach (var unit in selectedUnits)
        {
            if (unit.Value != null)
            {
                if (unit.Value.GetComponent<Unit>() != null)
                {
                    if (unit.Value.GetComponent<Unit>().Type == type)
                    {
                        return true;
                    }
                }
            }
            else
            {
                selectedUnits.Remove(unit.Key);
            }
        }
        return false;
    }
    public bool checkIfSelectIsUnit()
    {
        foreach (var unit in selectedUnits)
        {
            if (unit.Value.GetComponent<Unit>() != null)
            {
              return true;  
            }
        }
        return false;
    }
    public int CountOfUnits()
    {
        int count = 0;
        foreach (var unit in selectedUnits)
        {
            if (unit.Value != null)
            {
                if (unit.Value.GetComponent<Build>() != null || unit.Value.GetComponent<Unit>() != null)
                {
                    count++;
                } 
            }
            else
            {
                selectedUnits.Remove(unit.Key);
            }
            
        }
        return count;
    }
    public bool ContaisnBuildType(BuildType type)
    {
        foreach (var unit in selectedUnits)
        {
         
            if (unit.Value.GetComponent<Build>() != null)
            {
                if (unit.Value.GetComponent<Build>().buildtype == type)
                {
                    return true;
                }
            }
        }
        return false;
    }
    public List<GameObject> getCurrentSelectBuild(BuildType type)
    {
        List<GameObject> list = new List<GameObject>();

        foreach (var unit in selectedUnits)
        {
            if (unit.Value.GetComponent<Build>() != null)
            {
                if (unit.Value.GetComponent<Build>().buildtype == type)
                {
                    list.Add(unit.Value);
                }
            }
        }
        return list;
    }
    void FixedUpdate()
    {
        if (!gameManager.InConstructMode)
        {
            if (Input.GetKey(KeyCode.Mouse1))
            {
                if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 5000.0f, layerMask))
                {
                    selectedUnits = GameObject.FindAnyObjectByType<Selection_Dictonary>().selectedTable;
                    if (selectedUnits != null)
                    {
                        foreach (var unit in selectedUnits)
                        {
                            if (unit.Value.gameObject.GetComponent<Unit>() != null && unit.Value.tag == "Ally")
                            {
                                 unit.Value.gameObject.GetComponent<Unit>().MoveToPoint(hit.point);
                            }
                        }
                    }
                }
            }
        }    
    }
}
