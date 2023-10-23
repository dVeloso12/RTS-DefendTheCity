using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum BuildType
{
    Barricks,VillageCenter,FoodFarm,Tower
};
public class Build : MonoBehaviour
{
    
    [SerializeField] public BuildType buildtype;
    [SerializeField] Transform spawnPosition;
    GameManager gameManager;
    [SerializeField] public float UnitCraftCD;
    [SerializeField] GameObject UnitPrefab;
    [SerializeField] GameObject ArcherPrefab;
    GameObject unitToSpawn;
    [SerializeField] GameObject selectCircle;
    [SerializeField] float startYPosition = -16.7f;
    [SerializeField] public float EndYPosition = -5.7f;
    public bool isUsed;
    public float BuildHp = 100f;
    public float MaxBuildHp = 100f;


    public bool onBuilding;
    float Timer;
    bool canCraft;

    private void Start()
    {
        gameManager = GameObject.FindAnyObjectByType<GameManager>();
       
        if (buildtype != BuildType.VillageCenter)
        {
            onBuilding = true;
            transform.position = new Vector3(transform.position.x, startYPosition, transform.position.z);
        }
        
    }

    public void SpawnUnit(UnitType type)
    {
        canCraft = true;
        if(type == UnitType.Soldier || type == UnitType.Villager)
        {
            unitToSpawn = UnitPrefab;
        }
        else if(type == UnitType.Archer)
        {
            unitToSpawn = ArcherPrefab;
        }
    }

    public void StateOfSelectedState(bool isSelected)
    {
        if(isSelected)
        {
            selectCircle.SetActive(true);
        }
        else
        {
            selectCircle.SetActive(false);
        }
    }
    private void Update()
    {
        if (BuildHp > 0)
        {
            if (buildtype == BuildType.VillageCenter || buildtype == BuildType.Barricks || buildtype == BuildType.Tower)
            {
                if (!onBuilding)
                {
                    if (canCraft)
                    {
                        Timer += Time.deltaTime;
                        if (Timer >= UnitCraftCD)
                        {
                            var unit = Instantiate(unitToSpawn, spawnPosition.position, Quaternion.identity);
                            if (unit.GetComponent<Unit>() != null)
                            {
                                if (unit.GetComponent<Unit>().Type == UnitType.Villager)
                                {
                                    if (!gameManager.allVillagers.Contains(unit.GetComponent<Unit>()))
                                    {
                                        gameManager.allVillagers.Add(unit.GetComponent<Unit>());
                                    }
                                }
                            }
                            canCraft = false;
                            Timer = 0;
                        }
                    }
                }
            }
        }else
        {
            Destroy(gameObject);
        }
  
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.GetComponent<Unit>() != null)
        {
            if(collision.gameObject.GetComponent<Unit>().Type == UnitType.Villager)
            {
                collision.gameObject.GetComponent<Unit>().animator.SetBool("Building", true);
                collision.gameObject.GetComponent<Unit>().canMove = false;

            }
        }
    }

}
