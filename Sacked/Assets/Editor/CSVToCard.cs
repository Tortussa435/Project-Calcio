using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using static S_FootballEnums;
using NUnit.Framework;

public class CSVToCard
{
    private static string baseCardsCSVPath = "/Editor/CSVs/BaseCards - Cards.csv";

    [MenuItem("ProjectCalcio/Generate Base Cards")]
    public static void ConvertCSVtoCard()
    {
        string[] allLines = File.ReadAllLines(Application.dataPath+baseCardsCSVPath);
        foreach(string s in allLines.Skip(1))
        {
            string[] splitData = s.Split(',');
            GenerateBaseCard(splitData);
        }
        AssetDatabase.SaveAssets();
    }

    public static void GenerateBaseCard(string[] splitData)
    {
        SO_CardData card = ScriptableObject.CreateInstance<SO_CardData>();

        string savedirectory = SetCommonData(splitData, card);
        
        AssetDatabase.CreateAsset(card,"Assets/Resources/ScriptableObjects/"+savedirectory+$"/CD_{card.cardName}.asset");
    }



    //sets values that are common to all cards types
    public static string SetCommonData(string[] splitData, SO_CardData card)
    {
        card.cardName = splitData[0];
        //card.cardColor = splitData[2]; TODO, might use Enum in google sheet and fixed colors from palette
        
        string[] splitCardDescriptions = splitData[3].Split('$');
        card.cardDescriptions = new System.Collections.Generic.List<string>();
        foreach(string s in splitCardDescriptions)
        {
            Debug.Log(s);

            card.cardDescriptions.Add(s.Replace(';', ','));
            //; is used instead of , in google sheet to avoid confusing the comma used to split string from the one used for text
        }

        card.leftChoice = splitData[4].Replace(';', ',');
        card.rightChoice = splitData[5].Replace(';', ',');
        
        card.leftValues.addedMoney=int.Parse(splitData[6]);
        card.leftValues.addedTeam=int.Parse(splitData[7]);
        card.leftValues.addedPresident=int.Parse(splitData[8]);
        card.leftValues.addedSupporters=int.Parse(splitData[9]);

        card.rightValues.addedMoney = int.Parse(splitData[10]);
        card.rightValues.addedTeam = int.Parse(splitData[11]);
        card.rightValues.addedPresident = int.Parse(splitData[12]);
        card.rightValues.addedSupporters = int.Parse(splitData[13]);

        Sprite cardIcon = Resources.Load<Sprite>(splitData[14]);
        if(cardIcon!=null) card.cardIcon = cardIcon; //TODO, set card icon from sheet (might use string as directory?)
        
        //card.leftBranchCard.branchData = splitData[15]; //TODO, set generated card from sheet (might use string as directory?)
        card.leftBranchCard.addPosition = int.Parse(splitData[16]);
        
        //card.rightBranchCard.branchData = splitData[17]; //TODO, set generated card from sheet (might use string as directory?)
        card.rightBranchCard.addPosition = int.Parse(splitData[18]);

        card.canAppearMoreThanOnce = (splitData[19] == "1");

        card.scoreCard = new System.Collections.Generic.List<S_CardsScoreFormula>();
        for(int i = 20; i < splitData.Length; i++)
        {
            if (splitData[i] != "") GenerateCardFormula(splitData[i], card);
        }

        return splitData[1]; //splitdata[1] is the folder directory where the file gets stored
    }

    public static void GenerateCardFormula(string splitData, SO_CardData card)
    {
        string[] splitFormula = splitData.Split(';');
        S_CardsScoreFormula newformula = new S_CardsScoreFormula();
        if (System.Enum.TryParse(splitFormula[0].ToString(), out Rule foundrule))
        {
            newformula.desiredValue = foundrule;
        }
        if(splitFormula.Length>1) newformula.direction = splitFormula[1] == "Linear" ? ScoreDirection.Linear : ScoreDirection.InverseLinear;
        if(splitFormula.Length>2) newformula.scoreMultiplier = (float)int.Parse(splitFormula[2])/100;
        card.scoreCard.Add(newformula);
    }
}
