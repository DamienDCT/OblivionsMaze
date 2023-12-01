using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class Item : MonoBehaviour
{
    // Booléen servant à indiquer si l'item est droppable ou non
    public bool isDroppable;
    // Booléen pour savoir si l'item est récupérable par le joueur
    [SerializeField]
    protected bool isTakeable;
    // Booléen pour savoir si le joueur est dans la zone de l'item
    [SerializeField]
    protected bool onTarget;
    // Référence au temps de cooldown de l'item
    [SerializeField]
    public float cooldownTimerItem;
    // Booléen pour savoir si le joueur a l'item dans son inventaire
    protected bool isOnPlayer;
    // Particule de l'item s'il en a une
    public ParticleSystem particleEffect;
    // Méthode abstraite pour l'utilisation de l'item
    public abstract void UseItem();

    private void Awake()
    {
        // On initialise les variables
        isTakeable = false;
        onTarget = false;
        isOnPlayer = false;
        StartCoroutine(ItemTakeable());
    }

    private void Start(){
        StartCoroutine(ItemTakeable());
    }

    private void FixedUpdate()
    {
        // Si le joueur est sur l'item
        if (isTakeable && onTarget)
        {
            AudioManager.instance.Play("GrabItem");
            // On setup le script de l'item au script PlayerPowerup
            isOnPlayer = true;
            PlayerPowerup.instance.SwapPowerUps(this.gameObject, false);
            gameObject.transform.position = new Vector2(transform.position.x, transform.position.y - 10f);
            // On met à jour le GUI de l'item
            PlayerPowerup.instance.SetImageItem(GetComponent<SpriteRenderer>().sprite);
            // On désactive tout de l'item sauf le script
            gameObject.GetComponent<BoxCollider2D>().enabled = false;
            gameObject.GetComponent<SpriteRenderer>().enabled = false;
            // On détruit son rigidbody pour ne pas perdre en performance
            Destroy(GetComponent<Rigidbody2D>());
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Si le joueur rentre en contact avec l'item
        if (collision.CompareTag("Player"))
        {
            // On met à jour la variable onTarget
            onTarget = true;
        }
        // Si l'item touche le sol ou une plateforme
        else if (collision.CompareTag("Platform") || collision.CompareTag("Ground"))
        {
            // Pour faire apparaître l'item (animation)
            GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePosition;
        }
    }

    // Coroutine servant à rendre l'item récupérable sur le sol
    public IEnumerator ItemTakeable(){
        SetIsTakeable(false);
        yield return new WaitForSecondsRealtime(1f);
        SetIsTakeable(true);
    }

    // Si le joueur sort de la zone de l'item, il ne revient plus récupérable
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            onTarget = false;
    }

    // Setter pour la variable isTakeable
    public void SetIsTakeable(bool value){
        this.isTakeable = value;
    }

    // Setter pour la variable isOnPlayer
    public void SetIsOnPlayer(bool value){
        this.isOnPlayer = value;
    }
}
