using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "New Team Trait", menuName = "Traits/New Team Trait")]
public class SO_TeamTrait : ScriptableObject
{
    [System.Serializable]
    public enum TraitNames
    {
        None,
        Catenaccio,
        Pressing,
        Speedsters,
        Dribblers,
        Tall,
        Small,
        Hot_Heads,
        Depressed,
        Short_Bench
    }

    public TraitNames traitName;
    public bool positiveTrait;
    public UnityEvent traitEffect;

    public List<TraitNames> excludedTraits;

    public string GetTraitName() => traitName.ToString().Replace('_',' ');

    //Team Traits Library
    public void Test()
    {
        Debug.Log("Test func");
    }
}
