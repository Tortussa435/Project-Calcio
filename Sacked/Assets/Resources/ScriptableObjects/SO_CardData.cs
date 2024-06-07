using Mono.Cecil;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;
using NaughtyAttributes;

[CreateAssetMenu(fileName = "New Card Data", menuName = "Cards/Card")]
public class SO_CardData : ScriptableObject
{
    [HideInInspector] public GameObject ownerCard;

    public string cardName;
    public Color cardColor=Color.white;
    public List<string> cardDescriptions=new List<string>();
    public string leftChoice;
    public string rightChoice;
    public bool canAppearMoreThanOnce=false;
    [ShowIf("canAppearMoreThanOnce")]
    [ReadOnly]
    public int totalAppearances = 0;
    public bool decreaseCountDown = true;


    public string desiredCardPrefabDirectory=new string("");

    [System.Serializable]
    public struct ChangeValues
    {
        public float addedMoney;
        public float addedTeam;
        public float addedPresident;
        public float addedSupporters;
    };
    public ChangeValues leftValues;
    public ChangeValues rightValues;

    public UnityEvent leftEffects=new UnityEvent();
    public UnityEvent rightEffects=new UnityEvent();
    public UnityEvent onGeneratedEffects=new UnityEvent();

    [ReadOnly]
    public bool alreadyPicked=false;

    public Sprite cardIcon;

    [System.Serializable]
    public struct Branch
    {
        public SO_CardData branchData;
        public int addPosition;
        public float triggerChance;
        public List<object> extraData;
        public bool removeOnPhaseChange;
        
    }

    public Branch leftBranchCard;
    public Branch rightBranchCard;

    [Header("Scoring")]
    public List<S_CardsScoreFormula> scoreCard;

    [ReadOnly]
    public float cardScore;
    [ReadOnly]
    public float cardScoreNotNormalized;

    /// <summary>
    /// data passed from another card, useful for specific events/actions
    /// </summary>
    public List<object> passedExtraData = new List<object>();

    virtual public void leftEffect()
    {
        //Debug.Log("left effect");
        if (leftBranchCard.branchData != null)
        {
            if(Random.Range(0,100) < leftBranchCard.triggerChance)
                S_GlobalManager.deckManagerRef.AddCardToDeck(leftBranchCard.branchData, leftBranchCard.addPosition,null,leftBranchCard.extraData);
        }
        S_GlobalManager.SetMoney(leftValues.addedMoney);
        S_GlobalManager.SetPresident(leftValues.addedPresident);
        S_GlobalManager.SetSupporters(leftValues.addedSupporters);
        S_GlobalManager.SetTeam(leftValues.addedTeam);
        leftEffects.Invoke();
        leftEffects.RemoveAllListeners();

        if (!S_GlobalManager.DefeatCheck()) S_GlobalManager.deckManagerRef.GenerateCard(null,null,decreaseCountDown);
        
    }
    virtual public void rightEffect()
    {
        if (rightBranchCard.branchData != null)
        {
            if (Random.Range(0, 100) < rightBranchCard.triggerChance)
                S_GlobalManager.deckManagerRef.AddCardToDeck(rightBranchCard.branchData, rightBranchCard.addPosition,null,rightBranchCard.extraData);
        }

        S_GlobalManager.SetMoney(rightValues.addedMoney);
        S_GlobalManager.SetPresident(rightValues.addedPresident);
        S_GlobalManager.SetSupporters(rightValues.addedSupporters);
        S_GlobalManager.SetTeam(rightValues.addedTeam);
        rightEffects.Invoke();
        rightEffects.RemoveAllListeners();

        if (!S_GlobalManager.DefeatCheck()) S_GlobalManager.deckManagerRef.GenerateCard(null,null,decreaseCountDown); //when the player reaches a defeat, card generation is handled by S_ValueManager

        //Debug.Log("right effect");
    }
    
    public virtual void SetCardScore()
    {
        cardScore = 0;
        if(canAppearMoreThanOnce || !alreadyPicked)
        {
            foreach(S_CardsScoreFormula score in scoreCard) cardScore += score.CalculateScore(this);
        }
        cardScoreNotNormalized = cardScore;
    }
    public float GetCardScore()
    {
        return cardScore;
    }

