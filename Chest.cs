using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{ 
    // Variable contenant l'item dans le coffre
    [SerializeField]
    private GameObject item; 
    // Variable pour indiquer si le coffre est ouvert
    private bool isOpened; 
    // Variable pour indiquer si le joueur est sur le coffre
    private bool onTarget; 
    // Animation de l'ouverture du coffre
    [SerializeField]
    private Animator animationOpenChest; 
    // Variable pour le texte interactif du coffre
    [SerializeField]
    private Interaction interaction;

    private void Awake()
    {
        // On initialise les variables
        onTarget = false;
        isOpened = false;
        interaction = GetComponent<Interaction>();
    }

    private void Update()
    {
        // Si le joueur est sur le coffre, et que le joueur appuie sur E, et que le coffre est fermé
        if (onTarget && Input.GetKeyDown(KeyCode.E) && !isOpened)
        {
            // On ouvre le coffre
            Open();
            // On actualise l'état du coffre
            isOpened = true;
        }
    }

    // Méthode appelée si un objet entre en collision avec le trigger du coffre
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Si l'objet est le joueur
        if (collision.CompareTag("Player"))
        {
            // On actualise l'état d'onTarget
            onTarget = true;
            // Si le coffre est fermé, on affiche le texte du coffre
            if(!isOpened)
                interaction.PutText();
        }
    }

    // Méthode appelée si un objet sort de la collision avec le trigger du coffre
    private void OnTriggerExit2D(Collider2D collision)
    {
        // Si l'objet est un joueur
        if (collision.CompareTag("Player"))
        {
            // On actualise l'�tat d'onTarget
            onTarget = false;
            // Si le joueur sort du coffre, on efface le texte
            interaction.EraseText();
        }
    }

    // Méthode pour ouvrir le coffre
    private void Open()
    {
        // On fait le son pour le coffre qui s'ouvre
        AudioManager.instance.Play("ChestOpen");
        // On lance l'animation d'ouverture du coffre
        animationOpenChest.SetTrigger("OpenChest");
        // On lance une coroutine pour faire apparaître l'item
        StartCoroutine(SpawnItem());
    }

    // Méthode pour faire apparaître l'item
    public IEnumerator SpawnItem()
    {
        yield return new WaitForSeconds(.75f);
        // On créé le vecteur d'apparition du gameObject
        Vector3 pos = new Vector3(transform.position.x, transform.position.y + .5f, transform.position.z);
        // On instantiate l'item en question en gardant sa référence
        GameObject go = Instantiate(item);
        // On définit la position de l'item sur la position calculée précédemment
        go.transform.position = pos;
    }
}
