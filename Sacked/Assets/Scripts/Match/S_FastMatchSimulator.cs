using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public static class S_FastMatchSimulator
{
    [Header("Fast Simulation Values")]
    static float goalChanceDecreasePerGoal = 0.5f;
    static int goalCheckFrequency = 10;
    static SO_Curve fastGoalChanceCurve;

    public static List<(S_Calendar.Match match,Score score)> weekResults = new List<(S_Calendar.Match,Score)>();
    static S_FastMatchSimulator()
    {
        fastGoalChanceCurve = Resources.Load<SO_Curve>(S_ResDirs.fastGoalChanceCurve);
    }
    public static void ResetS_FastMatchSimulator()
    {
        weekResults = new List<(S_Calendar.Match match, Score score)>();
    }

    public struct Score
    {
        public int home;
        public int away;
        public bool HomeWinning() => home > away;
        public bool AwayWinning() => home < away;
        public bool Drawing() => home == away;
    }    
    public static int FastSimulateMatch(SO_Team homeTeam, SO_Team awayTeam)
    {
        
        Score finalScore;

        finalScore.home = 0;
        finalScore.away = 0;
        
        double homeTeamGoalChance = CalcFastGoalChance(homeTeam.SkillLevel,awayTeam.SkillLevel,0);
        double awayTeamGoalChance = CalcFastGoalChance(awayTeam.SkillLevel,homeTeam.SkillLevel,0);


        for(int i=0; i < 90; i += goalCheckFrequency)
        {
            float homeSeed = (float)Random.Range(0, 101) / 100;
            float awaySeed = (float)Random.Range(0, 101) / 100;
            //Debug.Log("Il semes �: " + seed);
            
            if (homeSeed <= homeTeamGoalChance)
            {
                homeTeamGoalChance = CalcFastGoalChance(homeTeam.SkillLevel,awayTeam.SkillLevel,finalScore.home);
                finalScore.home += 1;
            }

            if (awaySeed <= awayTeamGoalChance)
            {
                awayTeamGoalChance = CalcFastGoalChance(homeTeam.SkillLevel, awayTeam.SkillLevel, finalScore.away);
                finalScore.away += 1;
            }
        }

        //Debug.Log("Final Score: "+ homeTeam.teamName + " " + finalScore.home + " " + awayTeam.teamName + " " + finalScore.away);
        //Debug.Log("Final Score: " + finalScore.home + " "  + finalScore.away);
        S_Calendar.Match match;
        match.homeTeam = homeTeam;
        match.awayTeam = awayTeam;


        weekResults.Add((match, finalScore));

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
        weekResults.Clear();
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

    static double CalcFastGoalChance(int skillA,int skillB,int alreadyScoredGoals)
    {
        int skillDifference = skillA - skillB;
        double goalChance = Mathf.Clamp(fastGoalChanceCurve.curve.Evaluate(skillDifference) * Mathf.Pow(goalChanceDecreasePerGoal,alreadyScoredGoals),0,1);
        
        //double goalChance = defaultGoalChance + ((skillA - skillB) * 0.1f) * Mathf.Pow(goalChanceDecreasePerGoal,alreadyScoredGoals);
        
        if (alreadyScoredGoals == 0) goalChance = Mathf.Clamp((float)goalChance,0,0.1f);
        
        return goalChance;
    }
}
