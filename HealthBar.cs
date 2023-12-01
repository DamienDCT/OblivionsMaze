using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class HealthBar : MonoBehaviour
{
    // Référence à la barre de vie
    [SerializeField]
    private Slider slider;
    // Référence au texte indiquant le nombre de vies restant
    [SerializeField]
    private TextMeshProUGUI textNbLives;
    // Image de fond de la barre de vie
    [SerializeField]
    private Image rectFill;
    // Différentes couleurs pour la barre de vie et pour les debuff
    [Header("Couleurs healthbar")]
    [SerializeField]
    private Color more50HPColor;
    [SerializeField]
    private Color between20and50HPColor;
    [SerializeField]
    private Color lessThan20HPColor;
    [SerializeField]
    private Color colorPoisoned;
    [SerializeField]
    private Color colorBurnt;

    // Différents sprites pour les debuff
    [Header("Debuff Images/Sprites")]
    [SerializeField]
    private Sprite transparentSprite;
    [SerializeField]
    private Sprite poisonDebuffSprite;
    [SerializeField]
    private Sprite burntDebuffSprite;
    [SerializeField]
    private Image debuffImage;

    private void OnEnable(){
        // Premier lancement du jeu, les instances ne sont pas créées.
        if(PlayerHealth.instance != null)
            textNbLives.text = "x"+PlayerHealth.instance.nbLives;
    }

    private void Start(){
        // On met le debuff en tant que sprite transparent
        debuffImage.sprite = transparentSprite;
    }

    private void Update()
    {
        // On met à jour le nombre de vies et la couleur de la barre de vie s'il n'y a pas de debuff en cours sur le joueur
        if(PlayerHealth.instance != null){

            textNbLives.text = "x" + PlayerHealth.instance.nbLives;
            if(!PlayerHealth.instance.isPoisoned && !PlayerHealth.instance.isBurnt){
                if(PlayerHealth.instance.currentHealth > 66){
                    rectFill.color = more50HPColor;
                } else if(PlayerHealth.instance.currentHealth > 33 && PlayerHealth.instance.currentHealth < 66){
                    rectFill.color = between20and50HPColor;
                } else {
                    rectFill.color = lessThan20HPColor;
                }
            }
        }
    }

    // Méthode servant à empoisonner le joueur de manière visuelle
    public void SetPoison(bool value){
        if(value){
            rectFill.color = colorPoisoned;
            debuffImage.sprite = poisonDebuffSprite;
        } else {
            debuffImage.sprite = transparentSprite;
        }
    }

    // Méthode servant à brûler le joueur de manière visuelle
    public void SetBurn(bool value){
        if(value){
            rectFill.color = colorBurnt;
            debuffImage.sprite = burntDebuffSprite;
        } else {
            debuffImage.sprite = transparentSprite;
        }
    }

    // Méthode servant à reset la vie du joueur de manière visuelle
    public void SetMaxHealth(int health)
    {
        slider.maxValue = health;
        slider.value = health;
    }

    // Méthode servant à modifier la valeur visuelle de la barre de vie en fonction d'un nombre de points de vie
    public void SetHealth(int health)
    {
        if(health >= 0)
        {
            slider.value = health;
        }
    }
}
