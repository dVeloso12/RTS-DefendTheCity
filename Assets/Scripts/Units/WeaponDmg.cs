using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponDmg : MonoBehaviour
{
    [SerializeField] Unit unit;
    public bool hit;
    private void OnTriggerEnter(Collider other)
    {

        if ((other.gameObject.GetComponent<Unit>() ||
            other.gameObject.GetComponent<Build>()) && 
            (other.gameObject == unit.currentAllyToFight ||
            other.gameObject == unit.currentEnemyToFight))
        {
            Debug.Log(other.gameObject.name);
            hit = true;
        }
        else if(other.gameObject.GetComponent<VillageCenter>() != null)
        {
            Debug.Log(other.gameObject.name);
            hit = true;
        }
    }
  
}
