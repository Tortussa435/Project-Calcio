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
                valueToCheck = (float)S_GlobalManager.President/100.0f;
                break;
           
            case Rule.Team:
                valueToCheck = (float)S_GlobalManager.Team/100.0f;
                break;
            
            case Rule.Supporters:
                valueToCheck = (float)S_GlobalManager.Supporters/100.0f;
                break;
            
            case Rule.Money:
                valueToCheck = (float)S_GlobalManager.Money/100.0f;
                break;
            
            case Rule.TeamsSkillDifference:
                valueToCheck = (float)((S_GlobalManager.selectedTeam.SkillLevel - S_GlobalManager.nextOpponent.SkillLevel)+S_GlobalManager.MAXTEAMSKILLLEVEL)/(float)(S_GlobalManager.MAXTEAMSKILLLEVEL*2);
                break;

            case Rule.Constant:
                valueToCheck = 1.0f;
                break;

            case Rule.None:
                valueToCheck = 0.0f;
                break;

            case Rule.PlayerTacticGeneric:
                valueToCheck = S_GlobalManager.selectedTeam.teamTactics.teamTactic == SO_Tactics.Tactic.Generic ? 1.0f : 0.0f;
                break;

            case Rule.OpponentTacticGeneric:
                try
                {
                    valueToCheck = S_PlayerMatchSimulator.GetOpponentTeam().teamTactics.teamTactic == SO_Tactics.Tactic.Generic ? 1.0f : 0.0f;
                }
                catch
                {
                    valueToCheck = 0.0f;
                    Debug.LogWarning("Avversario o tattica avversario non trovata");
                }
                break;

            case Rule.PlayerWinning:
                valueToCheck = S_PlayerMatchSimulator.PlayerWinning() ? 1.0f : 0.0f;
                break;

            case Rule.PlayerLosing:
                valueToCheck = S_PlayerMatchSimulator.OpponentWinning() ? 1.0f : 0.0f;
                break;

            case Rule.PlayerDrawing:
                valueToCheck = S_PlayerMatchSimulator.matchScore.Drawing() ? 1.0f : 0.0f;
                break;

            case Rule.PlayerTrait:
                valueToCheck = S_GlobalManager.squad.TeamContainsTrait((SO_PlayerTrait.PlayerTraitNames)System.Enum.Parse(typeof(SO_PlayerTrait.PlayerTraitNames), compareString)) ? 1.0f : 0.0f;
                break;

            case Rule.Derby:
                valueToCheck = S_PlayerMatchSimulator.isDerby ? 1.0f : 0.0f;
                break;
        }

        valueToCheck = S_FootballEnums.GetScoreDirection(direction, valueToCheck, compareFloat);

        valueToCheck = (float)valueToCheck * (float)scoreMultiplier;

        return valueToCheck;
    }
}
