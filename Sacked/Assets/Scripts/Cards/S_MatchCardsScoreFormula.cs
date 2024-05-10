using System.Collections;
using System.Collections.Generic;
using static S_FootballEnums;
using static S_PlayerMatchSimulator;
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
                Debug.LogWarning("Rule not implemented!");
                valueToCheck = 1.0f;
                break;
            
            case MatchRule.Constant:
                valueToCheck = 1.0f;
                break;

            case MatchRule.Aggressivity:
                valueToCheck = (float)matchAggressivity.home + (float)matchAggressivity.away; //it should be in range of 0-6, so it should have a low score multiplier
                break;

            case MatchRule.SkillDifference:
                valueToCheck =(float)((S_GlobalManager.selectedTeam.SkillLevel - GetOpponentTeam().SkillLevel) + S_GlobalManager.MAXTEAMSKILLLEVEL) / (float)(S_GlobalManager.MAXTEAMSKILLLEVEL * 2);
                break;

            case MatchRule.YellowCards:
                //pay attention! the score has to have a very low score multiplier bc it just counts the amount of red cards!
                valueToCheck = (float)YellowCards.Count + (float)opponentYellowCards.Count;
                break;

            case MatchRule.RedCards:
                //pay attention! the score has to have a very low score multiplier bc it just counts the amount of red cards!
                valueToCheck = (float)RedCards.Count + (float)opponentRedCards.Count;
                break;

            case MatchRule.Derby:
                valueToCheck = isDerby ? 1.0f : 0.0f;
                break;

        }

        

        valueToCheck = S_FootballEnums.GetScoreDirection(direction, valueToCheck, compareFloat);

        valueToCheck = (float)valueToCheck * scoreMultiplier;
        return valueToCheck;

    }

}
