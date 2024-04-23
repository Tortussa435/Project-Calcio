using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
[CreateAssetMenu(fileName = "New Team", menuName = "Teams/New Team")]
public class SO_Team : ScriptableObject
{
    public string teamName;
    public int SkillLevel;
    public Color teamColor1=Color.black;
    public Color teamColor2=Color.black;
    public Sprite teamLogo;

    public List<SO_TeamTrait> teamTraits;

    public struct TeamValues
    {
        public float aggressivity;
        public float goalchanceboost;
    }
    public TeamValues teamValues;

    public void SetTeamValuesByTraits()
    {
        foreach(SO_TeamTrait trait in teamTraits)
        {
            trait.ApplyTrait(teamValues);
        }
        Debug.Log("Aggressivity: " + teamValues.aggressivity + "\nGoal Chance Boost: " + teamValues.goalchanceboost);

    }

    public void GenerateRandomTraits()
    {
        teamTraits = new List<SO_TeamTrait>();
        List<SO_TeamTrait> localpossibletraits = new List<SO_TeamTrait>(S_TraitsList.AllTraits);
        
        if (teamTraits.Count > 0) return;
     
        for(int i = 0; i < 3; i++)
        {
            if (localpossibletraits.Count < 1) break;

            SO_TeamTrait trait=localpossibletraits[Random.Range(0,localpossibletraits.Count)];

            teamTraits.Add(trait);

            for(int j = localpossibletraits.Count-1; j > 0; j--)
            {
                if (trait.excludedTraits.Count < 1) break;

                if (trait.excludedTraits.Contains(localpossibletraits[j].traitName))
                {
                    localpossibletraits.Remove(localpossibletraits[j]);
                }
            }

            localpossibletraits.Remove(trait);
            
            Debug.Log(teamTraits[i].traitName);
        }
    }
}

