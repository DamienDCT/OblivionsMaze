using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    // Particule de fumée de la boule de feu
    [SerializeField]
    private ParticleSystem smokeParticle;
    
    // Vitesse de la boule de feu
    [SerializeField]
    private float speed;
    // Dégâts au joueur de la boule de feu
    [SerializeField]
    private int damageAmount;

    // Vecteur de direction 
    private Vector3 direction;

    // Méthode appelée dés que la boule de feu est instantié
    private void Start()
    {
        // On vérifie si les VFX sont activés, si ce n'est pas le cas, on désactive la traînée de fumée
        if(!SettingsJSON.instance.settings.videoSettings.isVFXToggled)
            smokeParticle.gameObject.SetActive(false);

        // On récupère la position du joueur
        Vector3 pos = GameObject.FindGameObjectWithTag("Player").transform.position;
        // On ajuste la hauteur de la boule de feu
        pos += new Vector3(0f, 0.5f, 0f);
        // Calcul de la trajectoire et orientation de la boule de feu
        direction = pos - transform.position;
        float rotationZ = Mathf.Atan2(direction.y, direction.x);
        transform.rotation = Quaternion.Euler(0f, 0f, rotationZ * Mathf.Rad2Deg);
        // On met à jour le vecteur vitesse de la boule de feu
        GetComponent<Rigidbody2D>().velocity = direction * speed;
    }


    private void destroyBullet(){
        if(gameObject != null)
            Destroy(gameObject);
    }

    public void OnCollisionEnter2D(Collision2D collision){
        // Si le joueur est touché par la boule de feu
        if(collision.collider.CompareTag("Player")){
            // le joueur prend des dégâts
            PlayerMovement.instance.GetComponent<PlayerHealth>().TakeDamage(damageAmount);
        }
        // On détruit la boule de feu qu'importe ce que touche la boule de feu
        destroyBullet();
    }
}
