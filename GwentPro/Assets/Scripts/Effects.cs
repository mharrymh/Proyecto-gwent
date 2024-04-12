using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class Effects
{

    public Dictionary<EffectType, Action<Card>> CardEffects; 
    public Board board = Board.Instance;
    public GameManager gm;

    public Effects()
    {
        CardEffects = new Dictionary<EffectType, Action<Card>>
        {
            { EffectType.AssignProm, AssignProm },
            { EffectType.CleanFile, CleanFile },
            { EffectType.CleanRangedFile, CleanRangedFile },
            { EffectType.CleanSiegeFile, CleanSiegeFile },
            { EffectType.Clearance, Clearance },
            { EffectType.Climate, Climate },
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
            //Draw random card from graveyard
        };

        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    //Multiply the power of the card for all the instances of 
    //card "brothers" in board
    //Ligth: Quimera, Satiro, Manticora
    //Dark: Medusa, Gorgona, Hidra
    private  void TimesTwins(Card card)
    {
        int brothers = 1;

        var PlayerSections = board.sections[card.Owner.ID];
        foreach (var RangeSection in PlayerSections.Values)
        {
            for (int i = 0; i < RangeSection.Count; i++)
            {
                if (RangeSection[i].effectType == EffectType.TimesTwins 
                    && RangeSection[i] != card)
                {
                    brothers++;
                }
            }
        }

        if (card is Card.UnityCard unity_card)
        {
            unity_card.Power *= brothers;
        }
    }
    //Draw the most powerfulcard from player graveyard
    private void TakeCardFromGraveYard(Card card)
    {
        Card MostPowerfulCard = null;
        int maxPower = int.MinValue;
        List<Card> Graveyard = card.Owner.GraveYard;
        int pos = 0;

        for (int i = Graveyard.Count - 1; i >= 0; i--)
        {
            if (Graveyard[i] is Card.UnityCard unity_card && unity_card.Power > maxPower)
            {
                maxPower = unity_card.Power;
                MostPowerfulCard = unity_card;
                pos = i;
            }
        }

        if (MostPowerfulCard != null)
        {
            //Remove card from player graveyard
            Graveyard.RemoveAt(pos);
            //Add card to the player hand
            MostPowerfulCard.Owner.Hand.Add(MostPowerfulCard);
            //Instantiate the card
            gm.InstantiateCard(MostPowerfulCard);
        }
        Debug.Log("Se aplico el efecto");
    }
    private  void TakeCardFromDeck(Card card)
    {
        
    }
    private  void None(Card card)
    {
    }

    private  void KeepRandomCard(Card card)
    {
        
    }

    //Draw extra card in the next round 
    //Leader effect
    private void DrawExtraCard(Card card)
    {
        
    }

    //Delete most powerful card between both players
    private void DeleteMostPowerCard(Card card)
    {
        Card.UnityCard MostPowerfulCard = null;
        int maxPower = int.MinValue;
        int pos = 0;
        string range = "";
        List<Card> aux = new List<Card>();

        var AllSections = board.sections;
        foreach (var PlayerSection in AllSections)
        {
            foreach(var RangeSection in PlayerSection.Value)
            {
                aux = RangeSection.Value;
                for (int i = 0; i < aux.Count; i++)
                {
                    if (aux[i] is Card.UnityCard unity_card && unity_card.Power > maxPower)
                    {
                        maxPower = unity_card.Power;
                        MostPowerfulCard = unity_card;
                        pos = i;
                        range = RangeSection.Key;
                    }
                }
            }
        }

        if (MostPowerfulCard != null && range != "")
        {
            //Remove card from backend board
            board.sections[MostPowerfulCard.Owner.ID][range].RemoveAt(pos);
            //Add card to player graveyard
            MostPowerfulCard.Owner.GraveYard.Add(MostPowerfulCard);
            //Unable drag and drop property
            MostPowerfulCard.IsPlayed = false;
            //Destroy card from frontend board
            gm.CardBeaten(MostPowerfulCard);
        }

    }
    //Delete the less poweful card of the opponent
    private void DeleteLessPowerCard(Card card)
    {
        Player opponent;
        Card.UnityCard LessPowerfulCard = null;
        int minPower = int.MaxValue;
        int position = 0;
        string range = "";
        List<Card> aux = new List<Card>(); 

        if (card.Owner == gm.player1) opponent = gm.player2;
        else opponent = gm.player1;

        var PlayerSection = board.sections[opponent.ID];
        foreach (var RangeSection in PlayerSection)
        {
            aux = RangeSection.Value;
            for (int i = 0; i < aux.Count; i++)
            {
                if (aux[i] is Card.UnityCard unity_card && unity_card.Power < minPower)
                {
                    minPower = unity_card.Power;
                    LessPowerfulCard = unity_card;
                    position = i;
                    range = RangeSection.Key;
                }
            }
        }

        if (LessPowerfulCard != null && range != "")
        {
            //Remove card from backend board
            board.sections[LessPowerfulCard.Owner.ID][range].RemoveAt(position);
            //Add card to player graveyard
            LessPowerfulCard.Owner.GraveYard.Add(LessPowerfulCard);
            //Unable drag and drop property
            LessPowerfulCard.IsPlayed = false;
            //Destroy card from frontend board
            gm.CardBeaten(LessPowerfulCard);
        }
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
    public void Decoy(string name, string range, string player, Card.SpecialCard decoy)
    {
        Card Taken = null;

        if (range == "Increment")
        {
            Card[] cards = board.increment_section[player];
            for (int i = 0; i < cards.Length; i++)
            {         
                if (cards[i] != null
                    && cards[i].Name == name)
                {
                    Taken = cards[i];
                    cards[i] = decoy;
                }
            }
        }

        //  Que no afecte a las cartas clima

        //else if (range == "Climate")
        //{
        //    for (int i = 0; i < board.climate_section.Length; i++)
        //    {
        //        if (board.climate_section[i] != null 
        //            && board.climate_section[i].Name == name
        //            && board.climate_section[i].Owner.ID == player)
        //        {
        //            Taken = board.climate_section[i];
        //            board.climate_section[i] = decoy;
        //        }
        //    }
        //}

        else
        {
            List<Card> cards = board.sections[player][range];
            if (cards != null)
            {
                for (int i = cards.Count - 1; i >= 0; i--)
                {
                    if (cards[i].Name == name)
                    {
                        Taken = cards[i];
                        cards.RemoveAt(i);
                    }
                }
            }
            //Add decoy card to that zone
            cards.Add(decoy);
        }

        if (Taken != null)
        {
            Taken.IsPlayed = false;
            Taken.Owner.Hand.Add(Taken);
        }
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

    private void Clearance(Card card)
    {
        for (int i = 0; i < board.climate_section.Length; i++)
        {
            if (board.climate_section[i] != null)
            {
                board.climate_section[i].IsPlayed = false;
                board.climate_section[i].Owner.GraveYard.Add(board.climate_section[i]);
                CleareanceAux(board.climate_section[i]);
                gm.CardBeaten(board.climate_section[i]);
                board.climate_section[i] = null;
            }
        }
    }

    private void CleareanceAux(Card card)
    {
        if (card is Card.SpecialCard climate_card)
        {
            //Iterate through all cards in that range for both players
            var AllSections = board.sections;
            foreach (var PlayerSection in AllSections)
            {
                foreach (Card Card in PlayerSection.Value[climate_card.Range])
                {
                    //One point less for all cards in those files
                    if (Card is Card.UnityCard unityCard)
                    {
                        unityCard.Power++;
                    }
                }
            }
        }
    }
    private  void CleanSiegeFile(Card card)
    {
        
    }

    private  void CleanRangedFile(Card card)
    {
        
    }

    //Clean file with less cards between both players
    private void CleanFile(Card card)
    {
        int length = int.MaxValue;
        List<Card> aux = new List<Card>();
        Player opponent;
        if (card.Owner == gm.player1) opponent = gm.player2;
        else opponent = gm.player1;
        

        foreach (List<Card> RangeSection in board.sections[opponent.ID].Values)
        {
            if (RangeSection.Count < length && RangeSection.Count != 0)
            {
                length = RangeSection.Count;
                aux = RangeSection;
                if (length == 1) break;
            }
        }

        if (length == 1)
        {
            aux[0].Owner.GraveYard.Add(aux[0]);
            aux[0].IsPlayed = false;
            if (aux[0] is Card.UnityCard unity_card) gm.CardBeaten(unity_card);
            aux.RemoveAt(0);
        }
        else
        {
            foreach (List<Card> RangeSection in board.sections[card.Owner.ID].Values)
            {
                if (RangeSection.Count < length && RangeSection.Count != 0)
                {
                    length = RangeSection.Count;
                    aux = RangeSection;
                    if (length == 1) break;
                }
            }
            if (length != int.MaxValue)
            {
                for (int i = length - 1; i >= 0; i--)
                {
                    aux[i].Owner.GraveYard.Add(aux[i]);
                    aux[i].IsPlayed = false;
                    if (aux[i] is Card.UnityCard unity_card) gm.CardBeaten(unity_card);
                    aux.RemoveAt(i);
                }
            }
        }
    }
    private  void AssignProm(Card card)
    {
        
    }

    
}
