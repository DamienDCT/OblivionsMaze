using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CounterItem : MonoBehaviour
{
    // Référence au BoxCollider2D du gameObject
    private BoxCollider2D boxCollider2D;

    private void Start(){
        // On récupère la référence du BoxCollider2D
        boxCollider2D = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        // Si le joueur n'a pas de powerup en sa possession
        if(PlayerPowerup.instance.currentItem == null)
            // On empêche le joueur de passer
            boxCollider2D.isTrigger = false;
        else
            // Sinon on met la BoxCollider2D en trigger pour que le joueur passe
            boxCollider2D.isTrigger = true;
    }
}
