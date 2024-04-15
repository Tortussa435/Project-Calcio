using NUnit.Framework;
using UnityEditor;
using UnityEngine;

public class CalendarViewer : EditorWindow
{

    Vector2 scrollPosition = Vector2.zero;

    [MenuItem("ProjectCalcio/Calendar")]
    public static void ShowWindow()
    {
        GetWindow<CalendarViewer>("Calendar");
    }
    private void OnGUI()
    {
        scrollPosition = GUILayout.BeginScrollView(scrollPosition,false,true, GUILayout.ExpandHeight(true));
        GUILayout.Label("CALENDAR", EditorStyles.boldLabel);

        try
        {
            for (int i = 0; i < S_Calendar.calendar.Count; i++)
            {
                GUILayout.Label("\nGIORNATA "+i, EditorStyles.boldLabel);
                foreach (S_Calendar.Match match in S_Calendar.calendar[i])
                {
                    GUILayout.Label(match.homeTeam.teamName + " VS " + match.awayTeam.teamName);
                }
            }
        }
        catch
        {
            GUILayout.Label("Start the game to see values");
        }
        GUILayout.EndScrollView();

    }


}
