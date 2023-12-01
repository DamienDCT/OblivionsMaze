using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Levier : MonoBehaviour
{
    // Booléen pour savoir si le joueur est dans la zone
    private bool isPlayerOnZone;
    // Sprites pour le levier
    [SerializeField]
    private Sprite spriteOn;
    [SerializeField]
    private Sprite spriteOff;
    // Rendu du levier
    private SpriteRenderer spriteRenderer;
    // Référence aux plateformes à bouger avec le levier
    [SerializeField]
    private GameObject[] plateformeToMove;
    [SerializeField]
    private GameObject[] plateformeDelayedToMove;
    // Référence à la caméra Cinemachine
    private CinemachineVirtualCamera cinemachineVirtualCamera;
    // Référence à l'interaction pour le texte en bas de l'écran
    private Interaction interaction;
    // Booléen pour connaître l'état du levier
    private bool isTurnedOn;

    private void Start(){
        // On initialise les variables
        isTurnedOn = false;
        spriteRenderer = GetComponent<SpriteRenderer>();
        cinemachineVirtualCamera = GameObject.FindGameObjectWithTag("VirtualCamera").GetComponent<CinemachineVirtualCamera>();
        interaction = GetComponent<Interaction>();
        isPlayerOnZone = false;
    }

    private void Update(){
        // Si le joueur n'est pas dans la zone, on ne fait rien 
        if(!isPlayerOnZone)
            return;
        // Sinon si il appuie sur E, on fait le son du levier et on bouge les plateformes
        if(Input.GetKeyDown(KeyCode.E)){
            AudioManager.instance.Play("Levier");
            StartCoroutine(MovePlateformes());
        }
    }

    // A chaque fois que le joueur rentre en contact avec la zone d'activation du levier
    public void OnTriggerEnter2D(Collider2D collider2D){
        if(collider2D.CompareTag("Player"))
        {
            // On met le texte et on met à jour la variable
            interaction.PutText();
            isPlayerOnZone = true;
        }
    }

    // A chaque fois que le joueur sort de la zone d'activation du levier
    public void OnTriggerExit2D(Collider2D collider2D){
        if(collider2D.CompareTag("Player"))
        {
            // On supprime le texte et on met à jour la variable
            interaction.EraseText();
            isPlayerOnZone = false;
        }
    }

    // Coroutine pour bouger les plateformes
    private IEnumerator MovePlateformes()
    {
        // On change l'état du levier et on met à jour son sprite
        isTurnedOn = !isTurnedOn;
        SwitchSprites();
        // On supprime le texte
        interaction.EraseText();
        // On immobilise le joueur
        PlayerMovement.instance.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
        yield return new WaitForSecondsRealtime(.5f);
        PlayerMovement.instance.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePosition | RigidbodyConstraints2D.FreezeRotation;
        // Pour chaque plateforme à déplacer, on focus la caméra dessus
        foreach (GameObject plateforme in plateformeToMove)
        {
            cinemachineVirtualCamera.Follow = plateforme.transform;
            cinemachineVirtualCamera.LookAt = plateforme.transform;
            yield return new WaitForSecondsRealtime(1.5f);
            plateforme.GetComponent<PlateformeLevier>().Move();
            yield return new WaitForSecondsRealtime(3f);
        }
        // On remet le joueur avec les contraintes qu'il avait au début
        PlayerMovement.instance.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
        yield return new WaitForSecondsRealtime(.5f);
        // On remet le texte d'interaction et on refocus la caméra sur le joueur
        interaction.PutText();
        cinemachineVirtualCamera.Follow = PlayerMovement.instance.gameObject.transform;
        cinemachineVirtualCamera.LookAt = PlayerMovement.instance.gameObject.transform;
        yield return new WaitForSecondsRealtime(3f);
        foreach(GameObject plateforme in plateformeDelayedToMove){
            plateforme.GetComponent<PlateformeLevier>().Move();
        }
    }

    private void SwitchSprites(){
        if(isTurnedOn){
            spriteRenderer.sprite = spriteOn;  
        } else {
            spriteRenderer.sprite = spriteOff;
        }
    }    
}
