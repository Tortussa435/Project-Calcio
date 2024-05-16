using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class S_PlayerIcon : MonoBehaviour
{
    [HideInInspector] public SO_PlayerData playerData;
    public TextMeshProUGUI playerName;
    public Image playerIcon;
    public Image playerSkill;
    
    private void Start()
    {
        if (playerData == null) return;

        GeneratePlayerFace();

        playerName.text = playerData.playerName;
        playerSkill.fillAmount = (float)playerData.skillLevel / 5.0f;
        
    }

    public void GeneratePlayerFace() { }

}
