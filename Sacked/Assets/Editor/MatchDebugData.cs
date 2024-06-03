using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using static S_PlayerMatchSimulator;
public class MatchDebugData : EditorWindow
{
    Vector2 scrollPosition = Vector2.zero;

    [MenuItem("ProjectCalcio/MatchDebugData")]
    public static void ShowWindow()
    {
        GetWindow<MatchDebugData>("Match Data");
    }
    private void OnGUI()
    {
        scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, true, GUILayout.ExpandHeight(true));

        try
        {
            /*
            string playingEleven = "Playing Eleven:\n";
            foreach(SO_PlayerData player in S_GlobalManager.squad.playingEleven)
            {
                playingEleven = string.Concat(playingEleven, player.playerName + "\n");
            }
            GUILayout.Label(playingEleven);
            */

            GUILayout.Label("Atk Skill Level: " + S_PlayerTeamStats.CalcSquadAtk(true));
            GUILayout.Label("Def Skill Level: " + S_PlayerTeamStats.CalcSquadDef(true));

            GUILayout.Label("\nAtk Boost: " + S_PlayerTeamStats.GetAtkBoost());
            GUILayout.Label("Def Boost: " + S_PlayerTeamStats.GetDefBoost());
            GUILayout.Label("Fitness Multiplier: " + S_PlayerTeamStats.GetFitnessBoost());
            GUILayout.Label("Chemistry Multiplier: " + S_PlayerTeamStats.GetChemistryBoost());
            GUILayout.Label("Free Kicks Multiplier: " + S_PlayerTeamStats.GetFreeKicksBoost());

            GUILayout.Label("Squad Tactic: " + S_GlobalManager.squad.teamLineup.ToString());
            GUILayout.Label("Players Number: " + S_GlobalManager.squad.playingEleven.Count);
            GUILayout.Label("Bench Size: " + S_GlobalManager.squad.bench.Count);

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

            GUILayout.Label("\n Infortuni: "+injuries);

            GUILayout.Label("\n Sostituzioni: ");
            GUILayout.Label("\n Casa: " + substitutions.home);
            GUILayout.Label("\n Trasferta: " + substitutions.away);

            GUILayout.Label("\n Opponent Fake Players: ");
            foreach(string n in opponentTeamNames)
            {
                GUILayout.Label(n);
            }


        }

        catch
        {
            GUILayout.Label("Start the game to see values");
        }

        GUILayout.EndScrollView();
    }
    private void Update()
    {
        Repaint();
    }
}
