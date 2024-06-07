using System;
using UnityEngine;
using System.Collections.Generic;
public static class S_FootballEnums
{
    [System.Serializable]
    public enum Positivity
    {
        Negative,
        Neutral,
        Positive
    }

    [System.Serializable]
    public enum ScoreDirection { Linear, InverseLinear, Round, InverseRound, LowerThan, Equal, HigherThan, CustomCurve, Odd, Even}

    public static float GetScoreDirection(ScoreDirection scoreDirection, float valueToCheck, float compareFloat, AnimationCurve customCurve=null)
    {
        switch (scoreDirection)
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

            case ScoreDirection.CustomCurve:
                valueToCheck = customCurve.Evaluate(valueToCheck);
                break;
            case ScoreDirection.Odd:
                valueToCheck = valueToCheck % 2 != 0 ? 1 : 0;
                break;
            case ScoreDirection.Even:
                valueToCheck = valueToCheck % 2 == 0 ? 1 : 0;
                break;
        }
        return valueToCheck;
    }
    
    [System.Serializable]
    public enum Rule {
        President,
        Team,
        Supporters,
        Money,
        TeamsSkillDifference,
        Constant,
        None,
        PlayerTacticGeneric,
        OpponentTacticGeneric,
        PlayerWinning,
        PlayerDrawing,
        PlayerLosing,
        PlayerTrait,
        Derby,
        Injuries,
        GameMinute,
        PlayerSubstitutions,
        OpponentSubstitutions,
        Week,
        CardAppearances,
        CardsToNextPhase,
        TeamEnergy,
        PlayersWithTrait
    }

    [System.Serializable]
    public enum MatchRule {
        Aggressivity,
        SkillDifference,
        YellowCards,
        RedCards,
        Constant,
        Derby,
        Injuries,
        GameMinute,
        PlayerSubstitutions,
        OpponentSubstitutions,
        CardAppearances,
        CardsToNextPhase
    };

    public static List<string> vehicles = new List<string> { "Bici", "Elicottero", "Torpedone", "Trattore" };
}
