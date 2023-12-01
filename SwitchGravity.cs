using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchGravity : MonoBehaviour
{
    //booléen qui indique si le changeur de gravité est vers le haut ou non
    [SerializeField]
    private bool isReverseSwapper;

    //quand le joueur entre en collision avec le changeur de gravité, on appelle la méthode pour l'inverser
    private void OnTriggerEnter2D(Collider2D collider2D){
        if(collider2D.CompareTag("Player")){
            PlayerMovement.instance.SwapGravity(isReverseSwapper);
        }
    }
}
