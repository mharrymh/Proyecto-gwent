# Reporte del proyecto Gwent-Pro en Unity

## Scenes Folder:

### InitialMenuScene:

Esta es la escena inicial del juego, contiene incorporado además un objeto "OptionsMenu" que es activado en el Canvas tras hacer click en el botón Options que a su vez desactiva el objeto InitialMenu.

**Scripts relacionados a esta escena:**
#### InitialMenu.cs: 

Aquí es donde se implementa la funcionalidad del menú inicial, además se agregan los objetos 'audio' y 'audioFX' que serán los encargados de el audio durante todo el juego. Se utiliza el método DontDestroyOnLoad() de Unity para así no eliminar el objeto cuando se cambie de escena.

```csharp
public void Start()
{
    soundM = GameObject.Find("AudioSourceEffects").GetComponent<SoundManager>();

    DontDestroyOnLoad(audio);
    DontDestroyOnLoad(audioFX);
}
```

### ChooseFactionScene: 

Escena donde se prepara el usuario para jugar, aqui ambos jugadores ponen su nombre y eligen su faccion de manera interactiva. El nombre se escribe en un pbjeto de InputField de Unity, las facciones estan representadas por dos botones, uno con un Sol que representa a faccion Luz y el otro es una Luna que representa la faccion oscuridad.

**Scripts asociados a esta escena:**
#### FactionMenuManager.cs: 
Los inputs recibidos por el usuario son guardados como variables en una clase estática PlayerData que servirá luego para crear los objetos "player1" y "player2" en el GameManager de la GameScene

```csharp
public void Play()
{
    SavePlayer1Name();
    SavePlayer2Name();

    if (!Player1Chose || PlayerData.Player1Name == null
        || PlayerData.Player1Name == "" || !Player2Chose || PlayerData.Player2Name == null
        || PlayerData.Player2Name == "") soundM.PlayErrorSound();

    else
    {
        soundM.PlayButtonSound();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
```
Si cuando el usuario toque Play, este dejó el campo vacio o sin seleccionar, no sera posible cambiar de escena y se reproducira un efecto de sonido de error.

#### PlayerData.cs:
```csharp
public static string Player1Name = null;
public static string Player2Name = null;

public static CardFaction FactionPlayer1;
public static CardFaction FactionPlayer2;
```

### GameScene:

Esta es la escena donde se lleva a cabo la accion, aqui es donde el usuario jugara y pasara la mayoria de su tiempo.

**Objetos importantes de la escena:**

-Board:
Este objeto representa con una imagen el tablero de juego, como objetos hijos contiene paneles que 
representan las distintas secciones donde se pueden jugar las cartas en el tablero, diferenciando al player1 del player2, las
diferentes zonas de rango de ataque asi como las secciones especiales como la del clima, la del lider y las de incremento, estos paneles son los que despues permitiran (o no) a las cartas ser jugadas.

**Ademas contiene otros objetos hijos importantes como:**
 "Points" (objeto de texto que muestra el puntaje actual de cada jugador)
 "PassButton" (boton de pasar turno)
 "Lives" (recurso que muestra visualmente al jugador las rondas ganadas o perdidas)
-HandPlayer: 
Este objeto es un panel donde se instancias los CardPrefab por primera vez, representa la mano de cartas del jugador actual
-ShowPanel:
Panel que es usado para aumentar visualmente un cardPrefab cuando el puntero del mouse se situa encima de este.

**Scripts asociados:**

#### Card.cs:
Contiene la estructura basica de todas las cartas:

```csharp
public Sprite CardImage;
public string Description { get; set; }
public string Name { get; private set; }
public CardFaction Faction { get; private set; }
public EffectType effectType { get; private set; }
public Player Owner { get; set; }
public bool IsPlayed { get; set; }
public GameObject CardPrefab { get; set; }
```
Contiene clases hijas que heredan de esta, separando los distintos tipos de carta
-Ejemplo:
```csharp
public class UnityCard : Card
{
    public string Range { get; private set; }
    public UnityType UnityType { get; private set; }
    public int OriginalPower { get; private set; }
    public int Power { get; set; }

    public UnityCard(string name, CardFaction cardFaction, EffectType effectType, string Range, UnityType Type, int power,
        Sprite CardImage)
        : base(name, cardFaction, effectType, CardImage)
    {
        this.Range = Range;
        UnityType = Type;
        OriginalPower = power;
        Power = power;
    }
}
```

#### CardDatabase.cs
Aqui es donde se crean todas las cartas del deck en dos listas de cartas (divididas por faccion).
Ejemplo:
```csharp
LightDeck.Add(new Card.UnityCard("Kitsune", CardFaction.Light, EffectType.AddClimateCard, "M", UnityType.Gold, 6, Resources.Load<Sprite>("3")));
```

