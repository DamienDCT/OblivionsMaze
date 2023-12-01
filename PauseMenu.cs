using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    // Booléen indiquant si le jeu est en pause
    public static bool isGamePaused = false;
    // Panel pour les paramètres, menu de pause, gameover, et manuel
    [SerializeField]
    private GameObject settingsUI;
    public GameObject pauseUI;
    public GameObject gameoverUI;
    [SerializeField]
    private GameObject manuelUI;

    // Booléen indiquant si le menu des paramètres est ouvert
    private bool isSettingsOpened;
    // Tableau de boutons pour les boutons de validation d'options
    [SerializeField]
    private Button[] buttonsValidate;

    private void Awake(){
        // On initialise la variable
        isSettingsOpened = false;
    }
    // Méthode pour mettre à jour les GameObject pour rendre les panels invisibles 
    private void ResetUI(){
        pauseUI.SetActive(false);
        gameoverUI.SetActive(false);
        settingsUI.SetActive(false);
    }

    private void Update()
    {
        // Si le joueur appuie sur echap
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Si le jeu est en pause
            if (isGamePaused)
            {
                // Et que le menu de settings est déjà ouvert, on revient au menu de pause principal
                if(isSettingsOpened){
                    PauseGame();
                // Sinon on retourne en jeu
                } else {
                    ResumeGame();
                }
            // Si le jeu n'était pas en pause, on le pause
            } else {
                PauseGame();
            }
        }
    }

    // Méthode servant à mettre en pause le jeu
    public void PauseGame()
    {
        // On freeze le temps de jeu pour éviter de pouvoir se déplacer pendant le menu pause
        Time.timeScale = 0;
        // On active les panels nécessaires et on met à jour les variables
        isGamePaused = true;
        pauseUI.SetActive(true);
        settingsUI.SetActive(false);
        gameoverUI.SetActive(false);
        manuelUI.SetActive(false);
        isSettingsOpened = false;
    }

    // Méthode servant à désactiver tous les boutons du menu paramètres
    private void OffValidateButtons(){
        foreach(Button button in buttonsValidate){
            button.gameObject.SetActive(false);
        }
    }

    // méthode servant à ouvrir le menu du manuel
    public void OpenManuelMenu(){
        // On active les panels
        pauseUI.SetActive(false);
        gameoverUI.SetActive(false);
        settingsUI.SetActive(false);
        manuelUI.SetActive(true);
        AudioManager.instance.Play("ClickUI");
    }

    // Méthode pour revenir en jeu
    public void ResumeGame()
    {
        // On remet le temps qui s'écoule à 1
        Time.timeScale = 1;
        // On reset les variables et on désactive tous les panels
        isGamePaused = false;
        pauseUI.SetActive(false);
        settingsUI.SetActive(false);
        manuelUI.SetActive(false);
    }

    // Méthode pour ouvrir le menu des paramètres
    public void OpenSettingsMenu(){
        // On met à jour les variables
        isSettingsOpened = true;
        // On active et désactive les panels pour voir le menu paramètres à l'écran
        pauseUI.SetActive(false);
        gameoverUI.SetActive(false);
        manuelUI.SetActive(false);
        settingsUI.SetActive(true);
        AudioManager.instance.Play("ClickUI");
    }
    
    // Méthode pour revenir au menu principal
    public void GoToMainMenu()
    {
        AudioManager.instance.Play("ClickUI");
        // On remet le timeScale à 1
        Time.timeScale = 1f;
        // On retire le powerup du joueur
        PlayerPowerup.instance.ResetPowerup();
        PlayerMovement.instance.SwapGravity(true);
        // On désactive la healthbar
        GameObject go = GameObject.FindGameObjectWithTag("HealthBar");

        // Retirer le texte
        GameObject interactionText = GameObject.FindGameObjectWithTag("InteractionText");
        if(interactionText != null)
        {
            interactionText.GetComponent<Text>().text = "";
        }
        
        if(go != null) go.SetActive(false);
        // On désactive tous les panels du menu pause
        pauseUI.SetActive(false);
        gameoverUI.SetActive(false);
        settingsUI.SetActive(false);
        manuelUI.SetActive(false);

        // Si on est sur un niveau avec un manager de camera
        if(CameraManager.instance != null){
            // On reset les caméras pour remettre la camera cinemachine en tant que MainCamera
            CameraManager.instance.ResetCameras();
        }
        // On détruit le menu de pause instancié précédemment
        Destroy(GameObject.FindGameObjectWithTag("IngameUIPrefab"));
        // On charge les données du joueur
        SaveGameData.instance.LoadPlayerDataFile();
        // On remet le joueur sur son parent d'origine (au cas où le joueur ait quitté le jeu en étant sur une plateforme)
        GameObject.FindGameObjectWithTag("Player").transform.SetParent(GameObject.FindGameObjectWithTag("ParentPlayer").transform);
        // On charge la scène du menu principal
        LevelLoader.instance.LoadLevel("MainMenu", "", false, go);
    }

    // Méthode pour aller à la testarea
    public void GoToTestArea()
    {
        // On remet l'écoulement de temps à la normal
        Time.timeScale = 1f;
        // On retire le powerup du joueur
        PlayerPowerup.instance.ResetPowerup();
        // On désactive la barre de vie
        GameObject go = GameObject.FindGameObjectWithTag("HealthBar");
        if(go != null) go.SetActive(false);
        // On désactive les panels du menu pause
        pauseUI.SetActive(false);
        gameoverUI.SetActive(false);
        settingsUI.SetActive(false);
        manuelUI.SetActive(false);

        // Si on est sur un niveau avec un manager de camera
        if(CameraManager.instance != null){
            // On reset les caméras pour remettre la camera cinemachine en tant que MainCamera
            CameraManager.instance.ResetCameras();
        }

        // On détruit le menu de pause instancié précédemment
        Destroy(GameObject.FindGameObjectWithTag("IngameUIPrefab"));
        // On charge les données du joueur
        SettingsJSON.instance.LoadSettingsFile();
        // On remet le joueur sur son parent d'origine (au cas où le joueur ait quitté le jeu en étant sur une plateforme)
        GameObject.FindGameObjectWithTag("Player").transform.SetParent(GameObject.FindGameObjectWithTag("ParentPlayer").transform);
        // On charge la scène de la testarea
        LevelLoader.instance.LoadLevel("TestArea", "", true, go);
    }
}
