using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
[CreateAssetMenu(fileName = "New Team", menuName = "Teams/New Team")]
public class SO_Team : ScriptableObject
{
    public string teamName;
    public string shortName;

    public int SkillLevel;
    public Color teamColor1=Color.black;
    public Color teamColor2=Color.black;
    public Sprite teamLogo;

    public List<SO_TeamTrait> teamTraits;
    
    public List<SO_Team> derbies;

    public SO_Tactics teamTactics;

    public void GenerateRandomTraits()
    {
        teamTraits = new List<SO_TeamTrait>();
        List<SO_TeamTrait> localpossibletraits = new List<SO_TeamTrait>(S_TraitsList.AllTraits);
        
        if (teamTraits.Count > 0) return;
     
        for(int i = 0; i < 3; i++)
        {
            if (localpossibletraits.Count < 1) break;

            SO_TeamTrait trait=localpossibletraits[Random.Range(0,localpossibletraits.Count)];

            trait.teamRef = this;

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
    
    public bool TeamHasTrait(SO_TeamTrait.TraitNames traitName)
    {
        foreach(SO_TeamTrait t in teamTraits)
        {
            if (t.traitName == traitName) return true;
        }
        return false;
    }
}

