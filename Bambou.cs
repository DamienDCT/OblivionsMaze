using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bambou : MonoBehaviour
{
    // Dégâts au joueur
    [SerializeField]
    private int damageAmount;
    // Référence au joueur
    private Transform player;
    // si flipLeft = true, le bambou tournera dans le sens anti-horaire
    // Sinon, il tournera dans le sens horaire
    private bool flipLeft;
    // Référence à quel Rodut'su a lancé le bambou
    public GameObject mobThrower;
    // Référence à la coroutine de rotation du bambou
    private Coroutine flipCoroutine;

    private void Start()
    {
        // On récupère les positions du joueur au moment du lancer, et on met à jour la variable flipLeft
        player = GameObject.FindGameObjectWithTag("Player").transform;
        if(player.transform.position.x < transform.position.x)
            flipLeft = true;
        else
            flipLeft = false;
        // On lance la rotation du bambou
        flipCoroutine = StartCoroutine(FlipBambou());
        Invoke("DestroyBullet", 3f);
    }
    private IEnumerator FlipBambou()
    {
        // On tourne le bambou selon la variable flipLeft
        while(gameObject != null){
            Vector3 rotation = gameObject.transform.rotation.eulerAngles;
            if(flipLeft)
                rotation.z += 75f;
            else 
                rotation.z -= 75f;
            gameObject.transform.rotation = Quaternion.Euler(rotation);
            yield return new WaitForSecondsRealtime(0.1f);
        }
    }

    // A chaque collision du bambou
    private void OnCollisionEnter2D(Collision2D collision2D)
    {
        // Si l'objet collisionné est le joueur
        if (collision2D.collider.CompareTag("Player"))
        {
            // On fait prendre des dégâts au joueur
            PlayerMovement.instance.GetComponent<PlayerHealth>().TakeDamage(damageAmount);
            return;
        }
        if(!collision2D.collider.CompareTag("Bouclier"))
        {
            // Si c'est autre chose que le joueur ou le bouclier, on détruit le bambou
            DestroyBullet();
        }
        else 
        {
            // Si c'est le bouclier, on renvoie le bambou droit vers le Rodut'su qui l'a lancé
            AudioManager.instance.Play("ShieldRenvoie");
            if(mobThrower == null)
                return;

            if(flipCoroutine != null){
                StopCoroutine(flipCoroutine);
                flipCoroutine = null;
            }
            Vector3 positionMob = mobThrower.transform.position;
            // On met la gravité à 0 car le lancer sera droit
            GetComponent<Rigidbody2D>().gravityScale = 0f;
            if(transform.position.x < positionMob.x)
            {
                Vector3 direction = positionMob - transform.position;
                Ray2D ray2D = new Ray2D(direction.normalized, direction.normalized * direction.magnitude);
                Debug.DrawRay(ray2D.origin, ray2D.direction, Color.red, 3f);
                GetComponent<Rigidbody2D>().velocity = direction * 5f;
                float rotationZ = Mathf.Atan2(direction.y, direction.x);
                transform.rotation = Quaternion.Euler(0f, 0f, rotationZ * Mathf.Rad2Deg);
            }

        }
    }

    private void DestroyBullet()
    {
        if (gameObject != null)
        {
            if(flipCoroutine != null)
            {
                StopCoroutine(flipCoroutine);
                flipCoroutine = null;
            }
            Destroy(gameObject);
        }
    }
}
