using NaughtyAttributes;
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
    [ReadOnly]
    public float chanceOfAppearance = 0.0f;

    [Header("Drop Chance Score")]
    public float totalScoreMultiplier = 1f;
    public List<CardDropChance> matchCardDropChance = new List<CardDropChance>();

    [SerializeField] private string formulaDescriptionInfo;
    public override void SetCardScore()
    {

        cardScore = 0;

        base.SetCardScore();

        if (canAppearMoreThanOnce || !alreadyPicked)
        {
            for (int i=0;i<matchScoreCard.Count;i++)
            {
                cardScore += matchScoreCard[i].CalculateScore(this);
            }
        }
        cardScoreNotNormalized = cardScore;

        chanceOfAppearance = 0;

        foreach(CardDropChance drop in matchCardDropChance)
        {
            chanceOfAppearance += drop.FindChance();
        }

        chanceOfAppearance = Mathf.Max(0, chanceOfAppearance*totalScoreMultiplier);
    }

    #region MATCH EVENTS
    #region INJURIES
    public void RandomInjury()
    {
        int playerChance = 1;
        int opponentChance = 1;

        playerChance += S_PlayerMatchSimulator.IsPlayerHomeTeam() ? (int)S_PlayerMatchSimulator.injuryChance.home : (int)S_PlayerMatchSimulator.injuryChance.away;

        opponentChance += S_PlayerMatchSimulator.IsPlayerHomeTeam() ? (int)S_PlayerMatchSimulator.injuryChance.away : (int)S_PlayerMatchSimulator.injuryChance.home;

        int seed = Random.Range(0, playerChance + opponentChance + 1); //opponent has slightly higher chance of injury by default


        if (seed > playerChance)
        {
            if (S_PlayerMatchSimulator.IsPlayerHomeTeam())
            {
                S_PlayerMatchSimulator.injuries.home++;
            }
            else S_PlayerMatchSimulator.injuries.away++;

            PlayerInjury();
        }
        else
        {
            if (S_PlayerMatchSimulator.IsPlayerHomeTeam())
            {
                S_PlayerMatchSimulator.injuries.away++;
            }
            else S_PlayerMatchSimulator.injuries.home++;
            
            OpponentInjury();
        }
    }
    public void PlayerInjury()
    {
        SO_PlayerData player;

        if (Random.Range(0, 100) < 60) //[n]% of the times the player that gets an injury is a glass player
        {
            List<SO_PlayerData> glass = S_GlobalManager.squad.GetPlayersWithTrait(SO_PlayerTrait.PlayerTraitNames.Glass);
            if (glass.Count > 0) player = glass[Random.Range(0, glass.Count)];

            else player = S_GlobalManager.squad.playingEleven[Random.Range(0, S_GlobalManager.squad.playingEleven.Count)];
        }

        else player = S_GlobalManager.squad.playingEleven[Random.Range(0, S_GlobalManager.squad.playingEleven.Count)];

        //Injuries player for random amount of matches
        int injurySeed = Random.Range(0, 100);
        int injuryLength = 0;
        switch (injurySeed)
        {
            //the first day of injury is removed at the end of the match, therefore the count starts at 2
            case int n when n < 50:
                injuryLength = 2;
                break;

            case int n when n < 75:
                injuryLength = 3;
                break;

            case int n when n < 90:
                injuryLength = 4;
                break;

            case int n when n < 99:
                injuryLength = 5;
                break;

            default:
                injuryLength = Random.Range(7, 11);
                break;
        }
        player.injuried = injuryLength;
        

        string description = cardDescriptions[Random.Range(0, cardDescriptions.Count)];
        description = description.Replace("{Injuried}", player.playerName + " (" + S_GlobalManager.selectedTeam.shortName + ")");
        description = description.Replace("{InjLen}", injuryLength.ToString());

        ownerCard.GetComponent<S_Card>().cardDescription.text = description;

        //ownerCard.GetComponent<S_Card>().GenerateCardData(this);

    }
    public void OpponentInjury()
    {
        SO_PlayerData player = ScriptableObject.CreateInstance<SO_PlayerData>();

        player.playerName = S_PlayerMatchSimulator.RandomlyGetNewOrExistingOpponentPlayer();


        S_PlayerMatchSimulator.opponentTeamNames.Remove(player.playerName);

        int injuryLength = Random.Range(1, 12);

        string description = S_GlobalManager.ReplaceVariablesInString(cardDescriptions[Random.Range(0, cardDescriptions.Count)]);
        description = description.Replace("{Injuried}", player.playerName + " (" + S_PlayerMatchSimulator.GetOpponentTeam().shortName + ")");
        description = description.Replace("{InjLen}", injuryLength.ToString());

        ownerCard.GetComponent<S_Card>().cardDescription.text = description;

        
        SO_CardData substitution;
        substitution = ScriptableObject.Instantiate(Resources.Load<SO_CardData>(S_ResDirs.opponentSub));
        substitution.decreaseCountDown = false;
        
        //tells the substitution card what player is going to be replaced
        for(int i=0;i<substitution.cardDescriptions.Count;i++)
        {
            substitution.cardDescriptions[i] = substitution.cardDescriptions[i].Replace("{OppPlayer}", player.playerName);
        }

        S_GlobalManager.deckManagerRef.AddCardToDeck(substitution);

        S_PlayerMatchSimulator.UpdateOpponentMatchSkillLevel();
    }
    #endregion

    #region CARDS
    public void RandomYellowCard()
    {
        int playerChance = 1;
        int opponentChance = 1;

        playerChance += S_PlayerMatchSimulator.IsPlayerHomeTeam() ? (int)S_PlayerMatchSimulator.matchAggressivity.home : (int)S_PlayerMatchSimulator.matchAggressivity.away;

        opponentChance += S_PlayerMatchSimulator.IsPlayerHomeTeam() ? (int)S_PlayerMatchSimulator.matchAggressivity.away : (int)S_PlayerMatchSimulator.matchAggressivity.home;

        int seed = Random.Range(0, playerChance + opponentChance + 1);

        if (seed > playerChance)
        {
            PlayerYellowCard();
        }

        else
        {
            OpponentYellowCard();
        }
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
            List<SO_PlayerData> hotheads = S_GlobalManager.squad.GetPlayersWithTrait(SO_PlayerTrait.PlayerTraitNames.Hot_Head);
            if (hotheads.Count > 0) player = hotheads[Random.Range(0, hotheads.Count)];

            else  player=S_GlobalManager.squad.playingEleven[Random.Range(0, S_GlobalManager.squad.playingEleven.Count)];
        }

        else player = S_GlobalManager.squad.playingEleven[Random.Range(0, S_GlobalManager.squad.playingEleven.Count)];

        bool expelled = false;

        if (S_PlayerMatchSimulator.YellowCards.Contains(player)) //if player already has a yellow card, it gets expelled
        {
            expelled = true;
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

        int penaltyChance = expelled ? 25 : 5;
        
        //penalty
        if (Random.Range(0, 100) < penaltyChance)
        {
            GivePenaltyFromCard(true);
        }

        //ownerCard.GetComponent<S_Card>().GenerateCardData(this);


    }

    public void PlayerRedCard()
    {
        SO_PlayerData player;

        if (Random.Range(0, 100) < 60) //[n]% of the times the player that gets a yellow card is a hothead
        {
            List<SO_PlayerData> hotheads = S_GlobalManager.squad.GetPlayersWithTrait(SO_PlayerTrait.PlayerTraitNames.Hot_Head);
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

        //penalty
        if (Random.Range(0, 100) < 25)
        {
            GivePenaltyFromCard(true);
        }

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

                S_PlayerMatchSimulator.OnRedCard.Invoke(S_PlayerMatchSimulator.IsOpponentHomeTeam());


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

        float penaltyChance = expelled ? 15 : 5;
        if (Random.Range(0, 100) < penaltyChance)
        {
            GivePenaltyFromCard(false);
        }

    }

    public void OpponentRedCard()
    {
        SO_PlayerData player = ScriptableObject.CreateInstance<SO_PlayerData>();
        

        player.playerName = S_PlayerMatchSimulator.RandomlyGetNewOrExistingOpponentPlayer();

        S_PlayerMatchSimulator.opponentYellowCards.Remove(player);
        S_PlayerMatchSimulator.opponentRedCards.Add(player);
        S_PlayerMatchSimulator.opponentTeamNames.Remove(player.playerName);

        S_PlayerMatchSimulator.OnRedCard.Invoke(S_PlayerMatchSimulator.IsOpponentHomeTeam());

        string description = S_GlobalManager.ReplaceVariablesInString(cardDescriptions[Random.Range(0, cardDescriptions.Count)]);
        description = description.Replace("{Expelled}", player.playerName + " (" + S_PlayerMatchSimulator.GetOpponentTeam().shortName + ")");
        ownerCard.GetComponent<S_Card>().cardDescription.text = description;

        if (Random.Range(0, 100) < 25)
        {
            GivePenaltyFromCard(false);
        }
    }

    private void GivePenaltyFromCard(bool player)
    {
        SO_CardData penalty = ScriptableObject.Instantiate(Resources.Load<SO_CardData>(S_ResDirs.penaltyCard));

        penalty.onGeneratedEffects.RemoveAllListeners();

        if (player)
            penalty.onGeneratedEffects.AddListener(GeneratePlayerPenalty);
        else
            penalty.onGeneratedEffects.AddListener(GenerateOpponentPenalty);

        S_GlobalManager.deckManagerRef.AddCardToDeck(penalty);
    }
    #endregion

    #region SUBS
    public void OpponentSubstitution()
    {
        for(int i = 0; i < cardDescriptions.Count; i++)
        {
            string newName = S_PlayerMatchSimulator.RandomlyGetNewOrExistingOpponentPlayer();
            cardDescriptions[i] = cardDescriptions[i].Replace("{Sub}", newName);
        }
        ownerCard.GetComponent<S_Card>().RefreshCardData(this);

        if (S_PlayerMatchSimulator.IsOpponentHomeTeam())
        {
            if (S_PlayerMatchSimulator.substitutions.home == 3)
            {
                float f = Random.Range(0, 100);
                if (f < 0.005f)
                {
                    //sconfitta a tavolino REDO
                }
            }
            else S_PlayerMatchSimulator.substitutions.home++;
        }
        else
        {
            if (S_PlayerMatchSimulator.substitutions.away == 3)
            {
                float f = Random.Range(0, 100);
                if (f < 0.005f)
                {
                    //sconfitta a tavolino REDO
                }
            }
            S_PlayerMatchSimulator.substitutions.away++;
        }
        S_PlayerMatchSimulator.OnSubstitution.Invoke(S_PlayerMatchSimulator.IsOpponentHomeTeam());
    }
    

    public void ProposePlayerSubstitution()
    {

    }
    public void ApplyPlayerSubstitution()
    {

    }
    #endregion

    #region PENALTIES
    public void GenerateRandomPenalty()
    {
        if(Random.Range(0, 100) < 50)
        {
            GeneratePlayerPenalty();
        }

        else GenerateOpponentPenalty();
    }
    public void GeneratePlayerPenalty()
    {
        Debug.Log("RIGORE RIGORE RIGORE");
        //REDO approfondire calcolo rigori
        if (Random.Range(0, 100) < S_Chances.PENALTYGOALCHANCE)
        {
            SO_CardData card = S_PlayerMatchSimulator.GenerateGolCard(true,false);
            S_GlobalManager.deckManagerRef.AddCardToDeck(card,0,null,null,true);
            (card as SO_GoalCardData).goalDescription = " Gol! freddissimo dal dischetto!";
            
            Branch empty;
            empty.addPosition = 0;
            empty.branchData = null;
            empty.extraData = null;
            empty.removeOnPhaseChange = false;
            empty.triggerChance = 0;

            leftBranchCard = empty;
            rightBranchCard = empty;
        }

        foreach (string s in cardDescriptions)
        {
            s.Replace("{PenTeam}", S_GlobalManager.selectedTeam.teamName);
        }

    }
    public void GenerateOpponentPenalty()
    {
        Debug.Log("RIGORE RIGORE RIGORE 2");
        //REDO approfondire calcolo rigori
        if (Random.Range(0, 100) < S_Chances.PENALTYGOALCHANCE)
        {
            Branch branch;
            branch.addPosition = 0;
            branch.triggerChance = 100;
            branch.branchData = S_PlayerMatchSimulator.GenerateGolCard(true, false);
            branch.extraData = null;
            branch.removeOnPhaseChange = true;

            (branch.branchData as SO_GoalCardData).goalDescription = " Gol! freddissimo dal dischetto!";

            leftBranchCard = branch;

            rightBranchCard = branch;
        }

        foreach (string s in cardDescriptions)
        {
            s.Replace("{PenTeam}", S_PlayerMatchSimulator.GetOpponentTeam().teamName);
        }
    }
    #endregion

    #endregion
}

