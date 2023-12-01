using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IceBloc : Item
{
    // Index du tableau arrowsDirection
    private int indexArrow;
    // Tableau de gameObject représentant les flèches dans les quatre directions
    [SerializeField]
    private GameObject[] arrowsDirection;
    // Référence à arrowsDirection[indexArrow]
    private GameObject currentArrow;
    // Nombre d'utilisation restante
    [SerializeField]
    private int nbUseRemaining;
    // Prefab du glaçon
    [SerializeField]
    private GameObject iceblocPrefab;
    // Référence au texte à afficher en haut à droite dans le GUI
    [SerializeField]
    private Text nbUseRemainingText;

    private void Start()
    {
        // Initialisation des variables
        currentArrow = arrowsDirection[0];
        indexArrow = 0;
        nbUseRemaining = 5;
        nbUseRemainingText.gameObject.SetActive(true);
        nbUseRemainingText.text = "" + nbUseRemaining;
        // On démarre une méthode qui s'appellera en boucle pour changer la direction du tir
        InvokeRepeating("ChangeArrowDirection", 0f, 0.2f);
    }


    private void Update()
    {
        // Si l'objet est sur le joueur
        if(isOnPlayer){
            // On active le texte où est affiché le nombre de coups restants
            nbUseRemainingText.gameObject.SetActive(true);
            // On récupère la position du joueur
            Vector3 playerPosition = PlayerMovement.instance.gameObject.transform.position;
            // On positionne les quatre flèches directionnelles en fonction du joueur
            arrowsDirection[0].transform.position = new Vector2(playerPosition.x - 2f, playerPosition.y);
            arrowsDirection[1].transform.position = new Vector2(playerPosition.x, playerPosition.y + 2f);
            arrowsDirection[2].transform.position = new Vector2(playerPosition.x + 2f, playerPosition.y);
            arrowsDirection[3].transform.position = new Vector2(playerPosition.x, playerPosition.y - 2f);

            // Si le joueur appuie sur R
            if(Input.GetKeyDown(KeyCode.R))
            {
                // On change la flèche actuelle
                AudioManager.instance.Play("Interaction");
                indexArrow = (indexArrow + 1) % arrowsDirection.Length;
                currentArrow = arrowsDirection[indexArrow];
            }
        // Sinon on désactive les flèches et le texte indiquant le nombre de coups restants
        } else {
            nbUseRemainingText.gameObject.SetActive(false);
            foreach(GameObject arrow in arrowsDirection)
                arrow.SetActive(false);
        }
    }

    // Méthode utilisée pour changer de flèche
    private void ChangeArrowDirection(){
        // Si l'item est sur le joueur
        if(isOnPlayer){
            // On désactive toutes les flèches
            foreach(GameObject arrow in arrowsDirection)
            {
                arrow.SetActive(false);
            }
            // On active la flèche actuelle
            currentArrow.SetActive(true);
        }
    }

    //méthode pour détruire l'item
    private void DestroyItem(){
        Destroy(gameObject);
    }


    //méthode pour utiliser l'item
    public override void UseItem()
    {
        //on décrémente son nombre d'utilisations restantes
        nbUseRemaining--;
        nbUseRemainingText.text = "" + nbUseRemaining;
        //s'il est réduit à 0
        if(nbUseRemaining == 0){
            //on fait tomber l'item, puis on le détruit
            PlayerPowerup.instance.SwapPowerUps(null, false);
            Invoke("DestroyItem", .2f);
            return;
        }
            
        AudioManager.instance.Play("IceBlocThrow");
        //pour changer la direction du tir selon la position de la flèche
        switch(indexArrow){
            case 0:
                // Gauche 
                CreerGlacon(-0.5f, 0f, new Vector2(-2f, 0f));
                break;
            case 1:
                // Haut
                CreerGlacon(0f, 1f, new Vector2(0f, 2f));
                break;
            case 2:
                // Droite

                CreerGlacon(0.5f, 0f, new Vector2(2f, 0f));
                break;
            case 3: 
                // Bas
                CreerGlacon(0f, -1f, new Vector2(0f, -2f));
                break;
            default:
                break;
        }
        //cooldown
        StartCoroutine(PlayerPowerup.instance.CooldownTimer(cooldownTimerItem));
    }

    //méthode pour créer un bloc de glace
    private void CreerGlacon(float offsetX, float offsetY, Vector2 velocity){
        //on récupère la position du joueur
        Vector3 playerPosition = PlayerMovement.instance.gameObject.transform.position;
        //on calcule la position depuis laquelle on tire, avec un offset pour ne pas toucher le joueur
        Vector2 positionShoot = new Vector2(playerPosition.x + offsetX, playerPosition.y + offsetY);
        GameObject go = Instantiate(iceblocPrefab, positionShoot, Quaternion.identity);
        //on lui applique une vitesse
        go.GetComponent<Rigidbody2D>().velocity = velocity * 5f;
    }
}
