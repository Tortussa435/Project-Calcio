using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class S_PentagonInput : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerClickHandler
{
    private S_InputHandler ownerCard;
    private bool disableDrag;
    private float clickLength = 0.0f;
    private GameObject pentagonCardRef;

    public GameObject pentagonCardPrefab;
    
    private void Start()
    {
        ownerCard = transform.parent.GetComponent<S_InputHandler>(); //REDO trisnonno momento
        disableDrag = S_GlobalManager.IsMatchPlaying();
        S_PlayerMatchSimulator.OnMatchStart.AddListener(() => disableDrag = true);
        S_PlayerMatchSimulator.OnMatchEnd.AddListener(() => disableDrag = false);
    }

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
            OpenPentagonCard();
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

    public void OpenPentagonCard()
    {
        if (pentagonCardRef == null)
        {
            SO_CardData pentagonCard = ScriptableObject.CreateInstance<SO_CardData>();
            pentagonCard.decreaseCountDown = false;

            pentagonCardRef = S_GlobalManager.deckManagerRef.GenerateCard(pentagonCard, pentagonCardPrefab, false);

            S_GlobalManager.deckManagerRef.SetCardOnTop(pentagonCardRef);

            pentagonCard.rightEffects.AddListener(() => pentagonCardRef = null); //reference must be lost even before than card destruction

            Destroy(pentagonCardRef.transform.Find("Pentagon").GetComponent<S_PentagonInput>());
        }

        else Destroy(pentagonCardRef);

    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!disableDrag)
            ownerCard.OnBeginDrag(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!disableDrag)
            ownerCard.OnEndDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!disableDrag)
            ownerCard.OnDrag(eventData);
    }
}
