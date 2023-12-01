using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class BoutonPoussoir : MonoBehaviour
{
    // Référence à la porte à ouvrir après que le bloc a touché le bouton à pression
    [SerializeField]
    private GameObject doorToOpen;
    // Référence à la caméra cinemachine pour la transition
    private CinemachineVirtualCamera cinemachineVirtualCamera;
    // Booléen indiquant si la porte a déjà été ouverte
    private bool hasOpened;
    // Référence au sprite du bouton à pression
    private SpriteRenderer spriteRenderer;
    // Référence au sprite quand le bouton est appuyé
    [SerializeField]
    private Sprite spriteIn;

    [SerializeField]
    private BoxCollider2D pulledBoxCollider;
    [SerializeField]
    private BoxCollider2D pressedBoxCollider;

    private void Start(){
        // On setup les variables
        spriteRenderer = GetComponent<SpriteRenderer>();
        cinemachineVirtualCamera = GameObject.FindGameObjectWithTag("VirtualCamera").GetComponent<CinemachineVirtualCamera>();
        hasOpened = false;
    }

    // Méthode appelée à chaque gameObject rentrant en contact avec le bouton à pression
    public void OnCollisionEnter2D(Collision2D collision2D){
        // Si la porte a déjà été ouverte, on ne fait rien
        if(hasOpened)
            return;
        // Si l'objet en question est un le cube interactif
        if(collision2D.collider.CompareTag("CubeInteractif")){
            // On met à jour la variable de porte ouverte
            hasOpened = true;
            // On démarre la transition de camera
            StartCoroutine(OpenDoor());
            // On modifie la position en y pour que le sprite soit bien affiché
            pressedBoxCollider.enabled = true;
            pulledBoxCollider.enabled = false;
        }
    }

    // Coroutine servant à ouvrir une porte en faisant une transition de caméra sur la porte qui est ouverte
    public IEnumerator OpenDoor(){
        spriteRenderer.sprite = spriteIn;
        // On freeze la position en X du joueur
        PlayerMovement.instance.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
        // Après un certain temps, on freeze également la position en Y (pour éviter qu'il soit bloquer dans les airs)
        yield return new WaitForSecondsRealtime(2f);
        PlayerMovement.instance.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePosition | RigidbodyConstraints2D.FreezeRotation;
        // On dit à la caméra de regarder la porte
        cinemachineVirtualCamera.Follow = doorToOpen.transform;
        cinemachineVirtualCamera.LookAt = doorToOpen.transform;
        yield return new WaitForSecondsRealtime(1.5f);
        // Après 1.5s (temps de la transition), on ouvre la porte
        doorToOpen.GetComponent<Door>().Switch();
        yield return new WaitForSecondsRealtime(3f);
        // Après que la porte ait été ouverte, on unfreeze le joueur
        PlayerMovement.instance.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
        yield return new WaitForSecondsRealtime(.5f);
        // Et on dit à la caméra de regarder le joueur
        cinemachineVirtualCamera.Follow = PlayerMovement.instance.gameObject.transform;
        cinemachineVirtualCamera.LookAt = PlayerMovement.instance.gameObject.transform;
    }
}
