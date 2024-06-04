using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;


public class S_InputHandler : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{


    public bool isCardDragged = false;
    [HideInInspector] public float totalMovement = 0; //gives how much distance the card has made

    //the card is going left or right
    private enum Direction { Center, Left, Right };
    private Direction cardDirection = Direction.Center;
    
    private RectTransform rectTransform;
    private S_Card cardRef;

    private Vector3 targetRotation;
    private float maxCardRotation = 60f;
    private float cardYBound = 0.0f;
    private float cardXBound = 0.0f;

    private Vector2 cardVelocity=Vector2.one; //used to smoothdamp the card position
    
    [HideInInspector] public bool overrideMovement = false;

    public UnityEvent OnCardSwiped=new UnityEvent();

    public void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        cardRef = GetComponent<S_Card>();
        cardYBound = Screen.height * 0.165f;
        cardXBound = Screen.width * 0.66f;

    }
    public void OnDrag(PointerEventData eventData)
    {
        //REDO - Cards keep moving on x and y endlessly, creepy!
        if (cardDirection!=Direction.Center) return;

        if (!overrideMovement)
        {
            rectTransform.anchoredPosition = Vector2.SmoothDamp(rectTransform.anchoredPosition,rectTransform.anchoredPosition+eventData.delta,ref cardVelocity, 0.0025f);

            rectTransform.anchoredPosition =
                new Vector2(
                    Mathf.Clamp(rectTransform.anchoredPosition.x, -cardXBound, cardXBound),
                    Mathf.Clamp(rectTransform.anchoredPosition.y, -cardYBound, cardYBound)
                );
        }


        rectTransform.eulerAngles = new Vector3(
            0,
            0,
            Mathf.LerpUnclamped(
                0,
                -maxCardRotation,
                GetNormalizedCardXPosition()
                )
         );

        if(cardRef.leftChoice!=null) cardRef.leftChoice.alpha = GetNormalizedCardXPosition()*6;
        if (cardRef.rightChoice != null) cardRef.rightChoice.alpha = GetNormalizedCardXPosition()*-6;


        totalMovement += eventData.delta.magnitude;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {

    }
    public void OnEndDrag(PointerEventData eventData) 
    {
        if (GetNormalizedCardXPosition() > 0.25f)
        {
            cardDirection = Direction.Left;
            try { Destroy(GetComponent<S_KnobsHandler>()); }
            catch { }
            StartCoroutine("SwipeCard");
            cardRef.GoLeft();
        }
        else if (GetNormalizedCardXPosition() < -0.25f)
        {
            cardDirection = Direction.Right;
            try { Destroy(GetComponent<S_KnobsHandler>()); }
            catch { }
            StartCoroutine("SwipeCard");
            cardRef.GoRight();
        }
        else isCardDragged = false;
    }   
    
    public void OnPointerDown(PointerEventData eventData)
    {
        if (cardDirection!=Direction.Center) return;
        isCardDragged = true;
    }

    private void Update()
    {
        //moves card back to center if not dragged enough
        if (!isCardDragged)
        {
            rectTransform.anchoredPosition = Vector2.Lerp(rectTransform.anchoredPosition, new Vector2(0, 0) + S_GlobalManager.CardsSpawnOffset, Time.deltaTime * 10);
            rectTransform.eulerAngles = new Vector3(
            0,
            0,
            Mathf.LerpUnclamped(
                0,
                -maxCardRotation,
                GetNormalizedCardXPosition()
                )
            );
            if(cardRef.leftChoice!=null) cardRef.leftChoice.alpha = GetNormalizedCardXPosition() * 6;
            if(cardRef.rightChoice!=null) cardRef.rightChoice.alpha = GetNormalizedCardXPosition() * -6;

        }

        
    }

    public float GetNormalizedCardXPosition()
    {
        return rectTransform.anchoredPosition.x / Screen.width;
    }

    IEnumerator SwipeCard()
    {
        OnCardSwiped.Invoke();
        Vector2 cardSpeed = Vector2.one;
        Vector2 direction = cardDirection == Direction.Left ? new Vector2(Screen.width * 1.25f, 0) : new Vector2(-Screen.width * 1.25f, 0);
        //moves the card totally to the left/right if card dragged enough in that direction
        while ( (rectTransform.anchoredPosition - direction).sqrMagnitude > 0.05f) //REDO not really a good check to see if the card reached the border
        {
            rectTransform.anchoredPosition = Vector2.SmoothDamp(
                rectTransform.anchoredPosition,
                direction,
                ref cardSpeed,
                0.3f);
            
            
            yield return null;
        }
        

        Destroy(gameObject);
        yield break;

    }
}
