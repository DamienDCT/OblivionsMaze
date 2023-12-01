using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cible : MonoBehaviour
{
    // Référence au joueur
    private GameObject player;

    // Référence au canon
    [SerializeField]
    private Canon canon;

    private void Start(){
        // On initialise la variable
        player = PlayerMovement.instance.gameObject;
    }

    // Si le joueur touche la cible
    public void OnTriggerEnter2D(Collider2D collider2D){
        if(collider2D.CompareTag("Player")){
            player.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
            // le canon lié à la cible arrête de tirer 
            canon.StopShooting();
        }
    }
}
