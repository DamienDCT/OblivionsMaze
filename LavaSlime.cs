using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaSlime : Enemy
{
    // Booléen indiquant si le joueur est vu par le lavaslime
    [SerializeField]
    private bool isPlayerOnZone;
    // Vitesse de sortie de lave du lavaslime
    [SerializeField]
    private float maxSpeed;
    // Références aux positions qui sont respectivement la position une fois sortie de la lave, et la position dans la lave
    [SerializeField]
    private Transform waypointOut;
    [SerializeField]
    private Transform waypointBase;
    // Vecteur représentant la zone de détection
    [SerializeField]
    private Vector2 targetZone;
    // LayerMask du joueur
    [SerializeField]
    private LayerMask playerLayerMask;
    // Target actuelle du lavaslime
    private Transform currentTarget;
    // Booléen indiquant si le lavaslime est arrivé à destination
    private bool hasTouchTarget;
    // Vitesse actuelle du lavaslime
    private float speed;

    private void Start()
    {
        // Initialisation des variables
        currentTarget = waypointBase;
        hasTouchTarget = true;
        speed = 0f;
    }

    //méthode pour regarder si le joueur est à portée, et faire bouger le lavaSlime
    private void Update()
    {
        CheckPlayer();
        Move();
    }

    // Méthode appelée pour vérifier la présence du joueur
    private void CheckPlayer(){
        RaycastHit2D raycastHit2D = Physics2D.BoxCast(waypointOut.position, targetZone, 0, new Vector2(0f, 0f), 0, playerLayerMask);
        // Si le joueur est présent
        if(raycastHit2D.collider != null){
            // Si le joueur vient de rentrer dans la zone
            if(!isPlayerOnZone)
                // On fait bouger le lavaslime vers son waypoint de sortie
                hasTouchTarget = false;
            isPlayerOnZone = true;
            currentTarget = waypointOut;
            speed = maxSpeed;
        // Sinon on fait rentrer le lavaslime dans la lave
        } else {
            isPlayerOnZone = false;
            hasTouchTarget = false;
            currentTarget = waypointBase;
            speed = maxSpeed;
        }
    }

    //méthode pour faire bouger le lavaSlime
    public override void Move()
    {
        //s'il est déjà arrivé à destination, on ne fait rien
        if(hasTouchTarget)
            return;
        // Calcul de la distance jusqu'au prochain waypoint
        Vector3 dir = currentTarget.position - transform.position;
        // On déplace jusqu'au prochain waypoint
        transform.Translate(dir.normalized * speed * Time.deltaTime, Space.World);
        //s'il est arrivé à destination
        if (Vector3.Distance(transform.position, currentTarget.position) < 1f)
        {
            //on l'indique avec le booléen, et on ne le fait plus bouger
            hasTouchTarget = true;
            speed = 0f;
        }
    }

    public override void Destun(){ }

    public override void Geyser(){ }

    public override void Stun(){ }

    //Quand le lavaSlime entre en collision 
    public void OnTriggerEnter2D(Collider2D collider2D){
        //avec le joueur
        if(collider2D.CompareTag("Player")){
            //le joueur prend des dégats en conséquences
            PlayerHealth.instance.TakeDamage(damageAmount);
        //avec un bloc de glace
        } else if(collider2D.CompareTag("IceBloc")){
            AudioManager.instance.Play("IceBlocMelt");
            //on détruit le lavaslime
            Destroy(gameObject);
        }
    }

}
