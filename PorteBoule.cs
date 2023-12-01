using UnityEngine;

public class PorteBoule : MonoBehaviour
{
    // Si le mur rentre en contact avec une boule, on détruit le mur
    public void OnCollisionEnter2D(Collision2D collision2D){
        if(collision2D.collider.CompareTag("Boule")){
            AudioManager.instance.Play("WallDestroy");
            Destroy(gameObject);
        }
    }
}
