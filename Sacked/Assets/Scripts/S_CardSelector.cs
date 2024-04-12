using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using TMPro;
using UnityEngine;
using System.Linq;
using Unity.Collections.LowLevel.Unsafe;

public class S_CardSelector : MonoBehaviour
{
    public TextMeshProUGUI debugScoresViewer;
    [Header("Pools")]
    public CardsPool baseCardsPool;
    public CardsPool matchCardsPool;
    public CardsPool marketCardsPool;
    public CardsPool firstHalfBreakCardsPool;
    [SerializeField] private CardsPool currentPool;

    [Header("Card Stacks")]
    public List<SO_CardData.Branch> appendedWeekCards;
    public List<SO_CardData.Branch> appendedMatchCards;
    public List<SO_CardData.Branch> appendedMarketCards;
    public List<SO_CardData.Branch> currentListToRead;
    public List<SO_CardData> ChooseCardByScore(CardsPool pool=null, float minRequiredScore=0.75f)
    {
        if (pool == null) pool = currentPool;

        float bestScore = 0.0f;
        //calculate scores and find best
        foreach(SO_CardData card in pool.cardsPool)
        {    
            card.SetCardScore();
            bestScore = Mathf.Max(bestScore, card.GetCardScore());
        }

        List<SO_CardData> approvedCards = new List<SO_CardData>();
        //normalize scores and find if in range, in case add to list
        foreach(SO_CardData card in pool.cardsPool)
        {
            card.NormalizeCardScore(bestScore);
            if (card.GetCardScore() >= minRequiredScore)
            {
                //Debug.Log("Approved: " + card.cardName);
                approvedCards.Add(card);
            }
        }

#if UNITY_EDITOR
        debugScoresViewer.text = "";
        string text=new string("");
        foreach(SO_CardData card in pool.cardsPool)
        {
            text += "\n" + card.cardName +" Normalized Score: "+card.cardScore+" Score: "+card.cardScoreNotNormalized;
        }
        debugScoresViewer.text = text;
#endif

        if (approvedCards.Count <= 0)
        {
            Debug.Log("no card");
            return null;
        }
        else
        {
            return approvedCards;
        }
    }

    public void SetCurrentPool(S_GlobalManager.CardsPhase phase)
    {
        switch (phase)
        {
            case S_GlobalManager.CardsPhase.Contract:
                currentPool = baseCardsPool;
                currentListToRead = appendedWeekCards;
                break;
            case S_GlobalManager.CardsPhase.Week:
                currentListToRead = appendedWeekCards;
                currentPool = baseCardsPool;
                break;
            case S_GlobalManager.CardsPhase.Market:
                currentListToRead = appendedMarketCards;
                currentPool = marketCardsPool;
                break;
            case S_GlobalManager.CardsPhase.MatchFirstHalf:
                currentListToRead = appendedMatchCards;
                currentPool = matchCardsPool;
                break;
            case S_GlobalManager.CardsPhase.MatchSecondHalf:
                break;
        }
    }
}
