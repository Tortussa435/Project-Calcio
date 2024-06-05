using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_PentagonCardInfo : S_Card
{
    public override void GoLeft()
    {
    }

    public override void GoRight()
    {

    }
    public override void GenerateCardData(SO_CardData data)
    {
        void DestroyOnToggle(bool Toggle)
        {
            if (!Toggle)
            {
                Destroy(gameObject);
            }
        }

        cardData = data;
        S_GlobalManager.squad.OnToggleSquadViewer.AddListener(DestroyOnToggle);
    }

}