    public float NormalizeCardScore(float max)
    {
        cardScore /= max;
        return cardScore;
    }

    public void SetCardAlreadyPicked(bool picked=true) => alreadyPicked = (picked && !canAppearMoreThanOnce);

    [ContextMenu("Disable Already Picked")]
    private void OnDestroy()
    {
        alreadyPicked = false;
    }
    

    #region Card Events
    public void TestEvent()
    {
        Debug.Log("test");
    }

    public void SetPlayerTeamTactic(string tactic)
    {
        S_GlobalManager.selectedTeam.teamTactics = Resources.Load<SO_Tactics>(S_ResDirs.teamTacticsDir + tactic);
        Debug.Log(S_GlobalManager.selectedTeam.teamTactics);
        S_PlayerMatchSimulator.UpdateTacticsEffectiveness();
    }

    public void GeneratePossibleTactics()
    {
        List<SO_Tactics.Tactic> possibleTactics = new List<SO_Tactics.Tactic> 
        {
            SO_Tactics.Tactic.BallPossession,
            SO_Tactics.Tactic.Pressing,
            SO_Tactics.Tactic.Counterattack,
            SO_Tactics.Tactic.Catenaccio

        };

        SO_Tactics.Tactic left = possibleTactics[Random.Range(0, possibleTactics.Count)];
        possibleTactics.Remove(left);
        SO_Tactics.Tactic right = possibleTactics[Random.Range(0, possibleTactics.Count)];

        leftEffects = new UnityEvent();
        leftEffects.AddListener(()=>SetPlayerTeamTactic(left.ToString()));

        rightEffects = new UnityEvent();
        rightEffects.AddListener(() => SetPlayerTeamTactic(right.ToString()));

        leftChoice = left.ToString();
        rightChoice = right.ToString();

        ownerCard.GetComponent<S_Card>().RefreshCardData(this);

    }

    public void BorrowBike()
    {
        //REDO very ugly
        SO_PlayerData p = S_GlobalManager.squad.playingEleven[Random.Range(0, S_GlobalManager.squad.playingEleven.Count)];

        

        ReplaceCardDescription("{Renter}", p.playerName);
        string vehicle = S_FootballEnums.vehicles[Random.Range(0, S_FootballEnums.vehicles.Count)];
        ReplaceCardDescription("{Vehicle}", vehicle);

        leftBranchCard.extraData=new List<object> { p, vehicle };
    }

    public void BreakPlayerLeg()
    {
        int injLen = Random.Range(2, 4);
        (passedExtraData[0] as SO_PlayerData).injuried = injLen;
        ReplaceCardDescription("{Renter}", (passedExtraData[0] as SO_PlayerData).playerName);
        ReplaceCardDescription("{Vehicle}", passedExtraData[1] as string);
        ReplaceCardDescription("{InjDur}", injLen.ToString());
    }

    protected void ReplaceCardDescription(string placeholder, string output)
    {
        S_Card card;
        if(!ownerCard.TryGetComponent<S_Card>(out card)) return;
        string s = card.cardDescription.text;
        s = s.Replace(placeholder, output);
        card.cardDescription.text = s;
    }

    #region TRAINING

    public void ProposeBoosts()
    {
        S_Card card = ownerCard.GetComponent<S_Card>();
        (UnityAction left, UnityAction right, string nameA, string nameB, string statLeft, string statRight) events = S_PlayerTeamStats.FindTrainingBoosts();
        leftEffects.AddListener(events.left);
        rightEffects.AddListener(events.right);

        card.leftChoice.text = events.nameA;
        card.rightChoice.text = events.nameB;

        S_TrainingPentagonPreviewer prev = card.GetComponent<S_TrainingPentagonPreviewer>();
        prev.left = events.statLeft;
        prev.right = events.statRight;
    }
    
    public void RefillTeamEnergy(float energy) => S_GlobalManager.squad.RefillTeamEnergy(energy);
    
