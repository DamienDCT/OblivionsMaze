using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Interrupteur : MonoBehaviour
{
    // Référence à toutes les portes de l'interrupteur
    [SerializeField]
    private GameObject[] doors;
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
    // Booléen indiquant si l'interrupteur peut être à l'envers ou non
    [SerializeField]
    private bool isReverseButton;
    // Référence à la camera Cinemachine
    private CinemachineVirtualCamera cinemachineVirtualCamera;

    private void Start()
    {
        // Initialisation de la caméra cinemachine
        cinemachineVirtualCamera = GameObject.FindGameObjectWithTag("VirtualCamera").GetComponent<CinemachineVirtualCamera>();
    }

    public void OnCollisionEnter2D(Collision2D collision2D)
    {
        // Si le joueur rentre en contact avec l'interrupteur
        if (collision2D.collider.CompareTag("Player"))
        {
            // On regarde les points de contacts de la collision
            foreach (ContactPoint2D contact in collision2D.contacts)
            {
                // Si le joueur est à l'endroit
                if(!PlayerMovement.instance.gravityReversed){
                    // et que le bouton est aussi à l'endroit, on vérifie le contact
                    if(!isReverseButton)
                        if (contact.normal.y <= -.3f)
                        {
                            // On fait les actions nécessaires pour ouvrir la porte et sur l'interrupteur
                            PushButton();
                            StartCoroutine(SwitchDoors());
                            Invoke("PullButton", 1f);
                        }
                // Si le joueur est à l'envers
                } else {
                    // Et que le bouton est aussi à l'envers, on vérifie le contact
                    if(isReverseButton)
                        if (contact.normal.y >= .3f)
                        {
                            // On fait les actions nécessaires pour ouvrir la porte et sur l'interrupteur
                            PushButton();
                            StartCoroutine(SwitchDoors());
                            Invoke("PullButton", 1f);
                        }
                }
            }
        }
    }

    // Méthode servant à appuyer sur le bouton (effets visuels)
    private void PushButton()
    {
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

    // Méthode servant à gérer les portes liées à l'interrupteur
    private IEnumerator SwitchDoors()
    {
        // On freeze tout d'abord le joueur
        PlayerMovement.instance.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
        yield return new WaitForSecondsRealtime(.5f);
        PlayerMovement.instance.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePosition | RigidbodyConstraints2D.FreezeRotation;
        // Pour chaque porte, on fait en sorte que la caméra regarde cette porte, avec un délai pour voir la porte qui s'ouvre
        foreach (GameObject door in doors)
        {
            cinemachineVirtualCamera.Follow = door.transform;
            cinemachineVirtualCamera.LookAt = door.transform;
            yield return new WaitForSecondsRealtime(1.5f);
            door.GetComponent<Door>().Switch();
            yield return new WaitForSecondsRealtime(3f);
        }

        // Puis on remet le joueur avec les bonnes contraintes et on refocus la caméra sur lui
        PlayerMovement.instance.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
        yield return new WaitForSecondsRealtime(.5f);
        cinemachineVirtualCamera.Follow = PlayerMovement.instance.gameObject.transform;
        cinemachineVirtualCamera.LookAt = PlayerMovement.instance.gameObject.transform;
    }

}
