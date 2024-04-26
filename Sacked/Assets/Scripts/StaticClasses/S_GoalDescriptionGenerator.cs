using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class S_GoalDescriptionGenerator
{
    private static string goalDescriptionsDirectory = "ScriptableObjects/GoalDescriptions";
    private static List<SO_GoalDescriptions> goalDescriptionsDB;
    static S_GoalDescriptionGenerator()
    {
        goalDescriptionsDB = Resources.LoadAll<SO_GoalDescriptions>(goalDescriptionsDirectory).ToList();
    }
    public static string GenerateGoalDescription(SO_PlayerData goalScorer)
    {
        SO_GoalDescriptions.GoalReason goalReason = FindMostLikelyGoalReason(goalScorer);

        if (goalReason == SO_GoalDescriptions.GoalReason.None || Random.Range(0,100) < 25 ) //there's a 25% chance that the goal reason is not the most likely (to avoid that a tall player always scores with his head)
        {
            goalReason = (SO_GoalDescriptions.GoalReason)Random.Range(1, System.Enum.GetValues(typeof(SO_GoalDescriptions.GoalReason)).Length); //sets goal reason to a random one, starts from 1 because 0=none
        }

        List<string> possibleDescriptions = FindRightDescriptionsDatabase(goalReason);
        
        string goalDescription=possibleDescriptions[Random.Range(0, possibleDescriptions.Count)];

        return ReplaceVariablesInPlayerGoalDescripion(goalDescription, goalScorer.playerName); //Replaces {Player} with player name etc.

    }

    public static string GenerateOpponentGoalDescription()
    {
        //REDO add goal type chance that depends on team's traits

        SO_GoalDescriptions.GoalReason goalReason = (SO_GoalDescriptions.GoalReason)Random.Range(1, System.Enum.GetValues(typeof(SO_GoalDescriptions.GoalReason)).Length);
        List<string> possibleDescriptions = FindRightDescriptionsDatabase(goalReason);
        string goalDescription = possibleDescriptions[Random.Range(0, possibleDescriptions.Count)];
        return ReplaceVariablesInOpponentGoalDescripion(goalDescription);
    }

    private static SO_GoalDescriptions.GoalReason FindMostLikelyGoalReason(SO_PlayerData goalScorer)
    {
        if (goalScorer.playerTraits.Count == 0)
        {
            Debug.Log("QUESTO GIOCATORE NON HA TRATTI (SUS)");
            return SO_GoalDescriptions.GoalReason.None;
        }
        switch (goalScorer.playerTraits[0].traitName)
        {
            case SO_PlayerTrait.PlayerTraitNames.Longshot:
                return SO_GoalDescriptions.GoalReason.Longshot;
            
            case SO_PlayerTrait.PlayerTraitNames.Tall:
                return SO_GoalDescriptions.GoalReason.Head;
            
            default:
                break;
        }
        return SO_GoalDescriptions.GoalReason.None;
    }
    private static List<string> FindRightDescriptionsDatabase(SO_GoalDescriptions.GoalReason goalReason)
    {
        foreach(SO_GoalDescriptions gd in goalDescriptionsDB)
        {
            if (gd.goalReason == goalReason)
            {
                return gd.goalDescriptions;
            }
        }
        return new List<string> {"ha segnato in maniera abbastanza generica"};
    }

    private static string ReplaceVariablesInPlayerGoalDescripion(string description, string goalScorerName)
    {
        description = description.Replace("{Scorer}", goalScorerName);

        //REDO it is really ugly and it could happen that the assist man is the same player that scores
        description = description.Replace("{Supporter}", S_GlobalManager.squad.playingEleven[Random.Range(0, S_GlobalManager.squad.playingEleven.Count)].playerName);

        //REDO a little ugly, not the worst thing I've seen
        description = description.Replace("{Opponent}", S_PlayersGenerator.FindNameByNationality((SO_PlayerData.Nationality)Random.Range(0, System.Enum.GetValues(typeof(SO_PlayerData.Nationality)).Length),false));

        return description;
    }

    private static string ReplaceVariablesInOpponentGoalDescripion(string description)
    {
        string scorer = S_PlayersGenerator.CreateRandomName();
        string supporter = S_PlayersGenerator.CreateRandomName();
        string opponent = S_GlobalManager.squad.playingEleven[Random.Range(0,11)].playerName;

        description = description.Replace("{Scorer}", scorer);

        description = description.Replace("{Supporter}", supporter);

        description = description.Replace("{Opponent}", opponent);

        return description;
    }

}
