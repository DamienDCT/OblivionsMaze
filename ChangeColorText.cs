using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
public class ChangeColorText : MonoBehaviour
{
    //référence au texte auquel on veut modifier la couleur
    private TextMeshProUGUI textMeshProUGUI;

    //Couleur lorsqu'on a la souris qui n'est pas sur le texte (couleur par défaut donc)
    [SerializeField]
    private Color colorUnhandled;
    //Couleur lorsque la souris est sur le texte (hover)
    [SerializeField]
    private Color colorHandled;

    //on initialise la variable avec le texte
    private void Start(){
        textMeshProUGUI = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
    }

    //quand la souris pointe sur le texte on lui modifie la couleur en conséquence
    public void OnPointerEnter(BaseEventData eventData)
    {
        // Change la couleur du texte du bouton lorsque la souris entre dans le bouton
        textMeshProUGUI.color = colorHandled;
    }

    //quand la souris ne pointe plus sur le texte on lui modifie la couleur en conséquence
    public void OnPointerExit(BaseEventData eventData)
    {
        // Change la couleur du texte du bouton lorsque la souris quitte le bouton
        textMeshProUGUI.color = colorUnhandled;
    }

    //on met la couleur à sa couleur par défaut
    public void ResetColor(){
        textMeshProUGUI.color = colorUnhandled;
    }
    
}