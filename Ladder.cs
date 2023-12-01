using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ladder : MonoBehaviour
{
    // Booléen indiquant si le joueur est dans la range de l'échelle
    public bool isInRange;
    // Booléen indiquant si le joueur est en train de grimper à l'échelle
    public bool isClimbing;
    // Variable servant à récupérer la valeur de l'axe verticale
    private float getAxis;
    // Référence au rigidbody du joueur
    private Rigidbody2D rgb2D;
    // Référence à la plateforme d'atterrissage de l'échelle
    public PlatformEffector2D landedEffect;

    private float vertical;

    private void Awake()
    {
        rgb2D = PlayerMovement.instance.gameObject.GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        getAxis = Input.GetAxis("Vertical");
        vertical = getAxis * PlayerMovement.instance.movementSpeed * 1.5f * Time.deltaTime;
    }

    private void Update()
    {
        // Mise à jour du collider pour que le joueur puisse passer dans les deux sens de l'échelle
        if (landedEffect.rotationalOffset == 0 && getAxis < 0)
        {
            landedEffect.rotationalOffset = 180;
        }
        else if (landedEffect.rotationalOffset == 180 && getAxis > 0)
        {
            landedEffect.rotationalOffset = 0;
        }
        // Si le joueur est dans la range de l'échelle
        if (isInRange)
        {
            // Et qu'il se déplace sur l'axe vertical
            if(vertical != 0f){
                // On met à jour sa vélocité
                rgb2D.velocity = new Vector2(rgb2D.velocity.x, vertical);
                // On met à jour les variables
                isClimbing = true;
                PlayerMovement.instance.isClimbing = true;
                AudioManager.instance.PlayLoop("Ladder");
            } else {
                // Sinon on reset la plateforme d'atterrissage
                landedEffect.rotationalOffset = 0;
            }
        } else
        {
            // Sinon on reset la plateforme d'atterrissage
            landedEffect.rotationalOffset = 0;
        }
    }

    // Si le joueur entre en contact avec l'échelle sur sa collision non trigger
    private void OnCollisionEnter2D(Collision2D collision2D){
        if(collision2D.collider.CompareTag("Player")){
            // On stop le son de l'échelle
            AudioManager.instance.Stop("Ladder");
        }
    }

    // Si le joueur entre en contact avec la collision trigger de l'échelle
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            // On met à jour la variable isInRange
            isInRange = true;
        }
    }

    // Si le joueur n'est plus en contact avec la collision trigger de l'échelle
    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            // On met à jour les variables et on stop le son de l'échelle
            AudioManager.instance.Stop("Ladder");
            isInRange = false;
            PlayerMovement.instance.isClimbing = false;
            landedEffect.rotationalOffset = 0;
        }
    }
}
