using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class S_PlayerMatchSimulator
{
    public static int matchMinute = 0;
    public static S_Calendar.Match match;
    public static S_FastMatchSimulator.Score matchScore; //REDO its pretty ugly using a struct here coming from that class
    public static SO_CardData SimulateMatchSegment(int minMinutes=9,int maxMinutes=13)
    {
        matchMinute += Random.Range(minMinutes, maxMinutes);

        UpdateMatchTextData();
        
        return GenerateGolCard();
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
        matchMinute = 0;
        matchScore.home = 0;
        matchScore.away = 0;
        S_FastMatchSimulator.SimulateWeekMatches(S_GlobalManager.currentMatchDay,S_GlobalManager.selectedTeam);
        S_GlobalManager.currentMatchDay++;
        UpdateMatchTextData();

    }
    public static void UpdateMatchTextData()
    {
        S_GlobalManager.deckManagerRef.MatchScoreText.SetText(matchMinute.ToString() + "'\n" + match.homeTeam.teamName + " " + matchScore.home + " - " + matchScore.away + " " + match.awayTeam.teamName);
    }

    private static SO_CardData GenerateGolCard(bool homeTeam=true)
    {

        SO_CardData golCard = ScriptableObject.CreateInstance<SO_CardData>();
        golCard.cardDescription = "Goooool";
        golCard.desiredCardPrefabDirectory = "Prefabs/P_GolCard";
        golCard.decreaseCountDown = false;
        golCard.cardIcon = homeTeam ? match.homeTeam.teamLogo : match.awayTeam.teamLogo;
        golCard.cardColor = Color.red;

        return golCard;
    }
}
