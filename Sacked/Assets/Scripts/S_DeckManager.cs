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

public class S_DeckManager : MonoBehaviour
{

    [Header("Prefabs References")]
    public GameObject cardPrefab;
    public GameObject matchCardPrefab;

    [Header("Other")]
    public S_Card lastCard;
    public GameObject background;
    public GameObject deck;
    public SO_League selectedLeague;
    public TextMeshProUGUI PhaseText;
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
        
        cardSelector.currentListToRead = new List<SO_CardData.Branch>();
        ChangeCurrentPhase(0,0, CardsPhase.Contract);

        if (selectedTeam == null)
        {   
            
            List<SO_Team> possibleTeams = FindPossibleTeams();
            foreach(SO_Team team in possibleTeams)
            {
                SO_TeamCardData tcd = ScriptableObject.CreateInstance<SO_TeamCardData>();
                tcd.SetTeamData(team);
                GenerateCard(tcd);
            }
            
        }

        //the player already selected a team
        else
        {
            SetBackGroundColor(selectedTeam.teamColor1, selectedTeam.teamColor2);
            GenerateCard(null);
            GenerateCard(null);
        }

        nextOpponent = FindPossibleTeams()[0];
    }

    // Update is called once per frame
    void Update()
    {
        
    }



    public void AddCardToDeck(SO_CardData data, int position = 0)
    {
        SO_CardData.Branch branch;
        branch.branchData = data;
        branch.addPosition = position;
        for (int i = 0; i < cardSelector.currentListToRead.Count; i++)
        {
            if (branch.addPosition == cardSelector.currentListToRead[i].addPosition)
            {
                branch.addPosition += 1; //TODO make iterative until sure it does not coincide with another card in list
            }
        }
        cardSelector.currentListToRead.Add(branch);
        nextPhaseCountdown++;
    }

    public void GenerateCard(SO_CardData cardData=null, GameObject cardFormat = null)
    {
        if (S_GlobalManager.sacked) return; //do not generate cards if sacked
        
        nextPhaseCountdown -= 1;
        if (cardFormat == null)
        {
            Debug.Log("eccolo");
            cardFormat = cardPrefab;
        }
        if(cardData==null) cardData = DecreaseCardsCounter(cardSelector.currentListToRead); //if generate card has no imposed card, carddata is used to see if theres a branch card to add

        lastCard = Instantiate(cardFormat, transform.position+(Vector3)S_GlobalManager.CardsSpawnOffset, Quaternion.identity,deck.transform).GetComponent<S_Card>();
        lastCard.transform.SetAsFirstSibling();
        
        if (cardData == null) lastCard.GenerateCardData(FindNextCard());
        else lastCard.GenerateCardData(cardData);
    }

    public SO_CardData FindNextCard()
    {
        if (nextPhaseCountdown <= 0) //Change phase after n cards in each phase
        {
            Debug.Log("Ora siamo nel " + currentPhase);
            switch (currentPhase)
            {
                case CardsPhase.Contract:
                    break;
                case CardsPhase.Week:
                    ChangeCurrentPhase(matchDuration.min, matchDuration.max, CardsPhase.Match);
                    break;
                case CardsPhase.Market:
                    ChangeCurrentPhase(marketDuration.min, marketDuration.max, CardsPhase.Week);
                    break;
                case CardsPhase.Match:
                    ChangeCurrentPhase(matchDuration.min, matchDuration.max, CardsPhase.Week);
                    break;
            }
        }
        
        SO_CardData[] result;
        result = cardSelector.ChooseCardByScore().ToArray();
        return result[Random.Range(0,result.Length)];

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
            case CardsPhase.Match:
                PhaseText.text = "";//TODO cool graphics for season match
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
        if (sacked) return;
        nextPhaseCountdown = Random.Range(minCardsAmount, maxCardsAmount+1); //max exclusive
        SetPhaseText(newPhase);
        if (newPhase == CardsPhase.Match)
        {
            Destroy(deck.transform.GetChild(0).gameObject);
            GenerateCard(ScriptableObject.CreateInstance<SO_MatchOpponent>(), matchCardPrefab);
        }
        cardSelector.SetCurrentPool(newPhase);
        currentPhase = newPhase;
    }
}
