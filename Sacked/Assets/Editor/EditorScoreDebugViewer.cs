using UnityEngine;
using UnityEditor;
using System.Security.Policy;
using NUnit.Framework;
using System.Collections.Generic;
using static S_FootballEnums;

public class EditorScoreDebugViewer : EditorWindow
{
    static float aggressivitySlider = 0;
   
    private static int yellowCards = 0;
    private static int redCards = 0;

    private static int teamASkill = 1;
    private static int teamBSkill = 1;

    private static int injuries = 0;

    private static int gameMinute = 0;

    private static int homeSubs = 0;

    private static int awaySubs = 0;

    private static int week = 0;

    public static CardsFormulaHandle formulasmanager;

    [MenuItem("ProjectCalcio/MatchCardScoreCalculator")]

    public static void ShowWindow()
    {
        GetWindow<EditorScoreDebugViewer>("MatchCardScoreCalculator");
    }
    private void OnGUI()
    {
        try
        {
            formulasmanager = FindObjectOfType<CardsFormulaHandle>();
            GUILayout.Label("Formulas handler found");
        }
        catch
        {
            GUILayout.Label("Handler non trovato");
        }

        GUILayout.Label("Card score:", EditorStyles.boldLabel);
        
        GUILayout.Label("Aggressivity:", EditorStyles.boldLabel);
        aggressivitySlider = GUILayout.HorizontalSlider(aggressivitySlider, 0, 6,GUILayout.Width(300));
        GUILayout.Space(10);
        GUILayout.Label(aggressivitySlider.ToString());


        GUILayout.Label("Yellow Cards", EditorStyles.boldLabel);
        yellowCards = (int)GUILayout.HorizontalSlider(yellowCards,0,6,GUILayout.Width(300));
        GUILayout.Space(10);
        GUILayout.Label(yellowCards.ToString());

        GUILayout.Label("Red Cards", EditorStyles.boldLabel);
        redCards = (int)GUILayout.HorizontalSlider(redCards,0,6,GUILayout.Width(300));
        GUILayout.Space(10);
        GUILayout.Label(redCards.ToString());

        GUILayout.Label("Injuries", EditorStyles.boldLabel);
        injuries = (int)GUILayout.HorizontalSlider(injuries, 0, 6, GUILayout.Width(300));
        GUILayout.Space(10);
        GUILayout.Label(injuries.ToString());

        GUILayout.Label("Home team skill", EditorStyles.boldLabel);
        teamASkill = (int)GUILayout.HorizontalSlider(teamASkill, 1, 5, GUILayout.Width(300));
        GUILayout.Space(10);
        GUILayout.Label(teamASkill.ToString());

        GUILayout.Label("Away team skill", EditorStyles.boldLabel);
        teamBSkill = (int)GUILayout.HorizontalSlider(teamBSkill,1,5,GUILayout.Width(300));
        GUILayout.Space(10);
        GUILayout.Label(teamBSkill.ToString());

        GUILayout.Label("Game Minute", EditorStyles.boldLabel);
        gameMinute = (int)GUILayout.HorizontalSlider(gameMinute, 0, 90, GUILayout.Width(300));
        GUILayout.Space(10);
        GUILayout.Label(gameMinute.ToString());

        GUILayout.Label("Player Subs", EditorStyles.boldLabel);
        homeSubs = (int)GUILayout.HorizontalSlider(homeSubs, 0, 4, GUILayout.Width(300));
        GUILayout.Space(10);
        GUILayout.Label(homeSubs.ToString());

        GUILayout.Label("Opponent Subs", EditorStyles.boldLabel);
        awaySubs = (int)GUILayout.HorizontalSlider(awaySubs, 0, 4, GUILayout.Width(300));
        GUILayout.Space(10);
        GUILayout.Label(awaySubs.ToString());

        GUILayout.Label("Week", EditorStyles.boldLabel);
        week = (int)GUILayout.HorizontalSlider(week, 0, 38, GUILayout.Width(300));
        GUILayout.Space(10);
        GUILayout.Label(week.ToString());

        try
        {
            GUILayout.Label("Card Score = " + CalcFormula());
        }

        catch
        {
            GUILayout.Label("Formula Calc failed");
        }
    }

    private void OnInspectorUpdate()
    {
        
    }

    private float CalcFormula()
    {
        float cardScore = 0f;

        foreach(CardDropChance chance in formulasmanager.formulas)
        {
            cardScore+=CalcSingleChance(chance);
        }

        return cardScore;
    }

    private float CalcSingleChance(CardDropChance chance)
    {
        float score = 0;

        switch (chance.rule)
        {
            //REDO most values are >1, they should stay in a 0-1 range as much as possible
            case MatchRule.Aggressivity:
                score = (float)aggressivitySlider;
                break;

            case MatchRule.SkillDifference:
                score = (float)((teamASkill-teamBSkill)+5)/10;
                break;

            case MatchRule.YellowCards:
                score = (float)yellowCards;
                break;

            case MatchRule.RedCards:
                score = redCards;
                break;

            case MatchRule.Constant:
                score = 1;
                break;

            case MatchRule.Injuries:
                score = injuries;
                break;

            case MatchRule.GameMinute:
                score = gameMinute;
                break;

            case MatchRule.PlayerSubstitutions:
                score = homeSubs;
                break;

            case MatchRule.OpponentSubstitutions:
                score = awaySubs;
                break;

        }

        switch (chance.direction)
        {
            default:
                break;

            case ScoreDirection.InverseLinear:
                score = 1 - score;
                break;

            case ScoreDirection.Round:
                score = Mathf.Round(score);
                break;

            case ScoreDirection.InverseRound:
                score = 1 - Mathf.Round(score);
                break;

            case ScoreDirection.HigherThan:
                score = score > chance.compareFloat ? 1 : 0;
                break;

            case ScoreDirection.Equal:
                score = score == chance.compareFloat ? 1 : 0;
                break;

            case ScoreDirection.LowerThan:
                score = score < chance.compareFloat ? 1 : 0;
                break;
        }

        score *= chance.weight;

        return score;
    }

}
