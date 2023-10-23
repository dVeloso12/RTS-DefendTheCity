using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResourcesAmountUI : MonoBehaviour
{
    [SerializeField] List<TextMeshProUGUI> allAmountTxt;
    [SerializeField] List<TextMeshProUGUI> workersAmountTxt;
    UnitsController unitsController;
    GameManager gameManager;

    private void Start()
    {
        gameManager = GameObject.FindObjectOfType<GameManager>();
        unitsController = GameObject.FindAnyObjectByType<UnitsController>();
    }
    private void Update()
    {
        UpdateAmountInfo();
    }
    void UpdateAmountInfo()
    {
        allAmountTxt[0].text = (gameManager.Food).ToString();
        allAmountTxt[1].text = (gameManager.Wood).ToString();
        allAmountTxt[2].text = (gameManager.Gold).ToString();
        allAmountTxt[3].text = (gameManager.Iron).ToString();

        workersAmountTxt[0].text = (gameManager.FoodWorkers.Count).ToString();
        workersAmountTxt[1].text = (gameManager.WoodWorkers.Count).ToString();
        workersAmountTxt[2].text = (gameManager.GoldWorkers.Count).ToString();
        workersAmountTxt[3].text = (gameManager.IronWorkers.Count).ToString();

    }
    
}
