using Assets.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace Logica_del_juego_en_consola
{
    static class Effect
    {
        public static Dictionary<EffectType, Action<Card>> Effects = new Dictionary<EffectType, Action<Card>>
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

        
        public static void IncrementFile(Card card)
        {
            
        }
        public static void TakeCardFromGraveYard(Card card)
        {
            if (card.Owner.GraveYard.Count > 0)
            {
                List<Card> GraveYard = Player.Shuffle(card.Owner.GraveYard);
                card.Owner.Hand.Add(GraveYard[0]);
                card.Owner.GraveYard.Remove(GraveYard[0]);
            }
        }
        public static void TimesTwins(Card card)
        {
            var PlayerSection = card.Owner.board.sections[card.Owner.ID];
            foreach (var Cards in PlayerSection.Values)
            {
                foreach (Card card1 in Cards)
                {
                    if (card1 is Card.UnityCard unity_card && unity_card.UnityType == UnityType.Gold)
                    {
                        unity_card.Power *= 2;
                    }
                }
            }
        }
        public static void TakeCardFromDeck(Card card)
        {
            if (card.Owner.PlayerDeck.Count > 0)
            {
                card.Owner.Hand.Add(card.Owner.PlayerDeck[0]);
                card.Owner.PlayerDeck.RemoveAt(0);
            }
        }
        public static void None(Card card)
        {

        }
        public static void KeepRandomCard(Card card)
        {

        }
        public static void DrawExtraCard(Card card)
        {

        }
        //Elimina la carta mas poderosa en TODAS las regiones del tablero
        public static void DeleteMostPowerCard(Card card)
        {
            Card.UnityCard MostPowerCard = null;
            int max_power = int.MinValue;

            var AllSections = card.Owner.board.sections;
            foreach (var PlayerSection in AllSections)
            {
                foreach (var RangeSection in PlayerSection.Value)
                {
                    foreach (Card Card in RangeSection.Value)
                    {
                        if (Card is Card.UnityCard unity_card && unity_card.Power > max_power)
                        {
                            MostPowerCard = unity_card;
                        }
                    }
                }
            }

            if (MostPowerCard != null)
            {
                MostPowerCard.Owner.GraveYard.Add(MostPowerCard);

                var PlayerSection = MostPowerCard.Owner.board.sections[MostPowerCard.Owner.ID];
                foreach (var RangeSection in PlayerSection)
                {
                    foreach (Card card1 in RangeSection.Value)
                    {
                        if (card1 == MostPowerCard)
                        {
                            RangeSection.Value.Remove(MostPowerCard);
                        }
                    }
                }
            }
        }

        //Elimina la carta con menos poder del enemigo
        public static void DeleteLessPowerCard(Card card)
        {
            Card.UnityCard LessPowerCard = null;
            int min_power = int.MaxValue;

            var AllSections = card.Owner.board.sections;
            foreach (var PlayerSection in AllSections)
            {
                if (PlayerSection.Key != card.Owner.ID)
                {
                    foreach (var RangeSection in PlayerSection.Value)
                    {
                        foreach (Card card1 in RangeSection.Value)
                        {
                            if (card1 is Card.UnityCard unity_card && unity_card.Power < min_power)
                            {
                                LessPowerCard = unity_card;
                            }
                        }
                    }
                }
            }
            if (LessPowerCard != null)
            {
                LessPowerCard.Owner.GraveYard.Add(LessPowerCard);

                var PlayerSection = LessPowerCard.Owner.board.sections[LessPowerCard.Owner.ID];
                foreach (var RangeSection in PlayerSection)
                {
                    foreach (Card card1 in RangeSection.Value)
                    {
                        if (card1 == LessPowerCard)
                        {
                            RangeSection.Value.Remove(LessPowerCard);
                        }
                    }
                }
            }
        }
        public static void Decoy(Card card)
        {

        }

        //public static void Decoy(Card decoyCard, Card targetCard)
        //{
        //    // Comprueba si la carta señuelo y la carta objetivo pertenecen al mismo jugador
        //    if (decoyCard.Owner != targetCard.Owner)
        //    {
        //        throw new Exception("Decoy card and target card must belong to the same player.");
        //    }

        //    // Comprueba si la carta objetivo está en el campo
        //    var playerSections = decoyCard.Owner.board.sections[decoyCard.Owner.ID];
        //    bool isTargetCardOnField = playerSections.Any(section => section.Value.Contains(targetCard));
        //    if (!isTargetCardOnField)
        //    {
        //        throw new Exception("Target card must be on the field.");
        //    }

        //    // Devuelve la carta objetivo a la mano del jugador
        //    targetCard.Owner.Hand.Add(targetCard);

        //    // Elimina la carta objetivo del campo
        //    foreach (var section in playerSections)
        //    {
        //        section.Value.Remove(targetCard);
        //    }

        //    // Coloca la carta señuelo en el campo
        //    // Aquí necesitarás decidir en qué sección colocar la carta señuelo
        //    // En este ejemplo, la colocamos en la misma sección que la carta objetivo
        //    string targetCardRange = targetCard is Card.UnityCard unityCard ? unityCard.Range : null;
        //    if (targetCardRange != null)
        //    {
        //        playerSections[targetCardRange].Add(decoyCard);
        //    }
        //}

        public static void Climate(Card card)
        {
            if (card is Card.SpecialCard climate_card)
            {
                var AllSections = card.Owner.board.sections;
                //Iterate through all board sections
                foreach (var PlayerSection in AllSections)
                {
                    var CardsInSection = PlayerSection.Value[climate_card.Range];
                    //Iterate through each card in card.Range section
                    foreach (Card CardInSection in CardsInSection)
                    {
                        if (CardInSection is Card.UnityCard unity_card)
                        {
                            unity_card.Power--;
                        }
                    }
                }
            }
        }

        public static void RestoreOriginalPower(Card card)
        {
            var AllSections = card.Owner.board.sections;
            foreach (var PlayerSection in AllSections)
            {
                var RangeSection = PlayerSection.Value;
                foreach (var Cards in RangeSection.Values)
                {
                    foreach(Card card1 in Cards)
                    {
                        if (card1 is Card.UnityCard unity_card)
                        {
                            unity_card.Power = unity_card.OriginalPower;
                        }
                    }
                }
            }

            foreach(Card card2 in card.Owner.board.climate_section)
            {
                if (card2 != null)
                {
                    Climate(card2);
                }
            }
        }

        public static void Clearance(Card card)
        {
            var ClimateSection = card.Owner.board.climate_section;
            for (int i = 0; i < ClimateSection.Length; i++)
            {
                if (ClimateSection[i] != null)
                {
                    Card.SpecialCard ClimateCard = ClimateSection[i];
                    var AllSections = card.Owner.board.sections;
                    //Iterate through all board sections
                    foreach (var PlayerSection in AllSections)
                    {
                        var CardsInSection = PlayerSection.Value[ClimateCard.Range];
                        //Iterate through each card in card.Range section
                        foreach (Card CardInSection in CardsInSection)
                        {
                            if (CardInSection is Card.UnityCard unity_card)
                            {
                                unity_card.Power++;
                            }
                        }
                    }
                    ClimateCard.Owner.GraveYard.Add(ClimateCard);
                    ClimateSection[i] = null;
                }
            }
        }
        public static void CleanMeleeFile(Card card)
        {
            var AllSections = card.Owner.board.sections;
            foreach (var PlayerSection in AllSections)
            {
                var CardsInSection = PlayerSection.Value["M"];
                List<Card> cardsToMove = new List<Card>(CardsInSection);
                foreach (Card card1 in cardsToMove)
                {
                    card1.Owner.GraveYard.Add(card1);
                    CardsInSection.Remove(card1);
                }
            }
        }

        public static void CleanRangedFile(Card card)
        {
            var AllSections = card.Owner.board.sections;
            foreach (var PlayerSection in AllSections)
            {
                var CardsInSection = PlayerSection.Value["R"];
                List<Card> cardsToMove = new List<Card>(CardsInSection);
                foreach (Card card1 in cardsToMove)
                {
                    card1.Owner.GraveYard.Add(card1);
                    CardsInSection.Remove(card1);
                }
            }
        }
        public static void CleanSiegeFile(Card card)
        {
            var AllSections = card.Owner.board.sections;
            foreach (var PlayerSection in AllSections)
            {
                var CardsInSection = PlayerSection.Value["S"];
                List<Card> cardsToMove = new List<Card>(CardsInSection);
                foreach (Card card1 in cardsToMove)
                {
                    card1.Owner.GraveYard.Add(card1);
                    CardsInSection.Remove(card1);
                }
            }
        }
        public static void AssignProm(Card card)
        {

        }
    }
}
