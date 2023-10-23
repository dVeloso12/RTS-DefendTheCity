using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodTrigger : MonoBehaviour
{
    [SerializeField] GameObject FarmFoodScriptLoc;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Unit>() != null &&
          other.gameObject.GetComponent<Unit>().Type == UnitType.Villager &&
          other.gameObject.GetComponent<Unit>().state == Unit.UnitState.onFarming &&
          other.gameObject.GetComponent<Unit>().depositeResource == false &&
           other.gameObject.GetComponent<Unit>().currentResourceFarm == FarmFoodScriptLoc)
        {
            StartCoroutine(StartFarm(other));
        }
    }
    IEnumerator StartFarm(Collider other)
    {
        yield return new WaitForSeconds(0.4f);
        other.gameObject.GetComponent<Unit>().readyToFarm = true;
        other.gameObject.GetComponent<Unit>().StopMoving();
    }
}
