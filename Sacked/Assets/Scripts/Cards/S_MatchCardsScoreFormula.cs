using System.Collections;
using System.Collections.Generic;
using static S_FootballEnums;
using UnityEngine;

[System.Serializable]
public class S_MatchCardsScoreFormula : S_CardsScoreFormula
{

    public MatchRule matchFormula;
    
    private S_MatchCardsScoreFormula()
    {
        desiredValue = Rule.None;
    }

    //REDO al momento calculate score ignora i parametri imposti
    public override float CalculateScore()
    {
        float valueToCheck = 0.0f;

        switch (matchFormula)
        {
            default:
                valueToCheck = 100;
                break;
            
            case MatchRule.Constant:
                valueToCheck = 100;
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
