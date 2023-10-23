using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VillageCenter : MonoBehaviour
{
    GameManager gameManager;

    private void Start()
    {
        gameManager = GameObject.FindAnyObjectByType<GameManager>();    
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Unit>() != null &&
            other.gameObject.GetComponent<Unit>().Type == UnitType.Villager &&
            other.gameObject.GetComponent<Unit>().state == Unit.UnitState.onFarming &&
            other.gameObject.GetComponent<Unit>().depositeResource == true)
        {
            StartCoroutine(DepositeResourcesAndLeave(other));
        }
    }
    IEnumerator DepositeResourcesAndLeave(Collider other)
    {
        other.gameObject.GetComponent<Unit>().StopMoving();
        yield return new WaitForSeconds(1.5f);
        DepositeResources(other.gameObject.GetComponent<Unit>());
        if (other.gameObject.GetComponent<Unit>().currentResourceFarm.GetComponent<ResourceType>().type == ResourceTypes.Wood)
        {
            if (other.gameObject.GetComponent<Unit>().resourceFarmList.Count > 0) other.gameObject.GetComponent<Unit>().ReturnToResource();
        }
        else if(other.gameObject.GetComponent<Unit>().currentResourceFarm.GetComponent<ResourceType>().type == ResourceTypes.Food)
        {
            other.gameObject.GetComponent<Unit>().ReturnToResource();
        }
       
    }
    void DepositeResources(Unit villager)
    {
        gameManager.Wood += villager.WoodAmout;
        gameManager.Food += villager.FoodAmout;
        gameManager.Gold += villager.GoldAmout;
        gameManager.Iron += villager.IronAmout;
        villager.WoodAmout = 0;
        villager.FoodAmout = 0;
        villager.GoldAmout = 0;
        villager.IronAmout = 0;
        villager.depositeResource = false;
        villager.readyToFarm = false;
    }
}
