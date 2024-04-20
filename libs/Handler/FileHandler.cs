using System.Reflection.Metadata.Ecma335;

namespace libs;

using Newtonsoft.Json;
using System;

public static class FileHandler
{

    private static string path = "../Games/Levels/";
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
        if (Directory.Exists(path))
        {
            files = Directory.GetFiles(path);
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
}
