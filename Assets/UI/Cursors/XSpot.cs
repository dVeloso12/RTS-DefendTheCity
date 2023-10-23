using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XSpot : MonoBehaviour
{
    Camera cam;
    void Start()
    {
        cam = GameObject.FindAnyObjectByType<Camera>();
    }
    public void showX()
    {
        if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 5000.0f, (1 << 8)))
        {
            transform.position =  new Vector3(hit.point.x, -5.99f, hit.point.z);
        }
    }
    public void hideX()
    {
        transform.position = new Vector3(1000f,1000f,1000f);
    }
}
