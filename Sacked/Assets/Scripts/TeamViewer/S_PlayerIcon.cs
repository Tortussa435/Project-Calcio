using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

public class S_PlayerIcon : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerClickHandler
{
    [HideInInspector] public SO_PlayerData playerData;
    public TextMeshProUGUI playerName;
    public Image playerIcon;
    public Image playerSkill;
    public Image playerEnergy;

    public S_InputHandler ownerCard;

    private float clickLength = 0.0f;
    private void Start()
    {
        if (playerData == null) return;

        GeneratePlayerFace();

        playerName.text = playerData.playerName;

        playerSkill.fillAmount = (float)playerData.skillLevel / 5.0f ;

        playerEnergy.fillAmount = (float) playerData.playerEnergy / 100.0f ;

        ownerCard = transform.parent.parent.parent.GetComponent<S_InputHandler>(); //REDO trisnonno momento
    }

    public void GeneratePlayerFace() { }

    public void OnPointerDown(PointerEventData eventData)
    {
        ownerCard.OnPointerDown(eventData);
        
        StartCoroutine("ClickTimer");
    }

    public void OnPointerClick(PointerEventData eventData)
    {

        StopCoroutine("ClickTimer");
        if (clickLength < 0.2f)
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
        Debug.Log("TODO openplayercard");
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        ownerCard.OnBeginDrag(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        ownerCard.OnEndDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        ownerCard.OnDrag(eventData);
    }
}
