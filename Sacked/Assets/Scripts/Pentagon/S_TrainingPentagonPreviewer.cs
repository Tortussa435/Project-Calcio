using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using UnityEngine;

public class S_TrainingPentagonPreviewer : MonoBehaviour
{
    public S_TrainingPentagon pentagon;
    public RectTransform card;

    public string left;
    public string right;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float pos = NormalizedXPosition();
        
        pentagon.debugPentagonAtk = -1;
        pentagon.debugPentagonDef = -1;
        pentagon.debugPentagonChem = -1;
        pentagon.debugPentagonFK = -1;
        pentagon.debugPentagonRes = -1;
        
        if (pos < 0.025f && pos > -0.025f) return;
        if (pos > 0)
        {
            StatSwitch(left);
        }
        else if (pos < 0)
        {
            StatSwitch(right);
        }

        void StatSwitch(string s)
        {
            switch (s)
            {
                case "atk":
                    pentagon.debugPentagonAtk = S_PlayerTeamStats.GetAtkBoost() + 1;
                    break;
                case "def":
                    pentagon.debugPentagonDef = S_PlayerTeamStats.GetDefBoost() + 1;
                    break;
                case "chem":
                    pentagon.debugPentagonChem = S_PlayerTeamStats.GetChemistryBoost() + 1;
                    break;
                case "fk":
                    pentagon.debugPentagonFK = S_PlayerTeamStats.GetFreeKicksBoost() + 1;
                    break;
                case "res":
                    pentagon.debugPentagonRes = S_PlayerTeamStats.GetFitnessBoost() + 1;
                    break;
            }
        }

    }
    public float NormalizedXPosition() => card.anchoredPosition.x / Screen.width;
    
}
