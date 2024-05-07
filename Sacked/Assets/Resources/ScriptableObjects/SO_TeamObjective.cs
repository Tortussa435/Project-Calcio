using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SO_TeamObjective : SO_CardData
{
    public float objectiveGenerosity = 0; //the value increases when you tell the president that he's asking for a goal that is hard to reach
    public int minRequiredPlace;
    public int optimalRequiredPlace;
    
    private SO_TeamObjective()
    {
        decreaseCountDown = false;
    }
    public void SetTeamObjectivesData()
    {
        SO_TeamObjective nextTeamObjective=this;
        leftBranchCard.branchData = nextTeamObjective;
        leftBranchCard.addPosition = 0;
        rightValues.addedPresident = -10f;
        
        

        int skillLevel = S_GlobalManager.selectedTeam.SkillLevel;
        switch (skillLevel)
        {
            default:
                break;
            
            case int i when i <= 2: //max 2 stars team
                switch (objectiveGenerosity)
                {
                    case 0: //save from relegation
                        minRequiredPlace = 17;
                        optimalRequiredPlace = 14;
                        cardDescriptions.Add( "avoid relegation");
                        break;

                    case 1: //try saving from relegation
                        minRequiredPlace = 19;
                        optimalRequiredPlace = 17;
                        cardDescriptions.Add( "try avoiding relegation");
                        break;
                }
                break;
            case 3: //mid team
                switch (objectiveGenerosity)
                {
                    case 0: //between the left and right side of the table
                        minRequiredPlace = 12;
                        optimalRequiredPlace = 9;
                        cardDescriptions.Add("reach the mid of the table");
                        break;

                    case 1: //avoid relegation without pain
                        minRequiredPlace = 15;
                        optimalRequiredPlace = 12;
                        cardDescriptions.Add("avoid risking relegation");
                        break;
                }
                break;
            case 4: //good team
                switch (objectiveGenerosity)
                {
                    case 0: //reach champions league
                        minRequiredPlace = 4;
                        optimalRequiredPlace = 2;
                        cardDescriptions.Add("reach the top 4");
                        break;
                    case 1: //between conference and champions league
                        minRequiredPlace = 7;
                        optimalRequiredPlace = 4;
                        cardDescriptions.Add( "qualify for an international competition");
                        break;
                }
                break;
            case 5: //top team
                switch (objectiveGenerosity)
                {
                    case 0://win
                        minRequiredPlace = 1;
                        optimalRequiredPlace = 1;
                        cardDescriptions.Add( "Win the title");
                        break;
                    
                    case 1://fight for the title
                        minRequiredPlace = 3;
                        optimalRequiredPlace = 1;
                        cardDescriptions.Add( "Try winning the title");
                        break;
                }
                break;
        }

        leftChoice = "Accept";
        rightChoice = "Too hard";
    }

    public override void leftEffect()
    {
        S_GlobalManager.minRankingObjective = minRequiredPlace;
        S_GlobalManager.optimalRankingObjective = optimalRequiredPlace;

        //REDO add transfer market phase back
        //S_GlobalManager.deckManagerRef.ChangeCurrentPhase(1,1, S_GlobalManager.CardsPhase.Market);
        
        //REDO metodo per evitare che riappaia una carta scegli squadra molto grezzo
        S_GlobalManager.deckManagerRef.cardSelector.appendedWeekCards.Clear();

        S_GlobalManager.deckManagerRef.ChangeCurrentPhase(2,3, S_GlobalManager.CardsPhase.Week);
        

        S_GlobalManager.squad.GenerateTeam();

        
        //base.leftEffect();
        if (!S_GlobalManager.DefeatCheck()) S_GlobalManager.deckManagerRef.GenerateCard(null, null, decreaseCountDown);

    }
    public override void rightEffect()
    {
        SO_TeamObjective to = leftBranchCard.branchData as SO_TeamObjective;
        to.objectiveGenerosity += 1;
        if (to.objectiveGenerosity >= 2)
        {
            S_GlobalManager.deckManagerRef.AddCardToDeck(Resources.Load<SO_CardData>("ScriptableObjects/Sacking/Sacking_TooLowObjective"));
        }
        else
        {
            to.SetTeamObjectivesData();
            S_GlobalManager.deckManagerRef.AddCardToDeck(to);
        }
        if (!S_GlobalManager.DefeatCheck()) S_GlobalManager.deckManagerRef.GenerateCard(null, null, decreaseCountDown);
        //base.rightEffect();
    }

}
