using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    // Singleton
    public static PlayerHealth instance;

    // Nombre de vie actuel du joueur
    public int nbLives;
    // Nombre de points de vie max
    public int maxHealth = 100;
    // Points de vie actuel
    public int currentHealth;
    // Nombre de vies max
    public int maxLives = 3;
    // Booléen indiquant si le joueur peut se faire toucher
    [HideInInspector]
    public bool isInvincible;
    // Référence à la barre de vie
    [SerializeField]
    private HealthBar healthBar;

    // Booléens pour savoir respectivement si le joueur est empoisonné, ou brûlé
    public bool isPoisoned {get; private set;}
    public bool isBurnt {get; private set;}

    // Singleton
    private void Awake()
    {
        if(instance == null){
            instance = this;
        } else {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        // On initialise les variables
        isPoisoned = false;
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(currentHealth);
        nbLives = SaveGameData.instance.playerInfoData.nbLives;
        isInvincible = false;
    }

    // Méthode pour prendre des dégâts
    public void TakeDamage(int damage)
    {
        // Si le joueur n'est pas en mode créatif
        if(!CreativeMode.instance.isCreativeActivated)
        {
            // Si le joueur peut prendre des dégâts
            if (!isInvincible)
            {   
                // On joue le son du joueur qui prend des dégâts
                AudioManager.instance.Play("PlayerHit");
                // On lui retire le nombre de points de vie passé en paramètres
                currentHealth -= damage;
                // Si le joueur a en dessous de 0 points de vie, on le tue
                if(currentHealth <= 0){
                    GameObject.FindGameObjectWithTag("DeathManager").GetComponent<DeathDetection>().Death();
                }
                // On met à jour le slider de la barre de vie
                healthBar.SetHealth(currentHealth);
                // On le rend invincible
                isInvincible = true;
                // On démarre la coroutine pour qu'il redevienne touchable
                StartCoroutine(AfterDamage());
            }
        }
    }

    // Méthode pour prendre des dégâts de debuff
    private void TakeDebuffDamage(int damage)
    {
        // Si le mode créatif n'est pas activé
        if(!CreativeMode.instance.isCreativeActivated)
        {
            // On retire le nombre de points de vie prévu au joueur
            currentHealth -= damage;
            AudioManager.instance.Play("PlayerHit");
            // Si le joueur est en dessous de 0 points de vie, on le tue
            if(currentHealth <= 0){
                GameObject.FindGameObjectWithTag("DeathManager").GetComponent<DeathDetection>().Death();
            }
            // On met à jour la barre de vie
            healthBar.SetHealth(currentHealth);
        }
    }

    // Méthode servant à retirer l'invincibilité du joueur après un coup
    private IEnumerator AfterDamage()
    {
        yield return new WaitForSecondsRealtime(2f);
        isInvincible = false;
    }

    // Méthode servant à empoisonner le joueur
    public void Poison(){
        // Si le joueur n'est ni empoisonné, ni brûlé, on l'empoisonne
        if(!isPoisoned && !isBurnt){
            StartCoroutine(PoisonTimer());
        }
    }

    // Méthode servant à brûler le joueur
    public void Burn(){
        // Si le joueur n'est ni empoisonné, ni brûlé, on le brûle
        if(!isBurnt && !isPoisoned){
            StartCoroutine(BurnTimer());
        }
    }

    // Méthode pour le cooldown du poison
    private IEnumerator PoisonTimer(){
        // Au début, on met à jour l'interface pour le poison
        isPoisoned = true;
        healthBar.SetPoison(true);
        yield return new WaitForSecondsRealtime(1f);
        // On fait prendre au joueur les dégâts pendant le temps donné
        for(int i = 0; i < 5; i++)
        {
            TakeDebuffDamage(3);
            yield return new WaitForSecondsRealtime(1f);
        }
        // On remet l'interface comme au début
        healthBar.SetPoison(false);
        isPoisoned = false;
    }

    // Méthode pour le cooldown de la brûlure
    private IEnumerator BurnTimer(){
        // Au début, on met à jour l'interface pour la brûlure
        isBurnt = true;
        healthBar.SetBurn(true);
        yield return new WaitForSecondsRealtime(1f);
        // On fait prendre au joueur les dégâts pendant le temps donné
        for(int i = 0; i < 5; i++)
        {
            TakeDebuffDamage(3);
            yield return new WaitForSecondsRealtime(1f);
        }
        // On remet l'interface comme au début
        healthBar.SetBurn(false);
        isBurnt = false;
    }

    // Méthode pour ajouter des points de vie
    public void AddHealth(int healthToAdd){
        currentHealth += healthToAdd;
        if(currentHealth >= maxHealth)
            currentHealth = maxHealth % currentHealth;
        healthBar.SetHealth(currentHealth);
    }

    // Méthode pour reset les points de vie au nombre max de points de vie
    public void ResetHealth()
    {
        currentHealth = maxHealth;
        healthBar.SetHealth(maxHealth);
    }
}
