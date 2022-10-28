using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
public static class FileSystem
{
    
    public static bool Save(string fileName, string extension, string content)
    {
        try
        {
            File.WriteAllText(@$"{Application.persistentDataPath}\Data\{fileName}{extension}", content);
            return true;
        }
        catch
        {
            throw new IOException("File not saved");
        }
    }

    public static bool Load(string fileName, string extension, out string fileLoaded)
    {
        try
        {
            fileLoaded = File.ReadAllText(@$"{Application.persistentDataPath}\Data\{fileName}{extension}");
            return true;
        }
        catch
        {
            throw new IOException("File not loaded");
        }
    }

    public static bool SaveJson(string fileName, string extension, List<SavableEntity> savableEntities)
    {
        try
        {
            string json = string.Empty;
            foreach (var item in savableEntities)
            {
                json += JsonUtility.ToJson(item) + Environment.NewLine;
            }

            Save(fileName, extension, json);
            return true;
        }
        catch
        {
            throw new IOException("File not saved");
        }
    }

    public static bool LoadJson(string fileName, string extension, out List<SavableEntity> savableEntities)
    {
        try
        {
            savableEntities = JsonUtility.FromJson<List<SavableEntity>>(@$"{Application.persistentDataPath}\Data\{fileName}{extension}");
            savableEntities.ForEach(x => x.InstantiateEntity());
            return true;
        }
        catch
        {
            throw new IOException("File not loaded");
        }
    }
}
