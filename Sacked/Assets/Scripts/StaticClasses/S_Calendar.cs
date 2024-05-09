using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

static public class S_Calendar
{
    static SO_League currentLeague;
    public struct Match
    {
        public SO_Team homeTeam;
        public SO_Team awayTeam;
        public void Swap()
        {
            SO_Team handle = ScriptableObject.CreateInstance<SO_Team>();
            homeTeam = handle;
            homeTeam = awayTeam;
            awayTeam = handle;
        }
    }

    static public List<List<Match>> calendar;
    static S_Calendar()
    {
        currentLeague = S_GlobalManager.deckManagerRef.selectedLeague;
        calendar = new List<List<Match>>();
    }
    static bool WeekAlreadyContainsTeam(SO_Team team, List<Match> matches)
    {
        foreach (Match match in matches)
        {
            if (match.homeTeam.teamName == team.teamName || match.awayTeam.teamName == team.teamName) return true;
        }
        return false;
    }

    static Match ShuffleHomeAwayTeam(Match match, bool alwaysSwap=false)
    {
        bool shuffle = UnityEngine.Random.Range(0, 2) == 1;
        if (alwaysSwap) shuffle = true;

        if (shuffle)
        {
            SO_Team handle = match.homeTeam;
            match.homeTeam = match.awayTeam;
            match.awayTeam = handle;
        }
        return match;
    }

    static List<SO_Team> ShuffleTeams(List<SO_Team> teams)
    {
        for(int i = teams.Count - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            SO_Team temp = teams[i];
            teams[i] = teams[j];
            teams[j] = temp;
        }
        return teams;
    }

    public static SO_Team FindOpponent()
    {
        List<Match> matchday = calendar[S_GlobalManager.currentMatchDay];
        
        foreach(Match match in matchday)
        {
            if (match.homeTeam.teamName == S_GlobalManager.selectedTeam.teamName) return match.awayTeam;
            if (match.awayTeam.teamName == S_GlobalManager.selectedTeam.teamName) return match.homeTeam;
        }
        
        return null;
    }

    public static Match FindMatchByTeam(SO_Team team, int week)
    {
        foreach(Match match in calendar[week])
        {
            if (match.homeTeam.teamName == team.teamName || match.awayTeam.teamName == team.teamName) return match;
        }
        Match matchFailed;
        matchFailed.homeTeam = null;
        matchFailed.awayTeam = null;
        return matchFailed;
    }

    public static void GenerateCalendar()
    {
        List<SO_Team> availableTeams = new List<SO_Team>(ShuffleTeams(currentLeague.teamlist.ToList()));

        int numTeams = availableTeams.Count;
        int numRounds = availableTeams.Count - 1;
        int matchesPerRound = numTeams / 2;
        
        SO_Team[,] schedule = new SO_Team[numRounds, 2 * matchesPerRound];

        List<Match> newList = new List<Match>();

        for(int i = 0; i < numRounds; i++)
        {
            for(int j = 0; j < matchesPerRound; j++)
            {
                int team1Index = (i + j) % (numTeams - 1);
                int team2Index = (numTeams - 1 - j + i) % (numTeams - 1);
                if (j == 0)
                {
                    team2Index = numTeams - 1;
                }
                schedule[i, j * 2] = availableTeams[team1Index];
                schedule[i, j * 2 + 1] = availableTeams[team2Index];
            }
        }
        for(int i = 0; i < numRounds; i++)
        {
            for(int j = 0; j < 2 * matchesPerRound; j += 2)
            {
                Match match = new Match { homeTeam = schedule[i, j], awayTeam = schedule[i, j + 1] };
                newList.Add(match);
            }
            calendar.Add(new List<Match>(newList));
            newList.Clear();
        }

        for (int i = 0; i < calendar.Count; i++)
        {
            for (int j = 0; j < calendar[i].Count; j++)
            {
                calendar[i][j] = ShuffleHomeAwayTeam(calendar[i][j]);
            }
        }

        
        List<List<Match>> secondRound = new List<List<Match>>(calendar);
        secondRound.Reverse();

        for (int i = 0; i < secondRound.Count; i++)
        {
            List<Match> currentWeek = secondRound[i];
            List<Match> flippedWeek = new List<Match>();
            for(int j=0; j < currentWeek.Count; j++)
            {
                Match match;
                match.homeTeam = currentWeek[j].awayTeam;
                match.awayTeam = currentWeek[j].homeTeam;
                flippedWeek.Add(match);
            }
            calendar.Add(flippedWeek);
        }
        
    }
}

