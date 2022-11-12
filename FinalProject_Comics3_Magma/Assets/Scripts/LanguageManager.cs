using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanguageManager : ISubscriber
{
    private static Dictionary<string, string> _languageDictionary;
    string[][] Matrix;
    public LanguageManager(string[][] matrix)
    {
        Publisher.Subscribe(this, typeof(ChangeLanguageMessage));
        Matrix = matrix;
        ToDictionary(ELanguage.Italiano, matrix);
    }

    public static void ToDictionary(ELanguage eLanguage, string[][] matrix)
    {
        int index = 0;
        switch (eLanguage)
        {
            case ELanguage.Italiano:
                index = 1;
                break;
            case ELanguage.English:
                index = 2;
                break;
        }

        _languageDictionary = new Dictionary<string, string>();
        for (int i = 0; i < matrix.Length; i++)
        {
            _languageDictionary.Add(matrix[i][0], matrix[i][index]);
        }
        
    }

    public static string GetValue(string key)
    {
        return _languageDictionary[key];
    }

    public void OnPublish(IPublisherMessage message)
    {
        if (message is ChangeLanguageMessage changeLanguageMessage)
        {
            ToDictionary(changeLanguageMessage.CurrentLanguage, Matrix);
        }
    }

    public void OnDisableSubscribe()
    {
        Publisher.Unsubscribe(this, typeof(ChangeLanguageMessage));
    }
}
