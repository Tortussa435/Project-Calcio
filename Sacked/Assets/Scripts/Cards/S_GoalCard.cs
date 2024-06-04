using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class S_GoalCard : S_Card
{
    public TextMeshProUGUI goalDescription;
    public override void GenerateCardData(SO_CardData data)
    {
        base.GenerateCardData(data);
        goalDescription.text = (data as SO_GoalCardData).goalDescription;
    }
    public override void RefreshCardData(SO_CardData data)
    {
        base.RefreshCardData(data);
        goalDescription.text = (data as SO_GoalCardData).goalDescription;
    }

}
