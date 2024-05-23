using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class S_PlayerTraitsList
{
    public static List<SO_PlayerTrait> playerTraitsDatabase;
    static S_PlayerTraitsList()
    {
        playerTraitsDatabase = Resources.LoadAll<SO_PlayerTrait>(S_ResDirs.playerTraitsDatabase).ToList();
    }
}
