using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;
using Image = UnityEngine.UI.Image;

public class SettingsMenu : MonoBehaviour
{

    //boutons pour l'audio, les controles, et les options vidéos
    public Button audioSettingsButton;
    public Button controlsSettingsButton;
    public Button videoSettingsButton;

    //GameObject correspondants aux différents panels
    public GameObject audioPanel;
    public GameObject controlsPanel;
    public GameObject videoPanel;

    //Couleur quand le panel n'est pas celui sur lequel on a cliqué
    [SerializeField]
    private Color nonClickedColor;

    //couleur quand le panel est celui sur lequel on a cliqué
    [SerializeField]
    private Color clickedColor;

    //Par défaut on est sur le panel des options audio
    void Start()
    {
        audioSettingsButton.GetComponent<Image>().color = clickedColor;
        videoSettingsButton.GetComponent<Image>().color = nonClickedColor;
        controlsSettingsButton.GetComponent<Image>().color = nonClickedColor;

        audioPanel.SetActive(true);
        videoPanel.SetActive(false);
        controlsPanel.SetActive(false);
    }


    //si on clique sur le panel audio on le met actif et de la couleur cliquée (et on désactive le reste)
    public void ClickAudioButton()
    {
        AudioManager.instance.Play("ClickUI");
        this.gameObject.SetActive(true);
        audioPanel.SetActive(true);
        videoPanel.SetActive(false);
        controlsPanel.SetActive(false);

        audioSettingsButton.GetComponent<Image>().color = clickedColor;
        videoSettingsButton.GetComponent<Image>().color = nonClickedColor;
        controlsSettingsButton.GetComponent<Image>().color = nonClickedColor;
    }

    //si on clique sur le panel controles on le met actif et de la couleur cliquée (et on désactive le reste)
    public void ClickControlsButton()
    {
        AudioManager.instance.Play("ClickUI");
        Time.timeScale = 0f;
        this.gameObject.SetActive(true);
        audioPanel.SetActive(false);
        videoPanel.SetActive(false);
        controlsPanel.SetActive(true);

        audioSettingsButton.GetComponent<Image>().color = nonClickedColor;
        videoSettingsButton.GetComponent<Image>().color = nonClickedColor;
        controlsSettingsButton.GetComponent<Image>().color = clickedColor;
    }

    //si on clique sur le panel vidéo on le met actif et de la couleur cliquée (et on désactive le reste)
    public void ClickVideoButton()
    {
        AudioManager.instance.Play("ClickUI");
        this.gameObject.SetActive(true);
        audioPanel.SetActive(false);
        videoPanel.SetActive(true);
        controlsPanel.SetActive(false);

        audioSettingsButton.GetComponent<Image>().color = nonClickedColor;
        videoSettingsButton.GetComponent<Image>().color = clickedColor;
        controlsSettingsButton.GetComponent<Image>().color = nonClickedColor;
    }

}
