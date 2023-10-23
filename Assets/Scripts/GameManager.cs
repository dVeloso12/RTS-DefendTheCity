using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public bool InConstructMode;
    [Header("Resources")]
    public int Wood;
    public int Food;
    public int Gold;
    public int Iron;
    public List<Unit> WoodWorkers;
    public List<Unit> FoodWorkers;
    public List<Unit> GoldWorkers;
    public List<Unit> IronWorkers;
    public List<Unit> allVillagers;

    private void Start()
    {
        Wood = 10000; 
        Food = 10000; 
        Gold = 10000;
        Iron = 10000;
    }
}
