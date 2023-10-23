using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class blueprint_script : MonoBehaviour
{
    RaycastHit hit;
    Vector3 movepoint;
    public GameObject prefab;
    Camera cam;
    GameManager manager;
    UnitsController unitsController;

    private void Start()
    {
        unitsController = GameObject.FindAnyObjectByType<UnitsController>();    
        manager = GameObject.FindObjectOfType<GameManager>();   
        cam = GameObject.FindAnyObjectByType<Camera>();

    }

    private void Update()
    {
        if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 5000.0f, (1 << 8)))
        {
            transform.position = (hit.point);
        }

        if (Input.GetMouseButtonDown(0))
        {
            var build = Instantiate(prefab, transform.position, transform.rotation);
            foreach(GameObject villager in unitsController.getAllUnitsFromType(UnitType.Villager))
            {
                if (prefab.GetComponent<Build>().buildtype == BuildType.FoodFarm)
                {
                    villager.GetComponent<Unit>().currentBlueprint = build;
                    villager.GetComponent<Unit>().state = Unit.UnitState.onBuilding;
                    villager.GetComponent<Unit>().MoveToBuild(transform.position);
                    break;
                }
                else
                {
                    villager.GetComponent<Unit>().currentBlueprint = build;
                    villager.GetComponent<Unit>().state = Unit.UnitState.onBuilding;
                    villager.GetComponent<Unit>().MoveToBuild(transform.position);
                }
            }
            manager.InConstructMode = false;
            Destroy(gameObject);
        }
    }
}
