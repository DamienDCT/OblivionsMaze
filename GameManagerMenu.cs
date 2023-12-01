using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerMenu : MonoBehaviour
{
    // Singleton
    public static GameManagerMenu instance;

    // Référence au panel de paramètres
    [SerializeField]
    private GameObject settingsPanel;
    // Référence au panel du menu principal
    [SerializeField]
    private GameObject mainMenuPanel;
    // Référence au bouton de sauvegarde de settings
    [SerializeField]
    private GameObject saveSettings;
    [SerializeField]
    private GameObject winPanel;

    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        saveSettings.SetActive(false);
        GameObject menuUI = GameObject.Find("GameData");
        if(menuUI)
        {
            menuUI.SetActive(false);
        }
        mainMenuPanel.SetActive(true);
        settingsPanel.SetActive(false);
    }


    private void Update()
    {
        if(PlayerPowerup.instance.GetNbCoins() == 8)
        {
            winPanel.SetActive(true);
            return;
        } else {
            winPanel.SetActive(false);
        }
        if(Input.GetKeyDown(KeyCode.Escape) && settingsPanel.activeSelf)
        {
            GoBackMenu();
        }
    }

    public void ClickSettingsButton()
    {
        SettingsJSON.instance.LoadSettingsFile();
        AudioManager.instance.Play("ClickUI");
        saveSettings.SetActive(false);
        mainMenuPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void QuitApplication(){
        Application.Quit();
    }

    public void GoBackMenu()
    {
        Time.timeScale = 1f;
        mainMenuPanel.SetActive(true);
        settingsPanel.SetActive(false);
    }

    public void ChangeCreativeMode(bool value){
        AudioManager.instance.Play("ClickUI");
        CreativeMode.instance.isCreativeActivated = value;
    }

    public void ResetGame(){
        PlayerHealth.instance.nbLives = PlayerHealth.instance.maxLives;
        PlayerPowerup.instance.SetNbCoins(0);
        SaveGameData.instance.UpdatePlayerDataFile();
    }
}
