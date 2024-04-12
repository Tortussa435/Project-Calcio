using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(Image))]
public class S_Card : MonoBehaviour
{
    public SO_CardData cardData;
    public TextMeshProUGUI cardDescription;
    public TextMeshProUGUI leftChoice;
    public TextMeshProUGUI rightChoice;
    public Image cardIcon;
    public Image cardBackground;


    public virtual void GenerateCardData(SO_CardData data)
    {
        cardData = data;
        //Debug.Log(cardData.cardDescription);
        cardDescription.text = S_GlobalManager.ReplaceVariablesInString(cardData.cardDescription);
        cardBackground.color = data.cardColor;
        leftChoice.text = S_GlobalManager.ReplaceVariablesInString(data.leftChoice);
        rightChoice.text = S_GlobalManager.ReplaceVariablesInString(data.rightChoice);
        cardIcon.sprite = data.cardIcon;
    }
    // Start is called before the first frame update
    void Awake()
    {
        cardBackground = GetComponent<Image>();
        cardDescription = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    

    
    public void GoLeft() {
        cardData.leftEffect();
    }
    public void GoRight() {
        cardData.rightEffect();
    }


}
