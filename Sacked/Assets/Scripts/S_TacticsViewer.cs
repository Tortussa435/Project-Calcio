using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class S_TacticsViewer : MonoBehaviour
{
    public TextMeshProUGUI homeTactic;
    public TextMeshProUGUI awayTactic;
    public void ShowTeamsTactics()
    {
        try
        {
            homeTactic.text = S_PlayerMatchSimulator.match.homeTeam.teamTactics.teamTactic.ToString();
            awayTactic.text = S_PlayerMatchSimulator.match.awayTeam.teamTactics.teamTactic.ToString();
            if (homeTactic.text == "Generic") homeTactic.text = "";
            if (awayTactic.text == "Generic") awayTactic.text = "";
        }
        catch
        {

        }
    }

    private void Update() => ShowTeamsTactics();
    
}
