using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Interaction : MonoBehaviour
{
    // Référence à un string pour le texte d'interaction
    [SerializeField]
    private string interactionHint;

    // Méthode servant à mettre à jour le texte sur le canvas
    public void PutText(){
        GameObject text = GameObject.FindGameObjectWithTag("InteractionText");
        if(text != null)
            text.GetComponent<Text>().text = interactionHint;
    }

    // Méthode servant à supprimer le texte sur le canvas
    public void EraseText(){
        GameObject text = GameObject.FindGameObjectWithTag("InteractionText");
        if(text != null)
            text.GetComponent<Text>().text = "";
    }
}
