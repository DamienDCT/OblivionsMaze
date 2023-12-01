using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class InterrupteurEnnemi : MonoBehaviour
{
    // Référence au BoxCollider2D de l'interrupteur
    [SerializeField]
    private BoxCollider2D boxCollider2D;
    // Référence aux sprites de l'interrupteur ON et OFF
    [SerializeField]
    private Sprite spriteClose;
    [SerializeField]
    private Sprite spriteOpen;
    // Référence au rendu de l'interrupteur
    [SerializeField]
    private SpriteRenderer spriteRenderer;
    // Référence à tous les ennemis de l'interrupteur
    [SerializeField]
    private GameObject[] mobsToActive;
    // Booléen pour indiquer si le bouton a déjà été appuyé
    private bool alreadyActivated;


    private void Start(){
        // On initialise les variables
        alreadyActivated = false;
    }

    public void OnCollisionEnter2D(Collision2D collision2D)
    {
        // Si le joueur rentre en contact avec l'interrupteur
        if (collision2D.collider.CompareTag("Player"))
        {
            // On regarde les points de contacts de la collision
            foreach (ContactPoint2D contact in collision2D.contacts)
            {
                if (contact.normal.y <= -.3f)
                {
                    // On fait les actions relatives à l'interrupteur
                    PushButton();
                    SpawnMob();
                    Invoke("PullButton", 1f);
                }
            }
        }
    }

    // Méthode servant à appuyer sur le bouton (effets visuels)
    private void PushButton()
    {
        alreadyActivated = true;
        AudioManager.instance.Play("Interrupteur");
        boxCollider2D.enabled = false;
        spriteRenderer.sprite = spriteClose;
    }

    // Méthode servant à remettre le bouton en place (effets visuels)
    private void PullButton()
    {
        boxCollider2D.enabled = true;
        spriteRenderer.sprite = spriteOpen;
    }

    // Méthode servant à faire apparaître les ennemis de l'interrupteure
    private void SpawnMob()
    {
        if(alreadyActivated)
            return;
        foreach(GameObject enemy in mobsToActive)
        {
            enemy.SetActive(true);
        }
    }

}
