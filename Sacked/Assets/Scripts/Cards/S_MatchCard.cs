using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class S_MatchCard : S_Card
{
    public Image starsRating;
    public TextMeshProUGUI ranking;
    public override void GenerateCardData(SO_CardData data)
    {
        cardData = data;
        cardDescription.text = cardData.cardDescription;
        cardBackground.color = data.cardColor;
        cardIcon.sprite = data.cardIcon;
        starsRating.fillAmount = (data as SO_MatchOpponent).teamRating * 0.2f;
        ranking.text = S_Ladder.LeagueRank(S_GlobalManager.nextOpponent).ToString()+ "°";
    }
}
