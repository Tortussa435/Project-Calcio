using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Match Opponent", menuName = "Cards/Match/Match Opponent")]
public class SO_MatchOpponent : SO_CardData
{
    public int teamRating;
    public CardsPool previewPool;
    public CardsPool firsthalfbreakPool;
    [Tooltip("\nItem 1: trait name\nItem 2: positive trait?")]
    public List<(string,bool)> traitsInfo;
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
        
        S_GlobalManager.nextOpponent.GenerateRandomTraits();
        
        //REDO use cleaner methods
        SO_Team nextTeam = S_GlobalManager.nextOpponent;
        traitsInfo = new List<(string, bool)>();
        for(int i = 0; i < nextTeam.teamTraits.Count; i++)
        {
           traitsInfo.Add((nextTeam.teamTraits[i].traitName, nextTeam.teamTraits[i].positiveTrait));
        }
            
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

        SO_CardData firstPreviewCard = possibleScores[Random.Range(0, possibleScores.Count)];
        firstPreviewCard.decreaseCountDown = false;
        
        possibleScores.Remove(firstPreviewCard);
        
        SO_CardData secondPreviewCard = possibleScores[Random.Range(0, possibleScores.Count)];
        secondPreviewCard.decreaseCountDown = false;


        S_GlobalManager.deckManagerRef.AddCardToDeck(firstPreviewCard, 0);
        S_GlobalManager.deckManagerRef.AddCardToDeck(secondPreviewCard, 0);
        
        
        
        
        S_GlobalManager.deckManagerRef.GenerateCard(null,null,false);
    }

}
