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

        data.ownerCard = gameObject;
        //Debug.Log(cardData.cardDescription);

        //Debug.Log(data.cardDescriptions.Count + "è la contas");
        string cardText = data.cardDescriptions[Random.Range(0, data.cardDescriptions.Count)];
        cardDescription.text = S_GlobalManager.ReplaceVariablesInString(cardText);
        
        cardBackground.color = data.cardColor;
        if(leftChoice!=null) leftChoice.text = S_GlobalManager.ReplaceVariablesInString(data.leftChoice);
        if(rightChoice!=null) rightChoice.text = S_GlobalManager.ReplaceVariablesInString(data.rightChoice);
        cardIcon.sprite = data.cardIcon;
        
        cardData.onGeneratedEffects.Invoke();

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
