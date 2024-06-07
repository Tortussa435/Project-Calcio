using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public static class S_PlayerTraitsList
{
    public static List<SO_PlayerTrait> playerTraitsDatabase;
    static S_PlayerTraitsList()
    {
        playerTraitsDatabase = Resources.LoadAll<SO_PlayerTrait>(S_ResDirs.playerTraitsDatabase).ToList();
    }

    public static SO_PlayerTrait AssignPlayerTrait(int playerSkill, List<SO_PlayerTrait.PlayerTraitNames> bannedTraits=null) 
    {
        int seed = Random.Range(0, 6);
        List<SO_PlayerTrait> traitsList=new List<SO_PlayerTrait>();
        if (seed < playerSkill) traitsList = GetTraitsByValue(S_FootballEnums.Positivity.Negative,bannedTraits);
        //if (seed == playerSkill) traitsList = GetTraitsByValue(S_FootballEnums.Positivity.Neutral); currently there are no neutral traits
        if (seed >= playerSkill) traitsList = GetTraitsByValue(S_FootballEnums.Positivity.Positive,bannedTraits);
        return traitsList[Random.Range(0, traitsList.Count)];
    }

    private static List<SO_PlayerTrait> GetTraitsByValue(S_FootballEnums.Positivity positivity, List<SO_PlayerTrait.PlayerTraitNames> bannedTraits=null)
    {
        List<SO_PlayerTrait> traits = new List<SO_PlayerTrait>();
        foreach(SO_PlayerTrait trait in playerTraitsDatabase)
        {
            if (bannedTraits != null) if (bannedTraits.Contains(trait.traitName)) continue;

            if (trait.positiveTrait == positivity) traits.Add(trait);
        }
        return traits;
    }
}
