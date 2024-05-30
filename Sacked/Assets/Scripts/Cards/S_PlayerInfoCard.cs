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
    public TextMeshProUGUI playerName;
    public TextMeshProUGUI nationality;
    public Image playerSkillRef;

    [Header("Face")]
    public Image skin;
    public Image eyeL;
    public Image eyeR;
    public Image eyebrowL;
    public Image eyebrowR;
    public Image nose;
    public Image hair;
    public Image mouth;
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

        
        playerName.text = playerInfo.playerName;
        roleRef.text = playerInfo.playerRole.ToString();
        traitRef.text = playerInfo.playerTraits[0].traitName.ToString().Replace('_',' ');
        playerSkillRef.fillAmount = (float)playerInfo.skillLevel / 5.0f;
        nationality.text = playerInfo.playerNationality.ToString();
        GenerateFace();
    }

    private void GenerateFace()
    {
        SO_Face face = playerInfo.playerFace;
        
        skin.sprite = face.skin;
        
        eyeL.sprite = face.eyes;
        eyeR.sprite = face.eyes;

        eyebrowR.sprite = face.eyebrows;
        eyebrowL.sprite = face.eyebrows;
        eyebrowL.color = face.eyebrowsColor;
        eyebrowR.color = face.eyebrowsColor;

        mouth.sprite = face.mouth;

        nose.sprite = face.nose;

        hair.sprite = face.hair;

        hair.color = face.hairColor;

    }
}
