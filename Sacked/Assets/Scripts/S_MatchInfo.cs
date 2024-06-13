using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static S_PlayerMatchSimulator;
using UnityEngine.Events;

public class S_MatchInfo : MonoBehaviour
{
    private int currentScoreHome = 0;
    private int currentScoreAway = 0;

    private int currentMinute = 0;
    private int targetMinute = 0;
    public TextMeshProUGUI textRef;
    private string matchInfo;

    private void Start()
    {
        OnMatchEnd.AddListener(() => currentMinute = 0);
    }
    public void UpdateMatchInfo(int matchMinute,bool updateScore=true)
    {

        if (updateScore)
        {
            currentScoreHome = matchScore.home;
            currentScoreAway = matchScore.away;
        }
        if (gameObject.activeSelf)
        {
            matchInfo = "'\n" + match.homeTeam.teamName + " " + currentScoreHome + " - " + currentScoreAway + " " + match.awayTeam.teamName;
            StopCoroutine("LerpGameMinute");
            StartCoroutine("LerpGameMinute",matchMinute);
        }
        else Debug.LogWarning("Provato ad avviare coroutine in oggetto disattivo");
    }
    private IEnumerator LerpGameMinute(int minuteToReach)
    {
        targetMinute = minuteToReach;
        while (currentMinute < targetMinute)
        {
            //Debug.Log("Ziopera");
            currentMinute++;

            if(S_GlobalManager.currentPhase==S_GlobalManager.CardsPhase.MatchFirstHalf && currentMinute > 45)
                textRef.text = "45 +" + (currentMinute - 45).ToString() + matchInfo;

            else if (currentMinute > 90)
                textRef.text = "90 +" + (currentMinute - 90).ToString() + matchInfo;

            else textRef.text = currentMinute.ToString() + matchInfo;

            yield return new WaitForSeconds(FindLerpSpeed(currentMinute, targetMinute));
        }
        if (S_GlobalManager.currentPhase == S_GlobalManager.CardsPhase.MatchFirstHalf && currentMinute > 45)
            textRef.text = "45 +" + (currentMinute - 45).ToString() + matchInfo;
        
        else if (currentMinute > 90)
            textRef.text = "90 +" + (currentMinute - 90).ToString() + matchInfo;

        else textRef.text = currentMinute.ToString() + matchInfo;

        yield break;
    }

    private float FindLerpSpeed(int currentMinute, int targetMinute) => Mathf.Max((currentMinute / (float)targetMinute) * 0.15f, 0.05f);
}
