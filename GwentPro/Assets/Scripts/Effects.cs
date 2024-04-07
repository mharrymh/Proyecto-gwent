using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effects 
{

    public Dictionary<EffectType, Action<Card>> CardEffects; 
    public Board board = Board.Instance;

    public Effects()
    {
        CardEffects = new Dictionary<EffectType, Action<Card>>
        {
            { EffectType.AssignProm, AssignProm },
            { EffectType.CleanMeleeFile, CleanMeleeFile },
            { EffectType.CleanRangedFile, CleanRangedFile },
            { EffectType.CleanSiegeFile, CleanSiegeFile },
            { EffectType.Clearance, Clearance },
            { EffectType.Climate, Climate },
            { EffectType.Decoy, Decoy },
            { EffectType.IncrementFile, IncrementFile },
            { EffectType.DeleteLessPowerCard, DeleteLessPowerCard },
            { EffectType.DeleteMostPowerCard, DeleteMostPowerCard },
            { EffectType.DrawExtraCard, DrawExtraCard },
            { EffectType.KeepRandomCard, KeepRandomCard },
            { EffectType.None, None },
            { EffectType.TakeCardFromDeck, TakeCardFromDeck },
            { EffectType.TakeCardFromGraveYard, TakeCardFromGraveYard },
            { EffectType.TimesTwins, TimesTwins },
            //More effects
        };
    }

    
    private static void TimesTwins(Card card)
    {
        throw new NotImplementedException();
    }

    private static void TakeCardFromGraveYard(Card card)
    {
        throw new NotImplementedException();
    }

    private static void TakeCardFromDeck(Card card)
    {
        throw new NotImplementedException();
    }

    private static void None(Card card)
    {
       
    }

    private static void KeepRandomCard(Card card)
    {
        throw new NotImplementedException();
    }

    private static void DrawExtraCard(Card card)
    {
        throw new NotImplementedException();
    }

    private static void DeleteMostPowerCard(Card card)
    {
        throw new NotImplementedException();
    }

    private static void DeleteLessPowerCard(Card card)
    {
        throw new NotImplementedException();
    }

    private void IncrementFile(Card card)
    {
        if (card is Card.SpecialCard incrementCard)
        {
            //Iterate through all cards in that range and that player 
            foreach (Card Card in board.sections[incrementCard.Owner.ID][incrementCard.Range])
            {
                //Add one point for all cards in that file
                if (Card is Card.UnityCard unityCard)
                {
                    unityCard.Power++;
                }
            }
        }
    }

    private static void Decoy(Card card)
    {
        throw new NotImplementedException();
    }

    private void Climate(Card card)
    {
        if (card is Card.SpecialCard climate_card)
        {
            //Iterate through all cards in that range for both players
            var AllSections = board.sections;
            foreach(var PlayerSection in AllSections)
            {
                foreach(Card Card in PlayerSection.Value[climate_card.Range])
                {
                    //One point less for all cards in those files
                    if (Card is Card.UnityCard unityCard)
                    {
                        unityCard.Power--;
                    }
                }
            }
        }
    }

    private static void Clearance(Card card)
    {
        throw new NotImplementedException();
    }

    private static void CleanSiegeFile(Card card)
    {
        throw new NotImplementedException();
    }

    private static void CleanRangedFile(Card card)
    {
        throw new NotImplementedException();
    }

    private static void CleanMeleeFile(Card card)
    {
        throw new NotImplementedException();
    }

    private static void AssignProm(Card card)
    {
        throw new NotImplementedException();
    }

    
}
