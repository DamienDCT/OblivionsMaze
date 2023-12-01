using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Rodutsu : Enemy
{
    // Waypoints pour le déplacement
    [SerializeField]
    private Transform[] waypoints;
    // Waypoint actuel
    private Transform currentTarget;
    // Index du waypoint dans le tableau waypoints correspondant au waypoint actuel
    private int destIndex;
    // Vitesse de l'ennemi
    private float speed;
    // Prefab du bambou
    [SerializeField]
    private GameObject bambouPrefab;
    // Booléen indiquant si l'ennemi track le joueur
    [SerializeField]
    private bool isTrackingPlayer;
    // Vitesse maximale de l'ennemi
    [SerializeField]
    private float maxSpeed;
    // Coroutine de tir de l'ennemi
    private Coroutine shootCoroutine;
    // Référence au joueur
    private GameObject player;
    // Booléen indiquant si l'ennemi est en train de tirer
    private bool isShooting;
    // rayon du champ de vision de l'ennemi
    [SerializeField]
    private float range;
    // Points de vie de l'ennemi
    [SerializeField]
    private int health;
    // Dégât que fait un bambou sur l'ennemi
    [SerializeField]
    private int damageByHitBambou;
    // Text pour le nombre de coups restants
    [SerializeField]
    private Text hitText;
    // Prefab pour la clé
    [SerializeField]
    private GameObject clePrefab;

    private void Awake()
    {
        // On initialise les variables
        destIndex = 0;
        currentTarget = waypoints[0];
        ResetSpeed();
        isTrackingPlayer = false;
        InvokeRepeating("CheckPlayer", 0f, 0.5f);//pour ne pas faire trop de calculs par seconde on check 2 fois
                                                 //par seconde et non pas à chaque frame
        player = GameObject.FindGameObjectWithTag("Player");
        isShooting = false;
        hitText.text = "" + health;
    }

    private void Update()
    {
        Move();
    }

    // méthode pour regarder où est le joueur
    private void CheckPlayer()
    {
        // Si le joueur est présent dans le cercle de rayon "range"
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, range);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].CompareTag("Player"))
            {
                // Et que le joueur vient juste de rentrer dans le cercle
                if(isTrackingPlayer == false)
                {
                    // Si la coroutine est null (i.e. que le mob n'est pas en train de tirer)
                    if(shootCoroutine == null)
                    {
                        // On fait tirer l'ennemi
                        shootCoroutine = StartCoroutine(Shoot());
                    }
                    // On passe la variable à true
                    isTrackingPlayer = true;
                }
                return; 
            }
        }
        // Si la coroutine existe et que le joueur n'est pas dans la zone
        if (shootCoroutine != null)
        {
            // On arrête la séquence de tir
            StopCoroutine(shootCoroutine);
            shootCoroutine = null;
            isShooting = false;
        }   
        // on passe la variable à false car le joueur n'est plus dans la zone
        isTrackingPlayer = false;
    }

    // Coroutine de tir
    private IEnumerator Shoot()
    {
        while (true)
        {
            // Si l'ennemi ne tire pas
            if(!isShooting){
                // On lui met sa vitesse à 0, on le fait tirer au bout de 1.5s, puis on lui remet sa vitesse au bout de 4 secondes
                speed = 0f;
                Invoke("ShootCoroutine", 1.5f);
                Invoke("ResetSpeed", 4f);
                isShooting = true;
            }
            // Après qu'il a tiré, on attend 5 secondes pour qu'il retire
            yield return new WaitForSecondsRealtime(5f);

            isShooting = false;
        }
    }

    // Coroutine de tir
    private void ShootCoroutine(){
        StartCoroutine(ShootBullet());
    }

    // Coroutine pour tirer les bambous
    private IEnumerator ShootBullet(){
        // Si le joueur est dans la zone du joueur
        if(isTrackingPlayer){
            AudioManager.instance.Play("BambouThrow");
            Vector3 positionShoot = Vector3.zero;
            // On regarde où tirer le bambou 
            GetComponent<SpriteRenderer>().flipX = (transform.position.x < player.transform.position.x);
            if (GetComponent<SpriteRenderer>().flipX)
            {
                positionShoot = new Vector3(transform.position.x + 2.3f, transform.position.y, 0f);
            } else
            {
                positionShoot = new Vector3(transform.position.x - 2.3f, transform.position.y, 0f);
            }
            // On instancie le bambou à la bonne position et on lui assigne son lanceur au gameObject 
            GameObject projectile = Instantiate(bambouPrefab, positionShoot, Quaternion.identity);
            projectile.GetComponent<Bambou>().mobThrower = this.gameObject;
            // On ajoute une force de lancer de 5f
            AddForceToProjectile(projectile, 5f);
            yield return new WaitForSecondsRealtime(0.2f);

            AudioManager.instance.Play("BambouThrow");
            // On regarde où tirer le bambou 
            if (GetComponent<SpriteRenderer>().flipX)
            {
                positionShoot = new Vector3(transform.position.x + 2.3f, transform.position.y, 0f);
            } else
            {
                positionShoot = new Vector3(transform.position.x - 2.3f, transform.position.y, 0f);
            }
            // On instancie le bambou à la bonne position et on lui assigne son lanceur au gameObject 
            projectile = Instantiate(bambouPrefab, positionShoot, Quaternion.identity);
            projectile.GetComponent<Bambou>().mobThrower = this.gameObject;
             // On ajoute une force de lancer de 7f
            AddForceToProjectile(projectile, 7f);
            yield return new WaitForSecondsRealtime(0.2f);
            AudioManager.instance.Play("BambouThrow");
            // On regarde où tirer le bambou 
            if (GetComponent<SpriteRenderer>().flipX)
            {
                positionShoot = new Vector3(transform.position.x + 2.3f, transform.position.y, 0f);
            } else
            {
                positionShoot = new Vector3(transform.position.x - 2.3f, transform.position.y, 0f);
            }
            // On instancie le bambou à la bonne position et on lui assigne son lanceur au gameObject 
            projectile = Instantiate(bambouPrefab, positionShoot, Quaternion.identity);
            projectile.GetComponent<Bambou>().mobThrower = this.gameObject;
             // On ajoute une force de lancer de 9f
            AddForceToProjectile(projectile, 9f);
            yield return new WaitForSecondsRealtime(0.2f);   
        }
    }

    private void AddForceToProjectile(GameObject projectile, float valueForce){
        if(player.transform.position.x < transform.position.x)
            projectile.GetComponent<Rigidbody2D>().AddForce(new Vector2(-7f, valueForce), ForceMode2D.Impulse);
        else 
            projectile.GetComponent<Rigidbody2D>().AddForce(new Vector2(7f, valueForce), ForceMode2D.Impulse);
    }

    private void ResetSpeed(){
        GetComponent<SpriteRenderer>().flipX = (transform.position.x < currentTarget.position.x);
        speed = maxSpeed;
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("EndLevel"))
        {
            GetComponent<SpriteRenderer>().flipX = !GetComponent<SpriteRenderer>().flipX;
        } else if(collision.collider.CompareTag("Bambou")){
            health -= damageByHitBambou;
            AudioManager.instance.Play("BambouHitRodutsu");
            if(health == 0)
            {
                if(clePrefab != null)
                    Instantiate(clePrefab, transform.position, Quaternion.identity);
                Destroy(gameObject);
            }
            hitText.text = "" + health;

        }
    }

    public void OnTriggerEnter2D(Collider2D collider){
        if (collider.CompareTag("EndPath"))
        {
            GetComponent<SpriteRenderer>().flipX = !GetComponent<SpriteRenderer>().flipX;
            return;
        }
        if(collider.CompareTag("Player")){
            PlayerHealth.instance.TakeDamage(damageAmount);
        }
    }


    private void destroyEnemy()
    {
        if(clePrefab != null){
            GameObject go = Instantiate(clePrefab);
            go.transform.position = transform.position;   
        }
        Destroy(gameObject);
    }

    public override void Move()
    {
        Vector3 direction = currentTarget.position - transform.position;
        transform.Translate(direction.normalized * speed * Time.deltaTime, Space.World);
        if(Vector3.Distance(transform.position, currentTarget.position) < .3f){
            destIndex = (destIndex + 1) % waypoints.Length;
            currentTarget = waypoints[destIndex];
            GetComponent<SpriteRenderer>().flipX = (transform.position.x < currentTarget.position.x);
        }
    }

    public override void Geyser()
    {

    }

    public override void Stun() { }
    public override void Destun() { 

    }

}
