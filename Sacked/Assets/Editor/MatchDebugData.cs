using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MatchDebugData : EditorWindow
{
    [MenuItem("ProjectCalcio/MatchDebugData")]
    public static void ShowWindow()
    {
        GetWindow<MatchDebugData>("Match Data");
    }
    private void OnGUI()
    {
        try
        {

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
