using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TooltipManager : MonoBehaviour
{
   public static TooltipManager instance;
    [Header("Tooltip Apparence Stuff")]
    [SerializeField] GameObject Tooltip;
    [SerializeField] Image RecipeImage_Frist;
    [SerializeField] Image RecipeImage_Second;
    [SerializeField] GameObject GameobjRecipe_Second;
    [SerializeField] TextMeshProUGUI RecipeTxt_Frist;
    [SerializeField] TextMeshProUGUI RecipeTxt_Second;
    [SerializeField] List<Sprite> ResourceList;

    private void Awake()
    {
        if(instance != null && instance != this) 
        {
            Destroy(this.gameObject);
        }else
        {
            instance = this;
        }
    }
    private void Start()
    {
        gameObject.SetActive(false);
    }
    private void Update()
    { 
        transform.position = Input.mousePosition;
    }
    public void DisableTooltip()
    {
        Tooltip.SetActive(false);
    }
    public void SetToolTip(string typeFrist = "",int quantFrist = 0,
        string typeSecond = "",int quantSecond = 0)
    {
        switch(typeFrist)
        {
            case "Wood":
                {
                    RecipeImage_Frist.sprite = ResourceList[0];
                    RecipeTxt_Frist.text = quantFrist.ToString();
                    break;
                }
            case "Food":
                {
                    RecipeImage_Frist.sprite = ResourceList[1];
                    RecipeTxt_Frist.text = quantFrist.ToString();
                    break;
                }
            case "Iron":
                {
                    RecipeImage_Frist.sprite = ResourceList[2];
                    RecipeTxt_Frist.text = quantFrist.ToString();
                    break;
                }
            case "Gold":
                {
                    RecipeImage_Frist.sprite = ResourceList[3];
                    RecipeTxt_Frist.text = quantFrist.ToString();
                    break;
                }
            default:
                {
                    Tooltip.SetActive(false);
                    return;
                }
        }
        switch (typeSecond)
        {
            case "Wood":
                {
                    RecipeImage_Second.sprite = ResourceList[0];
                    RecipeTxt_Second.text = quantSecond.ToString();
                    GameobjRecipe_Second.SetActive(true);
                    break;
                }
            case "Food":
                {
                    RecipeImage_Second.sprite = ResourceList[1];
                    RecipeTxt_Second.text = quantSecond.ToString();
                    GameobjRecipe_Second.SetActive(true);
                    break;
                }
            case "Iron":
                {
                    RecipeImage_Second.sprite = ResourceList[2];
                    RecipeTxt_Second.text = quantSecond.ToString();
                    GameobjRecipe_Second.SetActive(true);
                    break;
                }
            case "Gold":
                {
                    RecipeImage_Second.sprite = ResourceList[3];
                    RecipeTxt_Second.text = quantSecond.ToString();
                    GameobjRecipe_Second.SetActive(true);
                    break;
                }
            default:
                {
                    GameobjRecipe_Second.SetActive(false);
                    break;
                }
        }
    }
}
