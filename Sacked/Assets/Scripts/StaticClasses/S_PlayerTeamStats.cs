using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using UnityEngine;
using UnityEngine.Events;

public static class S_PlayerTeamStats
{
    public readonly static int MAXBOOSTLEVEL = 6;

    private static int AverageSquadAtk;
    private static int AverageSquadDef;
    
    private static int SquadDefBoost;
    private static int SquadAtkBoost;

    private static int ChemistryBoost;
    private static int FitnessBoost;
    private static int FreeKicksBoost;
    
    public static void ResetS_PlayerTeamStats()
    {
        AverageSquadAtk = 0;
        AverageSquadDef = 0;
        SquadDefBoost = 0;
        SquadAtkBoost = 0;
        ChemistryBoost = 0;
        FitnessBoost = 0;
        FreeKicksBoost = 0;
    }

    private static float GetTotalSkillByRole(SO_PlayerData.PlayerRole role) 
    {
        float total = 0;
        foreach (SO_PlayerData p in S_GlobalManager.squad.playingEleven)
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
        foreach(SO_PlayerData p in S_GlobalManager.squad.playingEleven)
        {
            if (p.playerRole == role) num++;
        }
        return num;
    }
    public static int CalcSquadAtk(bool getOnly=false)
    {
        int midfieldBoost = 0;
        if (GetPlayersPerRole(SO_PlayerData.PlayerRole.Mid) > 2)
        {
            midfieldBoost=Mathf.RoundToInt(GetTotalSkillByRole(SO_PlayerData.PlayerRole.Mid) / 20.0f);
        }
        
        if(!getOnly) AverageSquadAtk = (int)((GetTotalSkillByRole(SO_PlayerData.PlayerRole.Atk) / 3.0f) + midfieldBoost);
        int final = Mathf.Clamp(AverageSquadAtk + (SquadAtkBoost / 3)  + ChemistryMultiplier(), 1, 5);
        return final;
    }
    public static int CalcSquadDef(bool getOnly=false)
    {
        int midfieldBoost = 0;
        if (GetPlayersPerRole(SO_PlayerData.PlayerRole.Mid) > 2)
        {
            midfieldBoost = Mathf.RoundToInt((GetTotalSkillByRole(SO_PlayerData.PlayerRole.Mid) / 20));
        }

        if (!getOnly) AverageSquadDef = (int)((GetTotalSkillByRole(SO_PlayerData.PlayerRole.Def) / 4) + midfieldBoost);
        int final = Mathf.Clamp(AverageSquadDef + (SquadDefBoost / 3) + ChemistryMultiplier(), 1, 5);
        return final;
    }
    public static int GetChemistryBoost() => ChemistryBoost;
    public static int GetFitnessBoost() => FitnessBoost;
    public static int GetFreeKicksBoost() => FreeKicksBoost;
    public static int GetAtkBoost() => SquadAtkBoost;
    public static int GetDefBoost() => SquadDefBoost;
    public static float FitnessMultiplier() => Mathf.Lerp( 0.5f, 1, 1 - Mathf.InverseLerp(0, 6, FitnessBoost)); //spent energy decrease multiplier can reach max 0.5
    public static float GetFKExtraChance() => FreeKicksBoost / 18.0f;
    public static int ChemistryMultiplier()
    {
        int baseChem = 0;
        if (ChemistryBoost == 0) baseChem = -1; //cannot do log of <1 numbers
        else baseChem = ((int)Mathf.Round(Mathf.Log(ChemistryBoost))) -1;
        baseChem -= S_GlobalManager.squad.GetPlayersWithTrait(SO_PlayerTrait.PlayerTraitNames.Does_Not_Know_The_Language).Count / 3; //every two players that do not know the language, chem decreases by 1
        return baseChem;
    }
    public static void IncreaseAtkBoost() => SquadAtkBoost = Mathf.Clamp(SquadAtkBoost + 1, 0, MAXBOOSTLEVEL);
    public static void IncreaseDefBoost() => SquadDefBoost = Mathf.Clamp(SquadDefBoost + 1, 0, MAXBOOSTLEVEL);
    public static void IncreaseFitnessBoost() => FitnessBoost = Mathf.Clamp(FitnessBoost + 1, 0, MAXBOOSTLEVEL);
    public static void IncreaseChemistryBoost() => ChemistryBoost = Mathf.Clamp(ChemistryBoost + 1, 0, MAXBOOSTLEVEL);
    public static void IncreaseFreeKicksBoost() => FreeKicksBoost = Mathf.Clamp(FreeKicksBoost + 1, 0, MAXBOOSTLEVEL);
    public static (UnityAction trainingA, UnityAction trainingB, string trAName, string trBName, string leftAction, string rightAction) FindTrainingBoosts()
    {
        //Function to find single action

        UnityAction FindAction(List<(UnityAction ua, int chance)> actionsList)
        {
            int total = 0;
            for (int i = 0; i < actionsList.Count; i++)
            {
                total += actionsList[i].chance;
                actionsList[i] = (actionsList[i].ua, total);
            }

            int ran = Random.Range(0, total+1);


            for (int i = 0; i < actionsList.Count; i++)
            {
                if (actionsList[i].chance >= ran)
                {
                    return actionsList[i].ua;
                }
            }
            return null;
        }
        //------------------------------

        List<(UnityAction ua, int chance)> actions = new List<(UnityAction ua, int chance)>
        {
            (IncreaseAtkBoost, MAXBOOSTLEVEL - SquadAtkBoost), (IncreaseChemistryBoost, MAXBOOSTLEVEL - ChemistryBoost), (IncreaseDefBoost, MAXBOOSTLEVEL - SquadDefBoost), (IncreaseFitnessBoost, MAXBOOSTLEVEL - FitnessBoost), (IncreaseFreeKicksBoost, MAXBOOSTLEVEL - FreeKicksBoost)
        };

        UnityAction firstAction = FindAction(actions);

        for (int i = 0; i < actions.Count; i++) if (actions[i].ua == firstAction) actions.RemoveAt(i);

        UnityAction secondAction = FindAction(actions);

        Dictionary<UnityAction, string> trainingNames = new Dictionary<UnityAction, string> 
        {
            { IncreaseAtkBoost, "Train Attack" },
            { IncreaseDefBoost, "Train Defense" },
            { IncreaseChemistryBoost, "Team Building" },
            { IncreaseFitnessBoost, "Increase Team Resistance" },
            { IncreaseFreeKicksBoost, "Train Free Kicks" },
        };

        string firstActionString = "";
        trainingNames.TryGetValue(firstAction, out firstActionString);
        
        string secondActionString = "";
        trainingNames.TryGetValue(secondAction, out secondActionString);

        Dictionary<UnityAction, string> shortNames = new Dictionary<UnityAction, string>
        {
            { IncreaseAtkBoost, "atk" },
            { IncreaseDefBoost, "def" },
            { IncreaseChemistryBoost, "chem" },
            { IncreaseFitnessBoost, "res" },
            { IncreaseFreeKicksBoost, "fk" },
        };

        string left = shortNames[firstAction];
        string right = shortNames[secondAction];

        return (firstAction, secondAction, firstActionString, secondActionString,left,right);
    }
}
