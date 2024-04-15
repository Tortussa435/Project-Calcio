using JetBrains.Annotations;
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
        List<SO_Team> availableTeams = new List<SO_Team>(ShuffleTeams(currentLeague.teamlist.ToList()));
        
        List<Match> newList = new List<Match>();


        
        for(int q=0; q < availableTeams.Count-1 ;q++)
        {
            for(int j = 0; j < availableTeams.Count; j += 2)
            {
                Match match;
                match.homeTeam = availableTeams[j];
                match.awayTeam = availableTeams[j+1];
                newList.Add(match);
            }

            for(int j = availableTeams.Count - 1; j > 1; j--)
            {
                SO_Team team = availableTeams[j - 1];
                availableTeams[j - 1] = availableTeams[j];
                availableTeams[j] = team;
            }

            if (newList.Count == 10)
            {
                calendar.Add(new List<Match>(newList));
                newList.Clear();
            }
        }

        for(int i = 0; i < calendar.Count; i++)
        {
            for(int j = 0; j < calendar[i].Count;j++)
            {
                calendar[i][j] = ShuffleHomeAwayTeam(calendar[i][j]);
            }
        }

        List<List<Match>> secondRound = new List<List<Match>>(calendar);
        secondRound.Reverse();
        for(int i = 0; i < secondRound.Count; i++)
        {
            calendar.Add(secondRound[i]);
        }
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
}

