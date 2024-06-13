using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class S_ImageHighlighter : MonoBehaviour
{
    private Image buttonImage;
    public Color highlightColor;
    public AnimationCurve highlightCurve;
    public float duration = 1.0f;
    // Start is called before the first frame update
    void Start()
    {
        buttonImage = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void HighlightButton()
    {
        Color startColor = buttonImage.color;
        StartCoroutine(HighlightImage(startColor));
    }

    IEnumerator HighlightImage(Color startColor)
    {
        float i = 0;
        while (i < 1.0f)
        {
            buttonImage.color = Color.Lerp(startColor, highlightColor, highlightCurve.Evaluate(i));
            i += Time.deltaTime / duration;
            yield return null;
        }
        yield break;
    }
}
