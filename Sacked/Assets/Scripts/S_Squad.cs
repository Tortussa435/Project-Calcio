using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class S_Squad : MonoBehaviour
{



    public List<SO_PlayerData> Goalkeepers;
    public List<SO_PlayerData> Defense;
    public List<SO_PlayerData> Midfield;
    public List<SO_PlayerData> Attack;

    public List<SO_PlayerData> playingEleven;

    public bool teamListVisible = false;
    
    public SO_PlayerData captain;
    // Start is called before the first frame update
    void Awake()
    {
        S_GlobalManager.squad = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GenerateTeam()
    {
        int skillLevel = S_GlobalManager.selectedTeam.SkillLevel;

        //generate 11 starting players + 7 bench players

        //Goalkeepers

        /*
         * (all skill values clamped (0,5)
         * first gk = random[skillLevel-1, skillLevel+1]
         * second gk = random[skillLevel-3, skillLevel-2]
         */
        Goalkeepers.Add(S_PlayersGenerator.GeneratePlayer(SO_PlayerData.PlayerRole.Gk,skillLevel - 1 , skillLevel + 1));
        Goalkeepers.Add(S_PlayersGenerator.GeneratePlayer(SO_PlayerData.PlayerRole.Gk, skillLevel -3 ,  skillLevel -2));

        //Defense
        /*
         * first 3 def = random[skillLevel-2, skillLevel+2
         * 4th def = random[skillLevel-3,skillLevel+1]
         * 5,6 = random[skillLevel-3,skillLevel]
         */

        Defense.Add(S_PlayersGenerator.GeneratePlayer(SO_PlayerData.PlayerRole.Def,skillLevel - 2, skillLevel + 2));
        Defense.Add(S_PlayersGenerator.GeneratePlayer(SO_PlayerData.PlayerRole.Def,skillLevel - 2, skillLevel + 1));
        Defense.Add(S_PlayersGenerator.GeneratePlayer(SO_PlayerData.PlayerRole.Def,skillLevel - 2, skillLevel + 1));

        Defense.Add(S_PlayersGenerator.GeneratePlayer(SO_PlayerData.PlayerRole.Def, skillLevel - 3, skillLevel + 0));

        Defense.Add(S_PlayersGenerator.GeneratePlayer(SO_PlayerData.PlayerRole.Def, skillLevel - 3, skillLevel-1));

        //Midfield
        /*
         * first 3 mid = random[skillLevel-2,skillLevel+2]
         * 4th mid = random[skillLevel-3,skillLevel+1]
         * 5,6 = random[skillLevel-3,skillLevel]
         */

        Midfield.Add(S_PlayersGenerator.GeneratePlayer(SO_PlayerData.PlayerRole.Mid, skillLevel - 2, skillLevel + 2));
        Midfield.Add(S_PlayersGenerator.GeneratePlayer(SO_PlayerData.PlayerRole.Mid, skillLevel - 2, skillLevel + 1));
        Midfield.Add(S_PlayersGenerator.GeneratePlayer(SO_PlayerData.PlayerRole.Mid, skillLevel - 2, skillLevel + 1));
        
        Midfield.Add(S_PlayersGenerator.GeneratePlayer(SO_PlayerData.PlayerRole.Mid, skillLevel - 3, skillLevel + 0));
        
        Midfield.Add(S_PlayersGenerator.GeneratePlayer(SO_PlayerData.PlayerRole.Mid, skillLevel - 3, skillLevel - 1));

        //Attack
        /*
         * first 2 att = random[skillLevel-1, skillLevel+1]
         * last 2 att = random[skillLevel-2,skillLevel]
         */

        Attack.Add(S_PlayersGenerator.GeneratePlayer(SO_PlayerData.PlayerRole.Atk, skillLevel - 1, skillLevel + 1));
        Attack.Add(S_PlayersGenerator.GeneratePlayer(SO_PlayerData.PlayerRole.Atk, skillLevel - 1, skillLevel + 1));

        Attack.Add(S_PlayersGenerator.GeneratePlayer(SO_PlayerData.PlayerRole.Atk, skillLevel - 2, skillLevel));
        Attack.Add(S_PlayersGenerator.GeneratePlayer(SO_PlayerData.PlayerRole.Atk, skillLevel - 2, skillLevel));

        //makes one random player skillLevel = random[4,5]

        int buffedRole = Random.Range(0, 4);
        switch (buffedRole)
        {

            case 0://Goalkeeper
                Goalkeepers[Random.Range(0, Goalkeepers.Count)].skillLevel = 5;
                break;
            
            case 1://Defense
                Defense[Random.Range(0, Goalkeepers.Count)].skillLevel = 5;
                break;
            
            case 2://Midfield
                Midfield[Random.Range(0, Goalkeepers.Count)].skillLevel = 5;
                break;
            
            case 3://Attack
                Attack[Random.Range(0, Goalkeepers.Count)].skillLevel = 5;
                break;
        }

        SetBestPlayingEleven();
        
    }

    public void ShowTeam()
    {
        if (teamListVisible)
        {
            //REDO avoid creating items each time the list pops up
            foreach (Transform child in gameObject.transform)
            {
                Destroy(child.gameObject);
            }
            gameObject.SetActive(false);
            teamListVisible = false;
            Debug.Log("misavero");
        }
        else
        {
            gameObject.SetActive(true);
            teamListVisible = true;
            GeneratePlayersSlots(Goalkeepers);
            GeneratePlayersSlots(Defense);
            GeneratePlayersSlots(Midfield);
            GeneratePlayersSlots(Attack);
        }
        
    }

    private void GeneratePlayersSlots(List<SO_PlayerData> playersList)
    {
        for (int i = 0; i < playersList.Count; i++)
        {
            GameObject teambox = Resources.Load<GameObject>("Prefabs/PlayerBox");
            teambox = Instantiate(teambox, Vector3.zero, Quaternion.identity, gameObject.transform);
            teambox.GetComponent<Image>().color = i % 2 == 0 ? Color.gray : Color.white; //REDO give color based on role
            teambox.transform.GetChild(0).GetComponent<TextMeshProUGUI>().SetText(playersList[i].playerName);
            teambox.transform.GetChild(1).GetComponent<TextMeshProUGUI>().SetText(playersList[i].skillLevel.ToString());
            teambox.transform.GetChild(2).GetComponent<TextMeshProUGUI>().SetText(playersList[i].playerRole.ToString());
            teambox.transform.GetChild(3).GetComponent<TextMeshProUGUI>().SetText(playersList[i].playerTraits[0].GetTraitName());
            teambox.transform.GetChild(4).GetComponent<TextMeshProUGUI>().SetText(playersList[i].playerNationality.ToString());
        }
    }
    
    private void SetBestPlayingEleven()
    {
        playingEleven = new List<SO_PlayerData>();
        //sets goalkeeper
        playingEleven.Add(Goalkeepers[0].skillLevel > Goalkeepers[1].skillLevel ? Goalkeepers[0] : Goalkeepers[1]);

        int def = 0;
        int mid = 0;
        int atk = 0;

        //sorts other players by skill
        List<SO_PlayerData> sortedPlayers = new List<SO_PlayerData>();
        sortedPlayers.AddRange(Defense);
        sortedPlayers.AddRange(Midfield);
        sortedPlayers.AddRange(Attack);
        sortedPlayers = sortTeamListBySkill(sortedPlayers);

        foreach(SO_PlayerData player in sortedPlayers)
        {
            switch (player.playerRole)
            {
                default:
                    break;

                case SO_PlayerData.PlayerRole.Def:
                    if (def >= 4) break;
                    playingEleven.Add(player);
                    def++;
                    break;

                case SO_PlayerData.PlayerRole.Mid:
                    if (mid >= 4) break;
                    playingEleven.Add(player);
                    mid++;
                    break;

                case SO_PlayerData.PlayerRole.Atk:
                    if (atk >= 3) break;
                    playingEleven.Add(player);
                    atk++;
                    break;
            }
            if (playingEleven.Count >= 11) break;
        }

        foreach(SO_PlayerData player in playingEleven)
        {
            Debug.Log(player.playerName);
        }
        Debug.Log("Formazione: " + def + mid + atk);
    }

    private List<SO_PlayerData> sortTeamListBySkill(List<SO_PlayerData> datalist)
    {
        int n = datalist.Count;

        for (int i = 0; i < n - 1; i++)

            for (int j = 0; j < n - i - 1; j++)

                if (datalist[j].skillLevel > datalist[j + 1].skillLevel)
                {
                    SO_PlayerData player = datalist[j];
                    datalist[j] = datalist[j + 1];
                    datalist[j + 1] = player;
                }
        datalist.Reverse();
        return datalist;
    }

    public int FindGameSkillLevel()
    {
        int totalskill = 0;
        foreach(SO_PlayerData player in playingEleven)
        {
            totalskill += player.skillLevel;
        }
        totalskill /= 11;
        return totalskill;
    }

    public List<SO_PlayerData> GetHotHeadPlayers()
    {
        List<SO_PlayerData> hotheads = new List<SO_PlayerData>();
        foreach(SO_PlayerData player in playingEleven)
        {
            if (player.playerTraits[0].traitName==SO_PlayerTrait.PlayerTraitNames.Hot_Head) hotheads.Add(player);
        }
        return hotheads;
    }

    public bool TeamContainsTrait(SO_PlayerTrait.PlayerTraitNames trait, bool playingElevenOnly=false)
    {
        if (playingElevenOnly)
        {
            foreach(SO_PlayerData player in playingEleven)
            {
                if (player.playerTraits[0].traitName == trait)
                {
                    return true;
                }
            }
        }
        else
        {
            foreach(SO_PlayerData player in Goalkeepers)
            {
                if (player.playerTraits[0].traitName == trait)
                {
                    return true;
                }
            }

            foreach (SO_PlayerData player in Defense)
            {
                if (player.playerTraits[0].traitName == trait)
                {
                    return true;
                }
            }

            foreach (SO_PlayerData player in Midfield)
            {
                if (player.playerTraits[0].traitName == trait)
                {
                    return true;
                }
            }

            foreach (SO_PlayerData player in Attack)
            {
                if (player.playerTraits[0].traitName == trait)
                {
                    return true;
                }
            }
        }


        return false;
    }
}
