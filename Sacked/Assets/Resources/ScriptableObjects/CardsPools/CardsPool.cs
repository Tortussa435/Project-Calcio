using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "New Cards Pool", menuName = "Cards/CardsPool")]
public class CardsPool : ScriptableObject
{
    public List<string> poolDirectories=new List<string>();
    public List<SO_CardData> cardsPool;
    public bool updateList;
    private void OnEnable()
    {
        if (updateList)
        {
            cardsPool = new List<SO_CardData>();
            foreach(string s in poolDirectories)
            {
                cardsPool.AddRange(Resources.LoadAll<SO_CardData>(s).ToList());
            }
            updateList = false;

            for(int i=cardsPool.Count-1;i>=0;i--)
            {
                if (cardsPool[i].scoreCard.Count == 0)
                {
                    try
                    {
                        if((cardsPool[i] as SO_MatchCardData).matchScoreCard.Count == 0)
                        {
                            cardsPool.RemoveAt(i);
                        }
                    }
                    catch
                    {
                        cardsPool.RemoveAt(i);
                    }
                }
            }
        }
    }



    [ContextMenu("Disable Already Picked")]
    public void DisableAlreadyPicked()
    {
        foreach(SO_CardData card in cardsPool)
        {
            card.alreadyPicked = false;
            card.totalAppearances = 0;
        }
    }
}
