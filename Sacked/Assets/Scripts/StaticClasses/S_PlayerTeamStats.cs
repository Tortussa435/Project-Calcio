using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class S_PlayerTeamStats
{
    private static List<SO_PlayerData> elevenRef = S_GlobalManager.squad.playingEleven;

    private static int AverageSquadAtk;
    private static int AverageSquadDef;
    
    private static int SquadDefBoost;
    private static int SquadAtkBoost;

    private static int ChemistryBoost;
    private static int FitnessBoost;
    private static int FreeKicksBoost;
    private static float GetTotalSkillByRole(SO_PlayerData.PlayerRole role) 
    {
        float total = 0;
        foreach (SO_PlayerData p in elevenRef)
        {
            if (p.playerRole == role)
            {
                total += p.GetPlayerCurrentSkillLevel();
            }
        }
        return total;
    }
    private static int GetPlayersPerRole(SO_PlayerData.PlayerRole role)
    {
        int num = 0;
        foreach(SO_PlayerData p in elevenRef)
        {
            if (p.playerRole == role) num++;
        }
        return num;
    }
    public static int CalcSquadAtk()
    {
        AverageSquadAtk = (int)((GetTotalSkillByRole(SO_PlayerData.PlayerRole.Atk) / 3) + (GetTotalSkillByRole(SO_PlayerData.PlayerRole.Mid)/25));
        Debug.LogWarning("ATTACCO: "+AverageSquadAtk);
        return Mathf.Clamp(AverageSquadAtk+(SquadAtkBoost/3)+ChemistryMultiplier(),0,5);
    }
    public static int CalcSquadDef()
    {
        AverageSquadDef = (int)((GetTotalSkillByRole(SO_PlayerData.PlayerRole.Def) / 3) + (GetTotalSkillByRole(SO_PlayerData.PlayerRole.Def) / 25));
        Debug.LogWarning("DIFESA: "+AverageSquadDef);
        return Mathf.Clamp(AverageSquadDef + (SquadDefBoost / 3) + ChemistryMultiplier(), 0, 5);
    }
    public static float FitnessMultiplier() => Mathf.Lerp( 0.5f, 1, 1 - Mathf.InverseLerp(0, 6, FitnessBoost)); //spent energy decrease multiplier can reach max 0.5
    private static int ChemistryMultiplier() => (ChemistryBoost - 3) / 3;
    public static int GetChemistryBoost() => ChemistryBoost;
    public static int GetFitnessBoost() => FitnessBoost;
    public static int GetFreeKicksBoost() => FreeKicksBoost;
    public static void IncreaseAtkBoost() => SquadAtkBoost = Mathf.Clamp(SquadAtkBoost += 1, 0, 6);
    public static void IncreaseDefBoost() => SquadDefBoost = Mathf.Clamp(SquadDefBoost += 1, 0, 6);
    public static void IncreaseFitnessBoost() => FitnessBoost = Mathf.Clamp(FitnessBoost += 1, 0, 6);
    public static void IncreaseChemistryBoost() => ChemistryBoost = Mathf.Clamp(ChemistryBoost += 1, 0, 6);
    public static void IncreaseFreeKicksBoost() => FreeKicksBoost = Mathf.Clamp(FreeKicksBoost += 1, 0, 6);
}
