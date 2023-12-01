using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AudioSettings : MonoBehaviour
{
    // Sliders pour modifier les sons dans les paramètres
    [SerializeField]
    private Slider generalSlider;
    [SerializeField]
    private Slider musicSlider;
    [SerializeField]
    private Slider effectSlider;

    // Textes pour afficher le son en pourcentage
    [SerializeField]
    private TextMeshProUGUI generalText;
    [SerializeField]
    private TextMeshProUGUI musicText;
    [SerializeField]
    private TextMeshProUGUI effectText;

    // Bouton pour valider les modifications
    [SerializeField]
    private Button validationBouton;

    // Au démarrage du jeu, on modifie les valeurs des sliders et on modifie les mixeurs en fonction du son choisi par le joueur
    private void Start(){
        SetValues();
        AudioManager.instance.UpdateMixerValues(generalSlider.value, musicSlider.value, effectSlider.value);
    }

    // Quand le menu d'audiosettings s'ouvre, on désactive le bouton de validation
    public void OnEnable(){
        validationBouton.gameObject.SetActive(false);
    }

    // Méthode pour mettre à jour les sliders et leurs valeurs en pourcentage
    // et pour mettre à jour les settings dans le singleton pour la sauvegarde
    public void SetValues()
    {
        SoundSettings soundSettings = SettingsJSON.instance.settings.sonSettings;
        generalSlider.value = soundSettings.generalVolume;  
        musicSlider.value = soundSettings.musicVolume;
        effectSlider.value = soundSettings.effectVolume;
        generalText.text = ((int)(generalSlider.value*100)).ToString() + "%";
        musicText.text = ((int)(musicSlider.value*100)).ToString() + "%";
        effectText.text = ((int)(effectSlider.value*100)).ToString() + "%";
    }

    // Méthode pour mettre à jour le texte du slider de son général
    public void editValueGeneral()
    {
        generalText.text = ((int)(generalSlider.value*100)).ToString() + "%";
        validationBouton.gameObject.SetActive(true);
    }

    // Méthode pour mettre à jour le texte du slider de son de musique
    public void editValueMusic()
    {
        musicText.text = ((int)(musicSlider.value*100)).ToString() + "%";
        validationBouton.gameObject.SetActive(true);
    }

    // Méthode pour mettre à jour le texte du slider de son d'effets spéciaux
    public void editValueEffect()
    {
        effectText.text = ((int)(effectSlider.value*100)).ToString() + "%";
        validationBouton.gameObject.SetActive(true);
    }

    // Méthode permettant de sauvegarder le son dans le fichier JSON de sauvegardes de paramètres
    public void SaveSound()
    {
        AudioManager.instance.Play("ClickUI");
        SettingsJSON.instance.settings.sonSettings.generalVolume = generalSlider.value;
        SettingsJSON.instance.settings.sonSettings.musicVolume = musicSlider.value;
        SettingsJSON.instance.settings.sonSettings.effectVolume = effectSlider.value;
        SettingsJSON.instance.UpdateSettingsFile();
        
        AudioManager.instance.UpdateMixerValues(generalSlider.value, musicSlider.value, effectSlider.value);
        // Sauvegarder param�tres de son INGAME
        validationBouton.gameObject.SetActive(false);
    }
}






