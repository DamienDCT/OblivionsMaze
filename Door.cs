using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    // Référence à la position de la porte quand elle est fermée
    [SerializeField]
    private Transform positionClosed;
    // Référence à la position de la porte quand elle est ouverte
    [SerializeField]
    private Transform positionOpened;
    // Booléen indiquant si la porte est ouverte ou fermée
    [SerializeField]
    private bool isOpened;
    // Booléen indiquant là où doit se situer la porte
    private Transform target;

    // Vitesse d'ouverture/fermeture de la porte
    private static float speed = 5f;

    void Start(){
        // On initialise les variables
        target = null;
    }

    private void Update(){
        if(target == null)
            return;

        // Calcul de la distance jusqu'au prochain waypoint
        Vector3 dir = target.position - transform.position;
        // On déplace jusqu'au prochain waypoint
        transform.Translate(dir.normalized * speed * Time.deltaTime, Space.World);

        // Si la porte est arrivée à son point de destination, on arrête de la déplacer
        if (Vector3.Distance(transform.position, target.position) < 0.05f)
        {
            target = null;
        }
    }

    // Pour changer d'état la porte, il faut inverser l'état de sa boîte de collision
    public void Switch(){
        GetComponent<BoxCollider2D>().enabled = !GetComponent<BoxCollider2D>().enabled;
        AudioManager.instance.Play("SlidingDoor");
        // On change la position cible de la porte
        if(isOpened){   
            target = positionClosed;
        } else {
            target = positionOpened;
        }
        // et on inverse le booléen pour mettre à jour l'état de la porte
        isOpened = !isOpened;
    }

    // Méthode pour ouvrir la porte uniquement
    public void OpenDoor(){
        GetComponent<BoxCollider2D>().enabled = false;
        target = positionOpened;
        isOpened = true;
    }
}
