using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public static class S_MatchSimulator
{
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
        
        double homeTeamGoalChance = Mathf.Abs(((homeTeam.SkillLevel - awayTeam.SkillLevel) + 1) / (S_GlobalManager.MAXTEAMSKILLLEVEL * 2)) ;
        double awayTeamGoalChance = Mathf.Abs(((homeTeam.SkillLevel - awayTeam.SkillLevel) + 1) / (S_GlobalManager.MAXTEAMSKILLLEVEL * 2));


        for(int i=0; i < 90; i += 5)
        {
            float homeSeed = (float)Random.Range(0, 11) / 10;
            float awaySeed = (float)Random.Range(0, 11) / 10;
            //Debug.Log("Il semes è: " + seed);
            
            if (homeSeed <= homeTeamGoalChance)
            {
                finalScore.home += 1;
                homeTeamGoalChance *=.75f;
            }

            if (awaySeed <= awayTeamGoalChance)
            {
                finalScore.away += 1;
                awayTeamGoalChance *=.5f;

            }
        }
        
        //Debug.Log("Final Score: "+ homeTeam.teamName + " " + finalScore.home + " " + awayTeam.teamName + " " + finalScore.away);

        if (finalScore.home == finalScore.away) return 0; //Golden punticino
        if (finalScore.home > finalScore.away) return 1;
        if (finalScore.home < finalScore.away) return 2;
        return -1;

    }

    public static void SimulateWholeTournament()
    {
        for(int i = 0; i < S_Calendar.calendar.Count; i++)
        {
            foreach(S_Calendar.Match match in S_Calendar.calendar[i])
            {
                switch(FastSimulateMatch(match.homeTeam, match.awayTeam))
                {
                    case 0:
                        S_Ladder.UpdateTeamPoints(match.homeTeam, 1);
                        S_Ladder.UpdateTeamPoints(match.awayTeam, 1);
                        Debug.Log("Pareggio");
                        break;
                    case 1:
                        S_Ladder.UpdateTeamPoints(match.homeTeam, 3);
                        Debug.Log("Vince casa");
                        break;
                    case 2:
                        S_Ladder.UpdateTeamPoints(match.awayTeam, 3);
                        Debug.Log("Vince trasferta");
                        break;
                }
            }
        }
    }
}
