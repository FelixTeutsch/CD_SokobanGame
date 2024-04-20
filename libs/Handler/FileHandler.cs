using System.Reflection.Metadata.Ecma335;

using System.Dynamic;
using System.Diagnostics;
namespace libs;

using Newtonsoft.Json;

public static class FileHandler
{
    private static string filePath = " ";
    private readonly static string envVar = "GAME_SETUP_PATH";

    static FileHandler()
    {
        Initialize();
    }

    private static void Initialize()
    {
        filePath = "../Setup.json";
    }

    public static void loadNewLevel(string path)
    {
        filePath = path;
    }

    public static dynamic ReadJson()
    {
        if (string.IsNullOrEmpty(filePath))
        {
            throw new InvalidOperationException("JSON file path not provided in environment variable");
        }

        try
        {
            string jsonContent = File.ReadAllText(filePath);
            dynamic jsonData = JsonConvert.DeserializeObject(jsonContent);
            return jsonData;
        }
        catch (FileNotFoundException)
        {
            throw new FileNotFoundException($"JSON file not found at path: {filePath}");
        }
        catch (Exception ex)
        {
            throw new Exception($"Error reading JSON file: {ex.Message}");
        }
    }

    /*public static dynamic UpdateJson(string path){

        if (string.IsNullOrEmpty(filePath))
        {
            throw new InvalidOperationException("JSON file path not provided in environment variable");
        }

    }*/

    public static void WriteJson(object data, string filePath)
{
    try
    {
        string jsonString = JsonConvert.SerializeObject(data, Formatting.Indented);
        File.WriteAllText(filePath, jsonString);
        Console.WriteLine($"JSON data written to file: {filePath}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error writing JSON data to file: {ex.Message}");
    }
}

    public static void saveGameState(GameObject? player , List<GameObject> gameObjects, Map map){

        filePath = "../Games/Saves/Save1.json";
        dynamic saveFileContent = ReadJson();

        List<dynamic> jsonGameObjects = new List<dynamic>();

        int i = 0;

        foreach(var gameObject in gameObjects)
    {

        dynamic jsonGameObject = new ExpandoObject();
        //jsonGameObject.Type = Enum.GetName(typeof(GameObjectType), gameObject.Types);
        jsonGameObject.Color = gameObject.Color;
        jsonGameObject.PosX = gameObject.PosX;
        jsonGameObject.PosY = gameObject.PosY;
        
        jsonGameObjects.Add(jsonGameObject);
    }

        WriteJson(jsonGameObjects, filePath);
        
    }

}
