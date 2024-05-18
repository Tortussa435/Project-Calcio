using System.Collections;
using System.Collections.Generic;
using System.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
            for (int i = 0; i < leagueLadder.Count; i++)
            {
                GameObject teambox = Resources.Load<GameObject>("Prefabs/Teambox");
                teambox = Instantiate(teambox, Vector3.zero, Quaternion.identity, gameObject.transform);
                teambox.transform.GetChild(0).GetComponent<TextMeshProUGUI>().SetText(leagueLadder[i].team.teamName);
                teambox.transform.GetChild(1).GetComponent<TextMeshProUGUI>().SetText(leagueLadder[i].points.ToString());
                switch (i+1)
                {
                    default:
                        teambox.GetComponent<Image>().color = Color.gray;
                        break;
                    case 1:
                        teambox.GetComponent<Image>().color = Color.cyan;
                        break;
                    case int n when n <= 7:
                        teambox.GetComponent<Image>().color = Color.green;
                        break;
                    case int n when n >= 18:
                        teambox.GetComponent<Image>().color = Color.red;
                        break;
                }
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
    static public int FindLeagueRank(SO_Team team)
    {
        List<teamPosition> localLadder = QuickSortLadder(leagueLadder, 0, leagueLadder.Count - 1); //REDO there's something wrong in quicksorting
        for (int i = 0; i < localLadder.Count; i++)
        {
            if (localLadder[i].team.teamName == team.teamName)
            {
                //Debug.Log(localLadder[i].team.teamName);
                return i + 1;
            }
        }
        return -1;
    }

    static public int FindPointsAtLadderPosition(int position) => QuickSortLadder(leagueLadder, 0, leagueLadder.Count - 1)[position].points;

    static public int FindTeamPoints(SO_Team team)
    {
        List<teamPosition> localLadder = QuickSortLadder(leagueLadder, 0, leagueLadder.Count - 1); //REDO there's something wrong in quicksorting
        for (int i = 0; i < localLadder.Count; i++)
        {
            if (localLadder[i].team.teamName == team.teamName)
            {
                return localLadder[i].points;
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

    static public void ClearLadder()
    {
        for(int i = 0; i < leagueLadder.Count; i++)
        {
            teamPosition newTeamPosition;
            newTeamPosition.team = leagueLadder[i].team;
            newTeamPosition.points = 0;
            leagueLadder[i] = newTeamPosition;
        }
    }
}
