using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public static class S_PlayerMatchSimulator
{
    public static int matchMinute = 0;
    public static S_Calendar.Match match;
    public static S_FastMatchSimulator.Score matchScore; //REDO its pretty ugly using a struct here coming from that class
    public static SO_Curve goalChancePerMinute;
    
    const float MAXGOALCHANCE = 0.5f;
    const float GOALCHANCEDECREASEPERGOAL = 0.75f;
    const float GOALREVOKEDCHANCE = 5;

    public static (float home, float away) matchAggressivity = ( 0, 0 );
    
    public static (float home, float away) injuryChance = ( 0, 0 );

    public static (float home, float away) traitsScoreChance = ( 0, 0 );

    public static (int home, int away) tacticEffectiveness = ( 0, 0);

    public static (int home, int away) substitutions = (0, 0);

    public static (int home, int away) luck = (0, 0);

    public static float homeGoalCheck = 0.0f;
    public static float awayGoalCheck = 0.0f;

    public static (int home, int away) injuries = (0,0);

    public static UnityEvent OnMatchEnd = new UnityEvent();
    public static UnityEvent OnMatchStart = new UnityEvent();

    public static bool isDerby;

    static int opponentMatchSkillLevel; //the skill level of the opponent may change depending on red cards, therefore it is stored here

    [Header("Yellow/Red Cards")]
    public static List<SO_PlayerData> YellowCards = new List<SO_PlayerData>();
    public static List<SO_PlayerData> RedCards = new List<SO_PlayerData>();

    public static List<SO_PlayerData> opponentYellowCards = new List<SO_PlayerData>();
    public static List<SO_PlayerData> opponentRedCards = new List<SO_PlayerData>();

    public static string refereeName;

    public static List<string> opponentTeamNames = new List<string>();

    static S_PlayerMatchSimulator()
    {
        goalChancePerMinute = Resources.Load<SO_Curve>(S_ResDirs.goalChancePerMinute);
        
    }
    #region MATCH SIMULATION
    public static SO_CardData SimulateMatchSegment(int minMinutes=9,int maxMinutes=13)
    {

        UpdateMatchTextData();

        SO_CardData rollData = ScoreGoalRoll();
        if (rollData == null)
        {
            List<SO_CardData> matchCards = S_GlobalManager.deckManagerRef.cardSelector.ChooseCardByScore();
            //rollData = matchCards[Random.Range(0, matchCards.Count)];
            rollData = FindChanceWeightedMatchCard(matchCards);
        }
        
        if(rollData.decreaseCountDown) matchMinute += Random.Range(minMinutes, maxMinutes);
        
        return rollData;

    }

    public static void StartMatch()
    {


        refereeName = S_PlayersGenerator.CreateRandomName(false); //the ref has only a surname
        
        S_GlobalManager.deckManagerRef.MatchScoreText.gameObject.SetActive(true);
        S_GlobalManager.deckManagerRef.PhaseText.gameObject.SetActive(false);

        match = S_Calendar.FindMatchByTeam(S_GlobalManager.selectedTeam,S_GlobalManager.currentMatchDay);
        matchScore.home = 0;
        matchScore.away = 0;
        injuries = (0,0);

        UpdateMatchTextData();

        //changes the team's skill level based on the player's playing 11
        S_GlobalManager.selectedTeam.SkillLevel = S_GlobalManager.squad.FindGameSkillLevel();
        
        opponentMatchSkillLevel = GetOpponentTeam().SkillLevel;

        S_GlobalManager.nextOpponent.GenerateRandomTraits();

        //REDO when giving the player the choice of the starting eleven this must be removed
        S_GlobalManager.squad.SetBestPlayingEleven();


        DerbyCheck(); //checks if match is derby, increases aggressivity by 1
        CheckPlayerOpponentTraitsInteraction();
        ApplyPlayersTraits();
        ApplyTeamTraits();
        UpdateTacticsEffectiveness();
        CalcTeamsLuck();

        OnMatchStart.Invoke();

    }

    public static void EndMatch()
    {

        EndMatchAddPoints();
        EndMatchParametersReaction();

        YellowCards.Clear();
        RedCards.Clear();

        opponentTeamNames.Clear();

        matchMinute = 0;
        matchScore.home = 0;
        matchScore.away = 0;
        injuries = (0,0);
        
        S_GlobalManager.currentMatchDay++;
        S_GlobalManager.nextOpponent = S_Calendar.FindOpponent();
        
        UpdateMatchTextData();

        //REDO non molto elegante metodo
        S_GlobalManager.selectedTeam.teamTactics = ScriptableObject.Instantiate<SO_Tactics>(Resources.Load<SO_Tactics>(S_ResDirs.genericTactic));

        S_GlobalManager.squad.RemoveExpulsions();
        S_GlobalManager.squad.ReduceInjuries();
        
        S_GlobalManager.squad.DecreaseElevenEnergy();
        S_GlobalManager.squad.RefillBenchEnergy();

        OnMatchEnd.Invoke();

    }
    
    private static void EndMatchAddPoints()
    {
        if (matchScore.home > matchScore.away) S_Ladder.UpdateTeamPoints(match.homeTeam, 3);
        if (matchScore.home < matchScore.away) S_Ladder.UpdateTeamPoints(match.awayTeam, 3);
        if (matchScore.home == matchScore.away)
        {
            S_Ladder.UpdateTeamPoints(match.homeTeam, 1);
            S_Ladder.UpdateTeamPoints(match.awayTeam, 1);
        }

    }
    #endregion

    #region UI
    public static void UpdateMatchTextData()
    {
        S_GlobalManager.deckManagerRef.MatchScoreText.GetComponent<S_MatchInfo>().UpdateMatchInfo(matchMinute);
    }
    #endregion

    #region GOAL
    public static SO_CardData GenerateGolCard(bool homeTeam=true, bool canBeRevoked=true)
    {

        SO_GoalCardData golCard = ScriptableObject.CreateInstance<SO_GoalCardData>();
        
        golCard.cardDescriptions.Add("GOOOOL");

        if (homeTeam == IsPlayerHomeTeam()) //returns if player has scored
        {
            golCard.goalDescription = S_GoalDescriptionGenerator.GenerateGoalDescription(FindPlayerGoalScorer());
        }

        //REDO generate gol description also for opponent
        else
        {
            golCard.goalDescription = S_GoalDescriptionGenerator.GenerateOpponentGoalDescription();
        }

        golCard.desiredCardPrefabDirectory = "Prefabs/P_GolCard";
        //golCard.decreaseCountDown = false;
        golCard.cardIcon = homeTeam ? match.homeTeam.teamLogo : match.awayTeam.teamLogo;
        golCard.cardColor = homeTeam ? match.homeTeam.teamColor1 : match.awayTeam.teamColor1;

        if (homeTeam) matchScore.home++;
        
        if (!homeTeam) matchScore.away++;

        //Gol revoked chance
        if (Random.Range(0, 100) < CalcGoalRevokedChance(homeTeam) && canBeRevoked)
        {
            SO_CardData revokeGoal = ScriptableObject.Instantiate(Resources.Load<SO_CardData>(S_ResDirs.invalidGoal));
            //S_GlobalManager.deckManagerRef.AddCardToDeck(revokeGoal, 0);
            SO_CardData.Branch revokeGoalBranch;
            revokeGoalBranch.addPosition = 0;
            revokeGoalBranch.branchData = revokeGoal;
            revokeGoalBranch.triggerChance = 100;

            golCard.leftBranchCard = revokeGoalBranch;
            golCard.rightBranchCard = revokeGoalBranch;

            if (homeTeam)
            {
                revokeGoal.leftEffects.AddListener(() => matchScore.home = Mathf.Max(0,matchScore.home - 1));
                revokeGoal.rightEffects.AddListener(() => matchScore.home = Mathf.Max(0, matchScore.home - 1));
            }
        
            else
            {
                revokeGoal.leftEffects.AddListener(() => matchScore.away = Mathf.Max(0, matchScore.away - 1));
                revokeGoal.rightEffects.AddListener(() => matchScore.away = Mathf.Max(0, matchScore.away - 1));
            } 
        }

        return golCard;
    }

    private static SO_CardData ScoreGoalRoll()
    {
        SO_CardData card=null;

        homeGoalCheck = GoalCheck(true);

        awayGoalCheck = GoalCheck(false);

        float homeRoll = (float)Random.Range(0, 1001) / 1000;
        float awayRoll = (float)Random.Range(0, 1001) / 1000;

        //the team with the lower goal chance tries to score first
        if (homeGoalCheck <= awayGoalCheck)
        {
            if (homeRoll <= homeGoalCheck)
            {
                card = GenerateGolCard(true);
            }

            else if (awayRoll <= awayGoalCheck)
            {
                card = GenerateGolCard(false);
            }
        }

        else
        {
            if (awayRoll <= awayGoalCheck) 
            {
                card = GenerateGolCard(false);
            }
            else if (homeRoll <= homeGoalCheck)
            {
                card = GenerateGolCard(true);
            }

        }
        
        return card;
    }

    private static float GoalCheck(bool homeTeam)
    {
        
        float goalCheck=0;

        //first calc = skill diff check
        if (homeTeam && IsPlayerHomeTeam()) goalCheck        =  GetSkillDifference(match.homeTeam.SkillLevel, opponentMatchSkillLevel);
        else if (homeTeam && !IsPlayerHomeTeam()) goalCheck  =  GetSkillDifference(opponentMatchSkillLevel, match.awayTeam.SkillLevel);
        else if (!homeTeam && IsPlayerHomeTeam()) goalCheck  =  GetSkillDifference(opponentMatchSkillLevel, match.homeTeam.SkillLevel);
        else if (!homeTeam && !IsPlayerHomeTeam()) goalCheck =  GetSkillDifference(match.awayTeam.SkillLevel, opponentMatchSkillLevel);

        //second calc = goal chance per minute
        goalCheck *= goalChancePerMinute.curve.Evaluate(matchMinute);

        //third calc = home team gets a goal chance boost
        if (homeTeam) goalCheck += 0.1f;

        //decrease goal chance for each already scored goal
        goalCheck *= Mathf.Pow(GOALCHANCEDECREASEPERGOAL, homeTeam ? matchScore.home : matchScore.away);
        
        //adds a -0.2:0.2 chance bonus based on tactic effectiveness
        goalCheck += GoalChanceFromTactics(homeTeam);
        //Debug.Log("la tattica stabilisce che il boost alla goal chance è" + GoalChanceFromTactics(homeTeam));

        //adds a chance bonus based on traits advantages
        goalCheck += GoalChanceFromTraits(homeTeam);

        //takes the score (should be in 0-1 range) and sets it in range of 0 - max possible score
        goalCheck = Mathf.Lerp(0, MAXGOALCHANCE, goalCheck);

        return goalCheck;
    }
    private static SO_PlayerData FindPlayerGoalScorer()
    {
        List<(SO_PlayerData player, float chance)> playerChance = new List<(SO_PlayerData player, float chance)>();

        float totalchance = 0;

        foreach(SO_PlayerData player in S_GlobalManager.squad.playingEleven)
        {
            float chance = 0;
            switch (player.playerRole)
            {
                case SO_PlayerData.PlayerRole.Def:
                    chance = 1;
                    break;
                
                case SO_PlayerData.PlayerRole.Mid:
                    chance = 2;
                    break;
                
                case SO_PlayerData.PlayerRole.Atk:
                    chance = 3;
                    break;
                
                case SO_PlayerData.PlayerRole.Gk:
                    chance = 0.005f;
                    break;
            }
            chance *= player.skillLevel;

            chance += totalchance;
            
            totalchance = chance;

            playerChance.Add((player, chance));
        }
        float random = Random.Range(0, totalchance + 1);
        
        for(int i = 0; i < playerChance.Count; i++)
        {
            if (playerChance[i].chance >= random)
            {
                Debug.Log(playerChance[i].player.playerName + " " + playerChance[i].chance);
                return playerChance[i].player;
            }
        }

        return null;
    }

    private static float CalcGoalRevokedChance(bool homeTeam)
    {
        CalcTeamsLuck();
        float revokeChance = GOALREVOKEDCHANCE;

        if (homeTeam) revokeChance += luck.home*2;
        else if (!homeTeam) revokeChance += luck.away*2;

        Debug.Log("Rischio annullamento gol: " + revokeChance+"%");
        return revokeChance;
    }

    #endregion

    #region UTILITIES
    private static float GetSkillDifference(int skillA, int skillB)
    {
        float skillDifference = skillA - skillB;
        skillDifference += 5;
        skillDifference /= 10;
        return skillDifference;
    }
    public static SO_Team GetOpponentTeam() => S_GlobalManager.selectedTeam.teamName == match.homeTeam.teamName ? match.awayTeam : match.homeTeam;
    public static bool IsOpponentHomeTeam() => !(S_GlobalManager.selectedTeam.teamName == match.homeTeam.teamName);
    public static bool IsPlayerHomeTeam() => (S_GlobalManager.selectedTeam.teamName == match.homeTeam.teamName);
    private static float GoalChanceFromTactics(bool homeTeam) => homeTeam ? (float)tacticEffectiveness.home/10.0f : (float)tacticEffectiveness.away/10.0f;
    private static float GoalChanceFromTraits(bool homeTeam) => homeTeam ? (float)traitsScoreChance.home/10.0f : (float)traitsScoreChance.away/10.0f;
    public static bool PlayerWinning() => (IsPlayerHomeTeam() && matchScore.HomeWinning()) || matchScore.AwayWinning();
    public static bool OpponentWinning() => (IsPlayerHomeTeam() && matchScore.AwayWinning()) || matchScore.HomeWinning();
    public static string GetGeneratedOpponentPlayer() => opponentTeamNames[Random.Range(0, opponentTeamNames.Count)];
    
    //If match is derby some cards may appear, also aggressivity increases by 1 for both teams
    public static void DerbyCheck()
    {
        isDerby = false;
        
        if (match.homeTeam.derbies.Contains(match.awayTeam))
        {
            isDerby = true;
            matchAggressivity.home += 1;
            matchAggressivity.away += 1;
        }
    }
    private static void ClampParameters()
    {
        matchAggressivity.home = Mathf.Clamp(matchAggressivity.home,0,100);
        matchAggressivity.away = Mathf.Clamp(matchAggressivity.away,0,100);
        
        injuryChance.home = Mathf.Clamp(injuryChance.home, 0, 100);
        injuryChance.away = Mathf.Clamp(injuryChance.away, 0, 100);

        traitsScoreChance.home = Mathf.Clamp(traitsScoreChance.home, 0, 100);
        traitsScoreChance.away = Mathf.Clamp(traitsScoreChance.away, 0, 100);
    }
    public static string RandomlyGetNewOrExistingOpponentPlayer(int newPlayerChance=49)
    {
        int ran = Random.Range(0, 100);
        
        if ((ran < newPlayerChance && opponentTeamNames.Count<11) || opponentTeamNames.Count==0) return S_PlayersGenerator.GenerateFakeOpponentPlayer();
        
        else return opponentTeamNames[Random.Range(0, opponentTeamNames.Count)];
        
    }

    public static void CalcTeamsLuck()
    {
        int playerLuck=0;
        int opponentLuck = 0;

        foreach(SO_PlayerData player in S_GlobalManager.squad.playingEleven)
        {
            if (player.playerTraits[0].traitName == SO_PlayerTrait.PlayerTraitNames.Unlucky)
            {
                playerLuck -= 1;
            }
            if (player.playerTraits[0].traitName == SO_PlayerTrait.PlayerTraitNames.Lucky)
            {
                playerLuck += 1;
            }
        }

        SO_Team t = GetOpponentTeam();
        foreach(SO_TeamTrait tt in t.teamTraits)
        {
            if (tt.traitName == SO_TeamTrait.TraitNames.Lucky)
            {
                opponentLuck++;
            }
            if (tt.traitName == SO_TeamTrait.TraitNames.Unlucky)
            {
                opponentLuck--;
            }
        }

        if (IsPlayerHomeTeam())
        {
            luck.home = playerLuck - opponentLuck;
            luck.away = opponentLuck - playerLuck;
        }
        else
        {
            luck.home = opponentLuck - playerLuck;
            luck.away = playerLuck - opponentLuck;
        }
    }

    #endregion

    #region TRAITS
    private static void ApplyTeamTraits()
    {
        foreach(SO_TeamTrait trait in GetOpponentTeam().teamTraits)
        {
            trait.traitEffect.Invoke();
        }
    }
    private static void ApplyPlayersTraits()
    {
        //REDO works with only 1 player trait
        foreach (SO_PlayerData player in S_GlobalManager.squad.playingEleven)
        {
            player.playerTraits[0].traitEffect.Invoke();
        }
    }
    private static void CheckPlayerOpponentTraitsInteraction()
    {
        foreach (SO_PlayerData player in S_GlobalManager.squad.playingEleven)
        {
            //REDO currently works only with players with one trait only, if players will have more than one trait it will be necessary to check against all of them
            S_TraitsCombinationsManager.CheckTraitsCombination(player, GetOpponentTeam());
        }

        ClampParameters();
    }

    #endregion

    #region TACTICS
    public static void UpdateTacticsEffectiveness()
    {
        tacticEffectiveness.home = match.homeTeam.teamTactics.FindEffectivenessAgainstTactic(match.awayTeam.teamTactics.teamTactic);
        tacticEffectiveness.away = match.awayTeam.teamTactics.FindEffectivenessAgainstTactic(match.homeTeam.teamTactics.teamTactic);
    }
    #endregion

    #region CARDS
    public static void ExpelPlayerFootballer(SO_PlayerData player)
    {
        YellowCards.Remove(player);
        RedCards.Add(player);

        S_GlobalManager.squad.playingEleven.Remove(player);

        S_GlobalManager.selectedTeam.SkillLevel = S_GlobalManager.squad.FindGameSkillLevel();

        UpdateOpponentMatchSkillLevel();
       
    }

    public static void UpdateOpponentMatchSkillLevel()
    {
        opponentMatchSkillLevel = (int)Mathf.Ceil((float)(opponentMatchSkillLevel * (11 - opponentRedCards.Count)) / 11);
        float injuriesSkillDecrease = IsPlayerHomeTeam() ? injuries.away : injuries.home;
        injuriesSkillDecrease /= 2;
        opponentMatchSkillLevel -= Mathf.FloorToInt(injuriesSkillDecrease);
    }
    #endregion

    #region MATCH CARD SELECTION
    public static SO_MatchCardData FindChanceWeightedMatchCard(List<SO_CardData> cards)
    {
        List<(SO_CardData card, float chance)> chancedCards = new List<(SO_CardData card, float chance)>();

        float totalscore = 0;

        foreach (SO_CardData card in cards)
        {
            totalscore += (card as SO_MatchCardData).chanceOfAppearance;
            chancedCards.Add((card, totalscore));
        }

        float selectedCardRoll = (float)Random.Range(0, totalscore);
        SO_MatchCardData chosencard=null;

        bool cardFound = false;

        

        for (int i = 0; i < chancedCards.Count; i++)
        {
            if (chancedCards[i].chance >= selectedCardRoll)
            {
                chosencard = chancedCards[i].card as SO_MatchCardData;
                cardFound = true;
                break;
            }
        }

        if (!cardFound)
        {
            Debug.LogWarning("Non è stata trovata nessuna match card con uno chance di apparire maggiore di 0!");
            chosencard = cards[0] as SO_MatchCardData;
        }

        return chosencard;
    }
    #endregion

    #region CALCULATE END MATCH REACTION
    private static void EndMatchParametersReaction()
    {
        float president = 0;
        float supporters = 0;
        float team = 0;

        //Find expectations level
        float teamsSkillDifference = (float)(S_GlobalManager.selectedTeam.SkillLevel - GetOpponentTeam().SkillLevel + 5 ) / 10.0f;

        //Find target ladder position distance
        int targetLadderDistance = S_Ladder.FindTeamPoints(S_GlobalManager.selectedTeam) - S_Ladder.FindPointsAtLadderPosition(S_GlobalManager.minRankingObjective);

        //supporters change their opinion rapidly with a change of results, they care more about the single match
        //president cares more about reaching the target ladder position
        //the team cares more about results continuity
        //money doesn't care about results

        if (matchScore.Drawing()) //match draw
        {
            supporters = -(teamsSkillDifference - 0.5f) * 10 + (targetLadderDistance / 3) * 2;
            team = -(teamsSkillDifference - 0.5f) * 10 + (targetLadderDistance / 3);
            president = -(teamsSkillDifference - 0.5f) * 5 + (targetLadderDistance / 3) * 4;
            if (isDerby)
            {
                supporters += 2;
                team += 2;
                president += 2;
            }
        }

        else if((matchScore.HomeWinning() && IsPlayerHomeTeam()) || (matchScore.AwayWinning() && !IsPlayerHomeTeam())) //player won
        {
            supporters = (1 - teamsSkillDifference) * 20 + Mathf.Max(3, targetLadderDistance);
            team = (1 - teamsSkillDifference) * 10 + Mathf.Max(3, targetLadderDistance * 2);
            president = (1 - teamsSkillDifference) * 5 + Mathf.Max(3, targetLadderDistance * 3);
            
            if (isDerby)
            {
                supporters += 15;
                team += 5;
                president += 5;
            }
        }

        else //player lost
        {
            //mathf.min to avoid giving positive values when the points distance is positive

            supporters = - ((teamsSkillDifference * 10) - Mathf.Min(0,(targetLadderDistance / 3) * 5));
            team = - ((teamsSkillDifference*10) - Mathf.Min(0,(targetLadderDistance / 6) * 5));
            president = - ((teamsSkillDifference * 5) - Mathf.Min((targetLadderDistance / 3) * 10));
            
            if (isDerby)
            {
                supporters -= 10;
                team -= 5;
                president -= 5;
            }
        }

        Debug.Log("Punti fine partita: " + " Supporters: " + supporters + " President: " + president + " Team: "+ team);

        S_GlobalManager.SetSupporters(supporters);
        S_GlobalManager.SetPresident(president);
        S_GlobalManager.SetTeam(team);

    }

    #endregion
}
