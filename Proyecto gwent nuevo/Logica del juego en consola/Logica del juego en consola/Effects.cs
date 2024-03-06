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
            { EffectType.DeleteLessPowerCard, DeleteLessPowerCard },
            { EffectType.DeleteMostPowerCard, DeleteMostPowerCard },
            { EffectType.DrawExtraCard, DrawExtraCard },
            { EffectType.KeepRandomCard, KeepRandomCard },
            { EffectType.None, None },
            { EffectType.TakeCardFromDeck, TakeCardFromDeck },
            { EffectType.TakeCardFromGraveYard, TakeCardFromGraveYard },
            { EffectType.TieIsWin, TieIsWin },
            { EffectType.TimesTwins, TimesTwins },
            //More effects
        };

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
                foreach(Card card1 in Cards)
                {
                    if (card1 is Card.UnityCard unity_card && unity_card.UnityType == UnityType.Gold)
                    {
                        unity_card.Power *= 2;
                    }
                }
            }
        }
        public static void TieIsWin(Card card)
        {

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
            int max_power = -1;

            var AllSections = card.Owner.board.sections;
            foreach(var PlayerSection in AllSections)
            {
                foreach(var RangeSection in PlayerSection.Value)
                {
                    foreach(Card Card in RangeSection.Value)
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
            int min_power = 100;

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
        public static void Clearance(Card card)
        {

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
