using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bouncer : MonoBehaviour
{
    // Référence à l'animator du rebondisseur
    [SerializeField]
    private Animator animationA; 
    // Référence à la force qu'a le rebondisseur
    [SerializeField]
    private float forceMode; 

    // Méthode qui est appellée si un objet rentre en collision avec le rebondisseur
    private void OnTriggerEnter2D(Collider2D collider)
    {
        // Si l'objet en question est le joueur
        if (collider.CompareTag("Player"))
        {
            // On joue le son du trampoline
            AudioManager.instance.Play("Trampoline");
            // On active une fois l'animation du bouncer
            animationA.SetTrigger("Bounce");
            // On modifie la vitesse du joueur en fonction de la force du bouncer
            PlayerMovement.instance.GetComponent<Rigidbody2D>().velocity = Vector2.up * forceMode;
        }
    }
}
