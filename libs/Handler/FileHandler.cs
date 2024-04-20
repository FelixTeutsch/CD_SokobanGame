using System.Reflection.Metadata.Ecma335;

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

        //saveFileContent.map.height = map.MapHeight;
        //saveFileContent.map.width = map.MapWidth;

        List<string> jsonObjects = new List<string>();

        int i = 0;

        foreach(var gameObject in saveFileContent.gameObjects){
            /*if(gameObject.Type == "Wall"){
                continue;
            }
            else{
                //saveFileContent.gameObjects[i].Type = gameObjects[i].Types;
                //saveFileContent.gameObjects[i].Type = Enum.GetName(typeof(GameObjectType), gameObjects[i].Types);
                saveFileContent.gameObjects[i].Color = gameObjects[i].Color;
                saveFileContent.gameObjects[i].PosX = gameObjects[i].PosX;
                saveFileContent.gameObjects[i].PosY = gameObjects[i].PosY;
            }*/

            jsonObjects.Add(JsonConvert.SerializeObject(gameObject));
            i++;
        }
        //saveFileContent.gameObject = gameObjects;*/

        string jsonContent = "[" + string.Join(",", jsonObjects) + "]";

        WriteJson(jsonContent, filePath);
        
    }

}
