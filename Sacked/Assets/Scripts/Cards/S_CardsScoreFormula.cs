using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class S_CardsScoreFormula
{
    [System.Serializable]
    public enum ScoreDirection { Linear, InverseLinear }
    public enum Rule {President, Team, Supporters, Money, AlreadyPicked }
    public Rule desiredValue;
    public ScoreDirection direction;
    public float scoreMultiplier=1.0f;
    public bool alreadyPicked;
    public float CalculateScore()
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
            
            case Rule.AlreadyPicked:
                valueToCheck = Convert.ToInt32(alreadyPicked) * 100;
                break;
        }
        
        valueToCheck = valueToCheck / 100;
        
        if (direction == ScoreDirection.InverseLinear)
        {
            valueToCheck = 1 - valueToCheck; //REDO to do cooler calculi
        }
        
        if (direction == ScoreDirection.Linear)
        {
            
        }
        
        valueToCheck = valueToCheck * scoreMultiplier;

        return valueToCheck;
    }
}
