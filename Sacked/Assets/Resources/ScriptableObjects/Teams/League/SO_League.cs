using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(fileName = "New League", menuName = "Teams/New League")]
public class SO_League : ScriptableObject
{
    [SerializeField]
    public List<SO_Team> teamlist;
    public string teamsDirectory="ScriptableObjects/Teams";

    
    public void GenerateTeamInstances()
    {
        List<SO_Team> teaminstances = new List<SO_Team>(teamlist);
        teamlist.Clear();
        
        foreach(SO_Team team in teaminstances)
        {
            teamlist.Add(ScriptableObject.Instantiate(team));
        }
    }

    [ContextMenu("Generate Teams list")]
    public void GenerateTeamsList()
    {
        teamlist = new List<SO_Team>();
        teamlist = Resources.LoadAll<SO_Team>(teamsDirectory).ToList();
    }
}
