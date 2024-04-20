using System.Reflection.Metadata.Ecma335;
using System.Text.Json.Nodes;

namespace libs;

using System.Security.Cryptography;
using Newtonsoft.Json;

public sealed class GameEngine
{
    private static GameEngine? _instance;
    private IGameObjectFactory gameObjectFactory;
    private int missingGoals = 0;
    private string levelName = "";

    public bool IsGameWon()
    {
        return missingGoals == 0;
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

        //Added for proper display of game characters
        Console.OutputEncoding = System.Text.Encoding.UTF8;
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

    public bool LoadNextLevel()
    {
        bool levelLeft = FileHandler.LoadNextLevel();
        if (levelLeft)
            Setup();
        return levelLeft;
    }

    public void Setup()
    {
        // reset previous things:
        gameObjects.Clear();
        map.Reset();

        dynamic gameData = FileHandler.ReadJson();

        map.MapWidth = gameData.map.width;
        map.MapHeight = gameData.map.height;

        levelName = gameData.levelName;

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

        PlaceGameObjects();

        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(levelName);
        //Render the map
        for (int i = 0; i < map.MapHeight; i++)
        {
            for (int j = 0; j < map.MapWidth; j++)
            {
                DrawObject(map.Get(i, j));
            }
            Console.WriteLine();
        }
        Console.WriteLine(missingGoals + " goal(s) Missing");
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
                    if (
                        map.Get(boxY, boxX).Type == GameObjectType.Wall
                        || map.Get(boxY, boxX).Type == GameObjectType.Box
                    )
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
                            missingGoals++;
                        }
                        if (map.Get(boxY, boxX).Type == GameObjectType.Goal)
                        {
                            missingGoals--;
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

        GameObject?[,] gameObjectLayer = map.GetGameObjectLayer();
        if (gameObjectLayer == null)
            return;

        // iterate through all objects and update their position
        for (int y = 0; y < gameObjectLayer.GetLength(0); y++)
            for (int x = 0; x < gameObjectLayer.GetLength(1); x++)
                if (gameObjectLayer[y, x] != null)
                    if (gameObjectLayer[y, x].Type == GameObjectType.Box)
                    {
                        gameObjectLayer[y, x].PosX = x;
                        gameObjectLayer[y, x].PosY = y;
                    }
                    else if (gameObjectLayer[y, x].Type == GameObjectType.Player)
                    {
                        _focusedObject.PosX = x;
                        _focusedObject.PosY = y;
                    }



        // Update the missing boxes
        List<Goal> goals = gameObjects.OfType<Goal>().ToList();
        missingGoals = goals.Count;

        foreach (var goal in goals)
            if (gameObjectLayer[goal.PosY, goal.PosX].Type == GameObjectType.Box)
                missingGoals--;


    }

    // Method to create GameObject using the factory from clients
    public GameObject CreateGameObject(dynamic obj)
    {
        return gameObjectFactory.CreateGameObject(obj);
    }

    public void AddGameObject(GameObject gameObject)
    {
        if (gameObject.Type == GameObjectType.Box)
            missingGoals++;

        gameObjects.Add(gameObject);
    }

    private void PlaceGameObjects()
    {
        map.Set(_focusedObject);
        gameObjects.ForEach(
            delegate (GameObject obj)
            {
                if (obj != _focusedObject)
                    map.Set(obj);
            }
        );
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
