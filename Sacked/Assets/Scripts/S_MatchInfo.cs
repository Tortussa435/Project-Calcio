using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static S_PlayerMatchSimulator;

public class S_MatchInfo : MonoBehaviour
{
    private int currentMinute = 0;
    private int targetMinute = 0;
    public TextMeshProUGUI textRef;
    private string matchInfo;

    public void UpdateMatchInfo(int matchMinute)
    {
        matchInfo = "'\n" + match.homeTeam.teamName + " " + matchScore.home + " - " + matchScore.away + " " + match.awayTeam.teamName;
        StartCoroutine(LerpGameMinute(matchMinute));
        
    }
    private IEnumerator LerpGameMinute(int minuteToReach)
    {
        targetMinute = minuteToReach;
        while (currentMinute < targetMinute)
        {
            Debug.Log("Ziopera");
            currentMinute++;
            textRef.text = currentMinute.ToString() + matchInfo;
            yield return new WaitForSeconds(0.1f);
        }
        textRef.text = currentMinute.ToString() + matchInfo;
        yield break;
    }
}