    public void SetTeamLineup(string lineup)
    {
        S_Squad.PossibleTeam lup = (S_Squad.PossibleTeam)System.Enum.Parse(typeof(S_Squad.PossibleTeam), lineup, true);
        S_GlobalManager.squad.SetLineUp(lup);
    }

    public void ZombifyPlayer()
    {
        SO_PlayerData randomPlayer = S_GlobalManager.squad.playingEleven[Random.Range(0, 11)];
        ReplaceCardDescription("{Zombie}", randomPlayer.playerName);
        randomPlayer.injuried = 5;
    }

    public void MakeLanguageLearner()
    {
        List<SO_PlayerData> learners = S_GlobalManager.squad.GetPlayersWithTrait(SO_PlayerTrait.PlayerTraitNames.Does_Not_Know_The_Language,false);
        SO_PlayerData langLearner = learners[Random.Range(0, learners.Count)];
        ReplaceCardDescription("{Lang}", langLearner.playerName);
        leftBranchCard.extraData=new List<object> { langLearner };
    }
    public void PlayerLearnLanguage()
    {
        SO_PlayerData langLearner = passedExtraData[0] as SO_PlayerData;
        ReplaceCardDescription("{Lang}", langLearner.playerName);
        langLearner.playerTraits.RemoveAt(0);
        langLearner.playerTraits.Add(S_PlayerTraitsList.AssignPlayerTrait(langLearner.skillLevel, new List<SO_PlayerTrait.PlayerTraitNames> { SO_PlayerTrait.PlayerTraitNames.Does_Not_Know_The_Language }));
    }
    #endregion
    #region END MATCH
    public void GenerateEndMatchData()
    {
        cardDescriptions.Clear();
        cardDescriptions.Add(GetCalendarResults());
        decreaseCountDown = false;

        //REDO che schifo
        leftEffects.AddListener(() => S_GlobalManager.deckManagerRef.MatchScoreText.gameObject.SetActive(false));
        rightEffects.AddListener(() => S_GlobalManager.deckManagerRef.MatchScoreText.gameObject.SetActive(false));
        leftEffects.AddListener(() => S_GlobalManager.deckManagerRef.PhaseText.gameObject.SetActive(true));
        rightEffects.AddListener(() => S_GlobalManager.deckManagerRef.PhaseText.gameObject.SetActive(true));

        S_PlayerMatchSimulator.matchMinute = 90+Random.Range(0,6);
        S_PlayerMatchSimulator.UpdateMatchTextData();
    
    }

    private string GetCalendarResults()
    {
        string result = new string("");
        result = ("Match Over!\n");
        result= string.Concat(result, GenerateMatchResultText(S_PlayerMatchSimulator.match.homeTeam.teamName, S_PlayerMatchSimulator.matchScore.home, S_PlayerMatchSimulator.matchScore.away, S_PlayerMatchSimulator.match.awayTeam.teamName));
        result = result + "\n";
        List<(S_Calendar.Match match, S_FastMatchSimulator.Score score)> matches = S_FastMatchSimulator.weekResults;
        
        for(int i = 0; i < matches.Count; i++)
        {
            string homeName = matches[i].match.homeTeam.teamName;
            string awayName = matches[i].match.awayTeam.teamName;
            int homeScore = matches[i].score.home;
            int awayScore = matches[i].score.away;
            result = string.Concat(result, GenerateMatchResultText(homeName, homeScore, awayScore, awayName));
        }
        
        return result;
    }

    private string GenerateMatchResultText(string homeTeam, int homeScore, int awayScore, string awayTeam)
    {
        string result = new string("");
        result = "\n"+homeTeam + " " + homeScore + " - " + awayScore + " " + awayTeam;
        return result;
    }
    #endregion

    #endregion

    #region Team Lineup Events
    public void FindNextLineupDescription()
    {
        //on card generated, updates the lineup based on the decided tactic, then sets the left choice to the next tactic
        S_GlobalManager.squad.SetLineUp(S_GlobalManager.squad.FindNextLineup());  
        leftChoice = S_GlobalManager.squad.FindNextLineup().ToString().Replace("_",""); //REDO pretty weird
    }
    #endregion
}
