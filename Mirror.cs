using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mirror : MonoBehaviour
{
    // LayerMask du joueur
    [SerializeField]
    private LayerMask playerLayerMask;
    // Booléen pour savoir si le joueur est dans la zone d'interaction du miroir
    [SerializeField]
    private bool canPlayerInteract;
    // Tableau de rotation sur l'axe Z d'un miroir
    [SerializeField]
    private float[] positionInZAxis;
    // Index du tableau positionInZAxis
    [SerializeField]
    private int currentPosition;
    // Référence à l'emetteur de lumière de la cible
    [SerializeField]
    private LightEmitter lightEmitter;
    // Référence au texte interaction du miroir 
    private Interaction interaction;

    private void Start(){
        // On initialise les variables
        interaction = GetComponent<Interaction>();
        canPlayerInteract = false;
        UpdateMirror();
    }

    private void Update()
    {
        // Si l'émetteur n'a pas encore ouvert la porte
        if(!lightEmitter.HasOpenDoor()){
            // On regarde si le jouer est à côté du miroir
            CheckPresence();
            // Si le joueur appuie sur E et qu'il est à côté du miroir
            if(Input.GetKeyDown(KeyCode.E)){
                if(canPlayerInteract){
                    // On joue le son d'interaction
                    AudioManager.instance.Play("Interaction");
                    // On modifie l'index du tableau positionInZAxis
                    currentPosition = (currentPosition + 1) % positionInZAxis.Length;
                    UpdateMirror();
                }
            }
        }
    }

    // Méthode pour tourner le miroir
    private void UpdateMirror(){
        transform.rotation = Quaternion.Euler(0f, 0f, positionInZAxis[currentPosition]);
    }

    // Méthode pour savoir si le joueur est à côté du miroir
    private void CheckPresence(){
        // On calcul un raycast autour du miroir
        RaycastHit2D hit = Physics2D.BoxCast(transform.position, new Vector2(2.5f, 4f), 0f, new Vector2(0f, 0f), 0f, playerLayerMask);
        // Si le raycast a touché le joueur
        if(hit){
            // et que l'émetteur n'a pas touché la cible et que le joueur vient tout juste de rentrer dans la zone
            // d'interaction du miroir
            if(!lightEmitter.HasOpenDoor() && canPlayerInteract == false)
                // On affiche le texte
                interaction.PutText();
            // Et on met à jour la variable canPlayerInteract
            canPlayerInteract = true;
            return;
        }
        // A l'inverse, si le joueur vient tout juste de sortir de la zone d'interaction du miroir
        // On supprime le texte et on met à jour la variable canPlayerInteract
        if(canPlayerInteract){
            interaction.EraseText();
            canPlayerInteract = false;
        }
    }
}
