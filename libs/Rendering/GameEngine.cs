using System.Reflection.Metadata.Ecma335;
using System.Text.Json.Nodes;

namespace libs;

using System.Security.Cryptography;
using Newtonsoft.Json;

public sealed class GameEngine
{
    private static GameEngine? _instance;
    private IGameObjectFactory gameObjectFactory;

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
                if (gameObject.Type == GameObjectType.Obstacle)
                {
                    // do not move the player
                    _focusedObject.UndoMove();
                }
                // check if the object is a box
                // if it is a box, check if there is an object behind it
                // if there is an object behind it, check if it is a goal
                // if it is a goal, remove the box and the goal
                // if it is not a goal, do not move the player
                // if there is no object behind it, move the player
            }
            // Get position
            // iterate through all other objects and check if there is already an object at that position
        }
    }

    // Method to create GameObject using the factory from clients
    public GameObject CreateGameObject(dynamic obj)
    {
        return gameObjectFactory.CreateGameObject(obj);
    }

    public void AddGameObject(GameObject gameObject)
    {
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