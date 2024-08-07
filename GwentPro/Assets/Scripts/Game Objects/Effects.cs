using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Timeline.Actions;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UIElements;
using static Unity.Burst.Intrinsics.X86;
//using static UnityEditor.PlayerSettings;

//TODO: Crear una clase para cada efecto

/// <summary>
/// Represent an effect
/// </summary>
public abstract class Effect {
    //Get the game manager object
    public GameManager gm = GameObject.Find("GameManager").GetComponent<GameManager>();

    // Get the board
    public Board board = Board.Instance;
};

/// <summary>
/// Represents active type effects, with a function to invoke the method
/// </summary>
public interface IActiveEffect {
    void Invoke(Card card);
}

/// <summary>
/// Assign promedy of points to all silver cards
/// </summary>
public class AssignProm : Effect, IActiveEffect
{
    public void Invoke(Card card)
    {
        //Get total of points
        int Sum = gm.GetPower(gm.player1) + gm.GetPower(gm.player2);
        //Count the cards on the board calculating the difference of the total number 
        //of cards with the cards that hasn't been played
        int AmountOfCardsOnBoard = 60 /*Amount of cards*/ - gm.player1.PlayerDeck.Count - gm.player2.PlayerDeck.Count
            - gm.player1.GraveYard.Count - gm.player2.GraveYard.Count - gm.player1.Hand.Count
            - gm.player2.Hand.Count - 1 /*Card with the effect*/;

        if (AmountOfCardsOnBoard != 0 && Sum != 0)
        {
            var AllSections = board.sections;
            foreach (var PlayerSection in AllSections)
            {
                foreach (var RangeSection in PlayerSection.Value)
                {
                    for (int i = 0; i < RangeSection.Value.Count; i++)
                    {
                        if (RangeSection.Value[i] is Card.SilverCard silver)
                        {
                            silver.Power = Sum / AmountOfCardsOnBoard;
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
                if (increment != null) 
                {
                    IncrementFile effect = new IncrementFile();
                    effect.Invoke(increment);
                }
            }
        }

        foreach (Card climate in board.climate_section)
        {
            if (climate != null) 
            {
                Climate effect = new Climate();
                effect.Invoke(climate);
            }
        }
    }
}
/// <summary>
/// Delete file with the less amount of cards
/// </summary>
public class CleanFile : Effect, IActiveEffect
{
    public void Invoke(Card card)
    {
        //Initialize the length in the greatest int
        int length = int.MaxValue;
        //Set an auxiliar list of cards
        CardCollection aux = new();
        //Create an auxiliar Player variable to point to the rival player
        Player opponent = (card.Owner == gm.player1)? gm.player2 : gm.player1;

        //Travel all the files of the rival
        foreach (CardCollection RangeSection in board.sections[opponent.ID].Values)
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
            foreach (CardCollection RangeSection in board.sections[card.Owner.ID].Values)
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
}
/// <summary>
/// Delete all the climate cards
/// </summary>
public class Clearance : Effect, IActiveEffect
{
    public void Invoke(Card card)
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
    void CleareanceAux(Card card)
    {
        //Reserved for climate cards
        Card.ClimateCard climate_card = (Card.ClimateCard)card;

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
/// <summary>
/// Decrease the power by one point to the silver cards in this card range for both players
/// </summary>
public class Climate : Effect, IActiveEffect
{
    public void Invoke(Card card)
    {
        //This effect is reserved for climate cards
        Card.ClimateCard climate_card = (Card.ClimateCard)card;

        //Iterate through all cards in that range for both players
        var AllSections = board.sections;
        foreach (var PlayerSection in AllSections)
        {
            foreach (Card Card in PlayerSection.Value[climate_card.Range])
            {
                //One point less for all cards in those files
                if (Card is Card.SilverCard silver)
                {
                    silver.Power--;
                }
            }
        }
    }
}
/// <summary>
/// Increase the power by one point to the silver cards in this card range
/// </summary>
public class IncrementFile : Effect, IActiveEffect
{
    public void Invoke(Card card)
    {
        //The only card that has this effect are increment cards
        Card.IncrementCard incrementCard = (Card.IncrementCard)card;

        //Iterate through all cards in that range and that player 
        foreach (Card Card in board.sections[incrementCard.Owner.ID][incrementCard.Range])
        {
            //Add one point for all cards in that file
            if (Card is Card.SilverCard silver)
            {
                silver.Power++;
            }
        }
    }
}
/// <summary>
/// Delete less powerful card of the opponent
/// </summary>
public class DeleteLessPowerCard : Effect, IActiveEffect
{
    public void Invoke(Card card)
    {
        Player opponent;
        Card.UnityCard LessPowerfulCard = null;
        int minPower = int.MaxValue;
        int position = 0;
        string range = "";
        CardCollection aux = new();

        if (card.Owner == gm.player1) opponent = gm.player2;
        else opponent = gm.player1;

        var PlayerSection = board.sections[opponent.ID];
        foreach (var RangeSection in PlayerSection)
        {
            aux = RangeSection.Value;
            for (int i = 0; i < aux.Count; i++)
            {
                if (aux[i] is Card.SilverCard silver
                    && silver.Power < minPower)
                {
                    minPower = silver.Power;
                    LessPowerfulCard = silver;
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
}
/// <summary>
/// Delete most powerful card between both players, 
/// </summary>
public class DeleteMostPowerCard : Effect, IActiveEffect
{
    public void Invoke(Card card)
    {
        Card.UnityCard MostPowerfulCard = null;
        int maxPower = int.MinValue;
        int pos = 0;
        string range = "";
        CardCollection aux = new();

        var AllSections = board.sections;
        foreach (var PlayerSection in AllSections)
        {
            foreach (var RangeSection in PlayerSection.Value)
            {
                aux = RangeSection.Value;
                for (int i = 0; i < aux.Count; i++)
                {
                    if (aux[i] is Card.SilverCard silver && silver.Power > maxPower)
                    {
                        maxPower = silver.Power;
                        MostPowerfulCard = silver;
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
    //Decoy cards
    public void Decoy(string name, string range, string player, Card.SpecialCard decoy)
    {
        Card Taken = null;

        CardCollection cards = board.sections[player][range];
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
}
/// <summary>
/// Leader Effect, draw an extra card between rounds
/// </summary>
public class DrawExtraCard : Effect
{
}
/// <summary>
/// Leader Effect, keep a random card between rounds
/// </summary>
public class KeepRandomCard : Effect
{
}
/// <summary>
/// Draw a card from the player deck
/// </summary>
public class TakeCardFromDeck : Effect, IActiveEffect
{
    public void Invoke(Card card)
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
        else gm.StartCoroutine(gm.SetAuxText("El deck esta vacio, no se robaron cartas"));
    }
}
/// <summary>
/// Draw a card from the player graveyard
/// </summary>
public class TakeCardFromGraveYard : Effect, IActiveEffect
{
    public void Invoke(Card card)
    {
        if (card.Owner.Hand.Count >= 10) return;

        Card MostPowerfulCard = null;
        int maxPower = int.MinValue;
        CardCollection Graveyard = card.Owner.GraveYard;
        int pos = 0;

        for (int i = Graveyard.Count - 1; i >= 0; i--)
        {
            if (Graveyard[i] is Card.SilverCard silver
                && silver.Power > maxPower)
            {
                maxPower = silver.Power;
                MostPowerfulCard = silver;
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
}
/// <summary>
/// Multiply the power of the card for all the same instances
/// </summary>
public class TimesTwins : Effect, IActiveEffect
{
    public void Invoke(Card card)
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
}
/// <summary>
/// Add a climate card from the hand or the deck of the player
/// </summary>
public class AddClimateCard : Effect, IActiveEffect
{
    public GameObject ClimateZone = GameObject.Find("ClimateZone");
    public void Invoke(Card card)
    {
        Card.SpecialCard aux = null;
        Dictionary<string, int> climate_pos = new()
        {
            {"M", 0},
            {"R", 1},
            {"S", 2},
        };
        foreach (Card Card in card.Owner.Hand)
        {
            if (Card is Card.ClimateCard climate)
            {
                if (board.climate_section[climate_pos[climate.Range]] == null)
                {
                    aux = climate;
                    climate.Owner = card.Owner;
                    AddClimateAux(climate_pos[climate.Range], climate, true);
                    break;
                }
            }
        }
        //Check in the deck
        if (aux == null)
        {
            foreach (Card Card in card.Owner.PlayerDeck)
            {
                if (Card is Card.ClimateCard climate)
                {
                    if (board.climate_section[climate_pos[climate.Range]] == null)
                    {
                        aux = climate;
                        climate.Owner = card.Owner;
                        AddClimateAux(climate_pos[climate.Range], climate, false);
                        break;
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
                //Apply effect
                Climate effect = new Climate();
                effect.Invoke(climate); 

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
                //Apply effect
                Climate effect = new Climate();
                effect.Invoke(climate); 
                
                climate.CardPrefab.tag = climate.Owner.ID;
            }
        }
    }
}
/// <summary>
/// Decoy card, it has its own invoke effect with more parameters
/// </summary>
public class Decoy : Effect
{
    public void Invoke(string name, string range, string player, Card.DecoyCard decoy)
    {
        Card Taken = null;
        CardCollection cards = board.sections[player][range];

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
        //Set the card properties as default
        if (Taken != null)
        {
            Taken.IsPlayed = false;
            Taken.Owner.Hand.Add(Taken);

            if (Taken is Card.UnityCard unity) unity.Power = unity.OriginalPower; 
            gm.StartCoroutine(gm.SetAuxText("La carta " + Taken.Name + " regres� a la mano de " + gm.currentPlayer.PlayerName));
        }
    }
}