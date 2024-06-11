using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public static class S_FacesDatabase
{
    static string facesDir = "Sprites/Faces/Skin";
    static string hairDir = "Sprites/Faces/Hair";
    static string eyebrowsDir = "Sprites/Faces/Face/Eyebrows";
    static string eyesDir = "Sprites/Faces/Face/Eyes";
    static string mouthDir = "Sprites/Faces/Face/Mouth";
    static string noseDir = "Sprites/Faces/Face/Nose";

    static List<Sprite> faces;
    static List<Sprite> hairs;
    static List<Sprite> eyebrows;
    static List<Sprite> eyes;
    static List<Sprite> mouths;
    static List<Sprite> noses;
    static S_FacesDatabase()
    {
        faces = Resources.LoadAll<Sprite>(facesDir).ToList();
        hairs = Resources.LoadAll<Sprite>(hairDir).ToList();
        eyebrows = Resources.LoadAll<Sprite>(eyebrowsDir).ToList();
        eyes = Resources.LoadAll<Sprite>(eyesDir).ToList();
        mouths = Resources.LoadAll<Sprite>(mouthDir).ToList();
        noses = Resources.LoadAll<Sprite>(noseDir).ToList();
    }

    public static (Sprite face,Sprite hair,Sprite eyebrows, Sprite eyes, Sprite mouth, Sprite nose) GenerateFace()
    {
        (Sprite face, Sprite hair, Sprite eyebrows, Sprite eyes, Sprite mouth, Sprite nose) fullFace;
        
        fullFace.face = faces[Random.Range(0, faces.Count)];

        //bald player chance
        int hairSeed = Random.Range(0, hairs.Count + 1);
        if (hairSeed != hairs.Count)
            fullFace.hair = hairs[Random.Range(0, hairs.Count)];
        else fullFace.hair = null;
        //------------------

        fullFace.eyebrows = eyebrows[Random.Range(0, eyebrows.Count)];
        fullFace.eyes = eyes[Random.Range(0, eyes.Count)];
        fullFace.mouth = mouths[Random.Range(0, mouths.Count)];
        fullFace.nose = noses[Random.Range(0, noses.Count)];
        
        return fullFace;
    }
}
