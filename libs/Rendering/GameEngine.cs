using System.Reflection.Metadata.Ecma335;
using System.Text.Json.Nodes;

namespace libs;

using System.Security.Cryptography;
using Newtonsoft.Json;

public sealed class GameEngine
{
    private static GameEngine? _instance;
    private IGameObjectFactory gameObjectFactory;

    private int missingBoxes = 0;

    public bool IsGameWon()
    {
        return missingBoxes == 0;
    }

    public static GameEngine Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GameEngine();
            }
            return _instance;
        }
    }

    private GameEngine()
    {
        //INIT PROPS HERE IF NEEDED
        gameObjectFactory = new GameObjectFactory();
    }

    private GameObject? _focusedObject;

    private Map map = new Map();

    private List<GameObject> gameObjects = new List<GameObject>();


    public Map GetMap()
    {
        return map;
    }

    public GameObject GetFocusedObject()
    {
        return _focusedObject;
    }

    public void Setup()
    {

        //Added for proper display of game characters
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        dynamic gameData = FileHandler.ReadJson();

        map.MapWidth = gameData.map.width;
        map.MapHeight = gameData.map.height;

        foreach (var gameObject in gameData.gameObjects)
        {
            AddGameObject(CreateGameObject(gameObject));
        }

        _focusedObject = gameObjects.OfType<PlayerSingelton>().First();

    }

    public void Render()
    {

        //Clean the map
        Console.Clear();

        map.Initialize();

        PlaceGameObjects();

        //Render the map
        for (int i = 0; i < map.MapHeight; i++)
        {
            for (int j = 0; j < map.MapWidth; j++)
            {
                DrawObject(map.Get(i, j));
            }
            Console.WriteLine();
        }
    }

    public void CheckCollision()
    {
        // player = _focusedObject   
        foreach (var gameObject in gameObjects)
        {
            if (gameObject == _focusedObject || gameObject.Type == GameObjectType.Goal)
            {
                continue;
            }
            if (gameObject.PosX == _focusedObject.PosX && gameObject.PosY == _focusedObject.PosY)
            {
                // check if wall
                if (gameObject.Type == GameObjectType.Wall)
                {
                    // do not move the player
                    _focusedObject.UndoMove();
                }
                else
                // check if box
                if (gameObject.Type == GameObjectType.Box)
                {

                    int boxX = gameObject.PosX + _focusedObject.getDx();
                    int boxY = gameObject.PosY + _focusedObject.getDy();

                    // check if there is a wall behind the box
                    if (map.Get(boxY, boxX).Type == GameObjectType.Wall || map.Get(boxY, boxX).Type == GameObjectType.Box)
                    {
                        // do not move the player
                        _focusedObject.UndoMove();
                    }
                    else
                    { // <- why does c# have ugly brackets?
                        // move the box
                        gameObject.PosX = boxX;
                        gameObject.PosY = boxY;

                        // this also works if we move box from target to target, because we are smart ppl
                        if (gameObject.Color == ConsoleColor.Green)
                        {
                            gameObject.Color = ConsoleColor.Yellow;
                            missingBoxes++;
                        }
                        if (map.Get(boxY, boxX).Type == GameObjectType.Goal)
                        {
                            missingBoxes--;
                            gameObject.Color = ConsoleColor.Green;
                        }
                    }
                }
            }
            // Get position
            // iterate through all other objects and check if there is already an object at that position
        }

        // Save the updated map
        map.Save();
    }

    public void Undo()
    {
        map.Undo();
        List<Box> boxes = gameObjects.OfType<Box>().ToList();
        foreach (var box in boxes)
        {
            gameObjects.Remove(box);
        }


        GameObject?[,] gameObjectLayer = map.GetGameObjectLayer();

        for (int x = 0; x < gameObjectLayer.GetLength(0); x++)
        {
            for (int y = 0; y < gameObjectLayer.GetLength(1); y++)
            {
                if (gameObjectLayer[x, y] != null)
                {
                    if (gameObjectLayer[x, y].Type == GameObjectType.Box)
                    {

                        Box newBox = new Box(//
                            gameObjectLayer[x, y].PosX, //
                            gameObjectLayer[x, y].PosY, //
                            gameObjectLayer[x, y].Color, //
                            gameObjectLayer[x, y].CharRepresentation, //
                            GameObjectType.Box);
                        AddGameObject(newBox);
                    }

                    else if (gameObjectLayer[x, y].Type == GameObjectType.Player)
                    {
                        _focusedObject = gameObjectLayer[x, y];
                        _focusedObject.PosX = y;
                        _focusedObject.PosY = x;
                    }
                }
            }
        }


        missingBoxes = boxes.Count;
        boxes = gameObjects.OfType<Box>().ToList();
        foreach (var box in boxes)
        {
            if (box.Color == ConsoleColor.Green)
            {
                missingBoxes--;
            }
        }
    }

    // Method to create GameObject using the factory from clients
    public GameObject CreateGameObject(dynamic obj)
    {
        return gameObjectFactory.CreateGameObject(obj);
    }

    public void AddGameObject(GameObject gameObject)
    {
        if (gameObject.Type == GameObjectType.Box)
            missingBoxes++;

        gameObjects.Add(gameObject);
    }

    private void PlaceGameObjects()
    {
        map.Set(_focusedObject);
        gameObjects.ForEach(delegate (GameObject obj)
        {
            if (obj != _focusedObject)
                map.Set(obj);
        });
    }

    private void DrawObject(GameObject gameObject)
    {

        Console.ResetColor();

        if (gameObject != null)
        {
            Console.ForegroundColor = gameObject.Color;
            Console.Write(gameObject.CharRepresentation);
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write(' ');
        }
    }
}