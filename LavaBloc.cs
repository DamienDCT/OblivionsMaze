using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaBloc : MonoBehaviour
{
    // force qu'on applique au joueur pour qu'il "rebondisse" au contact de la lave
    [SerializeField]
    private float forceUp;
    
    //on vérifie la collission avec le joueur et on lui applique la force et un effet de brulure
    public void OnCollisionEnter2D(Collision2D collision2D){
        if(collision2D.collider.CompareTag("Player")){
            PlayerHealth.instance.Burn();
            Rigidbody2D rb2D = PlayerMovement.instance.gameObject.GetComponent<Rigidbody2D>();
            rb2D.velocity = new Vector2(rb2D.velocity.x, 20f);
        }
    }
}
