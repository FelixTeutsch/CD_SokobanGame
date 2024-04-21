using System.Reflection.Metadata.Ecma335;

namespace libs;

using Newtonsoft.Json;
using System;
using System.Dynamic;
using System.Diagnostics;

public static class FileHandler
{

    private static string path = "../Games/";
    private static int level = -1;
    private static string[] files;
    private readonly static string envVar = "LEVELS_PATH";

    static FileHandler()
    {
        Initialize();
    }

    public static bool IsLevelsLeft()
    {
        return level < files.Length;
    }

    private static void Initialize()
    {

        if (Environment.GetEnvironmentVariable(envVar) != null)
        {
            path = Environment.GetEnvironmentVariable(envVar);
        };

        // Check if environment variable is set
        if (Directory.Exists(path + "Levels/"))
        {
            files = Directory.GetFiles(path + "Levels/");
            Array.Sort(files);
        }
        else
        {
            throw new DirectoryNotFoundException($"Directory not found at path: {path}");
        }

    }

    public static bool LoadNextLevel()
    {

        level++;
        return IsLevelsLeft();
    }

    public static dynamic ReadJson()
    {
        if (string.IsNullOrEmpty(files[level]))
        {
            throw new InvalidOperationException("JSON file path not provided in environment variable");
        }

        try
        {
            string jsonContent = File.ReadAllText(files[level]);
            dynamic jsonData = JsonConvert.DeserializeObject(jsonContent);
            return jsonData;
        }
        catch (FileNotFoundException)
        {
            throw new FileNotFoundException($"JSON file not found at path: {path}");
        }
        catch (Exception ex)
        {
            throw new Exception($"Error reading JSON file: {ex.Message}");
        }
    }


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

    /**
        * Save the current game state to a JSON file
        * @param gameObjects List of GameObjects
        * @param map Map object
        * @return string File path of the saved game state
        */
    public static string saveGameState(List<GameObject> gameObjects, Map map)
    {
        // get the number of current save files
        int saveFiles = Directory.GetFiles(path + "/Saves/").Length;
        string saveFilesString = (saveFiles + 1).ToString("D3");
        string levelNumberString = (level + 1).ToString("D3");

        string fileName = "Save_" + saveFilesString + "_Level_" + levelNumberString + ".json";

        // Construct the file path with leading zeros in the saveFiles part
        string filePath = Path.Combine(path, "Saves", fileName);

        //dynamic saveFileContent = ReadJson();

        dynamic jsonContent = new ExpandoObject();

        dynamic jsonMap = new ExpandoObject();
        jsonMap.height = map.MapHeight;
        jsonMap.width = map.MapWidth;
        jsonContent.levelNumber = level;
        jsonContent.levelName = map.LevelName;
        jsonContent.map = jsonMap;

        List<dynamic> jsonGameObjects = new List<dynamic>();
        foreach (var gameObject in gameObjects)
        {
            dynamic jsonGameObject = new ExpandoObject();
            jsonGameObject.Type = Enum.GetName(typeof(GameObjectType), gameObject.Type);
            jsonGameObject.Color = gameObject.Color;
            jsonGameObject.PosX = gameObject.PosX;
            jsonGameObject.PosY = gameObject.PosY;
            jsonGameObjects.Add(jsonGameObject);
        }
        jsonContent.gameObjects = jsonGameObjects;

        WriteJson(jsonContent, filePath);
        return fileName;
    }

}
