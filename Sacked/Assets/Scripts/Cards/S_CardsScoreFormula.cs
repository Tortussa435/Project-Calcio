using System;
using System.Collections;
using System.Collections.Generic;
using static S_FootballEnums;
using UnityEngine;
using NaughtyAttributes;
using Unity.VisualScripting;

[System.Serializable]
public class S_CardsScoreFormula
{



    private bool showBaseFormula() { return GetType().ToString() == "S_CardsScoreFormula"; }
    [ShowIf("showBaseFormula")]
    [AllowNesting]
    public Rule desiredValue;

    public ScoreDirection direction;

    [ShowIf("direction",ScoreDirection.Equal)]
    [AllowNesting]
    public string compareString;

    private bool needsCompareFloat() { return direction == ScoreDirection.HigherThan || direction == ScoreDirection.LowerThan || direction == ScoreDirection.Equal; }
    
    [ShowIf("needsCompareFloat")]
    [AllowNesting]
    public float compareFloat;

    [ShowIf("direction",ScoreDirection.CustomCurve)]
    [AllowNesting]
    public AnimationCurve customCurve;

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

            case Rule.Injuries:
                valueToCheck = (float)S_PlayerMatchSimulator.injuries.home+(float)S_PlayerMatchSimulator.injuries.away;
                break;

            case Rule.GameMinute:
                valueToCheck = (float)S_PlayerMatchSimulator.matchMinute;
                break;

            case Rule.PlayerSubstitutions:
                valueToCheck = S_PlayerMatchSimulator.IsPlayerHomeTeam() ? S_PlayerMatchSimulator.substitutions.home : S_PlayerMatchSimulator.substitutions.away;
                break;

            case Rule.OpponentSubstitutions:
                valueToCheck = !S_PlayerMatchSimulator.IsPlayerHomeTeam() ? S_PlayerMatchSimulator.substitutions.home : S_PlayerMatchSimulator.substitutions.away;
                break;
        }

        valueToCheck = S_FootballEnums.GetScoreDirection(direction, valueToCheck, compareFloat, customCurve);

        valueToCheck = (float)valueToCheck * (float)scoreMultiplier;

        return valueToCheck;
    }
}
