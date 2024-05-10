using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro; 
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;
using static S_GlobalManager;
using System.Globalization;

public class S_DeckManager : MonoBehaviour
{

    [Header("Debug")]
    public bool DebugImmortal = false;

    [Header("Prefabs References")]
    public GameObject cardPrefab;
    public GameObject matchCardPrefab;

    [Header("Other")]
    public S_Card lastCard;
    public GameObject background;
    public GameObject deck;
    public SO_League selectedLeague;

    public TextMeshProUGUI PhaseText;
    public TextMeshProUGUI MatchScoreText;
    
    public S_CardSelector cardSelector;
    public int nextPhaseCountdown;

    [Header("Phases Durations")]
    public IntRange weekDuration;
    public IntRange matchDuration;
    public IntRange marketDuration;


    // Start is called before the first frame update
    void Start()
    {
        deckManagerRef = this;

        selectedLeague = ScriptableObject.Instantiate(selectedLeague);
        selectedLeague.GenerateTeamInstances(); //makes instances of the teams scriptable objects to avoid editing the source assets during game time

        cardSelector.currentListToRead = new List<SO_CardData.Branch>();
        ChangeCurrentPhase(0,0, CardsPhase.Contract);
        
        S_Calendar.GenerateCalendar();

        if (selectedTeam == null)
        {   
            
            List<SO_Team> possibleTeams = FindPossibleTeams();
            foreach(SO_Team team in possibleTeams)
            {
                SO_TeamCardData tcd = ScriptableObject.CreateInstance<SO_TeamCardData>();
                tcd.SetTeamData(team);
                tcd.decreaseCountDown = false;
                GenerateCard(tcd,null,false);
            }
            
        }

        //the player already selected a team
        else
        {
            SetBackGroundColor(selectedTeam.teamColor1, selectedTeam.teamColor2);
            GenerateCard(null,null,false);
            
        }
        /*
        for (int i = 0; i < 1; i++)
        {
            S_MatchSimulator.SimulateWholeTournament();
            Debug.Log(S_Ladder.QuickSortLadder(S_Ladder.leagueLadder, 0, S_Ladder.leagueLadder.Count - 1)[0].team.teamName);
            S_Ladder.ClearLadder();
        }
        */

    }

    // Update is called once per frame
    void Update()
    {
        
    }



    public void AddCardToDeck(SO_CardData data, int position = 0, List<SO_CardData.Branch> listToAppend=null)
    {
        SO_CardData.Branch branch;
        branch.branchData = data;
        branch.addPosition = position;
        List<SO_CardData.Branch> listToManage = listToAppend;
        if (listToManage == null) listToManage = cardSelector.currentListToRead;

        if (position == 0)
        {
            for(int i = 0; i < listToManage.Count; i++)
            {
                if (listToManage[i].addPosition == 0)
                {
                    SO_CardData.Branch handle=listToManage[i];
                    handle.addPosition += 1;
                }
            }
        }

        //checks if there's a free spot for the card in that specific position until it finds a spot
        // >using do while unironically
        bool occupied;
        do
        {
            occupied = false;
            for (int i = 0; i < listToManage.Count; i++)
            {
                if (branch.addPosition == listToManage[i].addPosition)
                {
                    branch.addPosition += 1;
                    occupied = true;
                }
            }
        } while (occupied);


        listToManage.Add(branch);
    }

    public void GenerateCard(SO_CardData cardData=null, GameObject cardFormat = null,bool decreaseCountdown=true)
    {
        if (sacked && !deckManagerRef.DebugImmortal) return; //do not generate cards if sacked

        if (decreaseCountdown) nextPhaseCountdown--;
        
        if (nextPhaseCountdown < 0)
        {
            switch (currentPhase)
            {
                case CardsPhase.Contract:
                    ChangeCurrentPhase(weekDuration.min, weekDuration.max, CardsPhase.Week);
                    break;
                
                case CardsPhase.Week:
                    
                    S_PlayerMatchSimulator.StartMatch();

                    ChangeCurrentPhase(matchDuration.min, matchDuration.max, CardsPhase.MatchFirstHalf);

                    break;
                
                case CardsPhase.Market:
                    ChangeCurrentPhase(marketDuration.min, marketDuration.max, CardsPhase.Week);
                    break;
                
                case CardsPhase.MatchFirstHalf:    
                    ChangeCurrentPhase(matchDuration.min, matchDuration.max, CardsPhase.MatchSecondHalf);
                    break;
                
                case CardsPhase.MatchSecondHalf:

                    //generates end game card before erasing data about the match                    
                    SO_CardData endMatchCard = ScriptableObject.CreateInstance<SO_CardData>();

                    //REDO make better system for generating the end game card
                    //endMatchCard.onGeneratedEffects.AddListener(()=>endMatchCard.GenerateEndMatchData());
                    endMatchCard.GenerateEndMatchData();

                    //ends the match
                    S_PlayerMatchSimulator.EndMatch();

                    MatchScoreText.gameObject.SetActive(false);
                    PhaseText.gameObject.SetActive(true);
                    
                    ChangeCurrentPhase(matchDuration.min, matchDuration.max, CardsPhase.Week);
                    
                    //destroys the first week card and replaces it with the endgame card
                    Destroy(deck.transform.GetChild(0).gameObject);
                    
                    GenerateCard(endMatchCard, null, false);

                    break;
            }
            return;
        }

        if (cardFormat == null)
        {
            //Debug.Log("eccolo");
            cardFormat = cardPrefab;
        }
        
        //if generate card has no imposed card and the card counter is decreasing, carddata is used to see if theres a branch card to add
        if(cardData==null) cardData = DecreaseCardsCounter(cardSelector.currentListToRead); 


        if (cardData == null)
        {
            cardData = FindNextCard();
            
            if (cardData.desiredCardPrefabDirectory != "")
            {
                cardFormat = Resources.Load<GameObject>(cardData.desiredCardPrefabDirectory);
            }
        }
        

        lastCard = Instantiate(cardFormat, transform.position+(Vector3)CardsSpawnOffset, Quaternion.identity,deck.transform).GetComponent<S_Card>();
        
        lastCard.transform.SetAsFirstSibling();

        lastCard.GenerateCardData(cardData);
    }

