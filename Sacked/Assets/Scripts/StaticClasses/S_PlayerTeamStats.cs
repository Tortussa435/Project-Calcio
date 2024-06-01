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
    public static int CalcSquadAtk()
    {
        return AverageSquadAtk+SquadAtkBoost;
    }
    public static int CalcSquadDef()
    {
        return AverageSquadDef+SquadDefBoost;
    }

    public static int GetChemistryBoost() => ChemistryBoost;
    public static int GetFitnessBoost() => FitnessBoost;
    public static int GetFreeKicksBoost() => FreeKicksBoost;

    public static void IncreaseAtkBoost() => SquadAtkBoost = Mathf.Clamp(SquadAtkBoost += 1, 0, 6);
    public static void IncreaseDefBoost() => SquadDefBoost = Mathf.Clamp(SquadDefBoost += 1, 0, 6);
    public static void IncreaseFitnessBoost() => FitnessBoost = Mathf.Clamp(FitnessBoost += 1, 0, 6);
    public static void IncreaseChemistryBoost() => ChemistryBoost = Mathf.Clamp(ChemistryBoost += 1, 0, 6);
    public static void IncreaseFreeKicksBoost() => FreeKicksBoost = Mathf.Clamp(FreeKicksBoost += 1, 0, 6);
}
