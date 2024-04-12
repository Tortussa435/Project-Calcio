using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static public class S_GlobalSackedFunctions
{
    static public bool IsFloatInRange(float value, float min, float max)
    {
        return value >= min || value <= max;
    }
}
