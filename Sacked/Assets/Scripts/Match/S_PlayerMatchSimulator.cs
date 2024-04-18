using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;

public static class S_PlayerMatchSimulator
{
    public static int matchMinute = 0;
    public static S_Calendar.Match match;
    public static S_FastMatchSimulator.Score matchScore; //REDO its pretty ugly using a struct here coming from that class
    public static SO_Curve goalChancePerMinute;
    const float MAXGOALCHANCE = 0.75f;
    
    static S_PlayerMatchSimulator()
    {
        goalChancePerMinute = Resources.Load<SO_Curve>("ScriptableObjects/Curves/PlayerMatch/MatchGoalChanceMultiplier");
    }
    
    public static SO_CardData SimulateMatchSegment(int minMinutes=9,int maxMinutes=13)
    {
        matchMinute += Random.Range(minMinutes, maxMinutes);

        UpdateMatchTextData();

        SO_CardData rollData = ScoreGoalRoll();
        if (rollData!=null) return rollData;

        List<SO_CardData> matchCards = S_GlobalManager.deckManagerRef.cardSelector.ChooseCardByScore();
        return matchCards[Random.Range(0, matchCards.Count)];

    }

    public static void StartMatch()
    {
        S_GlobalManager.deckManagerRef.MatchScoreText.gameObject.SetActive(true);
        S_GlobalManager.deckManagerRef.PhaseText.gameObject.SetActive(false);

        match = S_Calendar.FindMatchByTeam(S_GlobalManager.selectedTeam,S_GlobalManager.currentMatchDay);
        matchScore.home = 0;
        matchScore.away = 0;
        UpdateMatchTextData();
    }

    public static void EndMatch()
    {

        EndMatchAddPoints();


        matchMinute = 0;
        matchScore.home = 0;
        matchScore.away = 0;
        
        S_FastMatchSimulator.SimulateWeekMatches(S_GlobalManager.currentMatchDay,S_GlobalManager.selectedTeam);
        
        S_GlobalManager.currentMatchDay++;
        
        UpdateMatchTextData();

    }
    private static void EndMatchAddPoints()
    {
        if (matchScore.home > matchScore.away) S_Ladder.UpdateTeamPoints(match.homeTeam, 3);
        if (matchScore.home < matchScore.away) S_Ladder.UpdateTeamPoints(match.awayTeam, 3);
        if (matchScore.home == matchScore.away)
        {
            S_Ladder.UpdateTeamPoints(match.homeTeam, 1);
            S_Ladder.UpdateTeamPoints(match.awayTeam, 1);
        }

    }
    public static void UpdateMatchTextData()
    {
        S_GlobalManager.deckManagerRef.MatchScoreText.SetText(matchMinute.ToString() + "'\n" + match.homeTeam.teamName + " " + matchScore.home + " - " + matchScore.away + " " + match.awayTeam.teamName);
    }

    private static SO_CardData GenerateGolCard(bool homeTeam=true)
    {

        SO_CardData golCard = ScriptableObject.CreateInstance<SO_CardData>();
        golCard.cardDescription = "Gooool";
        golCard.desiredCardPrefabDirectory = "Prefabs/P_GolCard";
        //golCard.decreaseCountDown = false;
        golCard.cardIcon = homeTeam ? match.homeTeam.teamLogo : match.awayTeam.teamLogo;
        golCard.cardColor = homeTeam ? match.homeTeam.teamColor1 : match.awayTeam.teamColor1;

        if (homeTeam) matchScore.home++;
        
        if (!homeTeam) matchScore.away++;

        return golCard;
    }

    private static SO_CardData ScoreGoalRoll()
    {
        SO_CardData card=null;

        float homeGoalCheck = GoalCheck(true);

        float awayGoalCheck = GoalCheck(false);

        float homeRoll = (float)Random.Range(0, 1001) / 1000;
        float awayRoll = (float)Random.Range(0, 1001) / 1000;

        //the team with the lower goal chance tries to score first
        if (homeGoalCheck <= awayGoalCheck)
        {
            if (homeRoll <= homeGoalCheck)
            {
                card = GenerateGolCard(true);
            }

            else if (awayRoll <= awayGoalCheck)
            {
                card = GenerateGolCard(false);
            }
        }

        else
        {
            if (awayRoll <= awayGoalCheck) 
            {
                card = GenerateGolCard(false);
            }
            else if (homeRoll <= homeGoalCheck)
            {
                card = GenerateGolCard(true);
            }

        }
        
        return card;
    }

    private static float GoalCheck(bool homeTeam)
    {
        //check if player's team to do extra calculi
        bool isCheckingPlayerTeam = homeTeam ? match.homeTeam.teamName == S_GlobalManager.selectedTeam.teamName : match.awayTeam.teamName == S_GlobalManager.selectedTeam.teamName;
        
        float goalCheck=0;
        
        //first calc = skill diff check
        if (homeTeam) goalCheck = GetSkillDifference(match.homeTeam.SkillLevel, match.awayTeam.SkillLevel);
        else if (!homeTeam) goalCheck = GetSkillDifference(match.awayTeam.SkillLevel, match.homeTeam.SkillLevel);

        //second calc = goal chance per minute
        goalCheck *= goalChancePerMinute.curve.Evaluate(matchMinute);

        //third calc = home team gets a goal chance boost
        if (homeTeam) goalCheck += 0.1f;

        goalCheck = Mathf.Lerp(0, MAXGOALCHANCE, goalCheck);

        return goalCheck;
    }

    private static float GetSkillDifference(int skillA, int skillB)
    {
        float skillDifference = skillA - skillB;
        skillDifference += 5;
        skillDifference /= 10;
        return skillDifference;
    }
}
