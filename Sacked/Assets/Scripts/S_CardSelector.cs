using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using TMPro;
using UnityEngine;
using System.Linq;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Events;

public class S_CardSelector : MonoBehaviour
{
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


    private void Start()
    {
        //REDO disattivare already picked in metodo piu gradevole
        baseCardsPool.DisableAlreadyPicked();
        matchCardsPool.DisableAlreadyPicked();
        marketCardsPool.DisableAlreadyPicked();
        firstHalfBreakCardsPool.DisableAlreadyPicked();

        S_PlayerMatchSimulator.OnMatchStart.AddListener(ResetMatchCardsAppearancesCounter);
    }

    //REDO avoid errors if cards do not have any formula in it
    public List<SO_CardData> ChooseCardByScore(CardsPool pool=null, float minRequiredScore=0.5f)
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
            float cardScore = card.GetCardScore();
            cardScore=card.NormalizeCardScore(bestScore);
            if (cardScore >= minRequiredScore && cardScore > 0 && card.cardName != S_GlobalManager.deckManagerRef.lastCard.cardData.cardName /*//REDO cringe check*/)
            {
                //Debug.Log("Approved: " + card.cardName);
                approvedCards.Add(card);
            }
        }

        if (approvedCards.Count <= 0)
        {
            SO_CardData errorCard=Resources.Load<SO_CardData>(S_ResDirs.errorCard);
            Debug.LogWarning("Card selector has found 0 valid cards");
            approvedCards.Add(errorCard);
        }



        return approvedCards;
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

    /// <summary>
    ///Tells if an appended card is going to spawn (aka countdown==0), used mostly to avoid changing phase when an appended card should spawn
    /// </summary>
    /// <returns></returns>
    public bool SpawningAppendedCard()
    {
        foreach(SO_CardData.Branch card in currentListToRead)
        {
            if (card.addPosition <= 0)
            {
                Debug.Log("No aspe un secondo");
                return true;
            }
        }
        return false;
    }

    public void ResetMatchCardsAppearancesCounter()
    {
        foreach(SO_CardData c in matchCardsPool.cardsPool)
        {
            c.totalAppearances = 0;
        }
    }
}
