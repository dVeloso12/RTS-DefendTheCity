using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InteractUIManager : MonoBehaviour
{
    UnitsController unitsController;
    [SerializeField] List<Sprite> allSprites;
    [SerializeField] List<Button> allButtons;
    [SerializeField] List<GameObject> allBlueprints;
    [SerializeField] List<Image> allCraftingTimers;
    [SerializeField] List<AnimationClip> allTimeCrafting;

    //BUG FIXING !!!
    //ao criar dois tipos diferentes , acaba por bugar o UI

    int onCrafting = 0;
    GameManager gameManager;
    private List<GameObject> LastListState;
    private bool swap;
    private void Start()
    {
        gameManager = GameObject.FindAnyObjectByType<GameManager>();
        unitsController = GameObject.FindAnyObjectByType<UnitsController>(); 
    }

    private void Update()
    {
        UpdateUIType();

        UpdateUnitsUIMode();
    }

    void UpdateUIType()
    {
        if (unitsController.CountOfUnits() > 1)
        {
            UpdateUnitsUIMode();

        }
        else if(unitsController.CountOfUnits() == 0 || swap == true)
        {
            ResetAllButtons();

            swap = false;
        }
    }
    void UpdateUnitsUIMode()
    {
            if (unitsController.ConstainsUnitType(UnitType.Villager))
            {
               if(LastListState != unitsController.getAllUnitsFromType(UnitType.Villager))
               {
                    swap = true;
                    LastListState = unitsController.getAllUnitsFromType(UnitType.Villager);
               }
                foreach (Button button in allButtons)
                {
                    if(button.gameObject.name == "Button_1")
                    {
                        if (!button.transform.GetChild(0).GetComponent<Image>().enabled)
                        {
                            var image = button.transform.GetChild(0).GetComponent<Image>();
                            image.enabled = true;
                            image.sprite = allSprites[1];
                            button.onClick.AddListener(SpawnBarricks);
                            button.GetComponent<Tooltip>().setToolTipThings("Wood", 200);
                         }
                        
                    }
                    else if (button.gameObject.name == "Button_3")
                    {
                       if (!button.transform.GetChild(0).GetComponent<Image>().enabled)
                         {
                                var image = button.transform.GetChild(0).GetComponent<Image>();
                                image.enabled = true;
                                image.sprite = allSprites[4];
                                button.onClick.AddListener(SpawnFoodFarm);
                                button.GetComponent<Tooltip>().setToolTipThings("Wood", 100);
                         }
                    }
                else if (button.gameObject.name == "Button_5")
                {
                    if (!button.transform.GetChild(0).GetComponent<Image>().enabled)
                    {
                        var image = button.transform.GetChild(0).GetComponent<Image>();
                        image.enabled = true;
                        image.sprite = allSprites[5];
                        button.onClick.AddListener(SpawnTower);
                        button.GetComponent<Tooltip>().setToolTipThings("Wood", 400,"Iron",300);
                    }
                }
            }
            
        }
        UpdateBuildsUIMode();
    }
    void UpdateBuildsUIMode()
    {
        //Debug.Log("Villager : " + unitsController.ContaisnBuildType(BuildType.VillageCenter));

        if (unitsController.ContaisnBuildType(BuildType.Barricks))
        {
            if (LastListState != unitsController.getAllUnitsFromType(UnitType.Villager))
            {
                swap = true;
                LastListState = unitsController.getCurrentSelectBuild(BuildType.Barricks);
            }
            foreach (Button button in allButtons)
            {
                if (button.gameObject.name == "Button_1")
                {
                    if (!button.transform.GetChild(0).GetComponent<Image>().enabled)
                    {
                        var image = button.transform.GetChild(0).GetComponent<Image>();
                        image.enabled = true;
                        image.sprite = allSprites[2];
                        button.onClick.AddListener(CraftSoldier);
                        button.GetComponent<Tooltip>().setToolTipThings("Food", 75, "Gold", 100);

                    }
                }
                else if (button.gameObject.name == "Button_3")
                {
                    if (!button.transform.GetChild(0).GetComponent<Image>().enabled)
                    {
                        var image = button.transform.GetChild(0).GetComponent<Image>();
                        image.enabled = true;
                        image.sprite = allSprites[6];
                        button.onClick.AddListener(CraftArcher);
                        button.GetComponent<Tooltip>().setToolTipThings("Food", 90, "Gold", 120);
                    }
                }
            }
            
        }
        else if (unitsController.ContaisnBuildType(BuildType.VillageCenter))
        {
            foreach (Button button in allButtons)
            {
                if (LastListState != unitsController.getAllUnitsFromType(UnitType.Villager))
                {
                    swap = true;
                    LastListState = unitsController.getCurrentSelectBuild(BuildType.VillageCenter);
                }

                if (button.gameObject.name == "Button_1")
                {
                    if (!button.transform.GetChild(0).GetComponent<Image>().enabled)
                    {
                        var image = button.transform.GetChild(0).GetComponent<Image>();
                        image.enabled = true;
                        image.sprite = allSprites[3];
                        button.onClick.AddListener(CraftVillager);
                        button.GetComponent<Tooltip>().setToolTipThings("Food", 50, "Wood",20);
                    }
                } 
            }    
        }
    }


    IEnumerator CraftTimerUI(float time, int spriteID)
    {
        onCrafting++;
        int saveID = onCrafting-1;
        allCraftingTimers[saveID].sprite = allSprites[spriteID];
        allCraftingTimers[saveID].enabled = true;
        allCraftingTimers[saveID].gameObject.GetComponent<Animation>().
            AddClip(getTimerAnimationClip(time), getTimerAnimationClip(time).name);
        allCraftingTimers[saveID].gameObject.GetComponent<Animation>().PlayQueued(getTimerAnimationClip(time).name);
        yield return new WaitForSeconds(time);
        allCraftingTimers[saveID].enabled = false;
        allCraftingTimers[saveID].sprite = allSprites[0];
        allCraftingTimers[saveID].fillAmount = 0;
        allCraftingTimers[saveID].gameObject.GetComponent<Animation>().RemoveClip(getTimerAnimationClip(time));
        onCrafting--;
    }
    AnimationClip getTimerAnimationClip(float Time)
    {
        if (Time == 1)
            return allTimeCrafting[0];
        else if (Time == 1)
            return allTimeCrafting[1];
        else
            return null;

    }
    void ResetAllButtons()
    {
        foreach (Button button in allButtons)
        {
             var image = button.transform.GetChild(0).GetComponent<Image>();
             image.enabled = false;
             image.sprite = allSprites[0];
             button.onClick.RemoveAllListeners();
            button.GetComponent<Tooltip>().setToolTipThings();
        }
    }
    void SpawnBarricks()
    {
        if (gameManager.Wood - 200 >= 0)
        {
            Instantiate(allBlueprints[0]);
            gameManager.Wood -= 200;
            gameManager.InConstructMode = true;
        }
    }
    void SpawnFoodFarm()
    {
        if (gameManager.Wood - 100 >= 0)
        {
            Instantiate(allBlueprints[1]);
            gameManager.Wood -= 100;
            gameManager.InConstructMode = true;
        }
    }
    void SpawnTower()
    {
        if (gameManager.Wood - 400 >= 0 && gameManager.Iron - 300 >= 0)
        {
            Instantiate(allBlueprints[2]);
            gameManager.Wood -= 400;
            gameManager.Iron -= 300;
            gameManager.InConstructMode = true;
        }
    }
    void CraftSoldier()
    {
        foreach(GameObject build in unitsController.getCurrentSelectBuild(BuildType.Barricks))
        {
            if (gameManager.Food - 75 >= 0 && gameManager.Gold - 100 >= 0)
            {
                build.GetComponent<Build>().SpawnUnit(UnitType.Soldier);
                StartCoroutine(CraftTimerUI(build.GetComponent<Build>().UnitCraftCD, 2));
                gameManager.Food -= 75;
                gameManager.Gold -= 100;
            }
        }
    }
    void CraftArcher()
    {
        foreach (GameObject build in unitsController.getCurrentSelectBuild(BuildType.Barricks))
        {
            if (gameManager.Food - 90 >= 0 && gameManager.Gold - 120 >= 0)
            {
                build.GetComponent<Build>().SpawnUnit(UnitType.Archer);
                StartCoroutine(CraftTimerUI(build.GetComponent<Build>().UnitCraftCD, 2));
                gameManager.Food -= 90;
                gameManager.Gold -= 120;
            }

        }
    }
    void CraftVillager()
    {
        foreach (GameObject build in unitsController.getCurrentSelectBuild(BuildType.VillageCenter))
        {
            if (gameManager.Food - 50 >= 0 && gameManager.Wood - 20 >= 0)
            {
                build.GetComponent<Build>().SpawnUnit(UnitType.Villager);
                StartCoroutine(CraftTimerUI(build.GetComponent<Build>().UnitCraftCD, 3));
                gameManager.Food -= 50;
                gameManager.Wood -= 20;
            }
        }
    }
}
