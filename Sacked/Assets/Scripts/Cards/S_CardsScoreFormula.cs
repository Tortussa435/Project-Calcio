using System;
using System.Collections;
using System.Collections.Generic;
using static S_FootballEnums;
using UnityEngine;
[System.Serializable]
public class S_CardsScoreFormula
{

    

    public Rule desiredValue;
    public ScoreDirection direction;
    public float scoreMultiplier=1.0f;

    public virtual float CalculateScore()
    {
        float valueToCheck = 0.0f;
        
        switch (desiredValue)
        {
            case Rule.President:
                valueToCheck = S_GlobalManager.President;
                break;
           
            case Rule.Team:
                valueToCheck = S_GlobalManager.Team;
                break;
            
            case Rule.Supporters:
                valueToCheck = S_GlobalManager.Supporters;
                break;
            
            case Rule.Money:
                valueToCheck = S_GlobalManager.Money;
                break;
            
            case Rule.TeamsSkillDifference:
                valueToCheck = ((S_GlobalManager.selectedTeam.SkillLevel - S_GlobalManager.nextOpponent.SkillLevel)+S_GlobalManager.MAXTEAMSKILLLEVEL)*(S_GlobalManager.MAXTEAMSKILLLEVEL*2);
                break;
            case Rule.Constant:
                valueToCheck = 100;
                break;
            case Rule.None:
                valueToCheck = 0;
                break;
        }
        
        valueToCheck = valueToCheck / 100;

        switch (direction)
        {
            case ScoreDirection.InverseLinear:
                valueToCheck = 1 - valueToCheck;
                break;
            case ScoreDirection.Linear:
                break;
            
            case ScoreDirection.Round:
                valueToCheck = Mathf.Round(valueToCheck);
                break;
           
            case ScoreDirection.InverseRound:
                valueToCheck = 1 - Mathf.Round(valueToCheck);
                break;
        }

        valueToCheck = valueToCheck * scoreMultiplier;

        return valueToCheck;
    }
}
