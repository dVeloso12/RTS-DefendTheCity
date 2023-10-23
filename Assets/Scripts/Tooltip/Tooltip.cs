using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tooltip : MonoBehaviour
{
    TooltipManager manager;
    [SerializeField]  string ResourceFrist;
    [SerializeField]  int quantFrist;
    [SerializeField]  string ResourceSecond;
    [SerializeField]  int quantSecond;

    public void setToolTipThings(string typeFrist = "", int quantFrist = 0,
        string typeSecond = "", int quantSecond = 0)
    {
        ResourceFrist = typeFrist;
        this.quantFrist = quantFrist;
        ResourceSecond = typeSecond;
        this.quantSecond = quantSecond;
    }
    public void MouseEnter()
    {
        TooltipManager.instance.gameObject.SetActive(true);
        TooltipManager.instance.SetToolTip(ResourceFrist, quantFrist, ResourceSecond, quantSecond);
    }
    public void MouseExit()
    {
        TooltipManager.instance.DisableTooltip();
    }


}
