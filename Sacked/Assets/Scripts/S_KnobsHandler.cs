using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static S_CoolFuncs;
public class S_KnobsHandler : MonoBehaviour
{
    private Image knob_president;
    private Image knob_money;
    private Image knob_team;
    private Image knob_supporters;

    private RectTransform president_rect;
    private RectTransform money_rect;
    private RectTransform team_rect;
    private RectTransform supporters_rect;

    private RectTransform rect;

    (bool president, bool money, bool team, bool supporters) leftValueChanges = (false, false, false, false);
    (bool president, bool money, bool team, bool supporters) rightValueChanges = (false, false, false, false);

    private void Awake()
    {
        knob_money = S_GlobalManager.deckManagerRef.Knob_Money.GetComponent<Image>();
        knob_president = S_GlobalManager.deckManagerRef.Knob_President.GetComponent<Image>();
        knob_team = S_GlobalManager.deckManagerRef.Knob_Team.GetComponent<Image>();
        knob_supporters = S_GlobalManager.deckManagerRef.Knob_Supporters.GetComponent<Image>();

        president_rect = knob_president.GetComponent<RectTransform>();
        money_rect = knob_money.GetComponent<RectTransform>();
        supporters_rect = knob_supporters.GetComponent<RectTransform>();
        team_rect = knob_team.GetComponent<RectTransform>();

        rect = GetComponent<RectTransform>();
        
    }
    private void Start()
    {
        SetValueChanges();
    }
    void SetValueChanges()
    {
        //should stay inverted, no questions allowed
        SO_CardData.ChangeValues right = GetComponent<S_Card>().cardData.leftValues;
        SO_CardData.ChangeValues left = GetComponent<S_Card>().cardData.rightValues;

        leftValueChanges.money = left.addedMoney != 0;
        leftValueChanges.president = left.addedPresident != 0;
        leftValueChanges.team = left.addedTeam != 0;
        leftValueChanges.supporters = left.addedSupporters != 0;

        rightValueChanges.money = right.addedMoney != 0;
        rightValueChanges.president = right.addedPresident != 0;
        rightValueChanges.team = right.addedTeam != 0;
        rightValueChanges.supporters = right.addedSupporters != 0;

        //destroy component if no value changes
        if (leftValueChanges == (false, false, false, false) && rightValueChanges == (false, false, false, false)) Destroy(this);

    }

    private void SetKnobOpacity(Image knob, bool left, bool right)
    {
        float dir = rect.anchoredPosition.x / Screen.width;
        if(dir > 0 && right) knob.color = ChangeAlpha(dir * 6,knob.color);
        if(dir < 0 && left) knob.color = ChangeAlpha(dir * -6, knob.color);
        if (dir == 0) knob.color = ChangeAlpha(0, knob.color);
    }

    private float SetKnobScale(RectTransform knob, int valueLeft, int valueRight)
    {
        int value = rect.anchoredPosition.x / Screen.width > 0 ? valueLeft : valueRight ;
        return Mathf.Lerp(0.5f, 2, ((float)Mathf.Abs(value) / 100.0f));
    }

    private void Update()
    {
        SO_CardData.ChangeValues right = GetComponent<S_Card>().cardData.leftValues;
        SO_CardData.ChangeValues left = GetComponent<S_Card>().cardData.rightValues;

        if (knob_money != null)
        {
            SetKnobOpacity(knob_money, leftValueChanges.money, rightValueChanges.money);
        }

        float moneySize = SetKnobScale(money_rect, (int)left.addedMoney, (int)right.addedMoney);
        money_rect.localScale = V3Lerpoler(money_rect.localScale, new Vector3(moneySize,moneySize,moneySize));

        if (knob_president != null)
        {
            SetKnobOpacity(knob_president, leftValueChanges.president, rightValueChanges.president);
        }

        float presidentSize = SetKnobScale(president_rect, (int)left.addedPresident, (int)right.addedPresident);
        president_rect.localScale = V3Lerpoler(president_rect.localScale, new Vector3(presidentSize, presidentSize, presidentSize));

        if (knob_team != null)
        {
            SetKnobOpacity(knob_team, leftValueChanges.team, rightValueChanges.team);
        }

        float teamSize = SetKnobScale(team_rect, (int)left.addedTeam, (int)right.addedTeam);
        team_rect.localScale = V3Lerpoler(team_rect.localScale, new Vector3(teamSize, teamSize, teamSize));

        if (knob_supporters != null)
        {
            SetKnobOpacity(knob_supporters, leftValueChanges.supporters, rightValueChanges.supporters);
        }

        float supportersSize = SetKnobScale(supporters_rect, (int)left.addedSupporters, (int)right.addedSupporters);
        supporters_rect.localScale = V3Lerpoler(supporters_rect.localScale, new Vector3(supportersSize, supportersSize, supportersSize));
    }

    private void OnDestroy()
    {
        knob_money.color = ChangeAlpha(0, knob_money.color);
        knob_president.color = ChangeAlpha(0, knob_president.color);
        knob_team.color = ChangeAlpha(0, knob_team.color);
        knob_supporters.color = ChangeAlpha(0, knob_supporters.color);
    }
    
}
