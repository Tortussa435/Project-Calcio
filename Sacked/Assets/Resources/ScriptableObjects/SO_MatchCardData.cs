using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Match Card Data", menuName = "Cards/Match Card")]
public class SO_MatchCardData : SO_CardData
{
    [Header("Scoring")]
    public List<S_MatchCardsScoreFormula> matchScoreCard;
    public override void SetCardScore()
    {
        base.SetCardScore();
        
        if (canAppearMoreThanOnce || !alreadyPicked)
        {
            for (int i=0;i<matchScoreCard.Count;i++)
            {
                cardScore += matchScoreCard[i].CalculateScore();
            }
        }
        cardScoreNotNormalized = cardScore;
        
    }
}
