using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
