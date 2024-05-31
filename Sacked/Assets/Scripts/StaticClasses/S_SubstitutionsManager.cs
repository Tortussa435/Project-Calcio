using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public static class S_SubstitutionsManager
{
    private static List<SO_PlayerData> elevenRef;
    private static List<SO_PlayerData> benchRef;
    
    public static void SetElevenAndBenchList(List<SO_PlayerData> eleven, List<SO_PlayerData> bench)
    {
        elevenRef = eleven;
        benchRef = bench;
    }

    public static (SO_PlayerData pOut, SO_PlayerData pIn) ProposeSubstitution()
    {
        (SO_PlayerData pOut, SO_PlayerData pIn) subs = (null, null);

        //REDO pretty ugly
        float skillDifference = (float)S_PlayerMatchSimulator.GetSkillDifference(S_GlobalManager.selectedTeam.SkillLevel, S_PlayerMatchSimulator.GetOpponentTeam().SkillLevel);

        //if player is winning against better team or drawing against very better team, propose defensive change

        if ((skillDifference < 0.5f && S_PlayerMatchSimulator.PlayerWinning()) || (skillDifference < 0.3f && S_PlayerMatchSimulator.IsMatchDrawing()))
        {
            if (CanMakeDefensiveSubstitution())
            {
                Debug.LogWarning("Propongo sostituzione difensiva");
                subs = FindDefensiveSubstitute();
                return subs;
            }
        }

        else if (BenchHasGoodSub())
        {
            Debug.LogWarning("Faccio entrare un giocatore buono");
            subs.pOut = ProposeGoodPlayerIn();
            subs.pIn = FindOptimalSubstitute(subs.pOut);
            return subs;
        }


        //if player is not winning propose offensive change
        else if (!S_PlayerMatchSimulator.PlayerWinning())
        {
            if (CanMakeOffensiveSubstitution())
            {
                Debug.LogWarning("Propongo sostituzione offensiva");
                subs = FindOffensiveSubstitute();
                return subs;
            }
        }

        else if (S_PlayerMatchSimulator.PlayerWinning() && S_PlayerMatchSimulator.GetGoalDifference() > 1)
        {
            Debug.LogWarning("Faccio uscire un giocatore stanco");
            subs.pOut = ProposeTiredPlayerOut();
            subs.pIn = FindOptimalSubstitute(subs.pOut);
            return subs;
        }

        Debug.LogWarning("Sostituzione a caso seeeee");
        subs=ProposeRandomSubstitution();
        return subs;
        
    }

    #region POSSIBLE SUBSTITUTIONS TYPES
    private static SO_PlayerData ProposeTiredPlayerOut()
    {
        List<SO_PlayerData> tiredPlayers = new List<SO_PlayerData>();

        foreach(SO_PlayerData p in elevenRef)
        {
            if (p.playerEnergy <= 60)
            {
                tiredPlayers.Add(p);
            }
        }
        return tiredPlayers.Count>0 ? tiredPlayers[Random.Range(0, tiredPlayers.Count)] : null;
    } 
    private static SO_PlayerData ProposeGoodPlayerIn()
    {
        List<SO_PlayerData> goodPlayers = new List<SO_PlayerData>();

        foreach(SO_PlayerData p in benchRef)
        {
            if (!p.CanPlay()) continue;
            if (p.skillLevel >= S_GlobalManager.selectedTeam.SkillLevel) goodPlayers.Add(p); //if player is better than the average of the team propose change
        }

        if (goodPlayers.Count == 0) return null;

        SO_PlayerData.PlayerRole pRole = goodPlayers[Random.Range(0, goodPlayers.Count)].playerRole;
        //it is not needed to transfer who is the good player, but to propose changing the worst player in that role

        (SO_PlayerData player, float score) worstPlayer = (null,100);
        
        //find worst player to swap with good player in bench
        foreach(SO_PlayerData pEl in elevenRef)
        {
            if (pEl.skillLevel < worstPlayer.score) worstPlayer = (pEl, pEl.skillLevel);    
        }
        return worstPlayer.player;

    }
    public static SO_PlayerData FindOptimalSubstitute(SO_PlayerData playerToSub,List<SO_PlayerData> impossibleSubs=null)
    {
        (SO_PlayerData player, float score) bestSub=(null,0);

        SO_PlayerData.PlayerRole role = playerToSub.playerRole;

        foreach(SO_PlayerData p in benchRef)
        {
            if (impossibleSubs != null)
                if (impossibleSubs.Contains(p)) continue;

            if (!p.CanPlay()) continue;

            float score = 0;

            //avoids putting goalkeepers where they shouldn't be
            if ((role != SO_PlayerData.PlayerRole.Gk && p.playerRole == SO_PlayerData.PlayerRole.Gk)|| (role == SO_PlayerData.PlayerRole.Gk && p.playerRole != SO_PlayerData.PlayerRole.Gk)) score = 0;

            //role must have higher priority than skill level
            //max score = 10 if role is the same, decreases by 1 for each role distance (ex. (int)atk=4 (int)def=2 -> 2), might do a better formula
            else score += (10 - (Mathf.Abs((int)p.playerRole - (int)role))*2);
            
            score += p.skillLevel;

            if (score > bestSub.score) bestSub = (p, score);
        }
        return bestSub.player;
    }
    private static (SO_PlayerData outP, SO_PlayerData inP) FindDefensiveSubstitute()
    {
        //decide whether to sub a mid or an atk

        int atkCount = 0;
        int midCount = 0;
        (SO_PlayerData pl, float score) worstAtk = (null, 100);
        (SO_PlayerData pl, float score) worstMid = (null, 100);

        foreach (SO_PlayerData p in elevenRef)
        {
            if (p.playerRole == SO_PlayerData.PlayerRole.Atk)
            {
                if (p.skillLevel < worstAtk.score) worstAtk = (p, p.skillLevel); 
                atkCount++;
            }

            if (p.playerRole == SO_PlayerData.PlayerRole.Mid)
            {
                if (p.skillLevel < worstMid.score) worstMid = (p, p.skillLevel);
                midCount++;
            }
        }

        SO_PlayerData playerToSub = null;
        
        if (atkCount >= midCount) playerToSub = worstAtk.pl;
        else playerToSub = worstMid.pl;

        //find best defender
        (SO_PlayerData def, float score) bestDef = (null, 0);
        foreach(SO_PlayerData p in benchRef)
        {
            if (!p.CanPlay()) continue;
            if (p.playerRole == SO_PlayerData.PlayerRole.Def)
            {
                if (p.skillLevel > bestDef.score) bestDef = (p, p.skillLevel);
            }
        }

        return (playerToSub, bestDef.def);
    }
    private static (SO_PlayerData outP, SO_PlayerData inP) FindOffensiveSubstitute()
    {
        //decide whether to sub a mid or an atk

        int defCount = 0;
        int midCount = 0;
        (SO_PlayerData pl, float score) worstDef = (null, 100);
        (SO_PlayerData pl, float score) worstMid = (null, 100);

        foreach (SO_PlayerData p in elevenRef)
        {
            if (p.playerRole == SO_PlayerData.PlayerRole.Def)
            {
                if (p.skillLevel < worstDef.score) worstDef = (p, p.skillLevel);
                defCount++;
            }

            if (p.playerRole == SO_PlayerData.PlayerRole.Mid)
            {
                if (p.skillLevel < worstMid.score) worstMid = (p, p.skillLevel);
                midCount++;
            }
        }

        SO_PlayerData playerToSub = null;

        if (defCount >= 4 || midCount <= 2) playerToSub = worstDef.pl;
        else playerToSub = worstMid.pl;

        //find best defender
        (SO_PlayerData atk, float score) bestAtk = (null, 0);
        foreach (SO_PlayerData p in benchRef)
        {
            if (!p.CanPlay()) continue;
            if (p.playerRole == SO_PlayerData.PlayerRole.Atk)
            {
                if (p.skillLevel > bestAtk.score) bestAtk = (p, p.skillLevel);
            }
        }

        return (playerToSub, bestAtk.atk);
    }
    private static (SO_PlayerData outP, SO_PlayerData inP) ProposeRandomSubstitution()
    {
        List<SO_PlayerData> eligibles = new List<SO_PlayerData>();
        foreach (SO_PlayerData p in benchRef)
        {
            if (p.playerRole == SO_PlayerData.PlayerRole.Gk || !p.CanPlay()) continue;
            eligibles.Add(p);
        }

        (SO_PlayerData outP, SO_PlayerData inP) sub=(null,null);
        sub.inP = eligibles[Random.Range(0, eligibles.Count)];
        
        foreach(SO_PlayerData p in elevenRef)
        {
            if (p.playerRole == sub.inP.playerRole) sub.outP = p;
        }
        return sub;
    }
    public static bool Substitute(SO_PlayerData pOut, SO_PlayerData pIn)
    {

        if (!elevenRef.Remove(pOut))
        {
            Debug.LogWarning("Uscita dal campo fallita!");
        }
        if (!benchRef.Remove(pIn))
        {
            Debug.Log("Rimozione da panchina fallita!");
        }

        benchRef.Add(pOut);
        elevenRef.Add(pIn);

        pOut.subbedOut = true;
        
        S_PlayerMatchSimulator.OnSubstitution.Invoke(S_PlayerMatchSimulator.IsPlayerHomeTeam());

        S_GlobalManager.squad.FindGameSkillLevel();

        return true;
    }
    private static bool CanMakeOffensiveSubstitution()
    {
        int def = 0;
        int mid = 0;
        int atk = 0;
        foreach (SO_PlayerData p in elevenRef)
        {
            switch (p.playerRole)
            {
                case SO_PlayerData.PlayerRole.Def:
                    def++;
                    break;
                case SO_PlayerData.PlayerRole.Mid:
                    mid++;
                    break;
                case SO_PlayerData.PlayerRole.Atk:
                    atk++;
                    break;
            }
            if (atk > 4) return false; //cannot have more than 5 defs 
        }

        foreach (SO_PlayerData p in benchRef) //checks if there's a def in bench
        {
            if (p.playerRole == SO_PlayerData.PlayerRole.Atk)
                if (p.CanPlay()) return true;
        }
        return false;
    }
    private static bool CanMakeDefensiveSubstitution()
    {
        int def = 0;
        int mid = 0;
        int atk = 0;
        foreach(SO_PlayerData p in elevenRef)
        {
            switch (p.playerRole)
            {
                case SO_PlayerData.PlayerRole.Def:
                    def++;
                    break;
                case SO_PlayerData.PlayerRole.Mid:
                    mid++;
                    break;
                case SO_PlayerData.PlayerRole.Atk:
                    atk++;
                    break;
            }
            if (def > 5) return false; //cannot have more than 5 defs 
        }

        foreach(SO_PlayerData p in benchRef) //checks if there's a def in bench
        {
            if (p.playerRole == SO_PlayerData.PlayerRole.Def)
                if (p.CanPlay()) return true;
        }

        return false;
    }
    private static bool BenchHasGoodSub()
    {
        foreach(SO_PlayerData p in benchRef)
        {
            if (!p.CanPlay()) continue;
            if (p.skillLevel >= S_GlobalManager.selectedTeam.SkillLevel) return true;
        }
        return false;
    }
    #endregion
}