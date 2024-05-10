using System.Collections;
using System.Collections.Generic;
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
        cardScore = 0;

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
    public void RandomYellowCard()
    {
        int playerChance = 1;
        int opponentChance = 1;

        playerChance += S_PlayerMatchSimulator.IsPlayerHomeTeam() ? (int)S_PlayerMatchSimulator.matchAggressivity.home : (int)S_PlayerMatchSimulator.matchAggressivity.away;

        opponentChance += S_PlayerMatchSimulator.IsPlayerHomeTeam() ? (int)S_PlayerMatchSimulator.matchAggressivity.away : (int)S_PlayerMatchSimulator.matchAggressivity.home;

        int seed = Random.Range(0, playerChance + opponentChance + 1);

        if (seed > playerChance) PlayerYellowCard();
        else OpponentYellowCard();
    
    }

    public void RandomRedCard()
    {
        int playerChance = 1;
        int opponentChance = 1;

        playerChance += S_PlayerMatchSimulator.IsPlayerHomeTeam() ? (int)S_PlayerMatchSimulator.matchAggressivity.home : (int)S_PlayerMatchSimulator.matchAggressivity.away;

        opponentChance += S_PlayerMatchSimulator.IsPlayerHomeTeam() ? (int)S_PlayerMatchSimulator.matchAggressivity.away : (int)S_PlayerMatchSimulator.matchAggressivity.home;

        int seed = Random.Range(0, playerChance + opponentChance + 1);

        if (seed > playerChance) PlayerRedCard();
        else OpponentRedCard();
    }

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
            //REDO expel description with variable string
            redcard.cardDescriptions.Add( player.playerName+" ("+S_GlobalManager.selectedTeam.shortName+")" + " è stato espulso per somma di gialli!" );
            redcard.cardColor = Color.red;
            redcard.decreaseCountDown = false;
            player.expelled = 2;
            S_PlayerMatchSimulator.ExpelPlayerFootballer(player);
            
            S_GlobalManager.deckManagerRef.AddCardToDeck(redcard, 0);
        }

        else
        {
            S_PlayerMatchSimulator.YellowCards.Add(player);
        }

        string description = cardDescriptions[Random.Range(0, cardDescriptions.Count)];
        description = description.Replace("{Expelled}", player.playerName + " ("+S_GlobalManager.selectedTeam.shortName+")");
        ownerCard.GetComponent<S_Card>().cardDescription.text = description;

        //ownerCard.GetComponent<S_Card>().GenerateCardData(this);


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

        string description = S_GlobalManager.ReplaceVariablesInString(cardDescriptions[Random.Range(0, cardDescriptions.Count)]);
        description = description.Replace("{Expelled}", player.playerName + " (" + S_GlobalManager.selectedTeam.shortName + ")" );
        ownerCard.GetComponent<S_Card>().cardDescription.text = description;
        
        player.expelled = 2;

    }

    public void OpponentYellowCard()
    {
        SO_PlayerData player=ScriptableObject.CreateInstance<SO_PlayerData>();

        player.playerName = S_PlayerMatchSimulator.RandomlyGetNewOrExistingOpponentPlayer();

        bool expelled = false;
        foreach(SO_PlayerData p in S_PlayerMatchSimulator.opponentYellowCards)
        {
            if(p.playerName == player.playerName)
            {
                //Append red card if double yellow
                SO_CardData redcard = ScriptableObject.CreateInstance<SO_CardData>();
                //REDO expel description with variable string
                redcard.cardDescriptions.Add(player.playerName+" ("+S_PlayerMatchSimulator.GetOpponentTeam().shortName+")" + " è stato espulso per somma di gialli!");
                redcard.cardColor = Color.red;
                redcard.decreaseCountDown = false;
                S_GlobalManager.deckManagerRef.AddCardToDeck(redcard, 0);


                S_PlayerMatchSimulator.opponentYellowCards.Remove(p);
                S_PlayerMatchSimulator.opponentRedCards.Add(p);

                //REDO ricalcolare skill level avversario in seguito ad expulsione
                
                S_PlayerMatchSimulator.opponentTeamNames.Remove(p.playerName);
                expelled = true;
                break;
            }
        }
        if (!expelled) S_PlayerMatchSimulator.opponentYellowCards.Add(player);

        string description = S_GlobalManager.ReplaceVariablesInString(cardDescriptions[Random.Range(0, cardDescriptions.Count)]);
        description = description.Replace("{Expelled}", player.playerName + " (" + S_PlayerMatchSimulator.GetOpponentTeam().shortName + ")");
        ownerCard.GetComponent<S_Card>().cardDescription.text = description;
    }

    public void OpponentRedCard()
    {
        SO_PlayerData player = ScriptableObject.CreateInstance<SO_PlayerData>();

        player.playerName = S_PlayerMatchSimulator.RandomlyGetNewOrExistingOpponentPlayer();

        S_PlayerMatchSimulator.opponentYellowCards.Remove(player);
        S_PlayerMatchSimulator.opponentRedCards.Add(player);
        S_PlayerMatchSimulator.opponentTeamNames.Remove(player.playerName);


        string description = S_GlobalManager.ReplaceVariablesInString(cardDescriptions[Random.Range(0, cardDescriptions.Count)]);
        description = description.Replace("{Expelled}", player.playerName + " (" + S_PlayerMatchSimulator.GetOpponentTeam().shortName + ")");
        ownerCard.GetComponent<S_Card>().cardDescription.text = description;
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
                score = (S_PlayerMatchSimulator.matchAggressivity.home + S_PlayerMatchSimulator.matchAggressivity.away); //6 should be the max aggressivity of a match 
                break;
            case MatchRule.SkillDifference:
                score = (float) (S_GlobalManager.selectedTeam.SkillLevel - S_PlayerMatchSimulator.GetOpponentTeam().SkillLevel+5) / 10;
                break;
            case MatchRule.YellowCards:
                score = (float)S_PlayerMatchSimulator.YellowCards.Count;
                break;
            case MatchRule.RedCards:
                score = (float)S_PlayerMatchSimulator.RedCards.Count; //REDO funny number
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