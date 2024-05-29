using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Face", menuName = "Faces/New Face")]
public class SO_Face : ScriptableObject
{
    public Sprite eyebrows;
    public Sprite eyes;
    public Sprite mouth;
    public Sprite nose;
    public Sprite hair;
    public Sprite skin;

    public Color eyebrowsColor;
    public Color hairColor;

    public void MakeFace(SO_PlayerData.Nationality nationality)
    {
        (Sprite face, Sprite hair, Sprite eyebrows, Sprite eyes, Sprite mouth, Sprite nose) face = S_FacesDatabase.GenerateFace();
        
        eyebrows = face.eyebrows;
        eyes = face.eyes;
        mouth = face.mouth;
        nose = face.nose;
        hair = face.hair;
        skin = face.face;

        eyebrowsColor = Random.ColorHSV();
        hairColor = Random.ColorHSV();

    }
}
