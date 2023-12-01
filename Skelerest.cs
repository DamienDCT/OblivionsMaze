using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skelerest : Enemy
{
    //booléen qui indique si le joueur est dans la zone d'action de l'ennemi
    [SerializeField]
    private bool playerInZone;

    //Zone d'action de l'ennemi
    [SerializeField]
    private Vector2 zoneTrigger;

    //LayerMask pour ne pas toucher les autres entités
    [SerializeField]
    private LayerMask enemyLayerMask;

    //Layermask pour que le champ d'action ne détecte que le joueur
    [SerializeField]
    private LayerMask playerLayerMask;

    //vitesse actuelle de l'ennemi
    [SerializeField]
    private float speed;

    //vitesse maximum de l'ennemi
    [SerializeField]
    private float maxSpeed;

    //Référence à un GameObject qui correspond au dos de l'ennemi quand il regarde à droite
    [SerializeField]
    private GameObject dosSkelerestLeft;

    //Référence à un GameObject qui correspond au dos de l'ennemi quand il regarde à gauche
    [SerializeField]
    private GameObject dosSkelerestRight;

    //Coroutine pour que l'ennemi se retourne avec un pattern précis
    private Coroutine coroutine;

    //référence qui va servir à savoir ce que le joueur touche en premier lieu
    private RaycastHit2D whatPlayerHit;

    //booléen qui indique si le joueur regarde le dos de l'ennemi et donc l'appeure
    [SerializeField]
    private bool isAfraid;

    private float initY;
    //on initialise nos différentes variables
    private void Start(){
        isAfraid = false;
        playerInZone = false;
        GetComponent<SpriteRenderer>().color = Color.blue;
        dosSkelerestLeft.SetActive(false);
        dosSkelerestRight.SetActive(true);
        speed = maxSpeed;
        coroutine = null;
        initY = transform.position.y;
    }

    public void OnDrawGizmos(){
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, zoneTrigger);
    }

    //on regarde si le joueur est dans le champ d'action
    private void CheckPlayer(){
        RaycastHit2D collider2D = Physics2D.BoxCast(transform.position, zoneTrigger, 0f, new Vector2(0f, 0f), 0f, playerLayerMask);

        if(collider2D.collider != null)
        {
            //si c'est le cas on modifie le booléen en conséquence et l'ennemi bouge
            playerInZone = true;
            GetComponent<SpriteRenderer>().color = Color.red;
            speed = maxSpeed;
        } else {
            //sinon on modifie également le booléen en conséquence et l'ennemi ne bouge plus
            playerInZone = false;
            GetComponent<SpriteRenderer>().color = Color.blue;
            speed = 0f;
        }
    }

    //on vérifie de manière constante si le joueur regarde le dos de l'ennemi, et on fait le mouvement de l'ennemi
    private void Update(){
        CheckPlayer();
        bool hasTouchSomething = RaycastPlayer();
        if(!hasTouchSomething) {
            whatPlayerHit = Physics2D.Raycast(new Vector2(-1000, -1000), Vector2.zero, 0, enemyLayerMask);
        }
        Move();
    }
    
    //si le joueur regarde à gauche ou à droite et qu'il regarde le dos on retourne true, sinon false
    private bool RaycastPlayer(){
        if(PlayerMovement.instance.isWatchingLeft){
            Ray2D ray2D = new Ray2D(PlayerMovement.instance.gameObject.transform.position, Vector2.left);
            RaycastHit2D[] hit = Physics2D.RaycastAll(ray2D.origin, ray2D.direction, 6f, enemyLayerMask);
            if(hit.Length > 0){
                whatPlayerHit =  hit[0];
                return true;
            }
        }
        else {
            Ray2D ray2D = new Ray2D(PlayerMovement.instance.gameObject.transform.position, Vector2.right); 
            RaycastHit2D[] hit = Physics2D.RaycastAll(ray2D.origin, ray2D.direction, 6f, enemyLayerMask);
            if(hit.Length > 0){
                whatPlayerHit = hit[0];
                return true;
            }

        } 
        return false;
    }

    //méthode pour faire bouger l'ennemi
    public override void Move()
    {
        bool watchLeft = GetComponent<SpriteRenderer>().flipX;
        Vector3 playerPosition = PlayerMovement.instance.gameObject.transform.position;
        //si le joueur est dans la zone
        if(playerInZone){
            Vector3 direction = PlayerMovement.instance.gameObject.transform.position - transform.position;
            direction.y = initY;
            //si le joueur est à gauche de l'ennemi
            if(playerPosition.x < transform.position.x){
                //et qu'il regarde à gauche
                if(watchLeft){
                    //l'ennemi fonce vers le joueur
                    transform.Translate(direction.normalized * speed * Time.deltaTime, Space.World);
                } else {
                    //sinon on regarde si le joueur regarde son dos et si c'est le cas, l'ennemi est appeuré
                    LookBack(true);
                }
            //si le joueur est à droite de l'ennemi
            } else {
                //et que le joueur regarde à droite  
                if(!watchLeft){
                    //l'ennemi fonce vers le joueur
                    transform.Translate(direction.normalized * speed * Time.deltaTime, Space.World);
                } else {
                    //sinon on regarde si le joueur regarde son dos et si c'est le cas, l'ennemi est appeuré
                    LookBack(false);
                }
            }    
            transform.position = new Vector3(transform.position.x, initY, transform.position.z);
        }
    }

    //méthode pour voir si le joueur regarde le dos de l'ennemi et changer les variables en conséquence
    public void LookBack(bool turnAround){
        //si le joueur regarde quelque chose
        if(whatPlayerHit.collider != null){
            //et que c'est le dos de l'ennemi
            if(whatPlayerHit.collider.CompareTag("Back")){
                //l'ennemi ne bouge plus
                speed = 0f;
                //et il est appeuré
                isAfraid = true;
                //si la coroutine est en cours, on l'arrête
                if(coroutine != null)
                {
                    StopCoroutine(coroutine);
                    coroutine = null;
                }
            }
        } else {
            //s'il ne regarde pas son dos, l'ennemi se retourne
            if(coroutine == null)
                coroutine = StartCoroutine(TurnAround(turnAround));
        }
    }

    //quand le joueur entre en collision avec l'ennemi
    public void OnCollisionEnter2D(Collision2D collision2D){
        if(collision2D.collider.CompareTag("Player")){
            //si l'ennemi est appeuré
            if(isAfraid){
                ContactPoint2D contactPoint2D = collision2D.GetContact(0);
                //et que le joueur touche l'ennemi par le haut
                if(contactPoint2D.normal.y == -1f){
                    AudioManager.instance.Play("MobHit");
                    //on le détruit
                    Invoke("DestroyEnemy", 0.1f);
                //s'il le touche par le côté le joueur prend des dégats
                } else if(contactPoint2D.normal.x == 1f || contactPoint2D.normal.x == -1f){
                    PlayerHealth.instance.TakeDamage(damageAmount);
                }
            }
        }
    }

    //méthode pour détruire l'ennemi
    private void DestroyEnemy(){
        Destroy(gameObject);
    }

    //coroutine pour que l'ennemi se retourne au bout d'un certain temps, et changer son dos de place
    public IEnumerator TurnAround(bool value){
        yield return new WaitForSecondsRealtime(1.5f);
        GetComponent<SpriteRenderer>().flipX = value;
        if(value){
            dosSkelerestLeft.SetActive(false);
            dosSkelerestRight.SetActive(true);
        } else {
            dosSkelerestLeft.SetActive(true);
            dosSkelerestRight.SetActive(false);
        }
        speed = maxSpeed;
        coroutine = null;
    }

    public override void Stun(){ }

    public override void Destun(){ }

    public override void Geyser(){ }   


}
