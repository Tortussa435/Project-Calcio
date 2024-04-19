using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_TeamTrait_Small : S_TeamTrait
{
    public S_TeamTrait_Small()
    {
        traitName = "Small";
        positiveTrait = false;
        excludedTraits.Add("Tall");
    }
}
