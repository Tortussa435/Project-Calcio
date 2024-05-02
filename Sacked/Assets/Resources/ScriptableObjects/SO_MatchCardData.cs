using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Events;
using static S_FootballEnums;

[CreateAssetMenu(fileName = "New Match Card Data", menuName = "Cards/Match Card")]
public class SO_MatchCardData : SO_CardData
{
    [Header("Scoring")]
    public List<S_MatchCardsScoreFormula> matchScoreCard;
    public float chanceOfAppearance = 0.0f;

    public List<CardDropChance> matchCardDropChance = new List<CardDropChance>();

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

        chanceOfAppearance = 0;
        foreach(CardDropChance drop in matchCardDropChance)
        {
            chanceOfAppearance += drop.FindChance();
        }
    }

    #region MATCH EVENTS
    public void PlayerYellowCard()
    {
        SO_PlayerData player;

        if (Random.Range(0, 100) < 60) //[n]% of the times the player that gets a yellow card is a hothead
        {
            List<SO_PlayerData> hotheads = S_GlobalManager.squad.GetHotHeadPlayers();
            if (hotheads.Count > 0) player = hotheads[Random.Range(0, hotheads.Count)];

            else  player=S_GlobalManager.squad.playingEleven[Random.Range(0, S_GlobalManager.squad.playingEleven.Count)];
        }

        else player = S_GlobalManager.squad.playingEleven[Random.Range(0, S_GlobalManager.squad.playingEleven.Count)];

        if (S_PlayerMatchSimulator.YellowCards.Contains(player)) //if player already has a yellow card, it gets expelled
        {
            SO_CardData redcard = ScriptableObject.CreateInstance<SO_CardData>();
            redcard.cardDescriptions.Add( player.playerName + " è stato espulso per somma di gialli!" );
            redcard.cardColor = Color.red;
            redcard.decreaseCountDown = false;

            S_PlayerMatchSimulator.ExpelPlayerFootballer(player);
            
            S_GlobalManager.deckManagerRef.AddCardToDeck(redcard, 0);
        }

        else
        {
            S_PlayerMatchSimulator.YellowCards.Add(player);
        }

        cardDescriptions.Clear();
        cardDescriptions.Add(player.playerName + " è stato ammonito");

        //REDO per qualche motivo il nome del giocatore espulso nel menu di debug non coincide con quello che appare sulla carta ammonito/espulso, indagare

    }

    public void PlayerRedCard()
    {
        SO_PlayerData player;

        if (Random.Range(0, 100) < 60) //[n]% of the times the player that gets a yellow card is a hothead
        {
            List<SO_PlayerData> hotheads = S_GlobalManager.squad.GetHotHeadPlayers();
            if (hotheads.Count > 0) player = hotheads[Random.Range(0, hotheads.Count)];

            else player = S_GlobalManager.squad.playingEleven[Random.Range(0, S_GlobalManager.squad.playingEleven.Count)];
        }

        else player = S_GlobalManager.squad.playingEleven[Random.Range(0, S_GlobalManager.squad.playingEleven.Count)];

        S_PlayerMatchSimulator.YellowCards.Remove(player);

        S_PlayerMatchSimulator.ExpelPlayerFootballer(player);

        cardDescriptions.Clear();
        cardDescriptions.Add(player.playerName + " è stato espulso!");

        //REDO per qualche motivo il nome del giocatore espulso nel menu di debug non coincide con quello che appare sulla carta ammonito/espulso, indagare

    }
    #endregion
}

[System.Serializable]
public class CardDropChance
{
    public MatchRule rule;
    public ScoreDirection direction;
    public float weight=1.0f;

    public float FindChance()
    {
        float score = 0;

        switch (rule)
        {
            //REDO most values are >1, they should stay in a 0-1 range as much as possible
            case MatchRule.Aggressivity:
                score = (S_PlayerMatchSimulator.matchAggressivity.home + S_PlayerMatchSimulator.matchAggressivity.away)/6; //6 should be the max aggressivity of a match 
                break;
            case MatchRule.SkillDifference:
                score = S_GlobalManager.selectedTeam.SkillLevel - S_PlayerMatchSimulator.GetOpponentTeam().SkillLevel;
                break;
            case MatchRule.YellowCards:
                score = S_PlayerMatchSimulator.YellowCards.Count;
                break;
            case MatchRule.RedCards:
                score = S_PlayerMatchSimulator.RedCards.Count;
                break;
            case MatchRule.Constant:
                score = 1;
                break;
        }

        switch (direction)
        {
            default:
                break;
            
            case ScoreDirection.InverseLinear:
                score = 1 - score;
                break;
           
            case ScoreDirection.Round:
                score=Mathf.Round(score);
                break;
            
            case ScoreDirection.InverseRound:
                score=1-Mathf.Round(score);
                break;
        }

        score *= weight;

        return score;
    }
}