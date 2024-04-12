using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Sacking Reason", menuName = "Cards/Sacking")]
public class SO_Sacking : SO_CardData
{
    public override void leftEffect()
    {
        Debug.Log("Zioperoid");
    }
    public override void rightEffect()
    {
        Debug.Log("Zioperongus");
    }
}
