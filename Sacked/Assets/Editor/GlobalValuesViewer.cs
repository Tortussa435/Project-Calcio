using UnityEngine;
using UnityEditor;

public class GlobalValuesViewer : EditorWindow
{

    [MenuItem("ProjectCalcio/GlobalValues")]
    public static void ShowWindow()
    {
        GetWindow<GlobalValuesViewer>("GlobalValues");
    }
    private void OnGUI()
    {
        GUILayout.Label("Global Variables:", EditorStyles.boldLabel);
        try
        {
            GUILayout.Label(S_GlobalManager.currentPhase.ToString());
            GUILayout.Label("Your team: "+S_GlobalManager.selectedTeam.teamName);
            GUILayout.Label("Next Opponent: "+S_GlobalManager.nextOpponent.teamName);
            GUILayout.Label("President: "+S_GlobalManager.President);
            GUILayout.Label("Money: "+S_GlobalManager.Money);
            GUILayout.Label("Team: "+S_GlobalManager.Team);
            GUILayout.Label("Supporters: "+S_GlobalManager.Supporters);

        }
        catch
        {
            GUILayout.Label("Start the game to see values");
        }

    }

    
    private void Update()
    {
        Repaint();
    }
}
