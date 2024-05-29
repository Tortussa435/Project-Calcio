using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class S_SubstitutionsManager
{
    private static List<SO_PlayerData> elevenRef;
    private static List<SO_PlayerData> benchRef;
    
    public static void SetElevenAndBenchList(List<SO_PlayerData> eleven, List<SO_PlayerData> bench)
    {
        elevenRef = eleven;
        benchRef = bench;
    }

    public static SO_PlayerData FindOptimalSubstitute(SO_PlayerData playerToSub, List<SO_PlayerData> impossibleSubs=null)
    {
        Debug.Log("Bench size: " + benchRef.Count);
        SO_PlayerData.PlayerRole role = playerToSub.playerRole;
        SO_PlayerData bestPlayer=null;
        foreach(SO_PlayerData p in benchRef)
        {
            if(impossibleSubs!=null)
                if (impossibleSubs.Contains(p)) continue;

            Debug.Log("Giocatore: " + p.playerName);
            if (p.playerRole == role)
            {
                if (bestPlayer == null) bestPlayer = p;
                else if (bestPlayer.skillLevel < p.skillLevel) bestPlayer = p;
            }
        }
        if (bestPlayer == null)
        {
            Debug.LogWarning("Nessuna sostituzione ottimale trovata");
        }
        else Debug.Log("Il miglior sostituto è indubbiamente: "+bestPlayer.playerName);

        return bestPlayer;
    }

    public static bool Substitute(SO_PlayerData pOut, SO_PlayerData pIn)
    {
        if (!elevenRef.Remove(pOut))
        {
            Debug.LogWarning("Uscita dal campo fallita!");
        }
        benchRef.Add(pOut);
        elevenRef.Add(pIn);

        S_PlayerMatchSimulator.OnSubstitution.Invoke(S_PlayerMatchSimulator.IsPlayerHomeTeam());
        
        return true;
    }
}