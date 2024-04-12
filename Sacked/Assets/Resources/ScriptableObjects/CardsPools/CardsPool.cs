using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Cards Pool", menuName = "Cards/CardsPool")]
public class CardsPool : ScriptableObject
{
   public SO_CardData[] cardsPool;
}
