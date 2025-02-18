using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Player", menuName = "Players/Player")]
public class SO_PlayerData : ScriptableObject
{
    public string playerName;
    public enum Nationality {
        Argentina,
        France,
        Belgium,
        England,
        Brazil,
        Portugal,
        Netherlands,
        Spain,
        Italy,
        Croatia,
        USA,
        Colombia,
        Morocco,
        Mexico,
        Uruguay,
        Germany,
        Senegal,
        Japan,
        Switzerland,
        Iran,
        Denmark,
        Ukraine,
        SouthKorea,
        Australia,
        Austria,
        Hungary,
        Sweden,
        Poland,
        Wales,
        Nigeria,
        VaticanCity,
        Albania,
        Romania
    }
    public enum PlayerRole { Gk,Def,Mid,Atk };

    public Nationality playerNationality;
    public PlayerRole playerRole;
    public int skillLevel;
    public List<SO_PlayerTrait> playerTraits=new List<SO_PlayerTrait>();

    public SO_Face playerFace;

    public float playerEnergy = 100f;

    public int expelled;
    public int injuried=0;
    public bool subbedOut = false;
    public void AddEnergy(float min, float max)
    {
        //energy consumed by gk is reduced a lot
        if (playerRole == PlayerRole.Gk && min < 0 && max < 0)
        {
            min /= 10;
            max /= 10;
        }

        playerEnergy += Random.Range(min, max);
        playerEnergy = Mathf.Clamp(playerEnergy, 0, 100);
    }

    public bool CanPlay() => !(expelled > 0 || injuried > 0 || subbedOut);

    public int GetPlayerCurrentSkillLevel()=> Mathf.Clamp(skillLevel * Mathf.CeilToInt(playerEnergy / 100),1,5);

}
