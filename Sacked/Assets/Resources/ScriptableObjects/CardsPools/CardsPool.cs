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
