using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piques : MonoBehaviour
{
    // Distance maximale de détection du joueur
    [SerializeField]
    private float range;
    // Booléen indiquant si le joueur est dans la zone des pics
    private bool isTrackingPlayer;
    // Référence à la coroutine de sorties des pics
    private Coroutine coroutine;
    // Position de base des pics
    private Vector2 positionBase;
    // Position quand les pics sont sortis
    private Vector2 positionSortie;
    // Dégâts au joueur
    [SerializeField]
    private int damageAmount;
    // Valeur à ajouter au Y de la position des pics
    [SerializeField]
    private float exitHeight;

    private void Awake()
    {
        // On initialise les variables
        isTrackingPlayer = false;
        coroutine = null;
        positionBase = transform.position;
        positionSortie = new Vector2(transform.position.x, transform.position.y + exitHeight);
    }

    private void Update()
    {
        // On regarde autour des pics si le joueur est présent
        Collider2D[] player = Physics2D.OverlapCircleAll(transform.position, range);
        foreach(Collider2D collider in player){
            if(collider.CompareTag("Player")){
                // Si le joueur vient de rentrer dans la zone des pics
                if(isTrackingPlayer == false){
                    isTrackingPlayer = true;
                    // On démarre la coroutine de sortie des pics
                    coroutine = StartCoroutine(ExitSpikes());
                }   
                return;
            }
        }
        // Sinon, on remet les pics à leur position initiale 
        isTrackingPlayer = false;
        transform.position = positionBase;
        // On stop la coroutine des pics
        if(coroutine != null){
            StopCoroutine(coroutine);
            coroutine = null;
        }

    }

    // Coroutine de sortie des pics
    private IEnumerator ExitSpikes(){
        // On sort les pics au bout de 0.5s, puis on les retracte au bout de 5s
        yield return new WaitForSecondsRealtime(.5f);
        ExtractSpikes();
        yield return new WaitForSecondsRealtime(5f);
        RetractSpikes();
    }

    // Si le joueur rentre en contact avec les pics, il prend des dégâts
    public void OnCollisionEnter2D(Collision2D collision2D){
        if(collision2D.collider.CompareTag("Player")){
            PlayerHealth.instance.TakeDamage(damageAmount);
            Invoke("RetractSpikes", 0.2f);
        }
    }

    // Méthode pour rétracter les pics
    private void RetractSpikes(){
        transform.position = positionBase;
    }
    // Méthode pour sortir les pics
    private void ExtractSpikes(){   
        transform.position = positionSortie;
    }
}
