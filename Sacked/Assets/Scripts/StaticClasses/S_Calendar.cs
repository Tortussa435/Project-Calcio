using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

static public class S_Calendar
{
    static SO_League currentLeague;

    public struct Match
    {
        public SO_Team homeTeam;
        public SO_Team awayTeam;
    }

    static public List<List<Match>> calendar;
    static S_Calendar()
    {
        currentLeague = S_GlobalManager.deckManagerRef.selectedLeague;
        calendar = new List<List<Match>>();
    }

    public static void GenerateCalendar()
    {

        List<SO_Team> availableTeams = new List<SO_Team>(currentLeague.teamlist.ToList());
        
        for(int i = 0; i < currentLeague.teamlist.Length - 1; i++)
        {
            List<Match> dayMatches = new List<Match>();
            calendar.Add(dayMatches);
        }
        
        for(int i = 0; i < availableTeams.Count; i++)
        {
            SO_Team currentTeam = availableTeams[i];
            //Debug.Log("team attuale: "+availableTeams[i]);
            //availableTeams.RemoveAt(i);
            List<SO_Team> availableMatchups = new List<SO_Team>(availableTeams);
            
            for(int j=0; j < calendar.Count; j++)
            {
                
                if (WeekAlreadyContainsTeam(currentTeam, calendar[j]))
                {
                    continue;
                }
                
                if (availableMatchups.Count < 1)
                {
                    continue;
                }
                availableMatchups.Remove(currentTeam);

                //TODO find way to randomize this
                int randomOpponent = 0;
                
                for(int q = 0; q < availableMatchups.Count; q++)
                {
                    if (!WeekAlreadyContainsTeam(availableMatchups[q], calendar[j]))
                    {
                        randomOpponent = q;
                        break;
                    }
                    randomOpponent = -1;
                }
                if (randomOpponent == -1)
                {
                    Debug.Log("tocchera continuare");
                    continue;
                }
                Match match;

                match.homeTeam = currentTeam;
                match.awayTeam = availableMatchups[randomOpponent];

                availableMatchups.Remove(match.awayTeam);

                calendar[j].Add(match);


            }

            //Randomizes if team plays home or away

            /*
            for(int q = 0; q < calendar.Count; q++)
            {
                foreach(Match match in calendar[q])
                {
                    ShuffleHomeAwayTeam(match);
                }
            }
            */
        }

        static bool WeekAlreadyContainsTeam(SO_Team team, List<Match> matches)
        {
            foreach(Match match in matches)
            {
                if (match.homeTeam.teamName == team.teamName || match.awayTeam.teamName==team.teamName) return true;
            }
            return false;
        }

        static void ShuffleHomeAwayTeam(Match match)
        {
            bool shuffle = UnityEngine.Random.Range(0, 2) == 1;
            if (shuffle)
            {
                SO_Team handle = match.homeTeam;
                match.homeTeam = match.awayTeam;
                match.awayTeam = handle;
            }
        }

    }

}

