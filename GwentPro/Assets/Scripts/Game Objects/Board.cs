using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// It represents the board in the backend
/// </summary>


//TODO: Maybe do it with an static class
public class Board
{
    //This is because a design pattern that is known as Singleton, just to make sure 
    //that only exists one instance of the board class inside my program
    private static Board _instance;
    /// <summary>
    /// Gets the instance depending of the private instance created
    /// </summary>
    /// <value></value>
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
    /// <summary>
    /// Key: Represents the player as string
    /// Value: Dictionary<string, List<Card>>
    ///     Key: Represent the ranges as string 
    ///     Value: Store all cards in that range of that player
    /// </summary>
    
    //TODO: Change it to an array
    //So i can delete the bool that count how many cards i put on the section
    public Dictionary<string, Dictionary<string, CardCollection>> sections;
    /// <summary>
    /// Represent with an array of specialCard the climate section
    /// </summary>
    public Card.SpecialCard[] climate_section;
    /// <summary>
    /// Represent the increment section with an array of special card
    /// </summary>
    public Dictionary<string, Card.SpecialCard[]> increment_section;
    private Board()
    {
        sections = new Dictionary<string, Dictionary<string, CardCollection>>()
            {
                {
                    //section of player 2
                    "player2", new Dictionary<string, CardCollection>()
                    {
                        {"S", new() },
                        {"R", new() },
                        {"M", new() }
                    }
                },
                {
                    //section of player 1
                    "player1", new Dictionary<string, CardCollection>()
                    {
                        {"M", new() },
                        {"R", new() },
                        {"S", new() }
                    }
                }
            };

        climate_section = new Card.SpecialCard[3];

        increment_section = new Dictionary<string, Card.SpecialCard[]>()
        {
            { "player1" , new Card.SpecialCard[3] },
            {"player2", new Card.SpecialCard[3] }
        };
    }
}
