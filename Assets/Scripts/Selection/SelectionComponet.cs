using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionComponet : MonoBehaviour
{
    
    
    void Start(){
        
        if(GetComponent<Build>() != null) 
        {
            GetComponent<Build>().StateOfSelectedState(true);
        }
        else if(GetComponent<Unit>() != null)
        {
            GetComponent<Unit>().StateOfSelectedState(true);

        }
    }
   private void OnDestroy() {

        if (GetComponent<Build>() != null)
        {
            GetComponent<Build>().StateOfSelectedState(false);

        }
        else if (GetComponent<Unit>() != null)
        {
            GetComponent<Unit>().StateOfSelectedState(false);

        }
    }
}
