using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class S_PlayerTrait
{
    public string traitName;

    [Tooltip("\ntrue=positive trait\nfalse=negative trait")]
    public bool positiveTrait;

    public List<string> excludedTraits = new List<string>();
    public virtual void ApplyTrait(SO_Team.TeamValues teamvalues)
    {

    }
}
