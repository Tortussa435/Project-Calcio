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

            GUILayout.Label("Home Tactics: "+ match.homeTeam.teamTactics.teamTactic + " " +  tacticEffectiveness.home +"\nAway Tactics: " + match.awayTeam.teamTactics.teamTactic + " " + tacticEffectiveness.away);
            
            GUILayout.Label("\nGiocatori Ammoniti:");

            foreach(SO_PlayerData player in YellowCards)
            {
                GUILayout.Label(player.playerName);
            }

            GUILayout.Label("\nGiocatori Espulsi:");
            foreach (SO_PlayerData player in RedCards)
            {
                GUILayout.Label(player.playerName);
            }
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
