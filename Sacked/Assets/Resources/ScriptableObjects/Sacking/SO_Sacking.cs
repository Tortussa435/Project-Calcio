using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "New Sacking Reason", menuName = "Cards/Sacking")]
public class SO_Sacking : SO_CardData
{
    public override void leftEffect()
    {
        ReloadGame();
    }
    public override void rightEffect()
    {
        ReloadGame();
    }

    private void ReloadGame()
    {
        S_GlobalManager.deckManagerRef.ResetStaticClasses();
        SceneManager.LoadScene(0,LoadSceneMode.Single);
    }
}
