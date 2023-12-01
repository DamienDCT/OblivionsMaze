using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.Linq;
using System;

[RequireComponent(typeof(PlayerMovement))]
public class PlayerPowerup : MonoBehaviour
{
    // Singleton
    public static PlayerPowerup instance;

    // Référence à l'objet porté par le joueur
    [HideInInspector]
    public GameObject currentItem;

    // Variable indiquant si le joueur peut utilisé le powerup
    private bool canUseItem = true;

    // Sprite transparent
    [SerializeField]
    private Sprite transparentSprite;
    // Référence à l'image d'où sera affiché l'item
    [SerializeField]
    private Image image;

    // Référence au slider pour le cooldown
    [SerializeField]
    private Slider cooldownSlider;

    // Nombre de clé du joueur
    [SerializeField]
    private int nbKeys;
    // Nombre de pièces du joueur
    [SerializeField]
    private int nbCoins;

    // Singleton
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        // Si le joueur appuie sur F
        if (Input.GetKeyDown(KeyCode.F))
        {
            // Si le joueur peut utilisé un item et qu'il a un item sur lui
            if (canUseItem && currentItem != null)
            {
                // On utilise l'item
                currentItem.GetComponent<Item>().UseItem();
            }
        // Si le joueur appuie sur A et qu'il a un item droppable, il lâche l'item
        } else if(Input.GetKeyDown(KeyCode.A)){
            if(currentItem != null && currentItem.GetComponent<Item>().isDroppable){
                SwapPowerUps(null, false);
            }
        }
    }

    // Setters / Getters
    public void SetImageItem(Sprite sprite)
    {
        image.sprite = sprite;
    }
    public void SetNbCoins(int nbCoins)
    {
        this.nbCoins = nbCoins;
    }
    public int GetNbKeys(){
        return this.nbKeys;
    }

    public int GetNbCoins(){
        return this.nbCoins;
    }

    public void SetNbKeys(int nbKeys){
        this.nbKeys = nbKeys;
    }
    public void IncrementNbCoins()
    {
        nbCoins++;
    }

    public void DecrementNbCoins()
    {
        if(nbCoins > 0)
            nbCoins--;
    }
    public void IncrementNbKeys(){
        this.nbKeys++;
    }

    // Méthode pour reset le powerup
    public void ResetPowerup()
    {
        this.currentItem = null;
        SetImageItem(transparentSprite);
    }

    // Méthode pour swap deux powerups
    // (si go = null, a pour action de drop le powerup)
    public void SwapPowerUps(GameObject go, bool zoneDrop){
        // Si le joueur possède un item
        if(currentItem != null){
            // On met à jour l'item pour qu'il redevienne récupérable et on reset le GUI
            SetImageItem(transparentSprite);
            currentItem.GetComponent<SpriteRenderer>().enabled = true;
            currentItem.GetComponent<BoxCollider2D>().enabled = true;
            currentItem.AddComponent<Rigidbody2D>();

            // On démarre la coroutine pour que l'item devienne récupérable
            StartCoroutine(currentItem.GetComponent<Item>().ItemTakeable());
            if(go != null){
                StartCoroutine(go.GetComponent<Item>().ItemTakeable());
            }
            if(zoneDrop)
                // On place l'item lâché à la position du joueur
                currentItem.transform.position = new Vector2(PlayerMovement.instance.gameObject.transform.position.x-2f, PlayerMovement.instance.gameObject.transform.position.y+0.5f);
            else
                // On place l'item lâché à la position du joueur
                currentItem.transform.position = new Vector2(PlayerMovement.instance.gameObject.transform.position.x, PlayerMovement.instance.gameObject.transform.position.y+0.5f);
            // On met que l'item n'est plus sur le joueur
            currentItem.GetComponent<Item>().SetIsOnPlayer(false);
        }
        // On met à jour la variable currentItem
        currentItem = go;
    }

    // Coroutine pour le délai des items
    public IEnumerator CooldownTimer(float timer)
    {
        // Mise à jour de la variable canUseItem pour éviter que le joueur ne réutilise un item avant la fin du cooldown
        canUseItem = false;
        // On met la variable à timer
        float animationTime = timer;
        // Tant que le compteur n'est pas arrivé à 0
        while (animationTime > 0f)
        {
            // On retire le temps depuis la dernière frame au timer
            animationTime -= Time.deltaTime;
            // On calcule une valeur pour avoir l'effet visuel sur le GUI
            float lerpValue = animationTime / timer;
            cooldownSlider.value = Mathf.Lerp(cooldownSlider.minValue, cooldownSlider.maxValue, lerpValue);
            yield return null;
        }
        // A la fin du chrono, on remet la variable canUseItem à true pour que le joueur puisse réutiliser l'item
        canUseItem = true;
    }

}
