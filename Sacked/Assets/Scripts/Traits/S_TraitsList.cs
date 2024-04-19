using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class S_TraitsList
{
    public static List<S_TeamTrait> AllTraits;
    static S_TraitsList()
    {
        AllTraits = new List<S_TeamTrait>
        {
            new S_TeamTrait_Violent(),
            new S_TeamTrait_Tall(),
            new S_TeamTrait_Small(),
            new S_TeamTrait_Dribblers()
        };
    }

}

