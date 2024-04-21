namespace libs;
using Newtonsoft.Json;
using System.Collections.Generic;

public class Map
{
    private char[,] RepresentationalLayer;

    private Stack<GameObject?[,]> history = new Stack<GameObject?[,]>();

    private int _mapWidth;
    private int _mapHeight;

    public GameObject?[,] GetGameObjectLayer()
    {
        if (history.Count == 0)
            return null;

        return history.Peek();
    }


    public Map()
    {
        _mapWidth = 30;
        _mapHeight = 8;
        RepresentationalLayer = new char[_mapHeight, _mapWidth];
        GameObject?[,] gameObjectLayer = new GameObject[_mapHeight, _mapWidth];
        history.Push(gameObjectLayer);
    }

    public Map(int width, int height)
    {
        _mapWidth = width;
        _mapHeight = height;
        RepresentationalLayer = new char[_mapHeight, _mapWidth];
        GameObject?[,] gameObjectLayer = new GameObject[_mapHeight, _mapWidth];
        history.Push(gameObjectLayer);
    }

    public void Initialize()
    {
        history.Clear();
        RepresentationalLayer = new char[_mapHeight, _mapWidth];

        GameObject?[,] gameObjectLayer = new GameObject[_mapHeight, _mapWidth];
        history.Push(gameObjectLayer);
        // Initialize the map with some default values
        for (int i = 0; i < gameObjectLayer.GetLength(0); i++)
        {
            for (int j = 0; j < gameObjectLayer.GetLength(1); j++)
            {
                gameObjectLayer[i, j] = new Floor();
            }
        }
    }

    public int MapWidth
    {
        get { return _mapWidth; } // Getter
        set { _mapWidth = value; Initialize(); } // Setter
    }

    public int MapHeight
    {
        get { return _mapHeight; } // Getter
        set { _mapHeight = value; Initialize(); } // Setter
    }

    public GameObject Get(int x, int y)
    {
        // Retrieve the latest GameObjectLayer from the stack
        GameObject?[,] currentLayer = GetGameObjectLayer();

        // Check if the coordinates are within bounds
        if (x >= 0 && x < _mapWidth && y >= 0 && y < _mapHeight)
        {
            // Return the object at the specified coordinates from the topmost layer
            return currentLayer[x, y];
        }

        // If coordinates are out of bounds, return null or throw an exception as per your requirement
        return null; // or throw new IndexOutOfRangeException("Coordinates are out of bounds");
    }

    public void Set(int x, int y, GameObject gameObject)
    {
        // Retrieve the latest GameObjectLayer from the stack
        GameObject?[,] currentLayer = GetGameObjectLayer();

        // Check if the coordinates are within bounds
        if (x >= 0 && x < _mapWidth && y >= 0 && y < _mapHeight)
        {
            // Set the object at the specified coordinates in the topmost layer
            currentLayer[x, y] = gameObject;
        }
        else
        {
            // If coordinates are out of bounds, you can choose to handle it here
            // For example, you can ignore the set operation, throw an exception, or resize the layer
            // Here, we'll just print a warning to the console
            Console.WriteLine("Warning: Coordinates are out of bounds.");
        }
    }



    public void Set(GameObject gameObject)
    {
        int posY = gameObject.PosY;
        int posX = gameObject.PosX;
        int prevPosY = gameObject.GetPrevPosY();
        int prevPosX = gameObject.GetPrevPosX();

        GameObject? obstacle = Get(posY, posX);

        if (obstacle != null &&
                (obstacle.Type == GameObjectType.Player ||
                (obstacle.Type == GameObjectType.Box &&
                gameObject.Type == GameObjectType.Goal))
            )
        {
            return;
        }

        if (prevPosX >= 0 && prevPosX < _mapWidth &&
                prevPosY >= 0 && prevPosY < _mapHeight)
        {
            Set(prevPosY, prevPosX, new Floor());
        }

        if (posX >= 0 && posX < _mapWidth &&
                posY >= 0 && posY < _mapHeight)
        {
            Set(posY, posX, gameObject);
            RepresentationalLayer[gameObject.PosY, gameObject.PosX] = gameObject.CharRepresentation;
        }
    }

    public void Save()
    {
        // Retrieve the latest GameObjectLayer from the stack
        GameObject?[,] currentLayer = GetGameObjectLayer();

        // Create a deep copy of the currentLayer
        GameObject?[,] clonedLayer = (GameObject?[,])currentLayer.Clone();

        // Push the clonedLayer onto the stack
        history.Push(clonedLayer);
    }


    public void Undo()
    {
        if (history.Count > 0)
        {
            history.Pop(); // Pop the top element from the stack and assign it to GameObjectLayer
        }
    }


    public void Reset()
    {
        Initialize();
    }
}