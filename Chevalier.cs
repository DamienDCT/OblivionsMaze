using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chevalier : Enemy
{
    // Booléen indiquant si Kask a son casque
    private bool hasHelmet;
  
    // Référence à l'objet droppable par Kask
    [SerializeField]
    private GameObject dropItem;
    // Référence à la vitesse de Kask
    [SerializeField]
    private float speed;

    [Header("Sound Effects")]
    // Son quand Kask se déplace
    [SerializeField]
    private AudioSource walkAudioSource;
    // Référence au Renderer de kask
    private Renderer rendu;

    private void Awake()
    {
        // Initialisation des variables
        rendu = GetComponent<Renderer>();
        hasHelmet = true;
    }


    private void Update()
    {
        // Si la caméra voit Kask, et que le son n'est pas encore joué, on le joue
        if(rendu.isVisible){
            if(!walkAudioSource.isPlaying){
                walkAudioSource.Play();
            }
        // Sinon on le stop
        } else {
            walkAudioSource.Stop();
        }
        Move();
    }

    // Méthode appelée lorsque le StunHammer est utilisé sur Kask
    public override void Stun()
    {
        if(hasHelmet){
            AudioManager.instance.Play("HelmetKaskFall");
            // On lui retire son casque
            hasHelmet = false;
        }
        GetComponent<SpriteRenderer>().color = Color.red;
    }

    // Méthode appelée lorsque Kask doit faire des dégâts au joueur
    public void HitPlayer()
    {
        PlayerHealth.instance.TakeDamage(damageAmount);
    }

    // Méthode pour déplacer Kask
    public override void Move()
    {
        // On calcule le vecteur de direction en fonction de là où il regarde
        Vector2 dir = new Vector2(0f, 0f);
        if (GetComponent<SpriteRenderer>().flipX)
        {
            dir.x = -1f;
        } else
        {
            dir.x = 1f;
        }
        transform.Translate(dir.normalized * speed * Time.deltaTime, Space.World);


    }

    // Si Kask rentre en contact avec un EndPath, il fait demi tour
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("EndPath"))
        {
            GetComponent<SpriteRenderer>().flipX = !GetComponent<SpriteRenderer>().flipX;
        }
    }

    //si l'ennemi entre en collision
    public void OnCollisionEnter2D(Collision2D collision)
    {
        //avec un EndLevel ou une plateforme
        if (collision.collider.CompareTag("EndLevel") || collision.collider.CompareTag("Platform"))
        {
            //il se retourne
            GetComponent<SpriteRenderer>().flipX = !GetComponent<SpriteRenderer>().flipX;
            return;
        }
        //s'il entre en collision avec le joueur
        if (collision.collider.CompareTag("Player"))
        {
            ContactPoint2D[] contactpoint = new ContactPoint2D[10];
            collision.GetContacts(contactpoint);
            Vector2 direction = contactpoint[0].normal;
            //par le côté
            if (direction.y == 0f)
            {
                //le joueur prend des dégats
                HitPlayer();
            }
            //par le haut
            else if(direction.y == -1)
            {
                //on fait rebondir le joueur
                PlayerMovement.instance.GetComponent<Rigidbody2D>().velocity = Vector2.up * 15f;
                //si l'ennemi à son casque
                if (hasHelmet)
                {
                    //le joueur prend des dégats
                    HitPlayer();
                } else
                {
                    //on détruit l'ennemi
                    AudioManager.instance.Play("MobHit");
                    Invoke("destroyEnemy", 0.1f);
                }
            }
        }
    }

    //méthode pour détruire l'ennemi
    private void destroyEnemy()
    {
        //s'il a un item à faire tomber
        if(dropItem != null)
        {
            //on lui fait tomber
            GameObject go = Instantiate(dropItem);
            go.transform.position = transform.position;
        }
        //puis on détruit l'ennemi
        Destroy(gameObject);
    }

    public override void Destun() { }
    public override void Geyser() { }
}
