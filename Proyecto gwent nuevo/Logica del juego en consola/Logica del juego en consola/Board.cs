using Logica_del_juego_en_consola;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
//using static UnityEditor.Experimental.GraphView.GraphView;

namespace Assets.Scripts
{
    public class Board
    {
        private static Board _instance;

        public static Board Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Board();
                }
                return _instance;
            }
        }


        
        //The board with its sections
        public Dictionary<string, Dictionary<string, List<Card>>> sections;
        //Climate Section
        public Card.SpecialCard[] climate_section;


        private Board()
        {
            sections = new Dictionary<string, Dictionary<string, List<Card>>>()
            {
                {
                    //section of player 2
                    "player2", new Dictionary<string, List<Card>>()
                    {
                        {"S", new List<Card>() },
                        {"R", new List<Card>() },
                        {"M", new List<Card>() }
                    }
                },
                {
                    //section of player 1
                    "player1", new Dictionary<string, List<Card>>()
                    {
                        {"M", new List<Card>() },
                        {"R", new List<Card>() },
                        {"S", new List<Card>() }
                    }
                }
            };

            climate_section = new Card.SpecialCard[3];
        }


    }
}
