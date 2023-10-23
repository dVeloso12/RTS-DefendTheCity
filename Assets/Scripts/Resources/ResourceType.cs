using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ResourceTypes
{
    Nothing,Wood,Food,Gold,Iron
}
public class ResourceType : MonoBehaviour
{
    public ResourceTypes type;
    [SerializeField] public float ResourceHP = 100;
    [SerializeField] public GameObject farmSpot;

    private void Update()
    {
        if (type == ResourceTypes.Wood)
        {
            if (ResourceHP <= 0 && ResourceHP != -10)
            {
                StartCoroutine(Die());
                ResourceHP = -10;
            }
        }
        else if (type == ResourceTypes.Food)
        {
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<Unit>() != null &&
            collision.gameObject.GetComponent<Unit>().Type == UnitType.Villager &&
            collision.gameObject.GetComponent<Unit>().state == Unit.UnitState.onFarming &&
            collision.gameObject.GetComponent<Unit>().depositeResource == false &&
             collision.gameObject.GetComponent<Unit>().currentResourceFarm == this.gameObject &&
             (type == ResourceTypes.Iron || type == ResourceTypes.Gold))
        {
            Debug.Log("Villager");
            collision.gameObject.GetComponent<Unit>().readyToFarm = true;
            collision.gameObject.GetComponent<Unit>().StopMoving(); 
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.GetComponent<Unit>() != null && 
            other.gameObject.GetComponent<Unit>().Type == UnitType.Villager &&
            other.gameObject.GetComponent<Unit>().state == Unit.UnitState.onFarming &&
            other.gameObject.GetComponent<Unit>().depositeResource == false &&
             other.gameObject.GetComponent<Unit>().currentResourceFarm == this.gameObject &&
             type == ResourceTypes.Wood)
        {
            StartCoroutine(StartFarm(other.gameObject));
        }
    }
    

    IEnumerator StartFarm(GameObject other)
    {
        yield return new WaitForSeconds(0.4f);
        other.gameObject.GetComponent<Unit>().readyToFarm = true;
        other.gameObject.GetComponent<Unit>().StopMoving();
    }
    IEnumerator Die()
    {
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }

}
