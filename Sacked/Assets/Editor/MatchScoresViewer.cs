using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MatchScoresViewer : EditorWindow
{
    [MenuItem("ProjectCalcio/MatchCardScoreViewer")]

    public static void ShowWindow()
    {
        GetWindow<MatchScoresViewer>("MatchCardScoreViewer");
    }

    private void OnGUI()
    {

        try
        {
            float totalScore = 0;

            foreach(SO_MatchCardData mcd in S_GlobalManager.deckManagerRef.cardSelector.matchCardsPool.cardsPool)
            {
                totalScore += mcd.chanceOfAppearance;
            }
            GUILayout.BeginHorizontal();
            GUILayout.Label("Name");
            GUILayout.Label("Score");
            GUILayout.Label("Normalized Score");
            GUILayout.EndHorizontal();
            
            foreach(SO_MatchCardData mcd in S_GlobalManager.deckManagerRef.cardSelector.matchCardsPool.cardsPool)
            {
                int cao = (int)((mcd.chanceOfAppearance/totalScore) * 100);

                GUILayout.BeginHorizontal();
                GUILayout.Label(mcd.cardName);
                GUILayout.Label(mcd.chanceOfAppearance.ToString());
                GUILayout.Label(cao+"%");
                GUILayout.EndHorizontal();
            }
        }
        catch
        {
            GUILayout.Label("Cannot find");
        }
    }

    private void Update()
    {
        Repaint();
    }
}
