using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Tactic", menuName = "Tactics/Tactic")]
public class SO_Tactics : ScriptableObject
{
    [SerializeField]
    public enum Tactic
    {
        Generic,
        Counterattack,
        BallPossession,
        Pressing,
        Catenaccio
    }
    public Tactic teamTactic;

    public List<Tactic> VeryUneffective;
    public List<Tactic> Uneffective;
    public List<Tactic> Strong;
    public List<Tactic> VeryStrong;

    /// <summary>
    /// returns a value in range -2:2 depending on how the tactic is offensively effective against the opponent
    /// </summary>
    /// <param name="tactic">Opponent's tactics</param>
    /// <returns></returns>
    public int FindEffectivenessAgainstTactic(Tactic tactic)
    {
        if (VeryUneffective.Contains(tactic)) return -2;

        if (Uneffective.Contains(tactic)) return -1;

        if (Strong.Contains(tactic)) return 1;

        if (VeryStrong.Contains(tactic)) return 2;

        return 0; //if tactic is neutral against opponent return 0
    }
}
