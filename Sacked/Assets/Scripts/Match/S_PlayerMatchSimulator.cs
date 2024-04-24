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
    const float GOALCHANCEDECREASEPERGOAL = 0.75f;

    public static (float home, float away) matchAggressivity = ( 0, 0 );
    
    public static (float home, float away) injuryChance = ( 0, 0 );

    public static (float home, float away) traitsScoreChance = ( 0, 0 );
    
    
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

        S_GlobalManager.nextOpponent.GenerateRandomTraits();

        CheckPlayerOpponentTraitsInteraction();
        ApplyPlayersTraits();
        ApplyTeamTraits();

    }

    public static void EndMatch()
    {

        EndMatchAddPoints();



        matchMinute = 0;
        matchScore.home = 0;
        matchScore.away = 0;
        
        S_FastMatchSimulator.SimulateWeekMatches(S_GlobalManager.currentMatchDay,S_GlobalManager.selectedTeam);
        
        S_GlobalManager.currentMatchDay++;
        S_GlobalManager.nextOpponent = S_Calendar.FindOpponent();
        
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

        //decrease goal chance for each already scored goal
        goalCheck *= Mathf.Pow(GOALCHANCEDECREASEPERGOAL, homeTeam ? matchScore.home : matchScore.away);

        //gives the game goal chance a range of 0,0.5
        goalCheck = Mathf.Lerp(0, 0.5f, goalCheck);

        //add goal chance boost/drop from teams traits (max score = 0.5)
        
        
        //takes the score (should be in 0-1 range) and sets it in range of 0 - max possible score
        goalCheck = Mathf.Lerp(0, MAXGOALCHANCE, goalCheck);

        //Debug.Log("goal chance: " + goalCheck);

        return goalCheck;
    }

    private static float GetSkillDifference(int skillA, int skillB)
    {
        float skillDifference = skillA - skillB;
        skillDifference += 5;
        skillDifference /= 10;
        return skillDifference;
    }

    private static SO_Team GetOpponentTeam() => S_GlobalManager.selectedTeam.teamName == match.homeTeam.teamName ? match.awayTeam : match.homeTeam;

    public static bool IsOpponentHomeTeam() => !(S_GlobalManager.selectedTeam.teamName == match.homeTeam.teamName);
    public static bool IsPlayerHomeTeam() => (S_GlobalManager.selectedTeam.teamName == match.homeTeam.teamName);
    private static void CheckPlayerOpponentTraitsInteraction()
    {
        //REDO really ugly way of doing this, also maybe we can store GetOpponentTeam() once instead of calling it each time
        foreach (SO_PlayerData player in S_GlobalManager.squad.Goalkeepers)
        {
            //REDO currently works only with players with one trait only, if players will have more than one trait it will be necessary to check against all of them
            S_TraitsCombinationsManager.CheckTraitsCombination(player, GetOpponentTeam());
        }

        foreach (SO_PlayerData player in S_GlobalManager.squad.Defense)
        {
            S_TraitsCombinationsManager.CheckTraitsCombination(player, GetOpponentTeam());
        }

        foreach (SO_PlayerData player in S_GlobalManager.squad.Midfield)
        {
            S_TraitsCombinationsManager.CheckTraitsCombination(player, GetOpponentTeam());
        }

        foreach (SO_PlayerData player in S_GlobalManager.squad.Attack)
        {
            S_TraitsCombinationsManager.CheckTraitsCombination(player, GetOpponentTeam());
        }

        ClampParameters();
    }
    private static void ClampParameters()
    {
        Mathf.Clamp(matchAggressivity.home,0,100);
        Mathf.Clamp(matchAggressivity.away,0,100);
        
        Mathf.Clamp(injuryChance.home, 0, 100);
        Mathf.Clamp(injuryChance.away, 0, 100);

        Mathf.Clamp(traitsScoreChance.home, 0, 100);
        Mathf.Clamp(traitsScoreChance.away, 0, 100);
    }

    private static void ApplyTeamTraits()
    {

    }

    private static void ApplyPlayersTraits()
    {

    }
}
