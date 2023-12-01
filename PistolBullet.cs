using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PistolBullet : MonoBehaviour
{
    // Vitesse de la balle du pistolet
    [SerializeField]
    private float speed;
    // Dégâts de la balle du pistolet
    [SerializeField]
    private int damageAmount;
    // Direction de la balle lors du tir
    private Vector3 direction;

    // Méthode pour setup les positions et les rotations de la balle
    public void SetupBullet(Vector3 mousePos){
        // On calcule sa direction
        direction = mousePos - transform.position;
        // On calcule et met à jour sa valeur de rotation 
        float rotationZ = Mathf.Atan2(direction.y, direction.x);
        transform.rotation = Quaternion.Euler(0f, 0f, rotationZ * Mathf.Rad2Deg);
        // On la fait partir dans la direction souhaitée 
        GetComponent<Rigidbody2D>().velocity = direction.normalized * speed;
    }

    // Méthode pour détruire la balle
    private void destroyBullet(){
        if(gameObject != null)
            Destroy(gameObject);
    }

    // Si la balle touche le joueur, on ne fait rien, sinon si la balle touche n'importe quoi d'autre, on la détruit
    public void OnCollisionEnter2D(Collision2D collision){
        if(collision.collider.CompareTag("Player"))
            return;
        destroyBullet();
    }

    // Si la balle touche Karzh'an, on lui fait des dégâts et on détruit la balle
    public void OnTriggerEnter2D(Collider2D collider2D){
        if(collider2D.CompareTag("Boss")){
            collider2D.gameObject.GetComponent<BossWorld1>().TakeDamage(damageAmount);
            destroyBullet();    
        }
    }
}
