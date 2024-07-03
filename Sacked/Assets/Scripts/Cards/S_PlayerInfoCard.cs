using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;

public class S_PlayerInfoCard : S_Card
{
    [Header("Images Handle")]
    public Sprite injuryIcon;
    public Sprite redCardIcon;

    [Header("Refs")]
    public TextMeshProUGUI roleRef;
    public TextMeshProUGUI traitRef;
    public TextMeshProUGUI playerName;
    public TextMeshProUGUI nationality;
    public Image playerSkillRef;
    public Image UnavIcon;
    public TextMeshProUGUI UnavText;
    [HideInInspector] public SO_PlayerData playerInfo;

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

    public void GeneratePlayerInfoData(SO_PlayerData player)
    {
        playerInfo = player;

        
        playerName.text = playerInfo.playerName;
        roleRef.text = playerInfo.playerRole.ToString();
        
        traitRef.text = playerInfo.playerTraits[0].traitName.ToString().Replace('_',' ');
        Color traitColor=Color.black;
        switch (playerInfo.playerTraits[0].positiveTrait)
        {
            case S_FootballEnums.Positivity.Positive:
                traitColor = Color.green*0.5f;
                break;
            case S_FootballEnums.Positivity.Neutral:
                traitColor = Color.black;
                break;
            case S_FootballEnums.Positivity.Negative:
                traitColor = Color.red;
                break;
        }
        traitRef.color = traitColor;

        playerSkillRef.fillAmount = (float)playerInfo.skillLevel / (float)S_GlobalManager.MAXTEAMSKILLLEVEL;
        nationality.text = playerInfo.playerNationality.ToString();
        GenerateFace();
        SetUnavInfo();
    }

    private void SetUnavInfo()
    {
        if (!playerInfo.CanPlay())
        {
            if (playerInfo.expelled > 0)
            {
                UnavIcon.sprite = redCardIcon;
                UnavText.text = playerInfo.expelled.ToString();
            }
            else if (playerInfo.injuried > 0)
            {
                UnavIcon.sprite = injuryIcon;
                UnavText.text = playerInfo.injuried.ToString();
            }
        }
        else
        {
            UnavIcon.gameObject.SetActive(false);
            UnavText.gameObject.SetActive(false);
        }
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
