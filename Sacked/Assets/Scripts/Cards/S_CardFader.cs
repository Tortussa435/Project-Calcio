using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class S_CardFader : MonoBehaviour
{
    private S_Card cardRef;
    public float fadeDuration = 0.5f;
    public Image fadeRef;
    // Start is called before the first frame update
    void Start()
    {
        cardRef = GetComponent<S_Card>();
        StartCoroutine(FadeCard());
    }

    IEnumerator FadeCard()
    {
        float t = 0;
        Color c= fadeRef.color;
        while (t < fadeDuration)
        {
            float alpha = 1 - Mathf.InverseLerp(0,fadeDuration,t);
            c = fadeRef.color;
            fadeRef.color = new Color(c.r,c.g,c.b, alpha);
            t += Time.deltaTime;
            yield return null;
        }
        fadeRef.color=new Color(c.r, c.g, c.b, 0);
        yield break;
    }
}
