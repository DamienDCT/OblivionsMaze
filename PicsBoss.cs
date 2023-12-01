using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PicsBoss : MonoBehaviour
{
    // Référence à la coroutine des pics qui sortent
    private Coroutine coroutine;

    // Position de base des pics
    private Vector2 positionBase;
    // Position quand les pics sont sortis
    private Vector2 positionSortie;
    // Valeur à ajouter au Y de la position des pics
    [SerializeField]
    private float exitHeight;
    // Dégâts au joueur
    [SerializeField]
    private int damageAmount;

    void Awake()
    {
        // On initialise les variables
        positionBase = transform.position;
        positionSortie = new Vector2(transform.position.x, transform.position.y + exitHeight);
    }

    void Start(){
        // On démarre la coroutine de sortie des pics
        coroutine = StartCoroutine(ExitSpikes());
    }

    public void StopSpikes(){
        // On stop la coroutine des pics et on les rétractes
        StopCoroutine(coroutine);
        coroutine = null;
        RetractSpikes();
    }

    public void RestartSpikes(){
        // On redémarre la coroutine des pics
        coroutine = StartCoroutine(ExitSpikes());
    }

    private IEnumerator ExitSpikes(){
        while(true){
            // Toutes les 5 secondes, on change l'état des pics
            yield return new WaitForSecondsRealtime(5f);
            ExtractSpikes();
            yield return new WaitForSecondsRealtime(5f);
            RetractSpikes();
        }
    }

    // Méthode pour rétracter les pics
    public void RetractSpikes(){
        transform.position = positionBase;
    }
    
    // Méthode pour sortir les pics
    private void ExtractSpikes(){
        transform.position = positionSortie;
    }

    // Si le joueur entre en contact avec les pics, il prend des dégâts
    public void OnCollisionEnter2D(Collision2D collision2D){
        if(collision2D.collider.CompareTag("Player")){
            PlayerHealth.instance.TakeDamage(damageAmount);
        }
    }
}
