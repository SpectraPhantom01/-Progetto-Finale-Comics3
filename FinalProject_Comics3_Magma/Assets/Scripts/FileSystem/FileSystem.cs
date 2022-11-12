using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
public static class FileSystem
{
    
    public static bool Save(string fileName, string extension, string content)
    {
        extension = extension.Replace(".", "");
        try
        {
            File.WriteAllText(@$"{Application.dataPath}\Data\{fileName}.{extension}", content);
            return true;
        }
        catch (Exception e)
        {
            throw new IOException($"File not saved {e.Message}");
        }
    }

    public static bool Load(string fileName, string extension, out string[] linesReaded)
    {
        extension = extension.Replace(".", "");
        try
        {
            linesReaded = File.ReadAllLines(@$"{Application.dataPath}\Data\{fileName}.{extension}");
            return true;
        }
        catch (Exception e)
        {
            throw new IOException($"File not loaded: {e.Message}");
        }
    }

    public static bool SaveJson(string fileName, string extension, SavableInfosList savableEntities)
    {
        try
        {
            string json = string.Empty;
            //foreach (var item in savableEntities)
            //{
            //    json += JsonUtility.ToJson(item) + Environment.NewLine;
            //}
            json = JsonUtility.ToJson(savableEntities);
            Save(fileName, extension, json);
            return true;
        }
        catch (Exception e)
        {
            throw new IOException($"File not saved {e.Message}");
        }
    }

    public static bool LoadJson(string fileName, string extension, out SavableInfosList savableInfos)
    {
        extension = extension.Replace(".", "");
        try
        {
            var json = File.ReadAllText(@$"{Application.dataPath}\Data\{fileName}.{extension}");
            savableInfos = JsonUtility.FromJson<SavableInfosList>(json);
            //savableInfos = JsonUtility.FromJson<List<SavableInfos>>(@$"{Application.dataPath}\Data\{fileName}.{extension}");
            return true;
        }
        catch (Exception e)
        {
            throw new IOException($"File not loaded: {e.Message}");
        }
    }
}
