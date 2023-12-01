using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DaylightUI : MonoBehaviour
{
    // Variables servant à stocker tous les sprites et animations pour le thème jour et le thème nuit
    // du menu principal
    [Header("Day Images")]
    [SerializeField]
    private Sprite daySettingsImage;
    [SerializeField]
    private Sprite dayTestAreaImage;
    [SerializeField]
    private Sprite daySelectLevelImage;
    [SerializeField]
    private Sprite dayExitGameImage;

    [Header("Night Images")]
    [SerializeField]
    private Sprite nightSettingsImage;
    [SerializeField]
    private Sprite nightTestAreaImage;
    [SerializeField]
    private Sprite nightSelectLevelImage;
    [SerializeField]
    private Sprite nightExitGameImage;

    [Header("Midnight Images")]
    [SerializeField]
    private Sprite midnightTestAreaImage;
    [SerializeField]
    private Sprite midnightSelectLevelImage;

    [Header("Boutons")]
    [SerializeField]
    private Button settingsButton;
    [SerializeField]
    private Button testAreaButton;
    [SerializeField]
    private Button selectLevelButton;
    [SerializeField]
    private Button exitGameButton;
    [SerializeField]
    private Animator animator;

    // A chaque fois que le menu principal est activé, on met à jour le thème
    private void OnEnable(){
        Invoke("SetAnimation", 0.05f);
    }

    private void SetAnimation(){
        if(SettingsJSON.instance != null)
        {
            if(SettingsJSON.instance.settings.videoSettings != null)
                // On change le thème en fonction de la variable isDayToggled dans les video settings
                if(SettingsJSON.instance.settings.videoSettings.isDayToggled)
                    SetDay();
                else
                    SetNight();
        }
    }

    // On change les sprites et l'animator pour la nuit
    public void SetNight(){
        settingsButton.GetComponent<Image>().sprite = nightSettingsImage;
        testAreaButton.GetComponent<Image>().sprite = midnightTestAreaImage;
        selectLevelButton.GetComponent<Image>().sprite = midnightSelectLevelImage;
        exitGameButton.GetComponent<Image>().sprite = nightExitGameImage;
        animator.SetTrigger("Nuit");
    }

    // On change les sprites et l'animator pour le jour
    public void SetDay(){
        settingsButton.GetComponent<Image>().sprite = daySettingsImage;
        testAreaButton.GetComponent<Image>().sprite = dayTestAreaImage;
        selectLevelButton.GetComponent<Image>().sprite = daySelectLevelImage;
        exitGameButton.GetComponent<Image>().sprite = dayExitGameImage;
        animator.SetTrigger("Jour");
    }
}
