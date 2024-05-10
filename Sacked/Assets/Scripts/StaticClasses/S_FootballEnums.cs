public static class S_FootballEnums
{
    [System.Serializable]
    public enum Positivity
    {
        Negative,
        Neutral,
        Positive
    }

    [System.Serializable]
    public enum ScoreDirection { Linear, InverseLinear, Round, InverseRound, LowerThan, Equal, HigherThan}
    
    [System.Serializable]
    public enum Rule { President, Team, Supporters, Money, TeamsSkillDifference, Constant, None, PlayerTacticGeneric, OpponentTacticGeneric, PlayerWinning, PlayerDrawing, PlayerLosing, PlayerTrait, Derby }

    [System.Serializable]
    public enum MatchRule { Aggressivity, SkillDifference, YellowCards, RedCards, Constant, Derby};
}