#### Player.cs
Clase que representa al objeto Player en el backend. Contiene la estructura del jugador.
Ejemplo del constructor de la clase y el método que asigna el "PlayerDeck" y el "Leader":
```csharp
public Player(CardFaction Faction, string ID, string name)
{
    this.Faction = Faction;
    GetPlayerDeck(Faction);
    this.ID = ID;
    Score = 0;
    RoundsWon = 0;
    Hand = new List<Card>();
    GraveYard = new List<Card>();
    HasPlayed = false;
    Changes = 0;
    PlayerName = name;
}
CardDatabase cards = new CardDatabase();
public void GetPlayerDeck(CardFaction Faction)
{
    if (Faction == CardFaction.Light)
    {
        PlayerDeck = cards.GetLightDeck();
    }
    else
    {
        PlayerDeck = cards.GetDarkDeck();
    }
    //Assign Leader property
    for (int i = 0; i < PlayerDeck.Count; i++)
    {
        if (PlayerDeck[i] is Card.LeaderCard leader)
        {
            Leader = leader;
            PlayerDeck.RemoveAt(i);
        }
    }
}
```

#### Board.cs:
Board es una clase Singleton porque la instanciación de la clase Board se realiza a través de la propiedad estática Instance. Si la instancia de Board no existe, se crea una nueva, de lo contrario, se devuelve la que ya existe. Este patrón asegura que solo exista una instancia de la clase Board en todo el programa.
```csharp
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
```
Esta clase representa el tablero y sus secciones en el backend, la seccion clima es un array de cartas,
la seccion incremento es un diccionario y la seccion del campo de batalla es un diccionario de diccionario,
dividiendo los jugadores y los rangos de ataque.

```csharp
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

    increment_section = new Dictionary<string, Card.SpecialCard[]>()
    {
        { "player1" , new Card.SpecialCard[3] },
        {"player2", new Card.SpecialCard[3] }
    };
}
```

#### DisplayCard.cs
Este script asociado al CardPrefab contiene la clase que se encarga de asignar a este
las caracteristicas de los objetos tipo Card y representarlas de manera visual.
a traves de un metodo ShowCard():
```csharp
public void ShowCard()
{
    //CardPrefab gets the name of the card
    this.name = card.Name;
    CardImage.sprite = card.CardImage;
    Description.text = card.Description;
    CardName.text = card.Name;

    if (card.Faction is CardFaction.Light) FactionImage.sprite = Resources.Load<Sprite>("Light");
    else FactionImage.sprite = Resources.Load<Sprite>("Dark");

    //Assign power if card is unity card type
    //Else assign 0
    if (card is Card.UnityCard unity_card)
    {
        Power.text = unity_card.Power.ToString();
    }
    else
    {
        Power.text = "";
    }
}
```

#### DragAndDrop.cs
Esta es una de las clases as importantes del juego, contiene toda la funcionalidad del Drag and Drop y 
de cuando la carta es jugada y posicionada, tanto en el tablero visual como en el Board();

```csharp
public void StartDrag()
{
    DisplayCard disp = gameObject.GetComponent<DisplayCard>();
    card = disp.card;
    if (!card.IsPlayed && !isDragging)
    {
        startPosition = gameObject.transform.position;
        isDragging = true;

        // Shoot the event when drag starts
        OnDragStart?.Invoke();
    }

    ////Save the pos to the leader card
    else if (card is Card.LeaderCard leader)
    {
        startPosition = gameObject.transform.position;
    }
}
public void EndDrag()
{
    if (!card.IsPlayed)
    {
        isDragging = false;

        if (DropZone != null) DropCard(card);
        else transform.position = startPosition;
    }
}
```
```csharp
public void PlayCard(Card card, string range = "")
{
    soundM.PlayCardSound();
    card.Owner.HasPlayed = true;
    //Set the card to true so that it 
    //wont interact anymore with the drag and drop
    card.IsPlayed = true;
    //Remove card from the player hand
    card.Owner.Hand.Remove(card);
    //Drop card
    transform.SetParent(DropZone.transform, false);
    //Disable passed property if you play a card 
    gm.currentPlayer.Passed = false;
    //Apply effect
    CardEffects.CardEffects[card.effectType].Invoke(card);

    //Add card in backend...

    //Update the power
     gm.SetPower(gm.player1);
     gm.SetPower(gm.player2);
     //Change turn
     gm.ChangeTurn();  
}
```

