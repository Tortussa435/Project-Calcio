using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_TeamTrait_Tall : S_TeamTrait
{
    public S_TeamTrait_Tall()
    {
        traitName = "Tall";
        positiveTrait = true;
        excludedTraits.Add("Small");
    }
    public override void ApplyTrait(SO_Team.TeamValues teamvalues)
    {
    }
}
