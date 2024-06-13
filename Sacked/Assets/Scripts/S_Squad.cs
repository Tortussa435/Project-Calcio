using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.Events;
using UnityEngine.UI;

public class S_Squad : MonoBehaviour
{

    public enum PossibleTeam
    {
        BestTeam = 0,
        Turnover = 1,
        Fit = 2,
        _343 = 4,
        _433 = 3,
        _532 = 5,
        _424 = 6,
        Violents = 7
    }

    public GameObject showTeamButtonRef;

    public GameObject teamStatsViewer;

    public PossibleTeam teamLineup = PossibleTeam.BestTeam;

    public List<SO_PlayerData> Goalkeepers;
    public List<SO_PlayerData> Defense;
    public List<SO_PlayerData> Midfield;
    public List<SO_PlayerData> Attack;

    public List<SO_PlayerData> playingEleven;
    public List<SO_PlayerData> bench;

    public bool teamListVisible = false;

    public  GameObject teamCardPrefab;
    private GameObject teamCardRef;


    public Dictionary<string, int> Scorers = new Dictionary<string, int>();

    public UnityEvent<bool> OnToggleSquadViewer = new UnityEvent<bool>();

    public SO_PlayerData freeKicksShooter;

    // Start is called before the first frame update
    void Awake()
    {
        S_GlobalManager.squad = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Start()
    {
          
    }
    public void GenerateTeam()
    {
        List<int> SkillLevelDistribution(int skillLevel)
        {
            List<int> skills = new List<int>();
            //19 players
            //remember random.range is max exclusive
            skills.Add(5);

            for (int i = 0; i < 4; i++)
                skills.Add(skillLevel);

            for (int i = 0; i < 4; i++)
                skills.Add(Random.Range(skillLevel - 1, skillLevel + 1));

            for (int i = 0; i < 3; i++)
                skills.Add(skillLevel - 1);

            for (int i = 0; i < 4; i++)
                skills.Add(Random.Range(1,3));

            for (int i = 0; i < 2; i++)
                skills.Add(Random.Range(skillLevel - 2, skillLevel + 3));
            
            skills.Add(3);

            return skills;
        }

        int FindSkillLevel(List<int> skillList)
        {
            int v = skillList[Random.Range(0, skillList.Count)];
            skillList.Remove(v);
            return v;
        }

        int skillLevel = S_GlobalManager.selectedTeam.SkillLevel;
        
        List<int> skills = SkillLevelDistribution(skillLevel);

        //generate 11 starting players + 7 bench players

        //Goalkeepers

        /*
         * (all skill values clamped (0,5)
         * first gk = random[skillLevel-1, skillLevel+1]
         * second gk = random[skillLevel-3, skillLevel-2]
         */
        Goalkeepers.Add(S_PlayersGenerator.GeneratePlayer(SO_PlayerData.PlayerRole.Gk,FindSkillLevel(skills)));
        Goalkeepers.Add(S_PlayersGenerator.GeneratePlayer(SO_PlayerData.PlayerRole.Gk, FindSkillLevel(skills)));

        //Defense
        /*
         * first 3 def = random[skillLevel-2, skillLevel+2
         * 4th def = random[skillLevel-3,skillLevel+1]
         * 5,6 = random[skillLevel-3,skillLevel]
         */

        Defense.Add(S_PlayersGenerator.GeneratePlayer(SO_PlayerData.PlayerRole.Def, FindSkillLevel(skills)));
        Defense.Add(S_PlayersGenerator.GeneratePlayer(SO_PlayerData.PlayerRole.Def, FindSkillLevel(skills)));
        Defense.Add(S_PlayersGenerator.GeneratePlayer(SO_PlayerData.PlayerRole.Def, FindSkillLevel(skills)));

        Defense.Add(S_PlayersGenerator.GeneratePlayer(SO_PlayerData.PlayerRole.Def, FindSkillLevel(skills)));

        Defense.Add(S_PlayersGenerator.GeneratePlayer(SO_PlayerData.PlayerRole.Def, FindSkillLevel(skills)));

        Defense.Add(S_PlayersGenerator.GeneratePlayer(SO_PlayerData.PlayerRole.Def, FindSkillLevel(skills)));

        //Midfield
        /*
         * first 3 mid = random[skillLevel-2,skillLevel+2]
         * 4th mid = random[skillLevel-3,skillLevel+1]
         * 5,6 = random[skillLevel-3,skillLevel]
         */

        Midfield.Add(S_PlayersGenerator.GeneratePlayer(SO_PlayerData.PlayerRole.Mid, FindSkillLevel(skills)));
        Midfield.Add(S_PlayersGenerator.GeneratePlayer(SO_PlayerData.PlayerRole.Mid, FindSkillLevel(skills)));
        Midfield.Add(S_PlayersGenerator.GeneratePlayer(SO_PlayerData.PlayerRole.Mid, FindSkillLevel(skills)));
        
        Midfield.Add(S_PlayersGenerator.GeneratePlayer(SO_PlayerData.PlayerRole.Mid, FindSkillLevel(skills)));
        
        Midfield.Add(S_PlayersGenerator.GeneratePlayer(SO_PlayerData.PlayerRole.Mid, FindSkillLevel(skills)));

        Midfield.Add(S_PlayersGenerator.GeneratePlayer(SO_PlayerData.PlayerRole.Mid, FindSkillLevel(skills)));

        //Attack
        /*
         * first 2 att = random[skillLevel-1, skillLevel+1]
         * last 2 att = random[skillLevel-2,skillLevel]
         */

        Attack.Add(S_PlayersGenerator.GeneratePlayer(SO_PlayerData.PlayerRole.Atk, FindSkillLevel(skills)));
        Attack.Add(S_PlayersGenerator.GeneratePlayer(SO_PlayerData.PlayerRole.Atk, FindSkillLevel(skills)));

        Attack.Add(S_PlayersGenerator.GeneratePlayer(SO_PlayerData.PlayerRole.Atk, FindSkillLevel(skills)));
        Attack.Add(S_PlayersGenerator.GeneratePlayer(SO_PlayerData.PlayerRole.Atk, FindSkillLevel(skills)));

        Attack.Add(S_PlayersGenerator.GeneratePlayer(SO_PlayerData.PlayerRole.Atk, FindSkillLevel(skills)));

        /*
        //makes one random player skillLevel = random[4,5]

        int buffedRole = Random.Range(0, 4);
        switch (buffedRole)
        {

            case 0://Goalkeeper
                Goalkeepers[Random.Range(0, Goalkeepers.Count)].skillLevel = 5;
                break;
            
            case 1://Defense
                Defense[Random.Range(0, Defense.Count)].skillLevel = 5;
                break;
            
            case 2://Midfield
                Midfield[Random.Range(0, Midfield.Count)].skillLevel = 5;
                break;
            
            case 3://Attack
                Attack[Random.Range(0, Attack.Count)].skillLevel = 5;
                break;
        }
        */

        //Italianize 5 players
        List<SO_PlayerData> fullPlayersList = GetAllPlayers();
        for(int i = 0; i < 5; i++)
        {
            int ran = Random.Range(0, fullPlayersList.Count);
            S_PlayersGenerator.ItalianizePlayer(fullPlayersList[ran]);
            fullPlayersList.RemoveAt(ran);
        }


        SetBestPlayingEleven();

        showTeamButtonRef.SetActive(true);

       
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
            totalskill += player.GetPlayerCurrentSkillLevel();
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

    public void ResetSubs()
    {
        //REDO pretty awful code ngl
        foreach (SO_PlayerData player in Goalkeepers) player.subbedOut=false;
        foreach (SO_PlayerData player in Defense) player.subbedOut = false;
        foreach (SO_PlayerData player in Midfield) player.subbedOut = false;
        foreach (SO_PlayerData player in Attack) player.subbedOut = false;
    }

    public void RefillBenchEnergy(float min=40.0f, float max=50.0f)
    {
        foreach(SO_PlayerData player in bench)
        {
            float mult = 1;
            if (player.subbedOut) mult = 0.75f; 
            player.AddEnergy(min*mult, max*mult);
        }
    }

    public void RefillTeamEnergy(float energy)
    {
        foreach (SO_PlayerData player in playingEleven)
        {
            player.AddEnergy(energy,energy);
        }
    }

    public void DecreaseElevenEnergy(float min=-10.0f, float max=-5.0f)
    {
        List<SO_PlayerData> players = S_GlobalManager.squad.playingEleven;
        
        min *= S_PlayerTeamStats.FitnessMultiplier();
        max *= S_PlayerTeamStats.FitnessMultiplier();

        foreach (SO_PlayerData p in players)
        {
            if (p.playerRole == SO_PlayerData.PlayerRole.Gk) continue; //gk does not get tired
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
        //open
        if (teamCardRef == null || forceSpawn)
        {
            OnToggleSquadViewer.Invoke(true);
            SO_CardData data = ScriptableObject.Instantiate(Resources.Load<SO_CardData>(S_ResDirs.teamCard));
            string tacticName = S_GlobalManager.squad.FindNextLineup().ToString();
            tacticName = tacticName.Replace("_", " ");
            data.leftChoice = tacticName;
            teamCardRef = S_GlobalManager.deckManagerRef.GenerateCard(data, teamCardPrefab, false);
            S_GlobalManager.deckManagerRef.SetCardOnTop(teamCardRef);
            data.rightEffects.AddListener(() => teamCardRef = null); //reference must be lost even before than card destruction
            teamStatsViewer.SetActive(true);
            S_PlayerTeamStats.CalcSquadDef();
            S_PlayerTeamStats.CalcSquadAtk();
        }
        //close
        else
        {
            teamStatsViewer.SetActive(false);
            OnToggleSquadViewer.Invoke(false);
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

    public SO_PlayerData DecidePlayerToExpel(int gkchance = 0 , int defchance = 35, int midchance = 70, int atkchance = 100)
    {
        int seed = Random.Range(0, 100);
        SO_PlayerData.PlayerRole roleToExpel;
        switch (seed)
        {
            case int n when n < gkchance:
                roleToExpel = SO_PlayerData.PlayerRole.Gk;
                break;

            case int n when n < defchance:
                roleToExpel = SO_PlayerData.PlayerRole.Def;
                break;

            case int n when n < midchance:
                roleToExpel = SO_PlayerData.PlayerRole.Mid;
                break;

            case int n when n < atkchance:
                roleToExpel = SO_PlayerData.PlayerRole.Atk;
                break;
            
            default:
                roleToExpel = SO_PlayerData.PlayerRole.Atk;
                break;
        }

        List<SO_PlayerData> expellables = new List<SO_PlayerData>();
        foreach(SO_PlayerData p in playingEleven)
        {
            if(p.playerRole==roleToExpel) expellables.Add(p);
        }

        return expellables[Random.Range(0, expellables.Count)];
    }

    /// <summary>
    /// If team has less than 11 players, adjusts team with players from bench
    /// </summary>
    public void FillTeamHoles()
    {
        for(int i = playingEleven.Count-1; i >= 0; i--)
        {
            if (!playingEleven[i].CanPlay())
            {
                bench.Add(playingEleven[i]);
                playingEleven.RemoveAt(i);
            }
        }

        if (playingEleven.Count >= 11) return;

        int gk = 0;
        int def = 0;
        int mid = 0;
        int atk = 0;
        //finds amount of gk, def, mid and atk in team
        foreach(SO_PlayerData p in playingEleven)
        {
            switch (p.playerRole)
            {
                case SO_PlayerData.PlayerRole.Gk:
                    gk++;
                    break;
                case SO_PlayerData.PlayerRole.Def:
                    def++;
                    break;
                case SO_PlayerData.PlayerRole.Mid:
                    mid++;
                    break;
                case SO_PlayerData.PlayerRole.Atk:
                    atk++;
                    break;
            }
        }

        int j = 0;
        while (playingEleven.Count < 11)
        {
            AddPlayers(j);
            j++;
        }

        void AddPlayers(int extraslot=0)
        {
            bench = SortTeamListBySkill(bench);
            for (int i=bench.Count-1; i>=0; i--)
            {
                if (playingEleven.Count == 11) return;

                if (bench[i].CanPlay())
                {
                    switch (bench[i].playerRole)
                    {
                        case SO_PlayerData.PlayerRole.Gk:
                            if (gk > 0) continue;
                            playingEleven.Add(bench[i]);
                            bench.RemoveAt(i);
                            break;

                        case SO_PlayerData.PlayerRole.Def:
                            if (def >= 4+extraslot) continue;
                            playingEleven.Add(bench[i]);
                            bench.RemoveAt(i);
                            break;

                        case SO_PlayerData.PlayerRole.Mid:
                            if (mid >= 4+extraslot) continue;
                            playingEleven.Add(bench[i]);
                            bench.RemoveAt(i);
                            break;

                        case SO_PlayerData.PlayerRole.Atk:
                            if (atk >= 3+extraslot) continue;
                            playingEleven.Add(bench[i]);
                            bench.RemoveAt(i);
                            break;
                    }
                }
            }
        }
    }

    
    public SO_PlayerData GetRandomPlayerRef() => playingEleven[Random.Range(0, playingEleven.Count)];
    public bool IsTeamTired(float tiredAverage = 70f)
    {
        float energy = 0;
        foreach(SO_PlayerData player in playingEleven)
        {
            energy += player.playerEnergy;
        }
        return energy / 11 < tiredAverage;
    }
    public float GetTeamAverageEnergy()
    {
        float energy = 0;
        foreach (SO_PlayerData player in playingEleven)
        {
            energy += player.playerEnergy;
        }
        return energy / 11.0f;
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

            case PossibleTeam._343:
                SetTeamByModule(3, 4, 3);
                break;

            case PossibleTeam._433:
                SetTeamByModule(4, 3, 3);
                break;

            case PossibleTeam._532:
                SetTeamByModule(5, 3, 2);
                break;

            case PossibleTeam._424:
                SetTeamByModule(4, 2, 4);
                break;
            case PossibleTeam.Violents:
                SetTeamByTrait(SO_PlayerTrait.PlayerTraitNames.Hot_Head);
                break;
        }
        S_PlayerTeamStats.CalcSquadDef(false);
        S_PlayerTeamStats.CalcSquadAtk(false);
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

        sortedPlayers = SortTeamListBySkill(sortedPlayers);

        AddPlayersToEleven(sortedPlayers);

    }

    public void SetFittestEleven()
    {
    //REDO missing a few checks to avoid having an energic but awful team on the field
        playingEleven = new List<SO_PlayerData>();
        bench = new List<SO_PlayerData>();
        


        //sorts other players by skill
        List<SO_PlayerData> sortedPlayers = new List<SO_PlayerData>();

        sortedPlayers.AddRange(Defense);
        sortedPlayers.AddRange(Midfield);
        sortedPlayers.AddRange(Attack);

        sortedPlayers = SortTeamListByEnergy(sortedPlayers);

        List<SO_PlayerData> selectedPlayers = new List<SO_PlayerData>();

        sortedPlayers.Reverse();

        int def = 0, mid = 0, atk = 0;

        RecursiveFitPlayers();

        playingEleven = new List<SO_PlayerData>(selectedPlayers);
        bench = new List<SO_PlayerData>(sortedPlayers);

        //sets goalkeeper
        AddGoalKeeper();


        void RecursiveFitPlayers(int minSkill=0)
        {
            for(int i = sortedPlayers.Count - 1; i >= 0; i--)
            {
                if (selectedPlayers.Count == 10)
                {
                    return;
                }
                if (sortedPlayers[i].skillLevel >= S_GlobalManager.selectedTeam.SkillLevel - minSkill)
                {
                    switch (sortedPlayers[i].playerRole)
                    {
                        case SO_PlayerData.PlayerRole.Atk:
                            if (atk >= 3) continue;
                            atk++;
                            break;
                        case SO_PlayerData.PlayerRole.Mid:
                            if (mid >= 4) continue;
                            mid++;
                            break;
                        case SO_PlayerData.PlayerRole.Def:
                            if (def >= 4) continue;
                            def++;
                            break;
                    }
                    selectedPlayers.Add(sortedPlayers[i]);
                    sortedPlayers.RemoveAt(i);
                }
            }

            if (selectedPlayers.Count < 10) RecursiveFitPlayers(minSkill + 1);
            else return;
            
        }
    }

    public void SetTurnOverEleven(float energyThreshold = 80f)
    {
        /* OLD
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

        sortedPlayers = SortTeamListBySkill(sortedPlayers);

        while (playingEleven.Count < 11)
        {
            AddPlayersToEleven(sortedPlayers, energyThreshold);
            energyThreshold += 5;
            if (energyThreshold > 100) break;
        }
        */

        /*OLD 2
        //starts creating turnover team starting from best team


        if (previousList == null)
        {
            SetBestPlayingEleven();
        }
        

        List<SO_PlayerData> turnoverEleven = new List<SO_PlayerData>();
        if (previousList != null) turnoverEleven = previousList;

        //Add goalkeeper
        if (turnoverEleven.Count == 0)
        {
            foreach(SO_PlayerData p in playingEleven)
            {
                if (p.playerRole == SO_PlayerData.PlayerRole.Gk) turnoverEleven.Add(p);
            }
            if (turnoverEleven.Count == 0)
            {
                for(int i = bench.Count - 1; i >= 0; i--)
                {
                    if (bench[i].playerRole == SO_PlayerData.PlayerRole.Gk)
                    {
                        turnoverEleven.Add(bench[i]);
                        bench.RemoveAt(i);

                    }
                }
            }
        }

        for(int i = playingEleven.Count-1; i >= 0; i--)
        {
            if (playingEleven[i].playerEnergy > energyThreshold && playingEleven[i].playerRole!=SO_PlayerData.PlayerRole.Gk)
            {
                if (turnoverEleven.Count == 11) break;
                turnoverEleven.Add(playingEleven[i]);
                switch (playingEleven[i].playerRole)
                {
                    case SO_PlayerData.PlayerRole.Def:
                        if (def > 3) continue;
                        def++;
                        break;
                    case SO_PlayerData.PlayerRole.Mid:
                        if (mid > 3) continue;
                        mid++;
                        break;
                    case SO_PlayerData.PlayerRole.Atk:
                        if (atk > 3) continue;
                        atk++;
                        break;
                }

                
            }
            else
            {
                bench.Add(playingEleven[i]);
                playingEleven.RemoveAt(i);
            }
        }

        for(int i = bench.Count-1; i >= 0; i--)
        {
            if (turnoverEleven.Count == 11) break;
            
            if(bench[i].CanPlay() && bench[i].playerEnergy > energyThreshold && bench[i].skillLevel >= S_GlobalManager.selectedTeam.SkillLevel-skillThreshold && bench[i].playerRole != SO_PlayerData.PlayerRole.Gk)
            {
                switch (bench[i].playerRole)
                {
                    case SO_PlayerData.PlayerRole.Def:
                        if (def > 4) continue;
                        def++;
                        break;
                    case SO_PlayerData.PlayerRole.Mid:
                        if (mid > 4) continue;
                        mid++;
                        break;
                    case SO_PlayerData.PlayerRole.Atk:
                        if (atk > 3) continue;
                        atk++;
                        break;
                }
                turnoverEleven.Add(bench[i]);
                bench.RemoveAt(i);
            }
        }


        if (turnoverEleven.Count == 11) playingEleven = new List<SO_PlayerData>(turnoverEleven);
        else SetTurnOverEleven(turnoverEleven, energyThreshold - 10, skillThreshold-1, def, mid, atk);
        */

        List<SO_PlayerData> p11Clone = new List<SO_PlayerData>(playingEleven);
        List<SO_PlayerData> benchClone = new List<SO_PlayerData>(bench);

        List<SO_PlayerData> turnoverEleven = new List<SO_PlayerData>();
        List<SO_PlayerData> turnoverBench = new List<SO_PlayerData>();

        bool gkAdded = false;
        for(int i=p11Clone.Count-1;i>=0;i--)
        {
            if (p11Clone[i].playerRole == SO_PlayerData.PlayerRole.Gk)
            {
                turnoverEleven.Add(p11Clone[i]);
                p11Clone.RemoveAt(i);
                gkAdded = true;
            }
        }


        for(int i = benchClone.Count - 1; i >= 0; i--)
        {
            if (benchClone[i].playerRole == SO_PlayerData.PlayerRole.Gk)
            {
                if (!gkAdded)
                {
                    turnoverEleven.Add(benchClone[i]);
                    benchClone.RemoveAt(i);
                    gkAdded = true;
                }
            }
        }
        

        int def = 0;
        int mid = 0;
        int atk = 0;

        RecursiveTurnover(turnoverEleven);

        playingEleven = new List<SO_PlayerData>(turnoverEleven);
        bench = new List<SO_PlayerData>(turnoverBench);


        void RecursiveTurnover(List<SO_PlayerData> previousList, float energyThreshold = 80f, int skillThreshold = 1)
        {
            for(int i=p11Clone.Count-1;i>=0;i--)
            {

                if (p11Clone[i].CanPlay() && p11Clone[i].playerEnergy >= energyThreshold && p11Clone[i].playerRole != SO_PlayerData.PlayerRole.Gk && turnoverEleven.Count<11)
                {
                    switch (p11Clone[i].playerRole)
                    {
                        case SO_PlayerData.PlayerRole.Def:
                            if (def > 3)
                            {
                                continue;
                            }
                            def++;
                            break;
                        case SO_PlayerData.PlayerRole.Mid:
                            if (mid > 3)
                            {
                                continue;
                            }
                            mid++;
                            break;
                        case SO_PlayerData.PlayerRole.Atk:
                            if (atk > 2)
                            {
                                continue;
                            }
                            atk++;
                            break;
                    }
                    turnoverEleven.Add(p11Clone[i]);
                    p11Clone.RemoveAt(i);
                }
            }

            for(int i = benchClone.Count-1; i >= 0; i--)
            {
                if(
                    turnoverEleven.Count < 11
                    && benchClone[i].skillLevel >= S_GlobalManager.selectedTeam.SkillLevel - skillThreshold 
                    && benchClone[i].CanPlay() 
                    && benchClone[i].playerEnergy >= energyThreshold 
                    && benchClone[i].playerRole != SO_PlayerData.PlayerRole.Gk
                )
                {
                    switch (benchClone[i].playerRole)
                    {
                        case SO_PlayerData.PlayerRole.Def:
                            if (def > 3)
                            {
                                continue;
                            }
                            def++;
                            break;
                        case SO_PlayerData.PlayerRole.Mid:
                            if (mid > 3)
                            {
                                continue;
                            }
                            mid++;
                            break;
                        case SO_PlayerData.PlayerRole.Atk:
                            if (atk > 2)
                            {
                                continue;
                            }
                            atk++;
                            break;
                    }
                    turnoverEleven.Add(benchClone[i]);
                    benchClone.RemoveAt(i);
                }
            }
            if (turnoverEleven.Count == 11)
            {
                turnoverBench = p11Clone.Concat(benchClone).ToList(); //add left players to bench
                return;
            }
            else RecursiveTurnover(turnoverEleven, energyThreshold - 10, skillThreshold + 1);
        }
    }

    void SetTeamByTrait(SO_PlayerTrait.PlayerTraitNames trait)
    {
        SetBestPlayingEleven();
        List<SO_PlayerData> traitPlayers = GetPlayersWithTrait(trait,false);
        if (traitPlayers.Count <= 0)
        {
            Debug.LogWarning("Non ci sono giocatori trattanti");
            return;
        }
        bool hasGk = false;
        for(int i = traitPlayers.Count - 1; i >= 0; i--)
        {
            if (traitPlayers[i].playerRole == SO_PlayerData.PlayerRole.Gk)
            {
                if (hasGk) traitPlayers.RemoveAt(i);
                else hasGk = true;
            }
        }
        for(int i = traitPlayers.Count - 1; i >= 0; i--)
        {
            if (playingEleven.Contains(traitPlayers[i]))
                traitPlayers.RemoveAt(i);
        }

        for(int i = traitPlayers.Count - 1; i >= 0; i--)
        {
            bool found = false;
            for(int j = playingEleven.Count - 1; j >= 0; j--)
            {
                if(!found)
                    if (traitPlayers[i].playerRole == playingEleven[j].playerRole)
                    {
                        Swap(traitPlayers[i], playingEleven[j]);
                        found = true;
                    }
            }
        }

        void Swap(SO_PlayerData inP, SO_PlayerData outP)
        {
            playingEleven.Add(inP);
            playingEleven.Remove(outP);
            bench.Add(outP);
            bench.Remove(inP);

        }
        
    }

    public void SetTeamByModule(int def, int mid, int atk)
    {
        List<SO_PlayerData> pgk = new List<SO_PlayerData>(SortTeamListBySkill(Goalkeepers));
        List<SO_PlayerData> pdef = new List<SO_PlayerData>(SortTeamListBySkill(Defense));
        List<SO_PlayerData> pmid = new List<SO_PlayerData>(SortTeamListBySkill(Midfield));
        List<SO_PlayerData> patk = new List<SO_PlayerData>(SortTeamListBySkill(Attack));

        playingEleven = new List<SO_PlayerData>();
        bench = new List<SO_PlayerData>();


        int gks = 0;
        foreach(SO_PlayerData p in pgk)
        {
            if (gks >= 1 || !p.CanPlay()) bench.Add(p);
            else
            {
                playingEleven.Add(p);
                gks++;
            } 
        }
        
        int defs = 0;
        foreach(SO_PlayerData p in pdef)
        {
            if (defs >= def || !p.CanPlay()) bench.Add(p);
            else
            {
                playingEleven.Add(p);
                defs++;
            }
        }

        int mids = 0;
        foreach(SO_PlayerData p in pmid)
        {
            if (mids >= mid || !p.CanPlay()) bench.Add(p);
            else
            {
                playingEleven.Add(p);
                mids++;
            }
        }

        int atks = 0;
        foreach (SO_PlayerData p in patk)
        {
            if (atks >= atk || !p.CanPlay()) bench.Add(p);
            else
            {
                playingEleven.Add(p);
                atks++;
            }
        }
    }

    #region SORTING
    public List<SO_PlayerData> SortTeamListBySkill(List<SO_PlayerData> datalist)
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
    public List<SO_PlayerData> SortTeamListByEnergy(List<SO_PlayerData> datalist)
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
    public List<SO_PlayerData> SortPlayersByRole(List<SO_PlayerData> datalist)
    {
        int n = datalist.Count;

        for (int i = 0; i < n - 1; i++)

            for (int j = 0; j < n - i - 1; j++)

                if ((int)datalist[j].playerRole > (int)datalist[j + 1].playerRole)
                {
                    SO_PlayerData player = datalist[j];
                    datalist[j] = datalist[j + 1];
                    datalist[j + 1] = player;
                }
        datalist.Reverse();
        return datalist;
    }
    #endregion

    #endregion
    public List<SO_PlayerData> GetAllPlayers() => Goalkeepers.Concat(Defense.Concat(Midfield.Concat(Attack))).ToList();
    
    public SO_PlayerData GetRandomPlayerRefByRole(SO_PlayerData.PlayerRole role)
    {
        foreach(SO_PlayerData p in playingEleven)
        {
            if (p.playerRole == role) return p;
        }
        return null;
    }
    public void AddGolScorer(string playerName)
    {
        if (Scorers.ContainsKey(playerName))
        {
            Scorers[playerName] += 1;
        }
        else Scorers.Add(playerName, 1);
    }

    public SO_PlayerData GetCaptain()
    {
        SO_PlayerData bestPlayer = ScriptableObject.CreateInstance<SO_PlayerData>();
        foreach(SO_PlayerData p in playingEleven)
        {
            if (!p.CanPlay()) continue;

            if(bestPlayer.skillLevel<p.skillLevel) bestPlayer = p;

            //prefer offensive player
            if (bestPlayer.skillLevel == p.skillLevel)
            {
                if ((int)bestPlayer.playerRole < (int)p.playerRole) bestPlayer = p;
            }

            //prefer player with positive trait
            else if (bestPlayer.skillLevel == p.skillLevel)
            {
                if (bestPlayer.playerTraits[0].positiveTrait != S_FootballEnums.Positivity.Positive)
                {
                    if (p.playerTraits[0].positiveTrait == S_FootballEnums.Positivity.Positive)
                    {
                        bestPlayer = p;
                    }
                }
            }
        }

        if (bestPlayer.skillLevel == 0) return null;
        return bestPlayer;
    }

    public SO_PlayerData GetFreeKicksShooter()
    {
        List<SO_PlayerData> eligibles = GetPlayersWithTrait(SO_PlayerTrait.PlayerTraitNames.Old_Wise_Man);
        if (eligibles.Count <= 0)
        {
            foreach(SO_PlayerData p in playingEleven)
            {
                if (
                    p.playerTraits[0].traitName == SO_PlayerTrait.PlayerTraitNames.Longshot
                    || p.playerTraits[0].traitName == SO_PlayerTrait.PlayerTraitNames.Lucky
                   ) eligibles.Add(p);
            }

            if (eligibles.Count <= 0)
            {
                foreach(SO_PlayerData p in playingEleven)
                {
                    if (
                        p.playerRole == SO_PlayerData.PlayerRole.Gk
                        || p.playerRole == SO_PlayerData.PlayerRole.Def
                        || p.playerTraits[0].traitName == SO_PlayerTrait.PlayerTraitNames.Bad_Long_Shot
                        || p.playerTraits[0].traitName == SO_PlayerTrait.PlayerTraitNames.Tall
                    ) continue;

                    eligibles.Add(p);
                }
            }
        }
        SO_PlayerData fks = eligibles[Random.Range(0, eligibles.Count)];
        freeKicksShooter = fks;
        return fks;
    }

}


