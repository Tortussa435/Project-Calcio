using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "New Player Trait", menuName = "Traits/New Player Trait")]

public class SO_PlayerTrait : ScriptableObject
{
    [System.Serializable]
    public enum PlayerTraitNames
    {
        None,
        Loves_Big_Matches,
        Longshot,
        Tall,
        Dribbler,
        Speedster,
        Lucky,
        Ludopatic,
        Hot_Head,
        Hates_Big_Matches,
        Bad_Long_Shot,
        Does_Not_Know_The_Language,
        Argues_With_Manager,
        Lazy,
        Old_Wise_Man,
        Small,
        Unlucky,
        Glass
    }

    public PlayerTraitNames traitName;
    public S_FootballEnums.Positivity positiveTrait;
    public UnityEvent traitEffect;

    public List<PlayerTraitNames> excludedTraits;

    public SO_PlayerData playerRef;

    public string GetTraitName() => traitName.ToString().Replace('_', ' ');

    #region TRAITS
    //Trait events library
    public void Test()
    {
        //Debug.Log("test func");
    }

    public void T_LovesBigMatches()
    {
        if (S_GlobalManager.nextOpponent.SkillLevel >= 4)
        {
            playerRef.skillLevel += 1;
        }
        //if the value gets clamped then it should not be decreased on match end (it would not be very epic!)
        if (playerRef.skillLevel > 5) playerRef.skillLevel = 5;
        else S_PlayerMatchSimulator.OnMatchEnd.AddListener(T_LovesBigMatchTrigger);
    }
    private void T_LovesBigMatchTrigger()
    {
        playerRef.skillLevel -= 1;
        S_PlayerMatchSimulator.OnMatchEnd.RemoveListener(T_LovesBigMatchTrigger);
    }
    
    
    public void IncreaseInjuryChance()
    {
        if (S_PlayerMatchSimulator.IsPlayerHomeTeam()) S_PlayerMatchSimulator.injuryChance.home++;
        
        else S_PlayerMatchSimulator.injuryChance.away++;
    }

    #endregion
}
