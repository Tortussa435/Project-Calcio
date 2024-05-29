using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class S_Squad : MonoBehaviour
{

    public enum PossibleTeam
    {
        BestTeam,
        Turnover,
        Fit
    }

    public GameObject showTeamButtonRef;

    public PossibleTeam teamLineup = PossibleTeam.BestTeam;

    public List<SO_PlayerData> Goalkeepers;
    public List<SO_PlayerData> Defense;
    public List<SO_PlayerData> Midfield;
    public List<SO_PlayerData> Attack;

    public List<SO_PlayerData> playingEleven;
    public List<SO_PlayerData> bench;

    public bool teamListVisible = false;

    public GameObject teamCardPrefab;
    private GameObject teamCardRef;

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

        showTeamButtonRef.SetActive(true);

        S_SubstitutionsManager.SetElevenAndBenchList(playingEleven, bench);
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
            GameObject teambox = Resources.Load<GameObject>(S_ResDirs.playerbox);
            teambox = Instantiate(teambox, Vector3.zero, Quaternion.identity, gameObject.transform);
            teambox.GetComponent<Image>().color = i % 2 == 0 ? Color.gray : Color.white; //REDO give color based on role
            teambox.transform.GetChild(0).GetComponent<TextMeshProUGUI>().SetText(playersList[i].playerName);
            teambox.transform.GetChild(1).GetComponent<TextMeshProUGUI>().SetText(playersList[i].skillLevel.ToString());
            teambox.transform.GetChild(2).GetComponent<TextMeshProUGUI>().SetText(playersList[i].playerRole.ToString());
            teambox.transform.GetChild(3).GetComponent<TextMeshProUGUI>().SetText(playersList[i].playerTraits[0].GetTraitName());
            teambox.transform.GetChild(4).GetComponent<TextMeshProUGUI>().SetText(playersList[i].playerNationality.ToString());
        }
    }
    

    public int FindGameSkillLevel()
    {
        int totalskill = 0;
        foreach(SO_PlayerData player in playingEleven)
        {
            totalskill += player.skillLevel * Mathf.CeilToInt(player.playerEnergy/100);
        }
        totalskill /= 11;
        return totalskill;
    }

    public List<SO_PlayerData> GetPlayersWithTrait(SO_PlayerTrait.PlayerTraitNames trait, bool playingElevenOnly=true)
    {
        List<SO_PlayerData> pWithTrait = new List<SO_PlayerData>();
        if (playingElevenOnly)
        {
            foreach (SO_PlayerData player in playingEleven)
            {
                if (player.playerTraits[0].traitName == trait)
                {
                    pWithTrait.Add(player);
                }
            }
        }
        else
        {
            List<SO_PlayerData> players = Goalkeepers.Union(Defense).ToList().Union(Midfield).ToList().Union(Attack).ToList();
            foreach(SO_PlayerData player in players)
            {
                if (player.playerTraits[0].traitName == trait)
                {
                    pWithTrait.Add(player);
                }
            }
        }
        return pWithTrait;
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

    public void RemoveExpulsions()
    {
        //REDO pretty awful code ngl
        foreach(SO_PlayerData player in Goalkeepers) if (player.expelled > 0) player.expelled--;
        foreach(SO_PlayerData player in Defense) if (player.expelled > 0) player.expelled--;
        foreach(SO_PlayerData player in Midfield) if (player.expelled > 0) player.expelled--;
        foreach(SO_PlayerData player in Attack) if (player.expelled > 0) player.expelled--;
        
    }
    
    public void ReduceInjuries()
    {
        //REDO pretty awful code ngl
        foreach (SO_PlayerData player in Goalkeepers) if (player.injuried > 0) player.injuried--;
        foreach (SO_PlayerData player in Defense) if (player.injuried > 0) player.injuried--;
        foreach (SO_PlayerData player in Midfield) if (player.injuried > 0) player.injuried--;
        foreach (SO_PlayerData player in Attack) if (player.injuried > 0) player.injuried--;
    }

    public void RefillBenchEnergy(float min=10.0f, float max=20.0f)
    {
        foreach(SO_PlayerData player in bench)
        {
            player.AddEnergy(min, max);
        }
    }

    public void DecreaseElevenEnergy(float min=-20.0f, float max=-10.0f)
    {
        List<SO_PlayerData> players = S_GlobalManager.squad.playingEleven;
        foreach (SO_PlayerData p in players)
        {
            p.AddEnergy(min, max);
        }
    }

    /// <summary>
    /// Difference between ShowTeamElevenCard() and GenerateNextTacticTeamElevenCard() is that show team eleven card does not change the tactic when called, while the other function does so.
    /// This Shitcode comes from the limitations of UnityEvents, that cannot have more than one parameter or a non void Return type
    /// </summary>
    /// <param name="forceSpawn"></param>
    public void ShowTeamElevenCard(bool forceSpawn = false)
    {
        if (teamCardRef == null || forceSpawn)
        {
            SO_CardData data = ScriptableObject.Instantiate(Resources.Load<SO_CardData>(S_ResDirs.teamCard));
            data.leftChoice = S_GlobalManager.squad.FindNextLineup().ToString();
            teamCardRef = S_GlobalManager.deckManagerRef.GenerateCard(data, teamCardPrefab, false);
            S_GlobalManager.deckManagerRef.SetCardOnTop(teamCardRef);
            data.rightEffects.AddListener(() => teamCardRef = null); //reference must be lost even before than card destruction
        }

        else
        {
            Destroy(teamCardRef);
        }
    }

    /// <summary>
    /// Difference between ShowTeamElevenCard() and GenerateNextTacticTeamElevenCard() is that show team eleven card does not change the tactic when called, while the other function does so.
    /// This Shitcode comes from the limitations of UnityEvents, that cannot have more than one parameter or a non void Return type
    /// </summary>
    /// <param name="forceSpawn"></param>
    public void GenerateNextTacticTeamElevenCard(bool forceSpawn = false)
    {
        if (teamCardRef == null || forceSpawn)
        {
            SO_CardData data = ScriptableObject.Instantiate(Resources.Load<SO_CardData>(S_ResDirs.teamCard));
            data.onGeneratedEffects.AddListener(data.FindNextLineupDescription);
            teamCardRef = S_GlobalManager.deckManagerRef.GenerateCard(data, teamCardPrefab, false);
            S_GlobalManager.deckManagerRef.SetCardOnTop(teamCardRef);
            data.rightEffects.AddListener(() => teamCardRef = null); //reference must be lost even before than card destruction
        }

        else
        {
            Destroy(teamCardRef);
        }
    }

    public string GetPlayingPlayerByRole(SO_PlayerData.PlayerRole role)
    {
        List<string> eligibleNames = new List<string>();
        foreach(SO_PlayerData p in playingEleven)
        {
            if (p.playerRole == role)
            {
                eligibleNames.Add(p.playerName);
            }
        }

        if (eligibleNames.Count < 1)
        {
            //Debug.Log("Niuno fu trovato");
            return "Niuno";
        }
        return eligibleNames[Random.Range(0, eligibleNames.Count)];
    }

    public string GetAnyRandomPlayer()
    {
        List<SO_PlayerData> players = Goalkeepers.Union(Defense).ToList().Union(Midfield).ToList().Union(Attack).ToList();
        return players[Random.Range(0, players.Count)].playerName;
    }
    #region LINEUPS
    public PossibleTeam FindNextLineup() => (PossibleTeam) ((int)(teamLineup + 1) % System.Enum.GetValues(typeof(PossibleTeam)).Length);
    public void AddGoalKeeper()
    {
        if (Goalkeepers.Count > 1)
        {
            if (Goalkeepers[0].skillLevel > Goalkeepers[1].skillLevel)
            {
                playingEleven.Add(Goalkeepers[0]);
                bench.Add(Goalkeepers[1]);
            }
            else
            {
                playingEleven.Add(Goalkeepers[1]);
                bench.Add(Goalkeepers[0]);
            }
        }
        else if(Goalkeepers.Count==0)
        {
            Debug.LogWarning("NON CI SONO PORTIERI!!!");
        }
        else playingEleven.Add(Goalkeepers[0]);
    }
    public void AddPlayersToEleven(List<SO_PlayerData> players, float energyThreshold = 0)
    {

        int def = 0, mid = 0, atk = 0;
        List<SO_PlayerData> localBench = new List<SO_PlayerData>(players);

        foreach (SO_PlayerData player in players)
        {
            //do not add expelled players to team
            if (player.expelled > 0 || player.injuried > 0 || player.playerEnergy < energyThreshold)
            {
                Debug.Log("Il giocatore è espulso e non puote giocar");
                continue;
            }
            switch (player.playerRole)
            {
                default:
                    break;

                case SO_PlayerData.PlayerRole.Def:
                    if (def >= 4) break;
                    playingEleven.Add(player);
                    localBench.Remove(player);
                    def++;
                    break;

                case SO_PlayerData.PlayerRole.Mid:
                    if (mid >= 4) break;
                    playingEleven.Add(player);
                    localBench.Remove(player);
                    mid++;
                    break;

                case SO_PlayerData.PlayerRole.Atk:
                    if (atk >= 3) break;
                    playingEleven.Add(player);
                    localBench.Remove(player);
                    atk++;
                    break;
            }
            if (playingEleven.Count >= 11)
            {
                bench.AddRange(localBench);
                break;
            }
        }

        //reruns the function with a lower energy threshold to avoid having a team with less than 11 players when asking to do turnover with tired players
        if (playingEleven.Count < 11 && energyThreshold > 0)
        {
            AddPlayersToEleven(localBench, energyThreshold - 10);
        }
    }
    public void SetLineUp(PossibleTeam desiredLineup)
    {
        teamLineup = desiredLineup;

        switch (desiredLineup)
        {
            case PossibleTeam.BestTeam:
                SetBestPlayingEleven();
                break;

            case PossibleTeam.Turnover:
                SetTurnOverEleven();
                break;

            case PossibleTeam.Fit:
                SetFittestEleven();
                break;
        }
    }
    
    //REDO these functions have a lot of redundancy and identical code
    public void SetBestPlayingEleven()
    {
        playingEleven = new List<SO_PlayerData>();
        bench = new List<SO_PlayerData>();

        AddGoalKeeper();

        //sorts other players by skill
        
        List<SO_PlayerData> sortedPlayers = new List<SO_PlayerData>();
        
        sortedPlayers.AddRange(Defense);
        sortedPlayers.AddRange(Midfield);
        sortedPlayers.AddRange(Attack);

        sortedPlayers = sortTeamListBySkill(sortedPlayers);

        AddPlayersToEleven(sortedPlayers);
    }

    public void SetFittestEleven()
    {
    //REDO missing a few checks to avoid having an energic but awful team on the field
        playingEleven = new List<SO_PlayerData>();
        bench = new List<SO_PlayerData>();
        
        //sets goalkeeper
        AddGoalKeeper();

        //sorts other players by skill
        List<SO_PlayerData> sortedPlayers = new List<SO_PlayerData>();

        sortedPlayers.AddRange(Defense);
        sortedPlayers.AddRange(Midfield);
        sortedPlayers.AddRange(Attack);

        sortedPlayers = SortTeamListByEnergy(sortedPlayers);

        AddPlayersToEleven(sortedPlayers);
    }

    public void SetTurnOverEleven(float energyThreshold=80f)
    {
        //Best team but players with energy below [n] are excluded
        playingEleven = new List<SO_PlayerData>();
        bench = new List<SO_PlayerData>();
        //sets goalkeeper
        AddGoalKeeper();

        //sorts other players by skill
        List<SO_PlayerData> sortedPlayers = new List<SO_PlayerData>();
        sortedPlayers.AddRange(Defense);
        sortedPlayers.AddRange(Midfield);
        sortedPlayers.AddRange(Attack);

        sortedPlayers = sortTeamListBySkill(sortedPlayers);

        AddPlayersToEleven(sortedPlayers, energyThreshold);

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

    private List<SO_PlayerData> SortTeamListByEnergy(List<SO_PlayerData> datalist)
    {
        int n = datalist.Count;

        for (int i = 0; i < n - 1; i++)

            for (int j = 0; j < n - i - 1; j++)

                if (datalist[j].playerEnergy > datalist[j + 1].playerEnergy)
                {
                    SO_PlayerData player = datalist[j];
                    datalist[j] = datalist[j + 1];
                    datalist[j + 1] = player;
                }
        datalist.Reverse();
        return datalist;
    }
    #endregion
}


