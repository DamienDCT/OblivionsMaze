using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndLevelDoor : MonoBehaviour
{
    // Référence aux Renderer de chaque clé 
    [SerializeField]
    private SpriteRenderer[] keys;

    // Référence au sprite du verrou avec une clé inséré
    [SerializeField]
    private Sprite spriteKeyLocked;

    // Référence au Transform qui servira à ouvrir la porte
    [SerializeField]
    private Transform targetWhenOpened;
    // référence à la vitesse d'ouverture de la porte
    [SerializeField]
    private float speed;

    // Nombre de clé nécessaire
    private int nbKeyNeeded;
    // Nombre de clé utilisée
    private int nbKeyUsed;
    // Booléen pour savoir si la porte est ouverte
    private bool isDoorOpen;
    // Booléen pour savoir si le joueur est dans la zone d'interaction de la porte
    private bool playerTarget;
    // Référence à une interaction
    private Interaction interaction;

    private void Start()
    {
        // On initialise les variables
        interaction = GetComponent<Interaction>();
        nbKeyNeeded = keys.Length;
    }

    private void Update()
    {
        // Si la porte est ouverte
        if(isDoorOpen){
            // Calcul de la distance jusqu'au prochain waypoint
            Vector3 dir = targetWhenOpened.position - transform.position;
            // On déplace jusqu'au prochain waypoint
            transform.Translate(dir.normalized * speed * Time.deltaTime, Space.World);
            // Si la porte est arrivée à son point de destination, on détruit la porte
            if (Vector3.Distance(transform.position, targetWhenOpened.position) < 0.3f)
            {
                Destroy(gameObject);
            }
        // Si elle n'est pas ouverte et qu'on appuie sur E
        } else if(Input.GetKeyDown(KeyCode.E)){
            // Si le joueur est dans la zone
            if(playerTarget && !isDoorOpen){
                // Si le joueur possède au moins une clé
                if(PlayerPowerup.instance.GetNbKeys() > 0){
                    // On ajoute les clés du joueur à la porte
                    UseKeys(PlayerPowerup.instance.GetNbKeys());
                }
            }
        }
    }

    // Méthode servant à utiliser des clés sur la porte
    private void UseKeys(int nbKeys)
    {
        // On met à jour les variables de la porte
        nbKeyUsed += nbKeys;
        // On retire le nombre de clés utilisés aux clés du joueur
        PlayerPowerup.instance.SetNbKeys(PlayerPowerup.instance.GetNbKeys() - nbKeys);
        // On met à jour le visuel de la porte
        RefreshGraphics();
    }

    // Méthode servant à mettre à jour le visuel de la porte
    private void RefreshGraphics(){
        // On change le sprite des nbKeyUsed premiers verrous
        for(int i = 0; i < nbKeyUsed; i++){
            keys[i].sprite = spriteKeyLocked;
        }
        // Si le joueur a inséré toutes les clés, on ouvre la porte
        if(nbKeyNeeded == nbKeyUsed){
            isDoorOpen = true;
        }
    }

    // Si le joueur rentre dans la zone, on met à jour les variables
    public void OnTriggerEnter2D(Collider2D collider2D){
        if(collider2D.CompareTag("Player")){
            interaction.PutText();
            playerTarget = true;
        }
    }    
    
    // Si le joueur sort de la zone, on met à jour les variables
    public void OnTriggerExit2D(Collider2D collider2D){
        if(collider2D.CompareTag("Player")){
            interaction.EraseText();
            playerTarget = false;
        }
    }  
}
