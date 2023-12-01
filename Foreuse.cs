using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Foreuse : Item
{
    // Référence au boss du monde 2 (item utilisable que dans ce cas)
    private BossWorld2 bossWorld2;
    // Référence au point central de la salle
    private Vector2 middleRoom;
    
    [SerializeField]
    private float groundCooldown;

    private void Start(){
        // On initialise les variables
        bossWorld2 = GameObject.FindGameObjectWithTag("Boss").GetComponent<BossWorld2>();
        middleRoom = bossWorld2.positionCameraCentered.position;
    }

    // Méthode appelée à chaque utilisation de l'item
    public override void UseItem()
    {
        // Si le joueur est dans les airs, on ne fait rien
        if(!PlayerMovement.instance.IsGrounded())
            return;

        AudioManager.instance.Play("Foreuse");
        int hit = bossWorld2.Hit();
        // Si la foreuse a fait des dégâts au joueur
        if(hit == 1){
            // On ejecte le joueur du côté opposé au boss grâce à la référence du centre de la salle
            float forceMagnitude = 9.81f*7.5f;
            Vector3 playerPos = PlayerMovement.instance.gameObject.transform.position;
            if(playerPos.x > middleRoom.x){
                PlayerMovement.instance.gameObject.GetComponent<Rigidbody2D>().velocity = (Vector2.left * forceMagnitude * 4 + Vector2.up * (forceMagnitude / 4f));
            } else {
                PlayerMovement.instance.gameObject.GetComponent<Rigidbody2D>().velocity = (Vector2.right * forceMagnitude * 4 + Vector2.up * (forceMagnitude / 4f));
            }
            // On démarre la coroutine du cooldown d'item
            StartCoroutine(PlayerPowerup.instance.CooldownTimer(cooldownTimerItem));
        } else if(hit == 2){
            // On démarre la coroutine du cooldown d'item
            StartCoroutine(PlayerPowerup.instance.CooldownTimer(groundCooldown));
        } else {
            // On démarre la coroutine du cooldown d'item
            StartCoroutine(PlayerPowerup.instance.CooldownTimer(cooldownTimerItem));
        }
    }
}
