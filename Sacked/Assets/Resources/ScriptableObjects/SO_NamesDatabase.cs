using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Names Group", menuName = "NamesGroup/Name Group")]
public class SO_NamesDatabase : ScriptableObject
{
    [System.Serializable]
    public struct NamesNations
    {
        public List<SO_PlayerData.Nationality> nationalitiesGroup;
        public List<string> names;
        public List<string> surnames;
        public List<string> nicknames;
    }

    public NamesNations namesNations;
    
}
