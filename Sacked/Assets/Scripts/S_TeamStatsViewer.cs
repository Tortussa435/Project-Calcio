using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static S_CoolFuncs;
public class S_TeamStatsViewer : MonoBehaviour
{
    public Image atk;
    public Image def;

    float currentDef;
    float currentAtk;
    void Update()
    {
        currentDef = Lerpoler(currentDef, S_PlayerTeamStats.CalcSquadDef(false),5);   
        currentAtk = Lerpoler(currentAtk, S_PlayerTeamStats.CalcSquadAtk(false),5);
        atk.fillAmount = Mathf.InverseLerp(0, 5, currentAtk);
        def.fillAmount = Mathf.InverseLerp(0, 5, currentDef);
    }
}
