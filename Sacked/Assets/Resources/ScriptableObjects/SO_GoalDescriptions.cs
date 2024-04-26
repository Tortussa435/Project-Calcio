using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Goal Reasons List", menuName = "Goals/Goal Reasons")]
public class SO_GoalDescriptions : ScriptableObject
{
    [System.Serializable]
    public enum GoalReason
    {
        None,
        Head,
        InArea,
        Longshot,
        Chipshot
    }
    public GoalReason goalReason;

    public List<string> goalDescriptions;
}
