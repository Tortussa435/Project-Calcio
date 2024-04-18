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

    public List<S_TeamTrait> teamTraits;

    public struct TeamValues
    {
        public float aggressivity;
        public float goalchanceboost;
    }
    public TeamValues teamValues;

    public void SetTeamValuesByTraits()
    {
        foreach(S_TeamTrait trait in teamTraits)
        {
            trait.ApplyTrait(teamValues);
        }
        Debug.Log("Aggressivity: " + teamValues.aggressivity + "\nGoal Chance Boost: " + teamValues.goalchanceboost);

    }

    public void GenerateRandomTraits()
    {
        teamTraits = new List<S_TeamTrait>();
        
        if (teamTraits.Count > 0) return;
     
        for(int i = 0; i < 3; i++)
        {
            teamTraits.Add(S_TraitsList.AllTraits[Random.Range(0,S_TraitsList.AllTraits.Count)]);
            Debug.Log(teamTraits[i].traitName);
        }
    }
}

