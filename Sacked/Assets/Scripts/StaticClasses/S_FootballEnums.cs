using System;
using UnityEngine;

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
    public enum ScoreDirection { Linear, InverseLinear, Round, InverseRound, LowerThan, Equal, HigherThan}

    public static float GetScoreDirection(ScoreDirection scoreDirection, float valueToCheck, float compareFloat)
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
        OpponentSubstitutions
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
        OpponentSubstitutions
    };
}
