using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Selection_Dictonary : MonoBehaviour
{
    public Dictionary<int,GameObject> selectedTable = new Dictionary<int, GameObject> ();
    
    public void addSelected(GameObject go)
    {
        int id = go.GetInstanceID();
        
        if(!selectedTable.ContainsKey(id) && (go.layer ==  3
            || go.layer == 6 && go.tag == "Ally"))
        {
            if (go.GetComponent<Build>() != null)
            {
                if(!go.GetComponent<Build>().onBuilding)
                {
                    selectedTable.Add(id, go);
                    go.AddComponent<SelectionComponet>();
                    Debug.Log("Added " + id + " to selected dict");
                }
            }
            else
            {
                selectedTable.Add(id, go);
                go.AddComponent<SelectionComponet>();
                Debug.Log("Added " + id + " to selected dict");
            }
        }
    }

    public void deselect(int id)
    {
        Destroy(selectedTable[id].GetComponent<SelectionComponet>());
        selectedTable.Remove(id);
        Debug.Log("Deselect");

    }

    public void deselectAll()
    {
        foreach (KeyValuePair<int,GameObject> pair in selectedTable) { 

                if(pair.Value != null)
                {
                    Destroy(selectedTable[pair.Key].GetComponent<SelectionComponet>());
                }
        }
        Debug.Log("Deselect all");
        selectedTable.Clear();
    }
}
