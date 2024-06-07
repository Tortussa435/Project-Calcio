using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class S_TeamCard : S_Card
{
    public readonly float ELEVENSCALE = 1f;
    public readonly float BENCHSCALE = 0.75f;

    public GameObject inputLocker;

    public GameObject gkLine;
    public GameObject defLine;
    public GameObject midLine;
    public GameObject atkLine;
    public GameObject bench;

    private UnityAction LockInput;
    private UnityAction UnlockInput;
    private void Start()
    {        
        inputLocker.SetActive(!S_GlobalManager.canEditLineup);
    }

    public override void GenerateCardData(SO_CardData data)
    {
        cardData = data;
       
        cardData.onGeneratedEffects.Invoke();

        leftChoice.text = cardData.leftChoice;
        rightChoice.text = cardData.rightChoice;


        List<GameObject> gk = new List<GameObject>();
        List<GameObject> def = new List<GameObject>();
        List<GameObject> mid = new List<GameObject>();
        List<GameObject> atk = new List<GameObject>();
        List<GameObject> benchlist = new List<GameObject>();

        foreach (SO_PlayerData player in S_GlobalManager.squad.playingEleven)
        {


            GameObject p = Resources.Load<GameObject>(S_ResDirs.playerPrefab);
            p.GetComponent<S_PlayerIcon>().playerData = player;
            p = Instantiate(p, new Vector2(0, 0), Quaternion.identity, this.transform);
            p.GetComponent<RectTransform>().localPosition = new Vector2(0, 0);
            p.GetComponent<RectTransform>().localScale = new Vector2(ELEVENSCALE, ELEVENSCALE);
            switch (player.playerRole)
            {
                case SO_PlayerData.PlayerRole.Gk:
                    gk.Add(p);
                    break;

                case SO_PlayerData.PlayerRole.Def:
                    def.Add(p);
                    break;

                case SO_PlayerData.PlayerRole.Mid:
                    mid.Add(p);
                    break;

                case SO_PlayerData.PlayerRole.Atk:
                    atk.Add(p);
                    break;
            }

            gk[0].transform.SetParent(gkLine.transform);
            
            foreach(GameObject d in def)
            {
                d.transform.SetParent(defLine.transform);
            }

            foreach (GameObject m in mid)
            {
                m.transform.SetParent(midLine.transform);
            }

            foreach (GameObject a in atk)
            {
                a.transform.SetParent(atkLine.transform);
            }
        }
        
        foreach(SO_PlayerData player in S_GlobalManager.squad.bench)
        {
            GameObject p = Resources.Load<GameObject>(S_ResDirs.playerPrefab);
            p.GetComponent<S_PlayerIcon>().playerData = player;
            p = Instantiate(p, new Vector2(0, 0), Quaternion.identity, this.transform);
            p.GetComponent<RectTransform>().localPosition = new Vector2(0, 0);
            benchlist.Add(p);
            foreach(GameObject pl in benchlist)
            {
                pl.GetComponent<RectTransform>().localScale = new Vector2(BENCHSCALE,BENCHSCALE);
                pl.transform.SetParent(bench.transform);
            }
        }


    }

    public override void GoLeft()
    {        
        S_GlobalManager.squad.GenerateNextTacticTeamElevenCard(true);
    }
    public override void GoRight()
    {
        base.GoRight();
    }
}
