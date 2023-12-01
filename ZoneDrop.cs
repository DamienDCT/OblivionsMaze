    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneDrop : MonoBehaviour
{

    //booléen qui indique si le player est dans la zone pour lui faire tomber son powerup
    [SerializeField]
    private bool isPlayerOnZone;

    //on vérifie de manière constante si le joueur est dans la zone, et s'il possède un powerup, on lui retire
    void FixedUpdate(){
        if(isPlayerOnZone){
            if(PlayerPowerup.instance.currentItem != null){
                PlayerPowerup.instance.SwapPowerUps(null, true);
            }
        }
    }

    //Si le joueur entre dans la zone, on l'indique avec le booléen isPlayerOnZone à true
    public void OnTriggerEnter2D(Collider2D collider2D){
        if(collider2D.CompareTag("Player"))
            isPlayerOnZone = true;
    }

    //Si le joueur sort de la zone, on l'indique avec le booléen isPlayerOnZone à false
    public void OnTriggerExit2D(Collider2D collider2D){
        if(collider2D.CompareTag("Player"))
            isPlayerOnZone = false;
    }
}
