using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class S_TraitsCombinationsManager : MonoBehaviour
{
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

    public static bool CheckTraitsCombination(SO_PlayerTrait.PlayerTraitNames playerTrait, SO_Team team)
    {
        TraitsEventCombination combinationToCheck = new TraitsEventCombination();
        
        foreach(TraitsEventCombination combination in globalTraitsCombation)
        {
            if (combination.playerTraitName == playerTrait)
            {
                combinationToCheck = combination;
                break;
            }
        }
        if (combinationToCheck.playerTraitName == SO_PlayerTrait.PlayerTraitNames.None) return false;

        foreach(TriggeredEvent tevent in combinationToCheck.reactions)
        {
            foreach(SO_TeamTrait trait in team.teamTraits)
            {
                if (tevent.teamTrait == trait.traitName)
                {
                    tevent.triggeredEvent.Invoke();
                    return true;
                }
            }
        }

        return false;
    }

    //TRAITS EVENTS LIBRARY

    public void TraitCombinationFound()
    {
        Debug.Log("Zio marconius!");
    }
}
