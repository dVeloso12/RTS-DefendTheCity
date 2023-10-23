using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using UnityEngine.UIElements;

public class BowDmg : MonoBehaviour
{

    [SerializeField] public GameObject arrowTarget;
    [SerializeField] ParticleSystem arrowEmission;
    [SerializeReference] Transform arrowReleasePoint;
    public float speed = 1.0f;
    [SerializeField] Unit unit;

    public void CorrectArrowLogic()
    {
        if (arrowTarget != null)
        {
            var main = arrowEmission.main;
            main.simulationSpeed = speed;
            arrowEmission.transform.position = arrowTarget.transform.position;
            var shape = arrowEmission.shape;
            shape.position = arrowEmission.transform.InverseTransformPoint(arrowReleasePoint.position);
            arrowEmission.Play();
            if (arrowTarget.GetComponent<Unit>() != null)
            {
                arrowTarget.GetComponent<Unit>().UnitHp -= unit.WeaponDmg;
                if (arrowTarget.GetComponent<Unit>().UnitHp <= 0)
                {
                    arrowTarget = null;
                }
            }
            else if (arrowTarget.GetComponent<Build>() != null)
            {
                arrowTarget.GetComponent<Build>().BuildHp -= unit.WeaponDmg;
                if (arrowTarget.GetComponent<Build>().BuildHp <= 0)
                {
                    arrowTarget = null;
                }
            }

        }
        else
        {
            arrowEmission.Stop();

        }
    }

}
