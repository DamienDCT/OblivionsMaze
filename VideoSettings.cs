using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VideoSettings : MonoBehaviour
{
    //booléen qui indique si les VFX sont activées ou non
    [SerializeField]
    private Toggle vfxToggled;

    //booléen qui indique si le jour ou la nuit est activé (dans le menu principal et le choix des niveaux)
    [SerializeField]
    private Toggle dayNightToggled;

    //Référence au bouton pour valider le changement effectué
    [SerializeField]
    private Button validationBouton;

    private Resolution[] resolutions;
    [SerializeField]
    private TMP_Dropdown dropdownResolution;

    private List<Resolution> filteredResolutions;
    private float currentRefreshRate;
    private int currentResolutionIndex = 0;

    // Quand le GameObject s'active, on désactive le bouton de validation
    public void OnEnable(){
        validationBouton.gameObject.SetActive(false);
    }

    private void Awake(){}

    private void Start(){
        resolutions = Screen.resolutions;
        dropdownResolution.ClearOptions();
        filteredResolutions = new List<Resolution>();
        currentRefreshRate = Screen.currentResolution.refreshRate;
        for(int i = 0; i < resolutions.Length; i++)
        {
            if(resolutions[i].refreshRate == currentRefreshRate)
            {
                filteredResolutions.Add(resolutions[i]);
            }
        }
        List<string> optionsInDropdown = new List<string>();
        for(int i = 7; i < filteredResolutions.Count; i++)
        {
            string resolutionOption = filteredResolutions[i].width + "x" + filteredResolutions[i].height + " @" + filteredResolutions[i].refreshRate + "Hz";
            optionsInDropdown.Add(resolutionOption);
            if(filteredResolutions[i].width == Screen.width && filteredResolutions[i].height == Screen.height)
            {
                currentResolutionIndex = i;
            }
        }
        dropdownResolution.AddOptions(optionsInDropdown);
        dropdownResolution.value = currentResolutionIndex;
        dropdownResolution.RefreshShownValue();
    }

    public void SetResolution(int resolutionIndex){
        currentResolutionIndex = resolutionIndex+7;
        Resolution resolution = filteredResolutions[currentResolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, true);
        validationBouton.gameObject.SetActive(true);
    }

    // Méthode appelée pour mettre à jour les videoSettings
    public void SetValues()
    {
        Video videoSettings = SettingsJSON.instance.settings.videoSettings;
        vfxToggled.isOn = videoSettings.isVFXToggled;
        if(dayNightToggled != null)
            dayNightToggled.isOn = videoSettings.isDayToggled;
    }

    // Méthode appelée une fois que le toggle VFX a été cliqué
    public void SetVFX(){
        AudioManager.instance.Play("ClickUI");
        validationBouton.gameObject.SetActive(true);
    }

    // Méthode appelée une fois que le toggle DayNight a été cliqué
    public void SetDayNight(){
        AudioManager.instance.Play("ClickUI");
        validationBouton.gameObject.SetActive(true);
    }

    // Méthode appelée lorsque le bouton validationBouton est pressé
    public void SaveVideo()
    {
        // On met à jour les valeurs 
        AudioManager.instance.Play("ClickUI");
        SettingsJSON.instance.settings.videoSettings.isVFXToggled = vfxToggled.isOn;

        SettingsJSON.instance.settings.videoSettings.resolutionWidth = filteredResolutions[currentResolutionIndex].width;
        SettingsJSON.instance.settings.videoSettings.resolutionHeight = filteredResolutions[currentResolutionIndex].height;
        SettingsJSON.instance.settings.videoSettings.resolutionRefreshRate = filteredResolutions[currentResolutionIndex].refreshRate;
        Resolution resolution = filteredResolutions[currentResolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, true);
        Debug.Log(Screen.currentResolution.ToString());
        if(dayNightToggled != null)
            SettingsJSON.instance.settings.videoSettings.isDayToggled = dayNightToggled.isOn;
        // Sauvegarder paramètres de son INGAME
        SettingsJSON.instance.UpdateSettingsFile();
        validationBouton.gameObject.SetActive(false);
    }


}


