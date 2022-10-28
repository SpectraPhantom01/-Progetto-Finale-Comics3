using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private LanguageManager languageManager;
    private void Awake()
    {
        if (!FileSystem.Load("LanguageManager", "csv", out string[] fileLoaded))
            throw new Exception("File LanguageManager non caricato correttamente, controllare eventuali posizioni del file o il nome del file o l'estensione del file");

        CSVFileReader.Parse(fileLoaded, ';', "", true);

        languageManager = new LanguageManager(CSVFileReader.FileMatrix);
    }

    private void Start()
    {
        Invoke("ChangeLanguage", 5);
    }
    private void ChangeLanguage()
    {
        Publisher.Publish(new ChangeLanguageMessage(ELanguage.Italiano));
    }
}
