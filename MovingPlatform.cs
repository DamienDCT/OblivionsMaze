using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    // Tableau de Transform pour les waypoints de déplacement de la plateforme
    [SerializeField]
    private Transform[] waypoints;
    // Tableau de sprites de la plateforme
    [SerializeField]
    private Sprite[] sprites;
    // Vitesse de la plateforme
    [SerializeField]
    private float speed;
    // référence au waypoint cible de la plateforme
    private Transform target;
    // index du waypoint cible dans le tableau waypoints
    private int destPoint = 0;
    // Distance entre la plateforme et la cible avant que la plateforme décélère
    [SerializeField]
    private float distanceStartDecelerate;
    // Vitesse maximale de la plateforme
    [SerializeField]
    private float maxSpeed;
    // Source audio de la plateforme
    [SerializeField]
    private AudioSource audioSource;
    // Référence au rendu de la plateforme
    private Renderer rendu;


    private void Awake(){
        // Initialisation des variables
        rendu = GetComponent<Renderer>();
        target = waypoints[0];
        GetComponent<SpriteRenderer>().sprite = sprites[0];
        speed = maxSpeed;
    }

    private void FixedUpdate(){
        // Si la plateforme n'est pas visible par la caméra
        if(!rendu.isVisible)
            //  On stop le son
            audioSource.Stop();
        else {
            // Sinon, si le son n'est pas en train d'être lancé, on le lance
            if(!audioSource.isPlaying)
                audioSource.Play();
        }
    }

    void Update()
    {
        // On déplace la plateforme chaque frame
        MovePlatform();
    }

    private void MovePlatform()
    {
        // Calcul de la distance jusqu'au prochain waypoint
        Vector3 dir = target.position - transform.position;
        // On déplace jusqu'au prochain waypoint
        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

        // On calcule la distance entre la plateforme et son prochain waypoint
        float distance = Vector3.Distance(transform.position, target.position);
        // Si la plateforme doit décélerer, on baisse sa vitesse
        if(distance <= distanceStartDecelerate){
            speed = Mathf.Lerp(speed, 0f, Time.deltaTime);
        }
        // Si la distance est très proche de la plateforme
        if (distance < 0.3f)
        {
            // On change le prochain waypoint
            destPoint = (destPoint + 1) % waypoints.Length;
            target = waypoints[destPoint];
            // On met la vitesse à 0 pour la remettre 0.5s plus tard (temps d'arrêt)
            speed = 0;
            Invoke("ResetSpeed", .5f);
            GetComponent<SpriteRenderer>().sprite = sprites[destPoint];
        }
    }

    // Méthode pour remettre la vitesse à sa vitesse maximale
    private void ResetSpeed(){
        speed = maxSpeed;
    }

    // Si le joueur rentre en contact avec la plateforme, on le met en enfant de la plateforme pour qu'il se déplace avec la plateforme
    public void OnCollisionEnter2D(Collision2D col)
    {
        if(col.collider.CompareTag("Player")){
            col.gameObject.transform.SetParent(gameObject.transform,true);
            gameObject.tag = "Platform";
        }
    }

    // Si le joueur descend de la plateforme, on le remet en enfant de son objet d'origine
    public void OnCollisionExit2D(Collision2D col)
    {
        if(col.collider.CompareTag("Player")){
            col.gameObject.transform.parent = GameObject.FindGameObjectWithTag("ParentPlayer").transform;
            gameObject.tag = "Platform";
        }
    }
}
