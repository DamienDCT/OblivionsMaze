using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class Geyser : Item
{
    // Référence à la flèche sur le sol
    [SerializeField]
    private GameObject arrow;
    // Référence au vecteur du centre du geyser (i.e. position de la flèche)
    private Vector2 centreGeyser;
    // Référence au joueur
    private GameObject player;
    // Booléen indiquant si le joueur peut utiliser l'item
    private bool canUseItem;

    private void Awake()
    {
        // Initialisation des variables
        centreGeyser = new Vector2(0f, 0f);
        player = PlayerPowerup.instance.gameObject;
        canUseItem = true;
    }

    private void Update()
    {
        // Si l'item est sur le joueur, on active la flèche
        if (isOnPlayer)
        {
            arrow.SetActive(true);
        }
        // Sinon on la désactive
        else
        {
            arrow.SetActive(false);
        }
        // Si le joueur regarde à gauche, on met le centre du geyser à gauche, sinon on le met à droite
        if (!PlayerMovement.instance.isWatchingLeft)
        {
            centreGeyser = new Vector2(player.transform.position.x + 5f, player.transform.position.y);
        }
        else
        {
            centreGeyser = new Vector2(player.transform.position.x - 5f, player.transform.position.y);
        }
        // Si la flèche est activée
        if (arrow.activeSelf)
        {
            // On regarde où doit atterrir la flèche et on place la flèche et le centre du geyser au bon endroit
            arrow.transform.position = new Vector2(centreGeyser.x, -15f);
            RaycastHit2D[] hit = Physics2D.RaycastAll(new Vector2(centreGeyser.x, player.transform.position.y), Vector2.down, 9f);

            for (int i = 0; i < hit.Length; i++)
            {
                if (hit[i].transform.gameObject.CompareTag("Ground") || hit[i].transform.gameObject.CompareTag("Platform"))
                {
                    arrow.transform.position = new Vector3(centreGeyser.x, hit[i].point.y+0.5f, 0f);
                    centreGeyser.y = hit[i].point.y;
                    break;
                }
            }
            canUseItem = (arrow.transform.position.y != -15f);
        }
    }

    //méthode pour l'utilisation de l'item
    public override void UseItem()
    {
        //si le joueur n'est pas sur le sol ou qu'il ne peut pas utiliser l'item, il ne fait rien
        if (!PlayerMovement.instance.IsGrounded())
            return;

        if(!canUseItem)
            return;
        
        AudioManager.instance.Play("Geyser");

        //on fait une liste des ennemis à portée
        RaycastHit2D[] raycastHit = Physics2D.RaycastAll(new Vector2(centreGeyser.x - 0.75f, centreGeyser.y), Vector2.up, 9f);
        RaycastHit2D[] raycastHit2 = Physics2D.RaycastAll(new Vector2(centreGeyser.x + 0.75f, centreGeyser.y), Vector2.up, 9f);
        RaycastHit2D[] raycastHit3 = Physics2D.RaycastAll(new Vector2(centreGeyser.x, centreGeyser.y), Vector2.up, 9f);
        RaycastHit2D[] allColliders = raycastHit.Concat(raycastHit2).ToArray();
        allColliders = allColliders.Concat(raycastHit3).ToArray();

        // On instantie la particule sur la scène
        GameObject go = Instantiate(particleEffect.gameObject);
        // On définit les positions de la particule
        go.transform.position = centreGeyser;
        go.transform.position = new Vector3(go.transform.position.x, centreGeyser.y, 0f);
        // On joue la particule qui se détruira toute seule à la fin de son animation.
        go.GetComponent<ParticleSystem>().Play();

        //pour chaque ennemis à portée, on lui applique l'effet du geyser (propre à chaque ennemi)
        for (int i = 0; i < allColliders.Length; i++)
        {
            if (allColliders[i].transform.gameObject.CompareTag("Ennemi"))
            {
                allColliders[i].transform.gameObject.GetComponent<Enemy>().Geyser();
            }
        }

        // cooldown
        StartCoroutine(PlayerPowerup.instance.CooldownTimer(cooldownTimerItem));
    }

}
