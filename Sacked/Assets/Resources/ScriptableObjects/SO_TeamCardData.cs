using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SO_TeamCardData : SO_CardData
{
    public SO_Team cardTeam;
    int minDesiredPlace = 1;
    int optDesiredPlace = 1;
    public override void rightEffect()
    {
        //respawn card after other 2 offers
        
        S_GlobalManager.deckManagerRef.AddCardToDeck(leftBranchCard.branchData,leftBranchCard.addPosition,null,rightBranchCard.extraData);

        if (!S_GlobalManager.DefeatCheck()) S_GlobalManager.deckManagerRef.GenerateCard(null, null, decreaseCountDown);


        //base.rightEffect();
    }
    public override void leftEffect()
    {
        S_GlobalManager.selectedTeam = cardTeam;
        S_GlobalManager.deckManagerRef.SetBackGroundColor(cardTeam.teamColor1, cardTeam.teamColor2);
        
        //REDO
        Destroy(S_GlobalManager.deckManagerRef.deck.transform.GetChild(0).gameObject);
        Destroy(S_GlobalManager.deckManagerRef.deck.transform.GetChild(1).gameObject);
        
        S_GlobalManager.nextOpponent = S_Calendar.FindOpponent();

        StartCareer();
    }

    private void SetTeamObjective()
    {
        int skillLevel = cardTeam.SkillLevel;

        string obj = "";
        switch (skillLevel)
        {
            case int i when i<=4:
                obj = "Avoid relegation";
                minDesiredPlace = 17;
                optDesiredPlace = 14;
                break;
            case int i when i<=6:
                obj = "Reach the mid of the table";
                minDesiredPlace = 14;
                optDesiredPlace = 10;
                break;
            case int i when i<=8:
                obj = "Reach the top 4";
                minDesiredPlace = 6;
                optDesiredPlace = 4;
                break;
            case int i when i<=10:
                obj = "Win the title";
                minDesiredPlace = 1;
                optDesiredPlace = 1;
                break;
        }

        ownerCard.transform.Find("T_Objective").GetComponent<TMPro.TextMeshProUGUI>().text = obj;
    }

    public void StartCareer()
    {
        S_GlobalManager.minRankingObjective = minDesiredPlace;
        S_GlobalManager.optimalRankingObjective = optDesiredPlace;

        //REDO metodo per evitare che riappaia una carta scegli squadra molto grezzo
        S_GlobalManager.deckManagerRef.cardSelector.appendedWeekCards.Clear();

        S_GlobalManager.squad.GenerateTeam();

        (int min, int max) weekduration = (S_GlobalManager.deckManagerRef.weekDuration.min, S_GlobalManager.deckManagerRef.weekDuration.max);
        S_GlobalManager.deckManagerRef.ChangeCurrentPhase(weekduration.min + 1, weekduration.max + 1, S_GlobalManager.CardsPhase.Week);

    }

    public void SetTeamData(SO_Team team)
    {
        cardTeam = team;
        leftBranchCard.branchData=this;
        leftBranchCard.addPosition=0;
        cardIcon = cardTeam.teamLogo;

        leftChoice = "Accept";
        rightChoice = "Refuse";
        
        //REDO - shit method to set color
        if ((cardTeam.teamColor1 == Color.black || cardTeam.teamColor1 == Color.white) && (cardTeam.teamColor2 == Color.black || cardTeam.teamColor2 == Color.white) ) cardColor = Color.gray;
        
        else if (cardTeam.teamColor1 == Color.black || cardTeam.teamColor1 == Color.white) cardColor = cardTeam.teamColor2;
        
        else cardColor = cardTeam.teamColor1;

        cardDescriptions.Add(team.teamName);

        onGeneratedEffects.AddListener(SetTeamObjective);
    }
    
}
