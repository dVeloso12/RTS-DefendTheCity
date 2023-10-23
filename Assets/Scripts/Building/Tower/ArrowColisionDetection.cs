using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowColisionDetection : MonoBehaviour
{
    [SerializeField] DetectinInTower tower;
    private void OnParticleCollision(GameObject other)
    {
        if (other.tag == "Enemy")
        {
           DealDmgToTarget(tower.arrowTarget);
        }
    }
    void DealDmgToTarget(GameObject target)
    {
        if(target.GetComponent<Unit>() != null)
        {
            target.GetComponent<Unit>().UnitHp -= tower.TurretDmg;
            if(target.GetComponent<Unit>().UnitHp <= 0)
            {
                tower.arrowTarget = null;
            }
        }
    }
}
