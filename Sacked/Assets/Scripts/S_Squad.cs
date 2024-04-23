using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class S_Squad : MonoBehaviour
{



    public List<SO_PlayerData> Goalkeepers;
    public List<SO_PlayerData> Defense;
    public List<SO_PlayerData> Midfield;
    public List<SO_PlayerData> Attack;

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
    
}
