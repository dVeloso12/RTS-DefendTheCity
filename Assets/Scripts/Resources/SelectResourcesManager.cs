using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;

public class SelectResourcesManager : MonoBehaviour
{
    GameManager gameManager;
    UnitsController unitsController;
    Camera cam;

    private void Start()
    {
        gameManager = GetComponent<GameManager>();
        unitsController = GameObject.FindAnyObjectByType<UnitsController>();   
        cam = GameObject.FindObjectOfType<Camera>();
    }

    private void FixedUpdate()
    {
        //if (Input.GetMouseButtonDown(1))
        if (Input.GetKey(KeyCode.E))
        {
            if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 5000.0f,
               LayerMask.GetMask("Wood","Food","Units","Gold","Iron")))
            {
                if (unitsController.ConstainsUnitType(UnitType.Villager))
                {
                   if(hit.transform.gameObject.GetComponent<ResourceType>() != null)
                   {
                        switch(hit.transform.gameObject.GetComponent<ResourceType>().type)
                        {
                            case ResourceTypes.Wood:
                            {
                                foreach(GameObject villager in unitsController.getAllUnitsFromType(UnitType.Villager))
                                {
                                    var unit = villager.GetComponent<Unit>();
                                        unit.PickUpResources(hit.transform.gameObject);
                                        Debug.Log("Wood");
                                }
                               break;
                            }
                            case ResourceTypes.Food:
                            {
                                    //Fix This!!! Quero que escolha apenas 1 dos villagers para ir para a food clicada
                                    int count = 0;
                                    foreach (GameObject villager in unitsController.getAllUnitsFromType(UnitType.Villager))
                                    {
                                        if(count == 0 && !hit.transform.gameObject.GetComponent<Build>().isUsed)
                                        {
                                            var unit = villager.GetComponent<Unit>();
                                            unit.PickUpResources(hit.transform.gameObject);
                                            hit.transform.gameObject.GetComponent<Build>().isUsed = true;
                                            Debug.Log("Food");
                                            count++;
                                        }
                                      
                                    }

                                    break;
                            }
                            case ResourceTypes.Gold:
                            {
                                    foreach (GameObject villager in unitsController.getAllUnitsFromType(UnitType.Villager))
                                    {
                                        var unit = villager.GetComponent<Unit>();
                                        unit.PickUpResources(hit.transform.gameObject);
                                        Debug.Log("Gold");
                                    }
                                    break;
                            }
                            case ResourceTypes.Iron:
                            {
                                    foreach (GameObject villager in unitsController.getAllUnitsFromType(UnitType.Villager))
                                    {
                                        var unit = villager.GetComponent<Unit>();
                                        unit.PickUpResources(hit.transform.gameObject);
                                        Debug.Log("Iron");
                                    }
                                    break;
                            }
                        }
                   }
                }
                else if(unitsController.ConstainsUnitType(UnitType.Soldier) ||
                    unitsController.ConstainsUnitType(UnitType.Archer))
                {
                   if(hit.transform.tag == "Enemy")
                   {
                        if (unitsController.ConstainsUnitType(UnitType.Soldier))
                        {
                            foreach (GameObject unit in unitsController.getAllUnitsAllyFromType(UnitType.Soldier))
                            {
                                unit.GetComponent<Unit>().MoveToEnemy(hit.transform.gameObject);
                            }
                        }
                        if(unitsController.ConstainsUnitType(UnitType.Archer))
                        {
                            foreach (GameObject unit in unitsController.getAllUnitsAllyFromType(UnitType.Archer))
                            {
                                unit.GetComponent<Unit>().MoveToEnemy(hit.transform.gameObject);
                            }
                        }
                   }
                }

            }
        }
    }
}


