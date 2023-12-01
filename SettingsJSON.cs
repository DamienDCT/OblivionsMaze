using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using System;


public class SettingsJSON : MonoBehaviour
{
    //instance du SettingJSON
    public static SettingsJSON instance;

    //référence aux options actuelles
    public Settings settings;

    void Awake()
    {
        //on fait un singleton
        if(instance == null)
        {
            instance = this;
        } else {
            Destroy(gameObject);
        }
        settings = new Settings();
        DontDestroyOnLoad(this);
    }

    //On charge les options et on met à jour les valeurs des différentes options dans les panels
    private void Start()
    {
        LoadSettingsFile();
        AudioManager.instance.UpdateMixerValues(settings.sonSettings.generalVolume, settings.sonSettings.musicVolume, settings.sonSettings.effectVolume);
    }

    //méthode pour mettre à jour les options (sauf quand on est en mode créatif)
    public void UpdateSettingsFile()
    {
        if(!CreativeMode.instance.isCreativeActivated){
            string settingsData = JsonUtility.ToJson(settings);
            string filePath = Application.persistentDataPath + "/SettingsData.json";
            File.WriteAllText(filePath, settingsData);
        }
    }

    //méthode pour charger les options (sauf quand on est en mode créatif)
    public void LoadSettingsFile()
    {
        if(!CreativeMode.instance.isCreativeActivated){
            //on accède au chemin pour le json
            string filePath = Application.persistentDataPath + "/SettingsData.json";
            string settingsData = "";
            try
            {
                //on essaye de récupérer les options
                settingsData = File.ReadAllText(filePath);
            } catch(IOException e)
            {
                //si il y a une erreur on les mets à jour et on recharge les options
                UpdateSettingsFile();
                LoadSettingsFile();
                return;
            }
            settings = JsonUtility.FromJson<Settings>(settingsData);
            Screen.SetResolution(settings.videoSettings.resolutionWidth, settings.videoSettings.resolutionHeight, true, settings.videoSettings.resolutionRefreshRate);
        }
    }
}


[System.Serializable]
public class Settings
{
    //références aux options pour le son, les controles et vidéos
    public SoundSettings sonSettings;
    public Controls controlSettings;
    public Video videoSettings;

    //on initialise les options
    public Settings(){
        sonSettings = new SoundSettings();
        controlSettings = new Controls();
        videoSettings = new Video();
    }
}

[System.Serializable]
public class SoundSettings
{
    //variables pour le volume général, de la musique, et des effets sonores par défaut
    public float generalVolume = 1;
    public float musicVolume = 0.5f;
    public float effectVolume = 0.5f;
}

[System.Serializable]
public class Controls{}

[System.Serializable]
public class Video
{
    //booléen qui indique si les effets visuels sont activés ou non
    public bool isVFXToggled = true;
    //booléen qui indique si le jour est activé ou non
    public bool isDayToggled = true;
    //string permettant de sauvegarder la largeur de la résolution
    public int resolutionWidth = 1920;
    //string permettant de sauvegarder la hauteur de la résolution
    public int resolutionHeight = 1080;
    //string permettant de sauvegarder le taux de rafraîchissement de la résolution
    public int resolutionRefreshRate = 144;
}