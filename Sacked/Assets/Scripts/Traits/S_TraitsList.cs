using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class S_TraitsList
{
    public static List<SO_TeamTrait> AllTraits;
    static S_TraitsList()
    {
        AllTraits = Resources.LoadAll<SO_TeamTrait>("ScriptableObjects/TeamTraits").ToList();
    }

}