#### GameManager.cs
Script principal del juego, es el encargado de llevar a cabo toda la funcionalidad y la logica del juego, el cambio de turnos
y rondas, la finalizacion del juego, la instanciacion de la mano, la limpieza del tablero tras el cambio de ronda, la rotacion del
tablero tras el cambio de turno, etc... 

**Metodos importantes:**
```csharp
IEnumerator InstanciarCartas(int n, Player player)
{
    //Leader effect
    if (player.Leader.effectType == EffectType.DrawExtraCard && Round > 1) n++;

    player.Ready = true;
    if (player.PlayerDeck.Count < n) n = player.PlayerDeck.Count;
    Shuffle(player.PlayerDeck);

    //Do not change turns while instantiating
    GivingCards = true;
    for (int i = 0; i < n; i++)
    {
        if (player.Hand.Count < 10)
        {
            Instanciar(player);
            Debug.Log(player.Hand.Count + player.Hand[player.Hand.Count - 1].Name);
            player.PlayerDeck.RemoveAt(0);
            yield return new WaitForSeconds(0.2f); // Wait .2 seconds before instantiate the new card
        }
        else
        {
            //If the hand is full send the card directly to the graveyard
            player.GraveYard.Add(player.PlayerDeck[0]);
            if (player.GraveYard.Count == 1)
            {
                player.GraveyardObj.SetActive(true);
            }
            player.PlayerDeck.RemoveAt(0);
        }
    }
    GivingCards = false;
}
public void Instanciar(Player player)
{
    if (player.PlayerDeck.Count > 0)
    {
        Card card = player.PlayerDeck[0];
        //Crate new instance of the card on the playerhand
        GameObject CardInstance = Instantiate(cardPrefab, HandPanel);
        //Get the DisplayCard component of the new Card
        disp = CardInstance.GetComponent<DisplayCard>();
        //Assign the card to it
        disp.card = card;
        //Add card to player hand
        player.Hand.Add(card);
        //Assign owner of the card
        card.Owner = player;
        //Assign tag to Instance of the card for decoy effect implementation
        CardInstance.tag = card.Owner.ID;
        //Assign the cardPrefab to the card
        card.CardPrefab = CardInstance;
        //Display card
        disp.ShowCard();

        if (!player.LeaderPlayed)
        {
            if (player == player1) CardInstance = Instantiate(cardPrefab, LeaderPlayer1);
            else if (player == player2) CardInstance = Instantiate(cardPrefab, LeaderPlayer2);

            disp = CardInstance.GetComponent<DisplayCard>();
            disp.card = player.Leader;
            disp.ShowCard();
            player.LeaderPlayed = true;
        }
    }
```
Las cartas se instancian a traves de una corrutina de unity para agregarle efecto visual, las corrutinas tienen 
la capacidad de pausar la ejecucion de un metodo durante cierta cantidad de tiempo, mientras otros metodos se siguen ejecutando.

```csharp
public void ChangeTurn()
{
    if (GivingCards) return;

    Player perspective = currentPlayer;
    //Change the current player
    if (currentPlayer == player1) currentPlayer = player2;
    else currentPlayer = player1;

    //End the round if both of the players passed
    if (player1.Passed && player2.Passed)
    {
        RoundOver();
        RoundOverAux = true;
    }

    //Rotate the scene if the current player changed
    if (perspective != currentPlayer)
    {
        RotateObjects();
    }
    //Change the hand panel to the other player
    ChangeHandPanel();

    //Set Power for both players
    SetPower(player1);
    SetPower(player2);

    StartCoroutine(VisualChangeTurn());
}
```
Metodo de cambio de turno.

