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
            { EffectType.DecreasePowerShortRange, DecreasePowerShortRange },
            { EffectType.IncreasePowerSilverCards, IncreasePowerSilverCards },
            // Agrega aquí otros efectos
        };

        

        public static void DecreasePowerShortRange(Card card)
        {
            Console.WriteLine("esto esta empingao");
        }

        public static void IncreasePowerSilverCards(Card card)
        {
            // Implementa aquí el efecto
        }

        // Agrega aquí otros métodos de efecto
    }

}
