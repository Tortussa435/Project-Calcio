using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class S_TraitsCombinationsManager : MonoBehaviour
{
    private static SO_PlayerData triggeringPlayer;

    [System.Serializable]
    public struct TriggeredEvent
    {
        public SO_TeamTrait.TraitNames teamTrait;
        public UnityEvent triggeredEvent;
    }

    [System.Serializable]
    public struct TraitsEventCombination
    {
        public SO_PlayerTrait.PlayerTraitNames playerTraitName;

        public List<TriggeredEvent> reactions;
    }

    [SerializeField]
    List<TraitsEventCombination> traitsEventsCombinations;

    public static List<TraitsEventCombination> globalTraitsCombation;

    //creates a list that can be accessed via static events
    void Start()
    {
        globalTraitsCombation = traitsEventsCombinations;
    }

    public static bool CheckTraitsCombination(SO_PlayerData player, SO_Team team)
    {
        triggeringPlayer = player;

        TraitsEventCombination combinationToCheck = new TraitsEventCombination();

        foreach (TraitsEventCombination combination in globalTraitsCombation)
        {
            if (combination.playerTraitName == player.playerTraits[0].traitName)
            {
                combinationToCheck = combination;
                break;
            }
        }

        if (combinationToCheck.playerTraitName == SO_PlayerTrait.PlayerTraitNames.None)
        {
            //Debug.Log("non trovo nulaaa");
            return false;
        }

        foreach (TriggeredEvent tevent in combinationToCheck.reactions)
        {
            foreach (SO_TeamTrait trait in team.teamTraits)
            {
                if (tevent.teamTrait == trait.traitName)
                {
                    //triggeringPlayer = 
                    Debug.Log("Tratto giocatore: " + combinationToCheck.playerTraitName + " Tratto di squadra:" + tevent.teamTrait);
                    tevent.triggeredEvent.Invoke();
                    return true;
                }
            }
        }

        //Debug.Log("Che amarezza");
        return false;
    }

    //TRAITS EVENTS LIBRARY

    public void TraitCombinationFound()
    {
        Debug.Log("Zio marconius!");
    }
    public void IncreasePlayerGoalChance(float increase = 1.0f)
    {
        if (S_PlayerMatchSimulator.IsPlayerHomeTeam())
        {
            S_PlayerMatchSimulator.traitsScoreChance.home += increase;
        }

        else S_PlayerMatchSimulator.traitsScoreChance.away += increase;
    }
    public void IncreaseOpponentGoalChance(float increase = 1.0f)
    {
        if (!S_PlayerMatchSimulator.IsPlayerHomeTeam())
        {
            S_PlayerMatchSimulator.traitsScoreChance.home += increase;
        }

        else S_PlayerMatchSimulator.traitsScoreChance.away += increase;
    }
    public void IncreasePlayerInjuryChance(float injuryChance)
    {
        if (S_PlayerMatchSimulator.IsPlayerHomeTeam())
        {
            S_PlayerMatchSimulator.injuryChance.home += injuryChance;
        }

        else S_PlayerMatchSimulator.injuryChance.away += injuryChance;
    }
    public void IncreaseOpponentInjuryChance(float injuryChance)
    {
        if (!S_PlayerMatchSimulator.IsPlayerHomeTeam())
        {
            S_PlayerMatchSimulator.injuryChance.home += injuryChance;
        }

        else S_PlayerMatchSimulator.injuryChance.away += injuryChance;
    }
    public void IncreasePlayerAggressivity(float aggressivity)
    {
        if (S_PlayerMatchSimulator.IsPlayerHomeTeam())
        {
            S_PlayerMatchSimulator.matchAggressivity.home += aggressivity;
        }

        else S_PlayerMatchSimulator.matchAggressivity.away += aggressivity;
    }
    public void IncreaseOpponentAggressivity(float aggressivity)
    {
        if (!S_PlayerMatchSimulator.IsPlayerHomeTeam())
        {
            S_PlayerMatchSimulator.matchAggressivity.home += aggressivity;
        }

        else S_PlayerMatchSimulator.matchAggressivity.away += aggressivity;
    }

    // TRAIT SPECIFIC EVENTS
    public void TallPlayerVSSmallTeam(float addedValue = 1.0f)
    {
        switch (triggeringPlayer.playerRole)
        {
            default:
                break;

            case SO_PlayerData.PlayerRole.Def:

                if (!S_PlayerMatchSimulator.IsPlayerHomeTeam())
                {
                    S_PlayerMatchSimulator.traitsScoreChance.home -= addedValue;
                }

                else S_PlayerMatchSimulator.traitsScoreChance.away -= addedValue;

                break;

            case SO_PlayerData.PlayerRole.Atk:

                if (S_PlayerMatchSimulator.IsPlayerHomeTeam())
                {
                    S_PlayerMatchSimulator.traitsScoreChance.home += addedValue;
                }

                else S_PlayerMatchSimulator.traitsScoreChance.away += addedValue;

                break;
        }
    }

}
