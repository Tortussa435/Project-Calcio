using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static S_GlobalManager;
public static class S_VarsDictionary
{
    static public Dictionary<string,string> GenerateVarsDictionary()
    {
        Dictionary<string, string> varsDictionary = new Dictionary<string, string>
        {
            {"{Team}", GetSafeTeamName()},
            {"{Opponent}", GetSafeOpponentName() },
            {"{Ref}", GetSafeRefName() },
            {"{Player}", GetSafePlayerName()},
            {"{OppPlayer}", GetSafeOppPlayer()},
            {"{Atk}", GetSafeRole(SO_PlayerData.PlayerRole.Atk)},
            {"{Mid}", GetSafeRole(SO_PlayerData.PlayerRole.Mid)},
            {"{Def}", GetSafeRole(SO_PlayerData.PlayerRole.Def)},
            {"{Gk}", GetSafeRole(SO_PlayerData.PlayerRole.Gk)},
            {"{AnyPlayer}", GetSafeAnyPlayer()},
            {"{HotHead}",GetSafeHotHead() }, //If hothead is not found, returns random player
        };
        return varsDictionary;
    }  
    
    private static string GetSafeTeamName() => selectedTeam.teamName != null ? selectedTeam.teamName : "No Player Team Found";

    private static string GetSafeOpponentName() => nextOpponent != null ? nextOpponent.teamName : "No Next Opponent Found";

    private static string GetSafeRefName() => S_PlayerMatchSimulator.refereeName != null ? "L'arbitro " + S_PlayerMatchSimulator.refereeName : "No ref found";

    private static string GetSafePlayerName()
    {
        try
        {
            return squad.playingEleven[UnityEngine.Random.Range(0, squad.playingEleven.Count)].playerName;
        }
        catch
        {
            return "no player found";
        }
    }

    private static string GetSafeOppPlayer()
    {
        try
        {
            return S_PlayerMatchSimulator.RandomlyGetNewOrExistingOpponentPlayer();
        }
        catch
        {
            return "No opponent player found";
        }
    }

    private static string GetSafeRole(SO_PlayerData.PlayerRole role)
    {
        try
        {
            return squad.GetPlayingPlayerByRole(role);
        }
        catch
        {
            return "No Atk Found";
        }
    }

    private static string GetSafeAnyPlayer()
    {
        try
        {
            return squad.GetAnyRandomPlayer();
        }
        catch {
            return "No random player found";
        }
    }

    private static string GetSafeHotHead()
    {
        List<SO_PlayerData> pl = squad.GetPlayersWithTrait(SO_PlayerTrait.PlayerTraitNames.Hot_Head,false);
        if (pl.Count > 0) return pl[Random.Range(0, pl.Count)].playerName;
        return GetSafeAnyPlayer();
    }
}

