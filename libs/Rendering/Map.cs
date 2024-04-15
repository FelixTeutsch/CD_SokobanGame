namespace libs;
using Newtonsoft.Json;

public class Map
{
    private char[,] RepresentationalLayer;

    private List<GameObject?[,]> history = new List<GameObject?[,]>();
    private GameObject?[,] GameObjectLayer;

    private int _mapWidth;
    private int _mapHeight;

    public GameObject?[,] GetGameObjectLayer()
    {
        return GameObjectLayer;
    }

    public Map()
    {
        _mapWidth = 30;
        _mapHeight = 8;
        RepresentationalLayer = new char[_mapHeight, _mapWidth];
        GameObjectLayer = new GameObject[_mapHeight, _mapWidth];
    }

    public Map(int width, int height)
    {
        _mapWidth = width;
        _mapHeight = height;
        RepresentationalLayer = new char[_mapHeight, _mapWidth];
        GameObjectLayer = new GameObject[_mapHeight, _mapWidth];
    }

    public void Initialize()
    {
        RepresentationalLayer = new char[_mapHeight, _mapWidth];
        GameObjectLayer = new GameObject[_mapHeight, _mapWidth];

        // Initialize the map with some default values
        for (int i = 0; i < GameObjectLayer.GetLength(0); i++)
        {
            for (int j = 0; j < GameObjectLayer.GetLength(1); j++)
            {
                GameObjectLayer[i, j] = new Floor();
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
        return GameObjectLayer[x, y];
    }

    public void Set(GameObject gameObject)
    {
        int posY = gameObject.PosY;
        int posX = gameObject.PosX;
        int prevPosY = gameObject.GetPrevPosY();
        int prevPosX = gameObject.GetPrevPosX();

        GameObject gameObjectCopy = new GameObject(posX, posY, gameObject.Color, gameObject.CharRepresentation, gameObject.Type);

        if (GameObjectLayer[posY, posX] != null && GameObjectLayer[posY, posX].Type == GameObjectType.Player)
        {
            return;
        }

        if (prevPosX >= 0 && prevPosX < _mapWidth &&
                prevPosY >= 0 && prevPosY < _mapHeight)
        {
            GameObjectLayer[prevPosY, prevPosX] = new Floor();
        }

        if (posX >= 0 && posX < _mapWidth &&
                posY >= 0 && posY < _mapHeight)
        {
            if (gameObject.Type == GameObjectType.Player)
                GameObjectLayer[posY, posX] = gameObject;
            else
                GameObjectLayer[posY, posX] = gameObjectCopy;
            RepresentationalLayer[gameObject.PosY, gameObject.PosX] = gameObject.CharRepresentation;
        }
    }

    public void Save()
    {
        history.Add(GameObjectLayer);
    }

    public void Undo()
    {
        if (history.Count > 0)
        {
            GameObjectLayer = history[history.Count - 1];
            history.RemoveAt(history.Count - 1);
        }
    }
}