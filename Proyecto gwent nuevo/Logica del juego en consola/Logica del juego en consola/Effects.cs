using Assets.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logica_del_juego_en_consola
{
    static class Effect
    {
        public static Dictionary<EffectType, Action<Card>> Effects = new Dictionary<EffectType, Action<Card>>
        {
            { EffectType.AddClimateCard, AddClimateCard },
            { EffectType.AssignProm, AssignProm },
            { EffectType.CleanFile, CleanFile },
            { EffectType.Clearance, Clearance },
            { EffectType.Climate, Climate },
            { EffectType.Decoy, Decoy },
            { EffectType.DeleteLessPowerCard, DeleteLessPowerCard },
            { EffectType.DeleteMostPowerCard, DeleteMostPowerCard },
            { EffectType.DrawExtraCard, DrawExtraCard },
            { EffectType.KeepRandomCard, KeepRandomCard },
            { EffectType.None, None },
            { EffectType.TakeCard, TakeCard },
            { EffectType.TieIsWin, TieIsWin },
            { EffectType.TimesTwins, TimesTwins },
            //More effects
        };

        public static void TimesTwins(Card card)
        {
            
        }
        public static void TieIsWin(Card card)
        {

        }
        public static void TakeCard(Card card)
        {

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
        public static void DeleteMostPowerCard(Card card)
        {

        }
        public static void DeleteLessPowerCard(Card card)
        {

        }
        public static void Decoy(Card card)
        {

        }
        public static void Climate(Card card)
        {

        }
        public static void Clearance(Card card)
        {

        }
        public static void CleanFile(Card card)
        {

        }

        public static void AddClimateCard(Card card)
        {
            
        }

        public static void AssignProm(Card card)
        {
            
        }

        
    }

}
