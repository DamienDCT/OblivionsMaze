using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class StunHammer : Item
{
    
    //méthode pour utilier le stun hammer
    public override void UseItem()
    {
        //si le joueur n'est pas sur le sol, l'item ne peut pas être utilisé
        if (!PlayerMovement.instance.IsGrounded())
            return;

        //on joue le son du stun hammer
        AudioManager.instance.Play("StunHammer");

        //on fait une secousse de caméra
        GameObject.FindGameObjectWithTag("VirtualCamera").GetComponent<CameraShake>().ShakeCamera(1f, 1f);
        
        // On instantie la particule sur la sc�ne
        GameObject go = Instantiate(particleEffect.gameObject);

        Vector2 positionParticle = new Vector2(PlayerMovement.instance.transform.position.x, PlayerMovement.instance.transform.position.y-2.5f);
        Vector2 positionRaycast= new Vector2(PlayerMovement.instance.transform.position.x + .5f, PlayerMovement.instance.transform.position.y);
        // On d�finit les positions de la particule
        go.transform.position = positionParticle;
        // On joue la particule qui se d�truira toute seule � la fin de son animation.
        go.GetComponent<ParticleSystem>().Play();


        GameObject player = PlayerPowerup.instance.gameObject;
        //on fait une liste d'ennemis à étourdir
        List<GameObject> enemiesToStun = new List<GameObject>();
        Collider2D[] enemies = Physics2D.OverlapCircleAll(positionRaycast, 3f);
        
        for (int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i].CompareTag("Ennemi"))
            {
                enemiesToStun.Add(enemies[i].gameObject);
            }
        }

        // cooldown
        StartCoroutine(PlayerPowerup.instance.CooldownTimer(cooldownTimerItem));

        //S'il y a au moins un ennemi dans le champ d'action, on appelle la coroutine de stun
        if (enemiesToStun.Count > 0)
        {
            StartCoroutine(StunEnnemies(enemiesToStun));
        }
    }

    //méthode pour effectuer la conséquence du stun hammer sur chaque ennemi dans la liste en paramètre
    private IEnumerator StunEnnemies(List<GameObject> _enemiesToStun)
    {
        //pour chaque ennemi dans la liste
        foreach (GameObject enemy in _enemiesToStun)
        {
            if(enemy != null)
                //on lui applique l'effet Stun (propre à chaque ennemi)
                enemy.GetComponent<Enemy>().Stun();

            yield return new WaitForSeconds(3f);

            //après l'attente de 3 secondes, si l'ennemi n'a pas été tué, on lui applique l'effet de Destun
            if (enemy != null)
            {
                enemy.GetComponent<Enemy>().Destun();
            }
        }
    }
}
