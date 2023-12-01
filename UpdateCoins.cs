using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateCoins : MonoBehaviour
{
    //référence a l'affichage du nombre de pièces que possède le joueur
    [SerializeField]
    private Text pieceText;

    //on change le texte en fonction du nombre de pièce que possède le joueur
    void Start()
    {
        pieceText.text = ""+PlayerPowerup.instance.GetNbCoins();
    }
}
