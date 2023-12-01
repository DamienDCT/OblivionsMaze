using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boule : MonoBehaviour
{
    [SerializeField]
    private int maxCasse;

    private int casse;
    // Référence au Renderer de la boule
    private Renderer rendu;
    // Référence à sa précédente position
    private Vector2 previousPosition;
    // Start is called before the first frame update
    [SerializeField]
    private Transform waypointStart;

    void Start()
    {
        // On setup les variables
        UpdatePreviousPosition();
        rendu = GetComponent<Renderer>();
        casse = maxCasse;
    }

    // méthode pour obtenir les positions de la dernière frame de la boule
    private void UpdatePreviousPosition(){
        previousPosition.x = transform.position.x;
        previousPosition.y = transform.position.y;
    }

    // Utilisation de fixedUpdate pour éviter d'avoir trop d'appels
    void FixedUpdate()
    {
        // Si la boule n'est pas visible sur la caméra, on ne fait rien
        if(!rendu.isVisible){
            AudioManager.instance.Stop("BouleRoule");
            return;
        }
        // Si la position de la boule est différente de sa position à la frame d'avant, on joue le son
        if(previousPosition != (Vector2)transform.position)
        {
            AudioManager.instance.PlayLoop("BouleRoule");
        // Sinon on arrête le son
        } else {
            AudioManager.instance.Stop("BouleRoule");
        }
        // On sauvegarde sa position actuelle
        UpdatePreviousPosition();
    }

    // Si la boule touche un mur en brique
    public void OnCollisionEnter2D(Collision2D collision2D){
        if(collision2D.collider.CompareTag("PorteBoule")){
            // On détruit la boule si elle ne peut plus casser de mur
            casse--;
            if(casse == 0)
                Invoke("DestroyBoule", .2f);
        }
    }

    private void DestroyBoule(){
        AudioManager.instance.Stop("BouleRoule");
        Destroy(gameObject);
    }

    public void Tp(){
        transform.position = waypointStart.position;
    }
}
