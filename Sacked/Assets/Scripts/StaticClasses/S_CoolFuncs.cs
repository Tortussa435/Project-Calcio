using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static public class S_CoolFuncs
{
    #region LERP
    /// <summary>
    /// Correct way to do Mathf.Lerp(0, 1, 0.5) with any given framerate
    /// https://youtu.be/LSNQuFEDOyQ?si=TytcB97O3NA_c0q6
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="decay"></param>
    /// <returns></returns>
    public static float Lerpoler(float a, float b, float decay = 16) => b + (a - b) * Mathf.Exp(-decay * Time.deltaTime);
    public static Vector3 V3Lerpoler(Vector3 a, Vector3 b, float decay = 16)
    {
        float x = Lerpoler(a.x, b.x);
        float y = Lerpoler(a.y, b.y);
        float z = Lerpoler(a.z, b.z);
        return new Vector3(x, y, z);
    }
    #endregion

    #region COLORS
    public static Color ChangeAlpha(float a, Color c) => new Color(c.r, c.g, c.b, a);
    #endregion

}

