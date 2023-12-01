using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateformeLevier : MonoBehaviour
{
    // Tableau de transform pour les waypoints de la plateforme
    [SerializeField]
    private Transform[] waypoints;
    // Tableau de sprites pour la plateforme
    [SerializeField]
    private Sprite[] sprites;
    // Sprite à mettre lorsque la plateforme ne bouge plus
    [SerializeField]
    private Sprite neutralPlatformSprite;
    // Rendu de la plateforme
    private SpriteRenderer spriteRenderer;
    // Index dans le tableau sprites
    private int indexSprite;
    // Transform actuel (correspond à waypoints[indexSprite])
    private Transform currentWaypoint;
    // Index dans le tableau waypoints
    private int indexWaypoint;
    // Vitesse de la plateforme
    [SerializeField]
    private float speed;
    // Source audio de la plateforme
    [SerializeField]
    private AudioSource audioSource;
    // Rendu de l'objet
    private Renderer rendu;
    // Booléen indiquant si la plateforme se déplace
    private bool isMoving;

    void Start()
    {
        // Initialisation des variables
        spriteRenderer = GetComponent<SpriteRenderer>();
        rendu = GetComponent<Renderer>();
        indexWaypoint = 0;
        indexSprite = 0;
        currentWaypoint = waypoints[0];
        isMoving = false;
    }

    private void FixedUpdate(){
        // Si la plateforme est visible par la caméra
        if(rendu.isVisible){
            // Si la plateforme se déplace
            if(isMoving){
                // et que l'audio n'est pas déjà lancé, alors on le lance
                if(!audioSource.isPlaying)
                    audioSource.Play();
            // Sinon on arrête le son
            } else {
                audioSource.Stop();
            }
        // Sinon on arrête le son
        } else {
            audioSource.Stop();
        }
    }


    private void Update(){
        if(!isMoving)
            return;
        // Si la plateforme a atteint son waypoint de destination
        if(Vector3.Distance(transform.position, currentWaypoint.position) < .05f){
            // On remet son sprite de départ et on indique qu'elle ne bouge plus
            spriteRenderer.sprite = neutralPlatformSprite;
            isMoving = false;
            return;
        }

        // Sinon, on met le sprite en fonction du waypoint vers lequel la platforme se déplace
        isMoving = true;
        spriteRenderer.sprite = sprites[indexSprite];
        Vector3 distance = currentWaypoint.position - transform.position;
        transform.Translate(distance.normalized * speed * Time.deltaTime, Space.World);
    }

    // Si une boule ou le joueur entre en collision avec la plateforme levier, on met l'objet en enfant de la plateforme
    public void OnCollisionEnter2D(Collision2D col)
    {
        if(col.collider.CompareTag("Player") || col.collider.CompareTag("Boule")){
            col.gameObject.transform.SetParent(gameObject.transform,true);
            gameObject.tag = "Platform";
        }
    }

    // Si une boule ou le joueur descend de la plateforme, on reset son parent
    public void OnCollisionExit2D(Collision2D col)
    {
        if(col.collider.CompareTag("Player")){
            col.gameObject.transform.parent = GameObject.FindGameObjectWithTag("ParentPlayer").transform;
            gameObject.tag = "Platform";
        } else if(col.collider.CompareTag("Boule"))
        {
            col.gameObject.transform.SetParent(null);
            gameObject.tag = "Platform";
        }
    }

    // Méthode de mouvement
    public void Move(){
        indexSprite = (indexSprite + 1) % sprites.Length;
        indexWaypoint = (indexWaypoint + 1) % waypoints.Length;
        currentWaypoint = waypoints[indexWaypoint];
        isMoving = true;
    }
}