    public SO_CardData FindNextCard()
    {

        //event if during match
        if (currentPhase == CardsPhase.MatchFirstHalf || currentPhase == CardsPhase.MatchSecondHalf)
        {
            SO_CardData matchCard = S_PlayerMatchSimulator.SimulateMatchSegment(45/matchDuration.max , 45/matchDuration.min);
            return matchCard;
        }

        else
        {
            SO_CardData[] result;
            result = cardSelector.ChooseCardByScore().ToArray();
            return result[Random.Range(0,result.Length)];
        }
    }

    public void SetBackGroundColor(Color color1, Color color2)
    {
        background.transform.GetChild(0).GetComponent<UnityEngine.UI.Image>().color = color1;
        background.transform.GetChild(1).GetComponent<UnityEngine.UI.Image>().color = color2;
    }

    
    public List<SO_Team> FindPossibleTeams()
    {
        // REDO - Do properly when the reputation system is actually implemented
        
        List<SO_Team> finalteams = new List<SO_Team>();
        finalteams = selectedLeague.teamlist.ToList();

        //removes teams randomly until only 3 remain
        while (finalteams.Count > 3)
        {
            int num = Random.Range(0, finalteams.Count);
            finalteams.RemoveAt(num);
        }

        return finalteams;
    }
    
    public void SetPhaseText(CardsPhase phase)
    {
        switch (phase)
        {
            case CardsPhase.Week:
                PhaseText.text = "Week"; //TODO write week number
                break;
            case CardsPhase.MatchFirstHalf:
                PhaseText.text = "";//TODO cool graphics for season match
                break;
            case CardsPhase.MatchSecondHalf:
                PhaseText.text = "";
                break;
            case CardsPhase.Market:
                PhaseText.text = "Transfer Market";
                break;
        }
    }

    public SO_CardData DecreaseCardsCounter(List<SO_CardData.Branch> branch)
    {
        for (int i = 0; i < branch.Count; i++)
        {
            SO_CardData.Branch a = branch[i];
            a.addPosition = Mathf.Clamp(a.addPosition-1,0,100);
            if (a.addPosition == 0)
            {
                branch.RemoveAt(i);
                return a.branchData;
            }
            branch.RemoveAt(i);
            branch.Insert(i, a);
        }
        return null;
    }

    public void ChangeCurrentPhase(int minCardsAmount, int maxCardsAmount, CardsPhase newPhase)
    {
        if (sacked && !DebugImmortal) return;
        nextPhaseCountdown = Random.Range(minCardsAmount, maxCardsAmount+1); //max exclusive
        SetPhaseText(newPhase);
        
        cardSelector.SetCurrentPool(newPhase);
        CardsPhase previousPhase = currentPhase;
        currentPhase = newPhase;

        switch (newPhase)
        {
            case CardsPhase.Week:
                GenerateCard();
                break;

            case CardsPhase.MatchFirstHalf:
                Destroy(deck.transform.GetChild(0).gameObject);
                GenerateCard(ScriptableObject.CreateInstance<SO_MatchOpponent>(), matchCardPrefab);
                break;
            
            case CardsPhase.MatchSecondHalf:

                List<SO_CardData> possibleSpeech = (cardSelector.ChooseCardByScore(cardSelector.firstHalfBreakCardsPool , 0.5f));
                SO_CardData speech = possibleSpeech[Random.Range(0, possibleSpeech.Count)];
                speech.decreaseCountDown = false;
                GenerateCard(speech);
                S_PlayerMatchSimulator.matchMinute = 45;
                S_PlayerMatchSimulator.UpdateMatchTextData();
                break;
        }

        

    }
}
