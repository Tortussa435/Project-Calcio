using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class S_MatchCard : S_Card
{
    public Image starsRating;
    public TextMeshProUGUI ranking;
    public List<TextMeshProUGUI> traits;
    public override void GenerateCardData(SO_CardData data)
    {
        cardData = data;
        cardDescription.text = cardData.cardDescription;
        cardBackground.color = data.cardColor;
        cardIcon.sprite = data.cardIcon;
        starsRating.fillAmount = (data as SO_MatchOpponent).teamRating * 0.2f;
        ranking.text = S_Ladder.LeagueRank(S_GlobalManager.nextOpponent).ToString()+ "°";

        //REDO as loop
        if (traits.Count < 1) return;
        traits[0].SetText((data as SO_MatchOpponent).traitsInfo[0].Item1);
        traits[0].color = (data as SO_MatchOpponent).traitsInfo[0].Item2 ? Color.green : Color.red;
        
        if (traits.Count < 2) return;
        traits[1].SetText((data as SO_MatchOpponent).traitsInfo[1].Item1);
        traits[1].color = (data as SO_MatchOpponent).traitsInfo[1].Item2 ? Color.green : Color.red;

        if (traits.Count < 3) return;
        traits[2].SetText((data as SO_MatchOpponent).traitsInfo[2].Item1);
        traits[2].color = (data as SO_MatchOpponent).traitsInfo[2].Item2 ? Color.green : Color.red;

    }
}
