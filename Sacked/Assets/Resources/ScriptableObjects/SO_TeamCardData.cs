using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SO_TeamCardData : SO_CardData
{
    public SO_Team cardTeam;
    public override void rightEffect()
    {
        //respawn card after other 2 offers
        S_GlobalManager.deckManagerRef.AddCardToDeck(leftBranchCard.branchData,leftBranchCard.addPosition);
        base.rightEffect();
    }
    public override void leftEffect()
    {
        S_GlobalManager.selectedTeam = cardTeam;
        S_GlobalManager.deckManagerRef.SetBackGroundColor(cardTeam.teamColor1, cardTeam.teamColor2);
        
        //REDO
        Destroy(S_GlobalManager.deckManagerRef.deck.transform.GetChild(0).gameObject);
        Destroy(S_GlobalManager.deckManagerRef.deck.transform.GetChild(1).gameObject);

        //Generates team objective
        SO_TeamObjective teamobjective = ScriptableObject.CreateInstance<SO_TeamObjective>();
        teamobjective.SetTeamObjectivesData();
        S_GlobalManager.deckManagerRef.AddCardToDeck(teamobjective);
        S_GlobalManager.nextOpponent = S_Calendar.FindOpponent();

        base.leftEffect();
        //----
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
        
        cardDescription = team.teamName;
    }
    
}
