using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Salamandre : Enemy
{
    // Référence à l'animator de la salamandre
    [SerializeField]
    private Animator animator;
    // Référence à l'objet droppable 
    [SerializeField]
    private GameObject dropItem;
    // Référence à la vitesse de la salamandre
    private float speed;
    // Prefab pour la boule de feu
    [SerializeField]
    private GameObject fireballPrefab;
    // Booléen indiquant si la salamandre voit le joueur
    [SerializeField]
    private bool isTrackingPlayer;
    // Référence à la vitesse maximale atteignable par la salamandre
    [SerializeField]
    private float maxSpeed;
    // Coroutine de tir de la salamandre
    private Coroutine shootCoroutine;
    // Référence au joueur
    private GameObject player;
    // Booléen indiquant si la salamandre est en train de tirer
    private bool isShooting;

    private void Awake()
    {
        // On initialise les variables
        ResetSpeed();
        isTrackingPlayer = false;
        InvokeRepeating("CheckPlayer", 0f, 0.5f);//pour ne pas faire trop de calculs par seconde on check 2 fois
                                                 //par seconde et non pas à chaque frame
        player = GameObject.FindGameObjectWithTag("Player");
        isStun = false;
        isShooting = false;
    }

    private void Update()
    {
        Move();
    }

    private void CheckPlayer()
    {
        // On regarde autour de la salamandre si elle voit le joueur
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 10f);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].CompareTag("Player"))
            {
                // Si le joueur vient juste de rentrer dans le champ de vision de la salamandre
                if(isTrackingPlayer == false)
                {
                    // la salamandre commence à tirer
                    if(shootCoroutine == null)
                    {
                        shootCoroutine = StartCoroutine(Shoot());
                    }
                    isTrackingPlayer = true;
                }
                return; 
            }
        }
        // Si le joueur n'est plus dans la zone de la salamandre, on stop la coroutine de tir et on réinitialise les variables
        if (shootCoroutine != null)
        {
            StopCoroutine(shootCoroutine);
            shootCoroutine = null;
            isShooting = false;
        }   
        isTrackingPlayer = false;
    }

    // Coroutine permettant de tirer les boules de feu
    private IEnumerator Shoot()
    {
        while (true)
        {
            // Si la salamandre n'est pas stun
            if(!isStun){
                // Si la salamandre peut tirer
                if(!isShooting){
                    // On stop la vitesse del a salamandre, puis on fait tirer la salamandre 1.5s plus tard
                    speed = 0f;
                    animator.SetBool("Shoot", true);
                    Invoke("ShootBullet", 1.5f);
                    isShooting = true;
                }
            }
            // On attends 5 secondes pour la prochaine boule de feu
            yield return new WaitForSecondsRealtime(5f);
            isShooting = false;
        }
    }

    // Méthode pour tirer un projectile
    private void ShootBullet(){
        // Si la salamandre voit le joueur
        if(isTrackingPlayer){
            // Si elle n'est pas stun
            if(!isStun){
                // On calcule la position de tir par rapport à là où est le joueur
                Vector3 positionShoot = Vector3.zero;
                
                if (GetComponent<SpriteRenderer>().flipX)
                {
                    positionShoot = new Vector3(transform.position.x + 2.3f, transform.position.y, 0f);
                } else
                {
                    positionShoot = new Vector3(transform.position.x - 2.3f, transform.position.y, 0f);
                }

                AudioManager.instance.Play("FireballThrow");
                // On instancie la boule de feu
                GameObject go = Instantiate(fireballPrefab, positionShoot, Quaternion.identity);
            }
        }
        // On reset la vitesse de la salamandre 1.5s plus tard
        Invoke("ResetSpeed", 1.5f);
    }

    // Méthode pour reset la vitesse de la salamandre
    private void ResetSpeed(){
        animator.SetBool("Shoot",false);
        speed = maxSpeed;
    }

    // Si la salamandre touche un objet de tag "EndLevel", on lui fait faire demi tour
    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("EndLevel"))
        {
            GetComponent<SpriteRenderer>().flipX = !GetComponent<SpriteRenderer>().flipX;
        }
    }

    public void OnTriggerEnter2D(Collider2D collider){
    // Si la salamandre touche un objet de tag "EndPath", on lui fait faire demi tour
        if (collider.CompareTag("EndPath"))
        {
            GetComponent<SpriteRenderer>().flipX = !GetComponent<SpriteRenderer>().flipX;
        // Sinon si elle touche le joueur
        } else if(collider.CompareTag("Player")){
            // On regarde si la salamandre a été touché par le joueur sur le dessus
            ContactPoint2D[] contactpoint = new ContactPoint2D[10];
            collider.GetContacts(contactpoint);
            Vector2 direction = contactpoint[0].normal;
            if (direction.y == 0f)
            {
                // Si c'est le cas, on fait rebondir le joueur
                PlayerMovement.instance.GetComponent<Rigidbody2D>().velocity = Vector2.up * 15f;
                // Si la salamandre est stun, on la tue
                if(isStun){
                    AudioManager.instance.Play("MobHit");
                    Invoke("destroyEnemy", 0.1f);
                // Sinon le joueur prend des dégâts
                } else {
                    PlayerHealth.instance.TakeDamage(damageAmount);
                }
            // Sinon le joueur prend des dégâts
            } else {
                PlayerHealth.instance.TakeDamage(damageAmount);
            }
        }
    }

    // Méthode pour détruire le gameObject
    private void destroyEnemy()
    {
        Destroy(gameObject);
    }

    // Méthode pour déplacer la salamandre
    public override void Move()
    {
        // Si la salamandre voit le joueur dans son champ de vision, on la retourne vers le joueur
        if(isTrackingPlayer){
            GetComponent<SpriteRenderer>().flipX = (transform.position.x < player.transform.position.x);
        }
        // Si elle n'est pas stun
        if(!isStun){
            // On calcule sa direction en fonction de là où regarde la salamandre
            Vector2 dir = new Vector2(0f, 0f);
            if (GetComponent<SpriteRenderer>().flipX)
            {
                dir.x = 1f;
            } else
            {
                dir.x = -1f;
            }
            // On la déplace selon l'environnement Space.World
            transform.Translate(dir.normalized * speed * Time.deltaTime, Space.World);
        }
    }

    // Action du geyser sur la salamandre
    public override void Geyser()
    {   
        // Si la salamandre n'est pas stun
        if(!isStun){
            // on la stun, on la propulse et on la retourne
            isStun = true;
            GetComponent<Rigidbody2D>().velocity = Vector2.up * 30f;
            GetComponent<SpriteRenderer>().flipY = true;
            // On remet la salamandre à l'endroit au bout de 5 secondes
            Invoke("Destun", 5f);
        }
    }

    public override void Stun() { }
    // Méthode de Destun de la salamandre
    public override void Destun() { 
        // On retourne et destun la salamandre
        isStun = false;
        GetComponent<SpriteRenderer>().flipY = false;
    }

}
