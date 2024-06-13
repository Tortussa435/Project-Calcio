using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.VisualScripting;
using System;

public class S_PlayerIcon : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerClickHandler
{
    [Header("Face Parts")]
    public Image Skin;
    public Image EyeL;
    public Image EyeR;
    public Image EyebrowL;
    public Image EyebrowR;
    public Image Mouth;
    public Image Hair;
    public Image Nose;




    [Header("Object References")]
    public TextMeshProUGUI playerName;
    public Image playerIcon;
    public Image playerSkill;
    public Image playerEnergy;
    public Image problemIcon;
    [HideInInspector] public SO_PlayerData playerData;

    [HideInInspector] public GameObject playerCardRef;
    public GameObject playerCardPrefab;

    [HideInInspector] public bool forceSpawn;

    public S_InputHandler ownerCard;

    private float clickLength = 0.0f;
    
    [Header("Icons")]
    public Sprite injuryIcon;
    public Sprite expelledIcon;

    private bool disableDrag = false;

    private void Start()
    {
        if (playerData == null) return;

        GeneratePlayerFace();

        playerName.text = playerData.playerName;

        playerSkill.fillAmount = (float)playerData.skillLevel / 5.0f ;

        if (playerData.expelled > 0 || playerData.injuried>0)
        {
            problemIcon.gameObject.SetActive(true);
            playerEnergy.transform.parent.gameObject.SetActive(false);
            if (playerData.injuried > 0)
            {
                problemIcon.sprite = injuryIcon;
            }
            if (playerData.expelled > 0)
            {
                problemIcon.sprite = expelledIcon;
            }
        }
        else
        {
            problemIcon.gameObject.SetActive(false);
            playerEnergy.transform.parent.gameObject.SetActive(true);
            playerEnergy.fillAmount = (float) playerData.playerEnergy / 100.0f ;
        }

        ownerCard = transform.parent.parent.parent.GetComponent<S_InputHandler>(); //REDO trisnonno momento

        disableDrag = !S_GlobalManager.canEditLineup;
    }

    public void GeneratePlayerFace()
    {
        SO_Face face = playerData.playerFace;
        
        Skin.sprite = face.skin;

        EyeL.sprite = face.eyes;
        EyeR.sprite = face.eyes;

        EyebrowL.sprite = face.eyebrows;
        EyebrowL.color = face.eyebrowsColor;
        EyebrowR.sprite = face.eyebrows;
        EyebrowR.color = face.eyebrowsColor;

        Hair.sprite = face.hair;
        Hair.color = face.hairColor;

        Nose.sprite = face.nose;

        Mouth.sprite = face.mouth;
    }

    public void OnPointerDown(PointerEventData eventData)
    {

        ownerCard.OnPointerDown(eventData);
        
        StartCoroutine("ClickTimer");
    }

    public void OnPointerClick(PointerEventData eventData)
    {

        StopCoroutine("ClickTimer");
        if (clickLength < 0.1f)
        {
            OpenPlayerCard();
        }
        clickLength = 0;
    }

    IEnumerator ClickTimer()
    {
        while (true)
        {
            clickLength += Time.deltaTime;
            yield return null;
        }
    }

    public void OpenPlayerCard()
    {
        if (playerCardRef == null || forceSpawn)
        {
            SO_CardData data = ScriptableObject.Instantiate(Resources.Load<SO_CardData>(S_ResDirs.playerCardDir));
            data.decreaseCountDown = false;

            playerCardRef = S_GlobalManager.deckManagerRef.GenerateCard(data, playerCardPrefab, false);

            playerCardRef.GetComponent<S_PlayerInfoCard>().GeneratePlayerInfoData(playerData);

            S_GlobalManager.deckManagerRef.SetCardOnTop(playerCardRef);

            data.rightEffects.AddListener(() => playerCardRef = null); //reference must be lost even before than card destruction
        }

        else Destroy(playerCardRef);
        
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if(!disableDrag)
            ownerCard.OnBeginDrag(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if(!disableDrag)
            ownerCard.OnEndDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("Grodolo");
        if(!disableDrag)
            ownerCard.OnDrag(eventData);
    }
}
