using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnResources : MonoBehaviour
{
    public Vector2 size;
    public int numberSpawnsTree;
    public int numberSpawnsIron;
    public int numberSpawnsGold;
    public List<GameObject> listOfPrefabs;
    [Header("Transforms")]
    public Transform treeTransform;
    public Transform IronTransform;
    public Transform GoldTransform;

    private void Start()
    {
        SpawnAll();
    }
    void SpawnAll()
    {
        for (int i = 0; i < numberSpawnsTree; i++)
        {
            Spawn(listOfPrefabs[0],treeTransform);
        }
        for (int i = 0; i < numberSpawnsIron; i++)
        {
            Spawn(listOfPrefabs[1], IronTransform);
        }
        for (int i = 0; i < numberSpawnsGold; i++)
        {
            Spawn(listOfPrefabs[2], GoldTransform);
        }
    }
    void Spawn(GameObject prefab,Transform save)
    {
        Vector3 randomSpawnPosition =
            new Vector3(Random.Range(transform.position.x, transform.position.x + size.x),
            transform.position.y,
            Random.Range(transform.position.z,transform.position.z + size.y));
        var prafab = Instantiate(prefab,randomSpawnPosition, Quaternion.identity, save);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        float radius = 1f;
        Gizmos.DrawSphere(transform.position, radius);
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(0f, 0f, size.y));
        Gizmos.DrawSphere(transform.position + new Vector3(size.x,0f,0f), radius);
        Gizmos.DrawLine(transform.position + new Vector3(0f, 0f, size.y), transform.position + new Vector3(size.x, 0f, size.y));
        Gizmos.DrawSphere(transform.position + new Vector3(0f, 0f, size.y), radius);
        Gizmos.DrawLine(transform.position + new Vector3(size.x, 0f, size.y), transform.position + new Vector3(size.x, 0f, 0f));
        Gizmos.DrawSphere(transform.position + new Vector3(size.x, 0f, size.y), radius);
        Gizmos.DrawLine(transform.position + new Vector3(size.x, 0f, 0f), transform.position);

    }
}
