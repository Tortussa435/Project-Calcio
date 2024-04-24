using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static S_PlayerMatchSimulator;
public class MatchDebugData : EditorWindow
{
    [MenuItem("ProjectCalcio/MatchDebugData")]
    public static void ShowWindow()
    {
        GetWindow<MatchDebugData>("Match Data");
    }
    private void OnGUI()
    {
        try
        {
            GUILayout.Label(matchMinute.ToString());
            GUILayout.Label(matchScore.home+" - "+matchScore.away);
            GUILayout.Label(match.homeTeam.teamName + " vs " + match.awayTeam.teamName);
            GUILayout.Label("Home goal check: "+homeGoalCheck+" Away goal check: "+awayGoalCheck);
            GUILayout.Label("Home Traits Score Chance: "+traitsScoreChance.home+"\nAway Traits Score Chance: "+traitsScoreChance.away);
            GUILayout.Label("Home Injury Chance: "+injuryChance.home+"\nAway Injury Chance: "+injuryChance.away);
            GUILayout.Label("Home Aggressivity: "+matchAggressivity.home+"\nAway Aggressivity: "+matchAggressivity.away);
        }
        catch
        {
            GUILayout.Label("Start the game to see values");
        }
        
    }
    private void Update()
    {
        Repaint();
    }
}
