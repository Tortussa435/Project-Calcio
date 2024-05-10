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

    public string compareString;
    public float compareFloat;

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

            case Rule.PlayerTacticGeneric:
                valueToCheck = Convert.ToInt32((S_GlobalManager.selectedTeam.teamTactics.teamTactic == SO_Tactics.Tactic.Generic)) * 100;
                break;

            case Rule.OpponentTacticGeneric:
                try
                {
                    valueToCheck = Convert.ToInt32(S_PlayerMatchSimulator.GetOpponentTeam().teamTactics.teamTactic == SO_Tactics.Tactic.Generic) * 100;
                }
                catch
                {
                    valueToCheck = 0;
                    Debug.LogWarning("Avversario o tattica avversario non trovata");
                }
                break;

            case Rule.PlayerWinning:
                valueToCheck = Convert.ToInt32(S_PlayerMatchSimulator.PlayerWinning())*100;
                break;

            case Rule.PlayerLosing:
                valueToCheck = Convert.ToInt32(S_PlayerMatchSimulator.OpponentWinning())*100;
                break;

            case Rule.PlayerDrawing:
                valueToCheck = Convert.ToInt32(S_PlayerMatchSimulator.matchScore.Drawing()) * 100;
                break;

            case Rule.PlayerTrait:
                valueToCheck = Convert.ToInt32(S_GlobalManager.squad.TeamContainsTrait((SO_PlayerTrait.PlayerTraitNames)System.Enum.Parse(typeof(SO_PlayerTrait.PlayerTraitNames), compareString)));
                valueToCheck *= 100;
                break;
            case Rule.Derby:
                valueToCheck = S_PlayerMatchSimulator.isDerby ? 100 : 0;
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
            
            case ScoreDirection.LowerThan:
                valueToCheck = Convert.ToInt32(valueToCheck < compareFloat);
                break;
            
            case ScoreDirection.HigherThan:
                valueToCheck = Convert.ToInt32(valueToCheck > compareFloat);
                break;
            
            case ScoreDirection.Equal:
                valueToCheck = Convert.ToInt32(valueToCheck == compareFloat);
                break;
        }

        valueToCheck = valueToCheck * scoreMultiplier;

        return valueToCheck;
    }
}
