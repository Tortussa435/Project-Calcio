using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class S_MatchIconsHandler : MonoBehaviour
{
    [SerializeField] GameObject homeIcons;
    [SerializeField] GameObject awayIcons;

    public Sprite redCard;
    public Sprite substitution;

    private List<GameObject> icons = new List<GameObject>();
    public void AddRedCard(bool homeTeam) => GenerateIcon(redCard, homeTeam, 0);
    public void AddSubstitution(bool homeTeam) => GenerateIcon(substitution, homeTeam, 1);
    public void ClearIcons()
    {
        foreach(GameObject go in icons) Destroy(go);
        icons.Clear();
    }
    private void GenerateIcon(Sprite icon,bool home, int child)
    {
        GameObject go=new GameObject();
        Image i = go.AddComponent<Image>();
        i.sprite = icon;
        Transform parent;
        parent = home ? homeIcons.transform.GetChild(child) : awayIcons.transform.GetChild(child);
        GameObject result = Instantiate(go, Vector3.zero, Quaternion.identity, parent);
        icons.Add(result);
    }
    private void Start()
    {
        S_PlayerMatchSimulator.iconsHandler = this;
        S_PlayerMatchSimulator.OnMatchEnd.AddListener(ClearIcons);

        S_PlayerMatchSimulator.OnRedCard.AddListener(AddRedCard);

        S_PlayerMatchSimulator.OnSubstitution.AddListener(AddSubstitution);
    }

}
