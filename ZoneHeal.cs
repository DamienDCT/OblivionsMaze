    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneHeal : MonoBehaviour
{

    //booléen qui indique si le joueur est dans la zone de régénération
    [SerializeField]
    private bool isPlayerOnZone;

    //entier pour indiquer le nombre de points de vie maximum que le joueur peut avoir avant de voir sa vie régénérer
    [SerializeField]
    private int maxHPBeforeHeal;

    //Si le joueur est dans la zone et que sa vie est en dessous de la limite donnée, on lui redonne toute sa vie
    void FixedUpdate(){
        if(isPlayerOnZone){
            if(PlayerHealth.instance.currentHealth < maxHPBeforeHeal){
                PlayerHealth.instance.ResetHealth();
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
