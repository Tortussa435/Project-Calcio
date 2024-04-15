using System.Collections;
using System.Collections.Generic;
using System.Data;
using TMPro;
using UnityEngine;

public class S_Ladder : MonoBehaviour
{
    public SO_League league;
    public bool ladderVisible = false;

    [System.Serializable]
    public struct teamPosition
    {
        public SO_Team team;
        public int points;
    }
    static public List<teamPosition> leagueLadder;
    private void Awake()
    {
        leagueLadder = new List<teamPosition>();
        foreach (SO_Team team in league.teamlist)
        {
            teamPosition tp;
            tp.team = team;
            //tp.points = Random.Range(0,100);
            tp.points = 0;
            leagueLadder.Add(tp);
        }
    }
    public void ShowLadder()
    {
        if (ladderVisible)
        {
            ladderVisible = false;
            gameObject.SetActive(false);

            //REDO avoid recreating each time the ladder instances
            foreach (Transform child in gameObject.transform)
            {
                Destroy(child.gameObject);
            }
        }
        else
        {
            ladderVisible = true;
            gameObject.SetActive(true);
            QuickSortLadder(leagueLadder, 0, leagueLadder.Count - 1);
            for (int i = 0; i < leagueLadder.Count - 1; i++)
            {
                GameObject teambox = Resources.Load<GameObject>("Prefabs/Teambox");
                teambox = Instantiate(teambox, Vector3.zero, Quaternion.identity, gameObject.transform);
                teambox.transform.GetChild(0).GetComponent<TextMeshProUGUI>().SetText(leagueLadder[i].team.teamName);
                teambox.transform.GetChild(1).GetComponent<TextMeshProUGUI>().SetText(leagueLadder[i].points.ToString());
            }
        }
    }

    public static List<teamPosition> QuickSortLadder(List<teamPosition> ladder, int leftIndex, int rightIndex)
    {

        var i = leftIndex;
        var j = rightIndex;

        teamPosition pivot = ladder[leftIndex];

        while (i <= j)
        {
            while (ladder[i].points > pivot.points)
            {
                i++;
            }

            while (ladder[j].points < pivot.points)
            {
                j--;
            }

            if (i <= j)
            {
                teamPosition temp = ladder[i];
                ladder[i] = ladder[j];
                ladder[j] = temp;
                i++;
                j--;
            }
        }

        if (leftIndex < j)
        {
            QuickSortLadder(ladder, leftIndex, j);
        }

        if (i < rightIndex)
        {
            QuickSortLadder(ladder, i, rightIndex);
        }

        return ladder;
    }
    static public int LeagueRank(SO_Team team)
    {
        List<teamPosition> localLadder = QuickSortLadder(leagueLadder, 0, leagueLadder.Count - 1); //TODO there's something wrong in quicksorting
        for (int i = 0; i < localLadder.Count; i++)
        {
            //Debug.Log(localLadder[i].team.teamName);
            if (localLadder[i].team == team)
            {
                return i + 1;
            }
        }
        return -1;
    }

    static public void UpdateTeamPoints(SO_Team team, int points)
    {
        for (int i = 0; i < leagueLadder.Count; i++)
        {
            if (leagueLadder[i].team.teamName == team.teamName)
            {
                teamPosition newTeamPosition;
                newTeamPosition.team = team;
                newTeamPosition.points = leagueLadder[i].points + points;

                leagueLadder[i] = newTeamPosition;
            }
        }
    }
}
