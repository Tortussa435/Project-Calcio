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
    public List<(SO_TeamTrait.TraitNames,S_FootballEnums.Positivity)> traitsInfo;
    public SO_MatchOpponent()
    {
        cardDescriptions.Add(S_GlobalManager.nextOpponent.teamName);
        cardColor = S_GlobalManager.nextOpponent.teamColor1;
        cardIcon = S_GlobalManager.nextOpponent.teamLogo;
        teamRating = S_GlobalManager.nextOpponent.SkillLevel;
    }


    private void OnEnable()
    {
        previewPool = Resources.Load<CardsPool>(S_ResDirs.matchPreviewPool);
        firsthalfbreakPool = Resources.Load<CardsPool>(S_ResDirs.firstHalfBreakPool);
        
        //S_GlobalManager.nextOpponent.GenerateRandomTraits();
        
        //REDO use cleaner methods
        SO_Team nextTeam = S_GlobalManager.nextOpponent;
        traitsInfo = new List<(SO_TeamTrait.TraitNames, S_FootballEnums.Positivity)>();
        for(int i = 0; i < nextTeam.teamTraits.Count; i++)
        {
           traitsInfo.Add((nextTeam.teamTraits[i].traitName, nextTeam.teamTraits[i].positiveTrait));
        }
            
    }
    public override void leftEffect()
    {
        bothSidesEffect();
    }
    public override void rightEffect()
    {
        bothSidesEffect();
    }

    public void bothSidesEffect()
    {
        SO_CardData previewcard = GenerateMatchPreviewCard();
        //previewcard.onGeneratedEffects.AddListener(() => GenerateMatchPreviewCard());
        S_GlobalManager.deckManagerRef.GenerateCard(null,null,false);
        S_GlobalManager.canEditLineup = false;
    }

    public SO_CardData GenerateMatchPreviewCard()
    {
        Debug.Log("Sto generando!");
        List<SO_CardData> possibleScores = S_GlobalManager.deckManagerRef.cardSelector.ChooseCardByScore(previewPool,0.25f);
        
        SO_CardData previewCard = possibleScores[Random.Range(0, possibleScores.Count)];
        
        previewCard = ScriptableObject.Instantiate(previewCard);
        
        previewCard.decreaseCountDown = false;

        S_GlobalManager.deckManagerRef.AddCardToDeck(previewCard, 0);

        return previewCard;
    }
}
