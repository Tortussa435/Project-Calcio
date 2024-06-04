using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Goal Reasons List", menuName = "Goals/Goal Reasons")]
public class SO_GoalDescriptions : ScriptableObject
{
    [System.Serializable]
    public enum GoalReason
    {
        None,
        Head,
        InArea,
        Longshot,
        Chipshot
    }
    public GoalReason goalReason;

    public List<string> goalDescriptions;

    public string GetRandomDescription(SO_PlayerData golScorer, bool playerGoal)
    {
        if (goalDescriptions.Count <= 0) return "Non ci sono descrizioni in questa lista";
        else return ReplaceVariablesInGoalDescription(golScorer,goalDescriptions[Random.Range(0, goalDescriptions.Count)],playerGoal);
    }

    public string ReplaceVariablesInGoalDescription(SO_PlayerData golScorer, string description, bool playerGoal)
    {
        if (playerGoal)
        {
            description = description.Replace("{Scorer}", golScorer.playerName);

            //REDO it is really ugly and it could happen that the assist man is the same player that scores
            description = description.Replace("{Supporter}", S_GlobalManager.squad.playingEleven[Random.Range(0, S_GlobalManager.squad.playingEleven.Count)].playerName);

            //REDO a little ugly, not the worst thing I've seen
            description = description.Replace("{Opponent}", S_PlayerMatchSimulator.RandomlyGetNewOrExistingOpponentPlayer());

            description = description.Replace("{Gk}", S_PlayerMatchSimulator.RandomlyGetNewOrExistingOpponentPlayer());
        }
        else
        {
            string scorer = S_PlayerMatchSimulator.RandomlyGetNewOrExistingOpponentPlayer();
            string supporter = S_PlayerMatchSimulator.RandomlyGetNewOrExistingOpponentPlayer();
            string opponent = S_GlobalManager.squad.playingEleven[Random.Range(0, S_GlobalManager.squad.playingEleven.Count)].playerName;
            string goalkeeper = S_GlobalManager.squad.GetPlayingPlayerByRole(SO_PlayerData.PlayerRole.Gk);
            
            description = description.Replace("{Scorer}", scorer);

            description = description.Replace("{Supporter}", supporter);

            description = description.Replace("{Opponent}", opponent);

            description = description.Replace("{Gk}", goalkeeper);
        }
        return description;
    }
}
