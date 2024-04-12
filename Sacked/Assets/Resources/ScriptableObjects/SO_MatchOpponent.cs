using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Match Opponent", menuName = "Cards/Match/Match Opponent")]
public class SO_MatchOpponent : SO_CardData
{
    public int teamRating;
    public CardsPool previewPool;
    public CardsPool firsthalfbreakPool;
    public SO_MatchOpponent()
    {
        cardDescription = S_GlobalManager.nextOpponent.teamName;
        cardColor = S_GlobalManager.nextOpponent.teamColor1;
        cardIcon = S_GlobalManager.nextOpponent.teamLogo;
        teamRating = S_GlobalManager.nextOpponent.SkillLevel;
    }
    private void OnEnable()
    {
        previewPool = Resources.Load<CardsPool>("ScriptableObjects/CardsPools/MatchPreviewCardsPool");
        firsthalfbreakPool = Resources.Load<CardsPool>("ScriptableObjects/CardsPools/MatchSpeechPool");
    }
    public override void leftEffect()
    {
        GenerateMatchPreviewCard();
    }
    public override void rightEffect()
    {
        GenerateMatchPreviewCard();
    }
    public void GenerateMatchPreviewCard()
    {
        List<SO_CardData> possibleScores = S_GlobalManager.deckManagerRef.cardSelector.ChooseCardByScore(previewPool,0.25f);
        S_GlobalManager.deckManagerRef.AddCardToDeck(possibleScores[Random.Range(0,possibleScores.Count)], 0);
        S_GlobalManager.deckManagerRef.AddCardToDeck(possibleScores[Random.Range(0, possibleScores.Count)], 1);
        
        
        
        
        S_GlobalManager.deckManagerRef.GenerateCard();
    }

}
