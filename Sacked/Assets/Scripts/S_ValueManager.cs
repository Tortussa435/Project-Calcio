using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class S_ValueManager : MonoBehaviour
{
    private UnityAction UpdateCurrency;
    private Image ValueIcon;
    private float targetValue;

    private bool lerpingValue=false;

    private string MoneySackingDirectory="ScriptableObjects/Sacking/Sacking_LowMoney";
    private string PresidentSackingDirectory= "ScriptableObjects/Sacking/Sacking_LowPresident";
    private string TeamSackingDirectory = "ScriptableObjects/Sacking/Sacking_LowTeam";
    private string SupportersSackingDirectory = "ScriptableObjects/Sacking/Sacking_LowSupporters";

    public Color increaseColor;
    public Color decreaseColor;
    public Color stillColor;

    [System.Serializable]
    public enum Currencies
    {
        Money, President, Team, Supporters
    }
    public Currencies currencyToUpdate;

    private void Awake()
    {
        ValueIcon = GetComponent<Image>();
        UpdateCurrency = new UnityAction(UpdateValue);
        switch (currencyToUpdate)
        {
            case Currencies.Money:
                if (S_GlobalManager.OnUpdateMoney == null) S_GlobalManager.OnUpdateMoney = new UnityEvent();
                S_GlobalManager.OnUpdateMoney.AddListener(UpdateCurrency);
                break;

            case Currencies.President:
                if (S_GlobalManager.OnUpdatePresident == null) S_GlobalManager.OnUpdatePresident = new UnityEvent();
                S_GlobalManager.OnUpdatePresident.AddListener(UpdateCurrency);
                break;

            case Currencies.Team:
                if (S_GlobalManager.OnUpdateTeam == null) S_GlobalManager.OnUpdateTeam = new UnityEvent();
                S_GlobalManager.OnUpdateTeam.AddListener(UpdateCurrency);
                break;

            case Currencies.Supporters:
                if (S_GlobalManager.OnUpdateSupporters == null) S_GlobalManager.OnUpdateSupporters = new UnityEvent();
                S_GlobalManager.OnUpdateSupporters.AddListener(UpdateCurrency);
                break;
        }
        
    }

    public void UpdateValue()
    {
        string ending="";
        float target = 0;
        switch (currencyToUpdate)
        {
            case Currencies.Money:
                target = S_GlobalManager.Money;
                ending = MoneySackingDirectory;
                break;
            case Currencies.President:
                ending = PresidentSackingDirectory;
                target = S_GlobalManager.President;
                break;
            case Currencies.Team:
                ending = TeamSackingDirectory;
                target = S_GlobalManager.Team;
                break;
            case Currencies.Supporters:
                ending = SupportersSackingDirectory;
                target = S_GlobalManager.Supporters;
                break;
        }

        if (Mathf.Abs(target - targetValue) < 1) return; //Checks if movement difference is very low
        
        targetValue = target;

        //generate ending if value reaches 0
        if (targetValue == 0 && !S_GlobalManager.deckManagerRef.DebugImmortal) //REDO remove this check in published version
        {
            SO_Sacking sackingReason = Resources.Load<SO_Sacking>(ending);
            S_GlobalManager.deckManagerRef.AddCardToDeck(sackingReason, 0);
            S_GlobalManager.deckManagerRef.GenerateCard();
            S_GlobalManager.sacked = true;
        }
        if (lerpingValue) StopCoroutine("LerpUpdateValue");
        StartCoroutine("LerpUpdateValue");
        
    }

    public IEnumerator LerpUpdateValue()
    {
        if (Mathf.Abs(ValueIcon.fillAmount - (targetValue / 100)) < .001f) yield break; //if difference between current and target is almost none, skip all
        
        lerpingValue = true;
        Color startColor = ValueIcon.color;
        bool direction = ValueIcon.fillAmount > targetValue / 100;
        float lerpoler = 0;
        Color targetColor = direction ? decreaseColor : increaseColor;
        
        float damper=0.5f; //value used by the smoothdamp function

        while (lerpoler < 1)
        {
            ValueIcon.fillAmount = Mathf.SmoothDamp(ValueIcon.fillAmount, targetValue/100f ,ref damper, .25f);

            ValueIcon.color = Color.Lerp(targetColor, startColor, lerpoler);
            lerpoler += Time.deltaTime;
            yield return null;
        }
        
        lerpingValue = false;
        ValueIcon.color = stillColor;
        yield break;

        /* OLD VERSION
        while (ValueIcon.fillAmount>targetValue/100f)
        {
            ValueIcon.fillAmount -= Time.deltaTime;
            ValueIcon.color = decreaseColor;

            if (ValueIcon.fillAmount < targetValue / 100f)
            {
                ValueIcon.fillAmount = targetValue / 100f;
            }

            yield return null;
        }
        while (ValueIcon.fillAmount < targetValue / 100f)
        {
            ValueIcon.fillAmount += Time.deltaTime;
            ValueIcon.color = increaseColor;
            
            if (ValueIcon.fillAmount > targetValue / 100f)
            {
                ValueIcon.fillAmount = targetValue / 100f;
            }

            yield return null;
        }

        ValueIcon.color = stillColor;
        */
    }
}
