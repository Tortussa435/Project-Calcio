using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Events;
using Unity.VisualScripting;

public static class S_GlobalManager
{

    //Constants
    public const int MAXTEAMSKILLLEVEL=5;
    //---------

    [System.Serializable]
    public struct IntRange
    {
        public int min;
        public int max;
    }
    public enum CardsPhase { Contract, Week, Market, MatchFirstHalf, MatchSecondHalf };
    static public CardsPhase currentPhase = CardsPhase.Contract;


    static public Vector2 CardsSpawnOffset = new Vector2(0,0);

    static public SO_Team selectedTeam;
    static public S_Squad squad;

    static public int reputation = 5;
    static public S_DeckManager deckManagerRef;

    static public int minRankingObjective;
    static public int optimalRankingObjective;

    static public bool sacked = false;

    //values ranging 0-100 with their invoke event
    public enum Values { President, Team, Supporters, Money};

    static public float President=50f;
    static public UnityEvent OnUpdatePresident;

    static public SO_Team nextOpponent;

    

    public static int currentMatchDay = 0;
    static S_GlobalManager()
    {
    
    }
    static public float SetPresident(float presidentAdd)
    {
        President += presidentAdd;
        President = Mathf.Clamp(President, 0, 100);
        if (OnUpdatePresident == null) OnUpdatePresident = new UnityEvent();
        OnUpdatePresident.Invoke();
        return President;
    }

    static public float Team=50f;
    static public UnityEvent OnUpdateTeam;
    static public float SetTeam(float teamAdd)
    {
        Team += teamAdd;
        Team=Mathf.Clamp(Team, 0, 100);
        if (OnUpdateTeam == null) OnUpdateTeam = new UnityEvent();
        OnUpdateTeam.Invoke();
        return Team;
    }

    static public float Supporters=50f;
    static public UnityEvent OnUpdateSupporters;
    static public float SetSupporters(float supportersAdd)
    {
        Supporters += supportersAdd;
        Supporters = Mathf.Clamp(Supporters, 0, 100);

        if (OnUpdateSupporters == null) OnUpdateSupporters = new UnityEvent();
        OnUpdateSupporters.Invoke();
        return Supporters;
    }
    
    static public float Money=50f;
    static public UnityEvent OnUpdateMoney;
    static public float SetMoney(float moneyAdd)
    {
        Money += moneyAdd;
        Money = Mathf.Clamp(Money, 0, 100);

        if (OnUpdateMoney == null) OnUpdateMoney = new UnityEvent();
        OnUpdateMoney.Invoke();
        return Money;
    }

    static public bool DefeatCheck()
    {
        return President <= 0 || Team <= 0 || Supporters <= 0 || Money <= 0;
    }

    static public string ReplaceVariablesInString(string inString)
    {
        try 
        {
            Dictionary<string, string> varsDictionary = new Dictionary<string, string>
            {
                { "{Team}", selectedTeam.teamName },
                { "{Opponent}", nextOpponent.teamName },
                { "{Ref}", "L'arbitro " + S_PlayerMatchSimulator.refereeName },
                { "{Player}", squad.playingEleven[Random.Range(0,squad.playingEleven.Count)].playerName },
                { "{OppPlayer}", S_PlayersGenerator.CreateRandomName() } //REDO bindare il create random name per giocatori avversari a generazione giocatori per match cosi da renderli persistenti
            };

            foreach (string key in varsDictionary.Keys)
            {
                if (varsDictionary.TryGetValue(key, out string outstring))
                {
                    inString = inString.Replace(key, outstring);
                }
            }
            return inString;
        }
        catch
        {
            Debug.Log("Variabili cringe non inizializzate");
            return inString;
        }
        
    }
    public static Color FindAppropriateColor(SO_Team team)
    {
        if (team.teamColor1 == Color.white && team.teamColor2 == Color.black) return Color.gray;
        if (team.teamColor1 == Color.black && team.teamColor2 == Color.white) return Color.gray;
        if (team.teamColor1 == Color.white || team.teamColor1 == Color.black) return team.teamColor2;
        else return team.teamColor1;
    }

    public static SO_CardData GetCardOnTop() => deckManagerRef.deck.transform.GetChild(0).GetComponent<S_Card>().cardData;
    
}
