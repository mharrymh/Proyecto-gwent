using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UIElements;
using static Unity.Burst.Intrinsics.X86;
//using static UnityEditor.PlayerSettings;

public class Effects
{
    public GameObject ClimateZone;
    public Dictionary<EffectType, Action<Card>> CardEffects;
    public Board board = Board.Instance;
    public GameManager gm;
    public Effects()
    {
        CardEffects = new Dictionary<EffectType, Action<Card>>
        {
            { EffectType.AssignProm, AssignProm },
            { EffectType.CleanFile, CleanFile },
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
            { EffectType.Decoy, Decoy },
            { EffectType.AddClimateCard, AddClimateCard },
        };

        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        ClimateZone = GameObject.Find("ClimateZone");
    }

    //Multiply the power of the card for all the same instances
    private void TimesTwins(Card card)
    {
        int brothers = 1;

        var PlayerSections = board.sections[card.Owner.ID];
        foreach (var RangeSection in PlayerSections.Values)
        {
            for (int i = 0; i < RangeSection.Count; i++)
            {
                if (RangeSection[i].Name == card.Name
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

        //Visual 
        gm.StartCoroutine(gm.SetAuxText("Existen " + brothers + " copias de esta carta en el campo" +
            ". Su poder se multiplic� por " + brothers));
    }
    //Draw the most powerful card from player graveyard
    private void TakeCardFromGraveYard(Card card)
    {
        if (card.Owner.Hand.Count >= 10) return;

        Card MostPowerfulCard = null;
        int maxPower = int.MinValue;
        List<Card> Graveyard = card.Owner.GraveYard;
        int pos = 0;

        for (int i = Graveyard.Count - 1; i >= 0; i--)
        {
            if (Graveyard[i] is Card.UnityCard unity_card && unity_card.UnityType == UnityType.Silver
                && unity_card.Power > maxPower)
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
            gm.InstantiateCard(MostPowerfulCard, gm.HandPanel);



            //Visual
            gm.StartCoroutine(gm.SetAuxText("Se a�adi� " + MostPowerfulCard.Name + " a la mano de " + gm.currentPlayer.PlayerName
           + " desde el cementerio"));
        }
        else gm.StartCoroutine(gm.SetAuxText("El cementerio est� vac�o, no se robo ninguna carta"));

    }
    //Draw extra card from deck

    private void TakeCardFromDeck(Card card)
    {
        if (card.Owner.PlayerDeck.Count > 0 && card.Owner.Hand.Count < 10)
        {
            //Get the card at 0 index in Player Deck
            Card DeckCard = card.Owner.PlayerDeck[0];
            //Add the owner to the DeckCard
            DeckCard.Owner = card.Owner;
            //Add card to the player hand
            DeckCard.Owner.Hand.Add(DeckCard);
            //Remove card from player deck
            card.Owner.PlayerDeck.RemoveAt(0);
            //Instantiate the card on HandPanel
            gm.InstantiateCard(DeckCard, gm.HandPanel);

            gm.StartCoroutine(gm.SetAuxText("Se rob� " + DeckCard.Name + " desde el deck de " + gm.currentPlayer.PlayerName));
        }
        else if (card.Owner.Hand.Count >= 10) gm.StartCoroutine(gm.SetAuxText("La mano est� llena, no se robaron cartas"));
    }
    private void None(Card card)
    {
        gm.StartCoroutine(gm.SetAuxText("No se aplicaron efectos en este turno"));
    }
    private void KeepRandomCard(Card card)
    {
        //This is a passive effect of the leader card
        //that is implemented in execution time
    }
    private void DrawExtraCard(Card card)
    {
        //This is a passive effect of the leader card
        //that is implemented in execution time
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
            foreach (var RangeSection in PlayerSection.Value)
            {
                aux = RangeSection.Value;
                for (int i = 0; i < aux.Count; i++)
                {
                    if (aux[i] is Card.UnityCard unity_card && unity_card.UnityType == UnityType.Silver
                        && unity_card.Power > maxPower)
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


            gm.StartCoroutine(gm.SetAuxText("Se elimin�  a " + MostPowerfulCard.Name + " de " + MostPowerfulCard.Owner.PlayerName));
        }
        else gm.StartCoroutine(gm.SetAuxText("No hay cartas plata en el tablero"));

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
                if (aux[i] is Card.UnityCard unity_card && unity_card.UnityType == UnityType.Silver
                    && unity_card.Power < minPower)
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
            if (LessPowerfulCard.Owner.GraveYard.Count == 1)
            {
                LessPowerfulCard.Owner.GraveyardObj.SetActive(true);
            }
            //Unable drag and drop property
            LessPowerfulCard.IsPlayed = false;
            //Destroy card from frontend board
            gm.CardBeaten(LessPowerfulCard);

            gm.StartCoroutine(gm.SetAuxText("Se elimin�  a " + LessPowerfulCard.Name + " de " + LessPowerfulCard.Owner.PlayerName));
        }
        else gm.StartCoroutine(gm.SetAuxText("No se eliminaron cartas"));
    }
    //Increment cards
    private void IncrementFile(Card card)
    {
        if (card is Card.SpecialCard incrementCard)
        {
            //Iterate through all cards in that range and that player 
            foreach (Card Card in board.sections[incrementCard.Owner.ID][incrementCard.Range])
            {
                //Add one point for all cards in that file
                if (Card is Card.UnityCard unityCard && unityCard.UnityType is UnityType.Silver)
                {
                    unityCard.Power++;
                }
            }
        }
    }
    //Decoy cards
    public void Decoy(string name, string range, string player, Card.SpecialCard decoy)
    {
        Card Taken = null;

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

        if (Taken != null)
        {
            Taken.IsPlayed = false;
            Taken.Owner.Hand.Add(Taken);

            if (Taken is Card.UnityCard unity) unity.Power = unity.OriginalPower; 
            gm.StartCoroutine(gm.SetAuxText("La carta " + Taken.Name + " regres� a la mano de " + gm.currentPlayer.PlayerName));
        }
    }
    //Climate cards
    private void Climate(Card card)
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
                    if (Card is Card.UnityCard unityCard && unityCard.UnityType is UnityType.Silver)
                    {
                        unityCard.Power--;
                    }
                }
            }
        }
    }
    //Cleareance cards
    private void Clearance(Card card)
    {
        for (int i = 0; i < board.climate_section.Length; i++)
        {
            if (board.climate_section[i] != null)
            {
                board.climate_section[i].IsPlayed = false;
                board.climate_section[i].Owner.GraveYard.Add(board.climate_section[i]);
                if (board.climate_section[i].Owner.GraveYard.Count == 1)
                {
                    board.climate_section[i].Owner.GraveyardObj.SetActive(true);
                }
                CleareanceAux(board.climate_section[i]);
                gm.CardBeaten(board.climate_section[i]);
                board.climate_section[i] = null;
            }
        }

        gm.StartCoroutine(gm.SetAuxText("Se eliminaron todas las cartas clima del campo"));
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
            if (aux[0].Owner.GraveYard.Count == 1)
            {
                aux[0].Owner.GraveyardObj.SetActive(true);
            }
            aux[0].IsPlayed = false;
            gm.CardBeaten(aux[0]);
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
                    if (aux[i].Owner.GraveYard.Count == 1)
                    {
                        aux[i].Owner.GraveyardObj.SetActive(true);
                    }
                    aux[i].IsPlayed = false;
                    gm.CardBeaten(aux[i]);
                    aux.RemoveAt(i);
                }
            }
        }

        gm.StartCoroutine(gm.SetAuxText("Se elimin� la fila con menos cartas"));
    }
    //Assign all silver cards the prom of points in the board
    private  void AssignProm(Card card)
    {
        int Sum = gm.GetPower(gm.player1) + gm.GetPower(gm.player2);
        int AmountOfCardsOnBoard = 60 /*Amount of cards*/ - gm.player1.PlayerDeck.Count - gm.player2.PlayerDeck.Count
            - gm.player1.GraveYard.Count - gm.player2.GraveYard.Count - gm.player1.Hand.Count
            - gm.player2.Hand.Count - 1 /*Card with the effect*/;

        if (AmountOfCardsOnBoard != 0 && Sum != 0)
        {
            List<Card> aux = new List<Card>();

            var AllSections = board.sections;
            foreach (var PlayerSection in AllSections)
            {
                foreach (var RangeSection in PlayerSection.Value)
                {
                    aux = RangeSection.Value;
                    for (int i = 0; i < aux.Count; i++)
                    {
                        if (aux[i] is Card.UnityCard unity && unity.UnityType is UnityType.Silver)
                        {
                            unity.Power = (Sum / AmountOfCardsOnBoard);
                        }
                    }
                }
            }
            gm.StartCoroutine(gm.SetAuxText("Se le asign� " + Sum / AmountOfCardsOnBoard + " a todas las cartas platas del campo"));
        }
        else gm.StartCoroutine(gm.SetAuxText("No hay cartas en el campo para calcular el promedio "));

        //Apply climate and increment effects again
        var IncrementSection = board.increment_section;

        foreach (Card[] cards in IncrementSection.Values)
        {
            foreach (Card increment in cards)
            {
                if (increment != null) CardEffects[increment.effectType].Invoke(increment);
            }
        }

        foreach (Card climate in board.climate_section)
        {
            if (climate != null) CardEffects[climate.effectType].Invoke(climate);
        }

    }

    //Decoy effect
    void Decoy(Card card)
    {
        //Empty because is implemented in execution time
    }
    
    void AddClimateCard(Card card)
    {
        Card.SpecialCard aux = null;

        foreach (Card Card in card.Owner.Hand)
        {
            if (Card is Card.SpecialCard climate && climate.Type is SpecialType.Climate)
            {
                climate.Owner = card.Owner;
                aux = climate;
                if (climate.Range == "M" && board.climate_section[0] == null)
                {
                    AddClimateAux(0, climate, true);
                    break;
                }
                else if (climate.Range == "R" && board.climate_section[1] == null)
                {
                    AddClimateAux(1, climate, true);
                    break;
                }
                else if (climate.Range == "S" && board.climate_section[2] == null)
                {
                    AddClimateAux(2, climate, true);
                    break;
                }
            }
        }

        if (aux == null)
        {
            foreach (Card Card1 in card.Owner.PlayerDeck)
            {
                if (Card1 is Card.SpecialCard climate && climate.Type is SpecialType.Climate)
                {
                    climate.Owner = card.Owner;
                    if (climate.Range == "M" && board.climate_section[0] == null)
                    {
                        AddClimateAux(0, climate, false);
                        break;
                    }
                    else if (climate.Range == "R" && board.climate_section[1] == null)
                    {
                        AddClimateAux(1, climate, false);
                        break;
                    }
                    else if (climate.Range == "S" && board.climate_section[2] == null)
                    {
                        AddClimateAux(2, climate, false);
                        break;
                    }
                }
            }
        }
    }

    void AddClimateAux(int pos, Card.SpecialCard climate, bool IsInHand)
    {
        if (IsInHand)
        {
            board.climate_section[pos] = climate;
            climate.CardPrefab.transform.SetParent(ClimateZone.transform, false);
            climate.Owner.Hand.Remove(climate);
            CardEffects[climate.effectType].Invoke(climate);
            climate.IsPlayed = true;
            gm.StartCoroutine(gm.SetAuxText("Se a�adi� la carta clima " + climate.Name + " desde la mano de " + gm.currentPlayer.PlayerName));
        }
        else
        {
            gm.StartCoroutine(gm.SetAuxText("Se a�adi� la carta clima " + climate.Name + " desde el deck de " + gm.currentPlayer.PlayerName));
            board.climate_section[pos] = climate;
            gm.InstantiateCard(climate, ClimateZone.transform);
            climate.Owner.PlayerDeck.Remove(climate);
            climate.IsPlayed = true;
            CardEffects[climate.effectType].Invoke(climate);
            climate.CardPrefab.tag = climate.Owner.ID;
        }
    }
}
