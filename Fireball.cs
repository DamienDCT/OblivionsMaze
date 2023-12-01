
using System.Collections;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    //particules qui sont derrière la boule de feu
    [SerializeField]
    private ParticleSystem smokeParticle; 

    //vitesse de la boule de feu
    [SerializeField]
    private float speed;


    //Lorsque la boule de feu est envoyée
    private void Start(){
        //on la détruit au bout d'un temps donné (ici 3 secondes)
        StartCoroutine(DestroyBullet());
        //on met les particules de fumée si les VFX sont bien activées
        if(!SettingsJSON.instance.settings.videoSettings.isVFXToggled)
            smokeParticle.gameObject.SetActive(false);
    }

    //Coroutine pour détruire la boule de feu au bout d'un temps donné
    private IEnumerator DestroyBullet(){
        yield return new WaitForSecondsRealtime(3f);
        if(gameObject != null)
            Destroy(gameObject);
    }

    //on calcule la trajectoire du tir et sa vitesse pour un tir vers la gauche
    public void LaunchLeft(){
        Vector2 direction = new Vector2(-5f, 0f);
        float rotationZ = Mathf.Atan2(direction.y, direction.x);
        transform.rotation = Quaternion.Euler(0f, 0f, rotationZ * Mathf.Rad2Deg);
        GetComponent<Rigidbody2D>().velocity = direction * speed;
    }

    //on calcule la trajectoire du tir et sa vitesse pour un tir vers la droite
    public void LaunchRight(){
        Vector2 direction = new Vector2(5f, 0f);
        float rotationZ = Mathf.Atan2(direction.y, direction.x);
        transform.rotation = Quaternion.Euler(0f, 0f, rotationZ * Mathf.Rad2Deg);
        GetComponent<Rigidbody2D>().velocity = direction * speed;
    }

    //si la boule de feu entre en contact avec le joueur
    public void OnCollisionEnter2D(Collision2D collision2D){
        if(collision2D.collider.CompareTag("Player")){
            //on lui applique un effet de brulure
            PlayerHealth.instance.Burn();
            //puis on détruit la boule de feu
            Destroy(gameObject);
        }
    }
}
