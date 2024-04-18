using Mono.Cecil;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card Data", menuName = "Cards/Card")]
public class SO_CardData : ScriptableObject
{

    public string cardName;
    public Color cardColor=Color.white;
    public string cardDescription;
    public string leftChoice;
    public string rightChoice;
    public bool canAppearMoreThanOnce=false;
    public bool decreaseCountDown = true;


    public string desiredCardPrefabDirectory=new string("");

    [System.Serializable]
    public struct ChangeValues
    {
        public float addedMoney;
        public float addedTeam;
        public float addedPresident;
        public float addedSupporters;
    };
    public ChangeValues leftValues;
    public ChangeValues rightValues;

    public bool alreadyPicked=false;

    public Sprite cardIcon;
    
    [System.Serializable]
    public struct Branch
    {
        public SO_CardData branchData;
        public int addPosition;
    }
    public Branch leftBranchCard;
    public Branch rightBranchCard;

    [Header("Scoring")]
    public List<S_CardsScoreFormula> scoreCard;
    

    public float cardScore;

    public float cardScoreNotNormalized;

    virtual public void leftEffect()
    {
        //Debug.Log("left effect");
        //if (leftBranchCard.branchData != null) S_GlobalManager.deckManagerRef.AddCardToDeck(leftBranchCard.branchData,leftBranchCard.addPosition);
        
        S_GlobalManager.SetMoney(leftValues.addedMoney);
        S_GlobalManager.SetPresident(leftValues.addedPresident);
        S_GlobalManager.SetSupporters(leftValues.addedSupporters);
        S_GlobalManager.SetTeam(leftValues.addedTeam);


        if (!S_GlobalManager.DefeatCheck()) S_GlobalManager.deckManagerRef.GenerateCard(null,null,decreaseCountDown);
    }
    virtual public void rightEffect()
    {
        //if(rightBranchCard.branchData!=null) S_GlobalManager.deckManagerRef.AddCardToDeck(rightBranchCard.branchData, rightBranchCard.addPosition);
        S_GlobalManager.SetMoney(rightValues.addedMoney);
        S_GlobalManager.SetPresident(rightValues.addedPresident);
        S_GlobalManager.SetSupporters(rightValues.addedSupporters);
        S_GlobalManager.SetTeam(rightValues.addedTeam);

        if(!S_GlobalManager.DefeatCheck()) S_GlobalManager.deckManagerRef.GenerateCard(null,null,decreaseCountDown); //when the player reaches a defeat, card generation is handled by S_ValueManager

        //Debug.Log("right effect");
    }
    
    public virtual void SetCardScore()
    {
        cardScore = 0;
        if(canAppearMoreThanOnce || !alreadyPicked)
        {
            foreach(S_CardsScoreFormula score in scoreCard) cardScore += score.CalculateScore();
        }
        cardScoreNotNormalized = cardScore;
    }
    public float GetCardScore()
    {
        return cardScore;
    }

    public void NormalizeCardScore(float max)
    {
        cardScore /= max;
    }
}
