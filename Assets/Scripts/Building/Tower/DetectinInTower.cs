using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectinInTower : MonoBehaviour
{
    [SerializeField] Build tower;
    [SerializeField] public GameObject arrowTarget;
    [SerializeField] ParticleSystem arrowEmission;
    [SerializeReference] Transform arrowReleasePoint;
    public float speed = 1.0f;
    [SerializeField] public float TurretDmg;
    public float TowerRange = 30f;

    private void Start()
    {
        StartCoroutine(ShootArrows());
    }
    private void OnTriggerStay(Collider other)
    {
        if (!tower.onBuilding)
        {
            if (other.tag == "Enemy")
            {
                if (other.gameObject.GetComponent<Unit>() != null)
                {
                    if (Vector3.Distance(transform.position, other.transform.position) <= TowerRange)
                    {
                        if (arrowTarget == null)
                        {
                            arrowTarget = other.gameObject;
                        }
                    }
                }
            }
        }
    }
    IEnumerator ShootArrows()
    {
        while (true)
        {
            if (!tower.onBuilding)
            {
                if (arrowTarget != null)
                {
                    CorrectArrowLogic();
                }
                else
                {
                    arrowEmission.Stop();
                }
            }
            yield return new WaitForSeconds(1f);
        }

    }
    void CorrectArrowLogic()
    {
        var main = arrowEmission.main;
        main.simulationSpeed = speed;
        arrowEmission.transform.position = arrowTarget.transform.position;
        var shape = arrowEmission.shape;
        shape.position = arrowEmission.transform.InverseTransformPoint(arrowReleasePoint.position);
        arrowEmission.Play();
    }
    private void Update()
    {
       
    }
   
}
