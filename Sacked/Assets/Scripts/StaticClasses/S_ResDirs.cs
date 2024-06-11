
using UnityEngine;

public static class S_ResDirs
{
    [Header("Tactics")]
    public static string teamTacticsDir = "ScriptableObjects/TeamTactics/";
    public static string genericTactic = "ScriptableObjects/TeamTactics/Generic";

    [Header("Cards")]
    public static string opponentSub = "ScriptableObjects/MatchCards/Match/SO_OpponentSubstitution";
    public static string penaltyCard = "ScriptableObjects/MatchCards/Match/SO_Penalty";
    public static string invalidGoal = "ScriptableObjects/MatchCards/Match/InvalidGoal/SO_InvalidGoal";
    public static string golCardDir = "Prefabs/CardTypes/P_GolCard";
    public static string missedPenalty = "ScriptableObjects/MatchCards/Match/PenaltyFailed/SO_PenaltyFailed";
    public static string forcedSubstitution = "ScriptableObjects/MatchCards/Match/ForcedSubstitution/SO_ForcedSubstitution";
    public static string playerSubstitution = "ScriptableObjects/MatchCards/Match/SO_PlayerSubstitution";
    public static string goalkeeperSubstitution = "ScriptableObjects/MatchCards/Match/ForcedSubstitution/SO_GKSubstitution";
    public static string gkSaveCard = "ScriptableObjects/MatchCards/Match/GKSave/SO_GKSave";
    public static string cornerFailed = "ScriptableObjects/MatchCards/Match/CornerFailed/SO_CornerFailed";
    public static string fkFailed = "ScriptableObjects/MatchCards/Match/FKFailed/SO_FKFailed";

    [Header("Pools")]
    public static string matchPreviewPool = "ScriptableObjects/CardsPools/MatchPreviewCardsPool";
    public static string firstHalfBreakPool = "ScriptableObjects/CardsPools/MatchSpeechPool";

    [Header("Objectives")]
    public static string objectiveTooLow = "ScriptableObjects/Sacking/Sacking_TooLowObjective";

    [Header("Curves")]
    public static string fastGoalChanceCurve = "ScriptableObjects/Curves/GoalChanceCurveSimplified";
    public static string goalChancePerMinute = "ScriptableObjects/Curves/PlayerMatch/MatchGoalChanceMultiplier";

    [Header("Prefabs")]
    public static string teambox = "Prefabs/Ladder/Teambox";
    public static string ladderLegend = "Prefabs/Ladder/LadderLegend";
    public static string playerbox = "Prefabs/PlayerBox";
    public static string playerPrefab = "Prefabs/P_Player";
    public static string chooseTeamCard = "Prefabs/CardTypes/P_ChooseTeamCard";

    [Header("Players")]
    public static string playerCardDir = "ScriptableObjects/TeamCard/PlayerCard";
    public static string namesDatabase = "ScriptableObjects/NamesDatabase";

    [Header("Traits")]
    public static string playerTraitsDatabase = "ScriptableObjects/PlayerTraits";
    public static string teamTraitsDatabase = "ScriptableObjects/TeamTraits";

    [Header("Team")]
    public static string teamCard = "ScriptableObjects/TeamCard/TeamCard";

    [Header("Errors")]
    public static string errorCard = "ScriptableObjects/OtherCards/SO_ErrorCard";

    [Header("Gol Descriptions")]
    public static string penaltyDescriptions = "ScriptableObjects/GoalDescriptions/SpecialDescriptions/PenaltyGoals";
    public static string cornerGoalDescriptions = "ScriptableObjects/GoalDescriptions/SpecialDescriptions/CornerGoals";
    public static string fkGoalDescriptions = "ScriptableObjects/GoalDescriptions/SpecialDescriptions/FKGoals";

    [Header("Stats")]
    public static string pentagon = "Prefabs/Pentagon/Pentagon";
}
