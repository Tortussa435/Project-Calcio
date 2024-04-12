using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(fileName = "New Team", menuName = "Teams/New Team")]
public class SO_Team : ScriptableObject
{
    public string teamName;
    public int SkillLevel;
    public Color teamColor1=Color.black;
    public Color teamColor2=Color.black;
    public Sprite teamLogo;

}

