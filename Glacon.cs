using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Glacon : MonoBehaviour
{
    // Méthode pour détruire le gameObject
    private void DestroyGlacon(){
        Destroy(gameObject);
    }

    // Si le glaçon touche autre chose que le confiner ou un objet de tag "Untagged", on le détruit
    public void OnCollisionEnter2D(Collision2D collision2D){
        if(!collision2D.collider.CompareTag("Confiner"))
            Invoke("DestroyGlacon", .1f);
    }
    
    // Si le glaçon touche autre chose que le confiner ou un objet de tag "Untagged", on le détruit
    public void OnTriggerEnter2D(Collider2D collider2D){
        if(!collider2D.CompareTag("Confiner"))
            Invoke("DestroyGlacon", .1f);
    }
}
