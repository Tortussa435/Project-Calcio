using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_TutorialHint : MonoBehaviour
{
    S_InputHandler ih;
    private float startAfterSeconds = 5;
    // Start is called before the first frame update
    void Start()
    {
        ih = GetComponent<S_InputHandler>();
        StartCoroutine(WaitPlayerInput());
        ih.OnCardSwiped.AddListener(() => Destroy(this));
    }

    IEnumerator WaitPlayerInput()
    {

        yield return new WaitForSeconds(startAfterSeconds);
        RectTransform rt = GetComponent<RectTransform>();
        Vector2 cardVelocity = Vector2.zero;
        bool reset = false;
        Vector2 dest = rt.anchoredPosition;
        float time = 0;
        while (true)
        {
            
            if (ih.totalMovement>1000)
            {
                ih.overrideMovement = false;
                Destroy(this);
                yield break;
            }

            //Animate Movement
            if (!ih.isCardDragged)
            {
                time += Time.deltaTime;

                if (reset)
                {
                    ih.overrideMovement = false;
                    reset = false;
                    yield return new WaitForSeconds(startAfterSeconds/2);
                }

                else
                {
                    ih.overrideMovement = true;
                }

                float direction = (int)(time/2) % 2 == 0 ? 1 : -1;

                rt.anchoredPosition = Vector2.SmoothDamp(rt.anchoredPosition, dest + new Vector2(500,0) * direction , ref cardVelocity, 0.3f);
                
            }

            else
            {
                ih.overrideMovement = false;
                reset = true;
                time = 0;
            }

            yield return null;
        }
    }
}