[System.Serializable]
public class CardDropChance
{
    public MatchRule rule;
    public ScoreDirection direction;
    public float weight=1.0f;
    
    private bool needsCompareFloat() { return direction == ScoreDirection.LowerThan || direction == ScoreDirection.HigherThan || direction == ScoreDirection.Equal; }
    
    [ShowIf("needsCompareFloat")]
    [AllowNesting]
    public float compareFloat = 0;

    [ShowIf("direction",ScoreDirection.CustomCurve)]
    [AllowNesting]
    public AnimationCurve customCurve;
    public float FindChance()
    {
        float score = 0;

        switch (rule)
        {
            //REDO most values are >1, they should stay in a 0-1 range as much as possible
            case MatchRule.Aggressivity:
                score = (S_PlayerMatchSimulator.matchAggressivity.home + S_PlayerMatchSimulator.matchAggressivity.away);
                break;
            case MatchRule.SkillDifference:
                score = (float) (S_GlobalManager.selectedTeam.SkillLevel - S_PlayerMatchSimulator.GetOpponentTeam().SkillLevel+5) / 10;
                break;
            case MatchRule.YellowCards:
                score = (float)S_PlayerMatchSimulator.YellowCards.Count + S_PlayerMatchSimulator.opponentYellowCards.Count;
                break;
            case MatchRule.RedCards:
                score = (float)S_PlayerMatchSimulator.RedCards.Count + S_PlayerMatchSimulator.opponentRedCards.Count;
                break;
            case MatchRule.Constant:
                score = 1;
                break;
            case MatchRule.Derby:
                score = S_PlayerMatchSimulator.isDerby ? 1 : 0;
                break;
            case MatchRule.Injuries:
                score = S_PlayerMatchSimulator.injuries.home + S_PlayerMatchSimulator.injuries.away;
                break;

            case MatchRule.GameMinute:
                score = S_PlayerMatchSimulator.matchMinute;
                break;
            case MatchRule.PlayerSubstitutions:
                score = S_PlayerMatchSimulator.IsPlayerHomeTeam() ? S_PlayerMatchSimulator.substitutions.home : S_PlayerMatchSimulator.substitutions.away;
                break;

            case MatchRule.OpponentSubstitutions:
                score = !S_PlayerMatchSimulator.IsPlayerHomeTeam() ? S_PlayerMatchSimulator.substitutions.home : S_PlayerMatchSimulator.substitutions.away;
                break;
        }

        score = S_FootballEnums.GetScoreDirection(direction, score, compareFloat,customCurve);

        score *= weight;

        return score;
    }
}