using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

static public class S_PlayersGenerator
{
    static public List<SO_NamesDatabase> possibleNames;
    
    static S_PlayersGenerator()
    {
        possibleNames = new List<SO_NamesDatabase>();
        possibleNames = Resources.LoadAll<SO_NamesDatabase>("ScriptableObjects/NamesDatabase").ToList();
    }
    static public SO_PlayerData GeneratePlayer(SO_PlayerData.PlayerRole role, int minSkill, int maxSkill)
    {
        SO_PlayerData playerData = ScriptableObject.CreateInstance<SO_PlayerData>();
        
        /*REDO
        List<string> names = new List<string>();
        names.AddRange(new string[] {"Mario","Luigi","Wario"});
        playerData.playerName = names[Random.Range(0,names.Count)];
        */

        playerData.playerRole = role;
        playerData.skillLevel = Random.Range(Mathf.Clamp(minSkill,0,5)+1, Mathf.Clamp(maxSkill,0,5)+1); //max exclusive
        playerData.playerNationality = (SO_PlayerData.Nationality)Random.Range(0, System.Enum.GetValues(typeof(SO_PlayerData.Nationality)).Length); //esoteric https://discussions.unity.com/t/using-random-range-to-pick-a-random-value-out-of-an-enum/119639/2
        playerData.playerName = FindNameByNationality(playerData.playerNationality, true) + " " + FindNameByNationality(playerData.playerNationality, false);
        playerData.name = playerData.playerName + " " + playerData.skillLevel;

        SO_PlayerTrait randomTrait = S_PlayerTraitsList.playerTraitsDatabase[Random.Range(0, S_PlayerTraitsList.playerTraitsDatabase.Count)];
        playerData.playerTraits.Add(randomTrait);
        
        
        return playerData;
    }

    static public string FindNameByNationality(SO_PlayerData.Nationality nationality, bool nameOrSurname)
    {
        string finalName = new string("");
        foreach(SO_NamesDatabase db in possibleNames)
        {
            if (db.namesNations.nationalitiesGroup.Contains(nationality))
            {
                if (nameOrSurname) finalName = db.namesNations.names[Random.Range(0, db.namesNations.names.Count)];
                
                else finalName = db.namesNations.surnames[Random.Range(0, db.namesNations.surnames.Count)];
                
                return finalName;
                
            }
        }
        return finalName;
    }
}

