using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

static public class S_PlayersGenerator
{
    static public List<SO_NamesDatabase> possibleNames;
    
    static S_PlayersGenerator()
    {
        possibleNames = Resources.LoadAll<SO_NamesDatabase>(S_ResDirs.namesDatabase).ToList();
    }
    static public SO_PlayerData GeneratePlayer(SO_PlayerData.PlayerRole role, int minSkill, int maxSkill=-1, bool forPlayer=true)
    {
        SO_PlayerData playerData = ScriptableObject.CreateInstance<SO_PlayerData>();
        
        /*REDO
        List<string> names = new List<string>();
        names.AddRange(new string[] {"Mario","Luigi","Wario"});
        playerData.playerName = names[Random.Range(0,names.Count)];
        */

        playerData.playerRole = role;

        if (maxSkill == -1) playerData.skillLevel = minSkill;
        else playerData.skillLevel = Random.Range(Mathf.Clamp(minSkill,1,5), Mathf.Clamp(maxSkill,1,5)+1);

        playerData.playerNationality = (SO_PlayerData.Nationality)Random.Range(0, System.Enum.GetValues(typeof(SO_PlayerData.Nationality)).Length); //https://discussions.unity.com/t/using-random-range-to-pick-a-random-value-out-of-an-enum/119639/2
        
        //if a player is from vatican it has to give the same nationality twice, to reduce chance of having the pope in your team (chance = 0.01% circa, 1.9% of having one vatican player in 19 players team)
        if(playerData.playerNationality==SO_PlayerData.Nationality.VaticanCity) playerData.playerNationality = (SO_PlayerData.Nationality)Random.Range(0, System.Enum.GetValues(typeof(SO_PlayerData.Nationality)).Length);
        
        playerData.playerName = FindNameByNationality(playerData.playerNationality, true) + " " + FindNameByNationality(playerData.playerNationality, false);
        playerData.name = playerData.playerName + " " + playerData.skillLevel;

        SO_PlayerTrait randomTrait = ScriptableObject.Instantiate<SO_PlayerTrait>(S_PlayerTraitsList.playerTraitsDatabase[Random.Range(0, S_PlayerTraitsList.playerTraitsDatabase.Count)]);
        randomTrait.playerRef = playerData;
        playerData.playerTraits.Add(randomTrait);

        if (forPlayer)
        {
            playerData.playerFace = ScriptableObject.CreateInstance<SO_Face>();
            playerData.playerFace.MakeFace(playerData.playerNationality);
        }

        return playerData;
    }

    static public void ItalianizePlayer(SO_PlayerData player)
    {
        player.playerNationality = SO_PlayerData.Nationality.Italy;
        player.playerName = FindNameByNationality(SO_PlayerData.Nationality.Italy, true) + " " + FindNameByNationality(SO_PlayerData.Nationality.Italy, false);
        player.name = player.playerName + " " + player.skillLevel;
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

    static public string CreateRandomName(bool onlySurname=false)
    {
        SO_PlayerData.Nationality nationality = (SO_PlayerData.Nationality)Random.Range(0, System.Enum.GetValues(typeof(SO_PlayerData.Nationality)).Length);
        
        if (onlySurname) return FindNameByNationality(nationality, false);

        return
        (
            FindNameByNationality(nationality, true)
            + " " +
            FindNameByNationality(nationality,false)
        );
    }

    public static string GenerateFakeOpponentPlayer()
    {
        if (S_PlayerMatchSimulator.opponentTeamNames.Count >= 11) return null;
        string name = CreateRandomName();
        S_PlayerMatchSimulator.opponentTeamNames.Add(name);
        return name;
    }
    
}

