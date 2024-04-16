using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public static class S_MatchSimulator
{
    static float goalChanceDecreasePerGoal = 0.5f;
    static float defaultGoalChance = 0.33f;
    static int goalCheckFrequency = 10;
    static SO_Curve goalChanceCurve;

    static private S_Calendar.Match playerMatch;
    static private Score playerScore;
    static private int currentGameMinute;
    public static SO_CardData SimulateNextMinutes(int minMinutesRange=9,int maxMinutesRange=13)
    {
        S_GlobalManager.deckManagerRef.nextPhaseCountdown++;

        if(currentGameMinute==0) StartMatch();

        currentGameMinute += UnityEngine.Random.Range(minMinutesRange, maxMinutesRange);

        if(currentGameMinute>=45 && S_GlobalManager.currentPhase==S_GlobalManager.CardsPhase.MatchFirstHalf) EndFirstHalf();

        Debug.Log("siamo al "+currentGameMinute+"o minuto di gioco");

        if(currentGameMinute>=90) EndMatch();

        return ScriptableObject.CreateInstance<SO_CardData>();

    }

    private static void StartMatch()
    {
        playerScore.home = 0;
        playerScore.away = 0;
        playerMatch = S_Calendar.FindMatchByTeam(S_GlobalManager.selectedTeam, S_GlobalManager.currentMatchDay);
    }

    private static void EndMatch()
    {
        if (playerScore.home > playerScore.away) S_Ladder.UpdateTeamPoints(playerMatch.homeTeam, 3);
        if (playerScore.home < playerScore.away) S_Ladder.UpdateTeamPoints(playerMatch.awayTeam, 3);
        else
        {
            S_Ladder.UpdateTeamPoints(playerMatch.awayTeam, 1);
            S_Ladder.UpdateTeamPoints(playerMatch.homeTeam, 1);
        }
        
        playerScore.home = 0;
        playerScore.away = 0;
        currentGameMinute = 0;

        S_GlobalManager.currentMatchDay++;

        S_GlobalManager.IntRange weekDuration = S_GlobalManager.deckManagerRef.weekDuration;

        S_GlobalManager.deckManagerRef.ChangeCurrentPhase(weekDuration.min, weekDuration.max, S_GlobalManager.CardsPhase.Week);

    }

    private static void EndFirstHalf()
    {
        S_GlobalManager.deckManagerRef.ChangeCurrentPhase(3,3,S_GlobalManager.CardsPhase.MatchSecondHalf);
        Debug.Log("tutti a bere un te caldo");
    }
    static S_MatchSimulator()
    {
        goalChanceCurve = Resources.Load<SO_Curve>("ScriptableObjects/Curves/GoalChanceCurveSimplified");
    }
    public struct Score
    {
        public int home;
        public int away;
    }
    public static int FastSimulateMatch(SO_Team homeTeam, SO_Team awayTeam)
    {
        Score finalScore;

        finalScore.home = 0;
        finalScore.away = 0;
        
        double homeTeamGoalChance = CalcGoalChance(homeTeam.SkillLevel,awayTeam.SkillLevel,0);
        double awayTeamGoalChance = CalcGoalChance(awayTeam.SkillLevel,homeTeam.SkillLevel,0);


        for(int i=0; i < 90; i += goalCheckFrequency)
        {
            float homeSeed = (float)UnityEngine.Random.Range(0, 101) / 100;
            float awaySeed = (float)UnityEngine.Random.Range(0, 101) / 100;
            //Debug.Log("Il semes è: " + seed);
            
            if (homeSeed <= homeTeamGoalChance)
            {
                homeTeamGoalChance = CalcGoalChance(homeTeam.SkillLevel,awayTeam.SkillLevel,finalScore.home);
                finalScore.home += 1;
            }

            if (awaySeed <= awayTeamGoalChance)
            {
                awayTeamGoalChance = CalcGoalChance(homeTeam.SkillLevel, awayTeam.SkillLevel, finalScore.away);
                finalScore.away += 1;
            }
        }
        
        //Debug.Log("Final Score: "+ homeTeam.teamName + " " + finalScore.home + " " + awayTeam.teamName + " " + finalScore.away);
        //Debug.Log("Final Score: " + finalScore.home + " "  + finalScore.away);

        if (finalScore.home == finalScore.away) return 0; //Golden punticino
        if (finalScore.home > finalScore.away)  return 1;
        if (finalScore.home < finalScore.away)  return 2;
        return -1;

    }

    public static void SimulateWholeTournament()
    {
        for(int i = 0; i < S_Calendar.calendar.Count; i++)
        {
            SimulateWeekMatches(i);
        }
    }

    public static void SimulateWeekMatches(int week, SO_Team excludedMatch=null)
    {
        foreach(S_Calendar.Match match in S_Calendar.calendar[week])
        {
            if(excludedMatch!=null) if (match.homeTeam.teamName == excludedMatch.teamName || match.awayTeam.teamName == excludedMatch.teamName) continue;
            
            switch (FastSimulateMatch(match.homeTeam, match.awayTeam))
            {
                case 0:
                    S_Ladder.UpdateTeamPoints(match.homeTeam, 1);
                    S_Ladder.UpdateTeamPoints(match.awayTeam, 1);
                    //Debug.Log("Pareggio");
                    break;
                case 1:
                    S_Ladder.UpdateTeamPoints(match.homeTeam, 3);
                    //Debug.Log("Vince casa");
                    break;
                case 2:
                    S_Ladder.UpdateTeamPoints(match.awayTeam, 3);
                    //Debug.Log("Vince trasferta");
                    break;
            }
        }
    }

    static double CalcGoalChance(int skillA,int skillB,int alreadyScoredGoals)
    {
        int skillDifference = skillA - skillB;
        double goalChance = Mathf.Clamp(goalChanceCurve.curve.Evaluate(skillDifference) * Mathf.Pow(goalChanceDecreasePerGoal,alreadyScoredGoals),0,1);
        
        //double goalChance = defaultGoalChance + ((skillA - skillB) * 0.1f) * Mathf.Pow(goalChanceDecreasePerGoal,alreadyScoredGoals);
        
        if (alreadyScoredGoals == 0) goalChance = Mathf.Clamp((float)goalChance,0,0.1f);
        
        return goalChance;
    }
}
