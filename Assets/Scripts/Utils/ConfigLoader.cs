using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Config
{

    private static Config instance;
    public static Config Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new Config();
                instance.LoadConfig();
            }

            return instance;
        }
    }

    public void LoadConfig()
    {
        // Get path to Streaming Assets folder
        string path = System.IO.Path.Combine(Application.streamingAssetsPath, "PortraitConfig.json");
        string configJson = System.IO.File.ReadAllText(path);
        Config config = JsonUtility.FromJson<Config>(configJson);
        Debug.Log("loading config: " + configJson);
    }
}