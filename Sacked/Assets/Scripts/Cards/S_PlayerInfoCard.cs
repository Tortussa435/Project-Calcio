using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class S_PlayerInfoCard : S_Card
{
    [HideInInspector] public SO_PlayerData playerInfo;
    public TextMeshProUGUI roleRef;
    public TextMeshProUGUI traitRef;
    public Image playerSkillRef;
    public override void GoLeft()
    {

    }
    public override void GoRight()
    {

    }
    public override void GenerateCardData(SO_CardData data)
    {
        cardData = data;
    }

    public void GeneratePlayerInfoData(SO_PlayerData player)
    {
        playerInfo = player;

        cardDescription.text = playerInfo.playerName;
        roleRef.text = playerInfo.playerRole.ToString();
        traitRef.text = playerInfo.playerTraits[0].traitName.ToString().Replace('_',' ');
        playerSkillRef.fillAmount = (float)playerInfo.skillLevel / 5.0f;
    }
}
