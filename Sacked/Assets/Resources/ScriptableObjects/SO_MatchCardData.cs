using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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
