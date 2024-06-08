using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static S_PlayerMatchSimulator;

public class S_MatchDirectionViewer : MonoBehaviour
{
    float direction = 0.5f;
    float targetDirection = 0.5f;
    Image fillBar;
    private void Start()
    {
        fillBar = GetComponent<Image>();
    }
    private void Update()
    {
        if(homeGoalCheck==0 && awayGoalCheck == 0)
        {
            targetDirection = 0.5f;
        }
        else targetDirection = homeGoalCheck / (homeGoalCheck+awayGoalCheck);
        targetDirection = Mathf.Clamp(targetDirection, 0.2f, 0.8f);//avoids showing absolute domination from one team
        direction = S_CoolFuncs.Lerpoler(direction, targetDirection,5);
        if (fillBar != null) fillBar.fillAmount = direction;
    }
}