```csharp
void CleanBoard()
{
    //Effect of the leader
    //Keep a random card on the board between rounds
    Player KeepRandomPlayer = GetPlayerWithLeader(EffectType.KeepRandomCard);
    Card.UnityCard Keeper = null;

    if (KeepRandomPlayer != null)
    {
        Keeper = GetRandomUnityCardOnBoard(KeepRandomPlayer);
        if (Keeper != null && Keeper) Keeper.Power = Keeper.OriginalPower;
    }


    //Clean the backend board removing each card of the lists of cards
    //that represents the unity zones
    var AllSections = board.sections;
    foreach (var PlayerSection in AllSections)
    {
        var RangeSection = PlayerSection.Value;
        foreach (var Cards in RangeSection.Values)
        {
            for (int i = Cards.Count - 1; i >= 0; i--)
            {
                if (Keeper != Cards[i])
                {
                    //Erase card from board and add it to the player graveyard
                    Cards[i].Owner.GraveYard.Add(Cards[i]);
                    if (Cards[i].Owner.GraveYard.Count == 1)
                    {
                        Cards[i].Owner.GraveyardObj.SetActive(true);
                    }
                    //Set the IsPlayed to false so it can be played again
                    Cards[i].IsPlayed = false;
                    //Destroy the instance (object)
                    Destroy(Cards[i].CardPrefab);
                    Cards.RemoveAt(i);
                }
            }

        }
    }

    //Clean the backend climate section
    for (int i = 0; i < board.climate_section.Length; i++)
    {
        if (board.climate_section[i] != null)
        {
            Card card = board.climate_section[i];
            card.Owner.GraveYard.Add(card);
            if (card.Owner.GraveYard.Count == 1)
            {
                card.Owner.GraveyardObj.SetActive(true);
            }
            //Destroy the object
            CardBeaten(card);
            board.climate_section[i] = null;
        }
    }

    //Clean the increment section backend
    var IncrementSection = board.increment_section;
    foreach (Card[] cards in IncrementSection.Values)
    {
        for (int i = 0; i < cards.Length; i++)
        {
            if (cards[i] != null)
            {
                Card card = cards[i];
                card.Owner.GraveYard.Add(card);
                if (card.Owner.GraveYard.Count == 1)
                {
                    card.Owner.GraveyardObj.SetActive(true);
                }
                //Destroy the object
                CardBeaten(cards[i]);
                cards[i] = null;
            }
        }
    }
}
```
Metodo de limpieza del tablero (visual y no visual).

#### Effects.cs
Clase que se encarga de los efectos de las cartas, contiene un diccionario que relaciona un EffectType (enum de la clase Card) con metodos.
```csharp
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
```
**Ejemplos de estos metodos:**

```csharp
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
        ". Su poder se multiplicó por " + brothers));
}
```
Este metodo itera por el diccionario en la seccion del player que jugo la carta, multiplica el poder de esta por todas las cartas existentes con su mismo nombre.

```csharp
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
        gm.StartCoroutine(gm.SetAuxText("Se añadió " + MostPowerfulCard.Name + " a la mano de " + gm.currentPlayer.PlayerName
       + " desde el cementerio"));
    }
    else gm.StartCoroutine(gm.SetAuxText("El cementerio está vacío, no se robo ninguna carta"));

}
```
Este metodo itera por todas las cartas del cementerio de cartas del jugador y devuelve al campo la mas poderosa

```csharp
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
        gm.StartCoroutine(gm.SetAuxText("La carta " + Taken.Name + " regresó a la mano de " + gm.currentPlayer.PlayerName));
    }
}
```
Metodo especial, no pertenece al diccionario, pues este es llamado desde el PlayCard() de la clase DragAndDrop
Llamado al metodo Decoy:

```csharp
else if (card is Card.SpecialCard decoy && decoy.Type is SpecialType.Decoy)
{
    //Get the card to move
    GameObject CardToMove = decoy.CardPrefab.transform.parent.gameObject;
    //Move the decoy card to the parent transform
    decoy.CardPrefab.transform.SetParent(CardToMove.transform.parent.transform, false);

    string Zone = CardToMove.transform.parent.name;
    string BackendZone = null;
    string PlayerZone = card.Owner.ID;

    if (Zone.Contains("Melee"))
    {
        BackendZone = "M";
    }
    else if (Zone.Contains("Range"))
    {
        BackendZone = "R";
    }
    else if (Zone.Contains("Siege"))
    {
        BackendZone = "S";
    }

    if (BackendZone != null) CardEffects.Decoy(CardToMove.name, BackendZone, PlayerZone, decoy);
    else
    {
        transform.position = startPosition;
        decoy.IsPlayed = false;
        //Add card to the player hand
        decoy.Owner.Hand.Add(card);
    }
    //Move the card to the player hand 
    CardToMove.transform.SetParent(gm.HandPanel.transform, false);
}
```

### GameOverScene:
Esta escena es la encargada de mostrar el ganador cuando se haya acabado el juego, el ganador es una variable guardada en la clase estatica PlayerData que es asignada en el GameOver() del GameManager.

```csharp
public void GameOver()
{
    if (winner != null)
    {
        PlayerData.Winner = winner.PlayerName;
    }
    //Change scene
    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
}
```
Metodo de la clase GameManager.

```csharp
public void Start()
{
    soundM = GameObject.Find("AudioSourceEffects").GetComponent<SoundManager>();

    if (PlayerData.Winner != null)
    {
        WinnerText.text = "Felicidades!!! Ganó " + PlayerData.Winner;
    }
    else
    {
        WinnerText.text = "Uhhh! Empataron, jueguen de nuevo";
    }
}

public void Exit()
{
    soundM.PlayButtonSound();
    Application.Quit();
}
```






