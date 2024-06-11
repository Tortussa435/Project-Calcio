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
    bool triggered = false;
    public virtual void GenerateCardData(SO_CardData data)
    {
        cardData = data;

        data.ownerCard = gameObject;
        //Debug.Log(cardData.cardDescription);

        //Debug.Log(data.cardDescriptions.Count + "è la contas");
        string cardText;
        if (data.cardDescriptions.Count > 0) cardText = data.cardDescriptions[Random.Range(0, data.cardDescriptions.Count)];
        
        else
        {
            Debug.LogWarning("This card has no default descriptions");
            cardText = "Ti sei scordato di mettere la descrizione a questa carta brutto coglione";
        }

        if(cardDescription!=null) cardDescription.text = S_GlobalManager.ReplaceVariablesInString(cardText);
        
        cardBackground.color = data.cardColor;
        if(leftChoice!=null) leftChoice.text = S_GlobalManager.ReplaceVariablesInString(data.leftChoice);
        if(rightChoice!=null) rightChoice.text = S_GlobalManager.ReplaceVariablesInString(data.rightChoice);
        if(cardIcon!=null) cardIcon.sprite = data.cardIcon;
        
        cardData.onGeneratedEffects.Invoke();
        
        cardData.SetCardAlreadyPicked();
        cardData.totalAppearances++;

        name = cardData.cardName;
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
    

    
    public virtual void GoLeft() {
        if (triggered) return;
        
        cardData.leftEffect();
        triggered = true;
    }
    public virtual void GoRight() {
        if (triggered) return;

        cardData.rightEffect();
        triggered = true;
    }

    public virtual void RefreshCardData(SO_CardData data)
    {
        cardData = data;

        data.ownerCard = gameObject;

        string cardText = data.cardDescriptions[Random.Range(0, data.cardDescriptions.Count)];
        cardDescription.text = S_GlobalManager.ReplaceVariablesInString(cardText);

        cardBackground.color = data.cardColor;
        if (leftChoice != null) leftChoice.text = S_GlobalManager.ReplaceVariablesInString(data.leftChoice);
        if (rightChoice != null) rightChoice.text = S_GlobalManager.ReplaceVariablesInString(data.rightChoice);
        cardIcon.sprite = data.cardIcon;

    }
    private void OnDestroy()
    {
        cardData.leftEffects.RemoveAllListeners();
        cardData.rightEffects.RemoveAllListeners();
    }
}
