using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshDebug : MonoBehaviour
{
    public bool velocity;
    public bool desiredVelocity;
    public bool Path;
    NavMeshAgent agent;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }
    private void OnDrawGizmos()
    {
       if(velocity) {
       
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, transform.position + agent.velocity);
       }
       if(desiredVelocity)
       {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position,transform.position + agent.desiredVelocity);
       }
       if(Path)
       {
            Gizmos.color= Color.black;
            var agentPath = agent.path;
            Vector3 prevCorner = transform.position;
            foreach(var corner in agentPath.corners)
            {
                Gizmos.DrawLine(prevCorner, corner);
                Gizmos.DrawSphere(corner, 0.01f);
                prevCorner = corner;
            }
       }
    }
}
