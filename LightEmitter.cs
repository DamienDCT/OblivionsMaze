using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;


public class LightEmitter : MonoBehaviour
{
    // Référence à la ligne de la lumière du joueur
    [SerializeField]
    private LineRenderer lineRenderer;
    // Distance max de la lumière
    [SerializeField]
    private float maxStepDistance = 40f;

    // Sprites de la cible
    [SerializeField]
    private Sprite targetOn;
    [SerializeField]
    private Sprite targetOff;
    // Référence pour la cible 
    [SerializeField]
    private GameObject target;
    // Référence du LayerMask pour les mirroirs 
    [SerializeField]
    private LayerMask mirrorLayerMask;
    // Référence au point de tir de l'émetteur
    [SerializeField]
    private Transform shootPoint;
    // Booléen servant à savoir si la lumière a touché la cible
    private bool hasTouchedTarget;
    // Entier servant à savoir combien de réflections doit faire le rayon au maximum
    private int maxDepth = 4;  
    // Booléen servant à connaître si le joueur est dans la zone de l'émetteur ou non
    private bool canPlayerInteract;
    // LayerMask du joueur
    [SerializeField]
    private LayerMask playerLayerMask;
    // Tableau d'angles de rotation pour l'émetteur
    [SerializeField]
    private float[] positionInZAxis;
    // Index du tableau de la rotation actuelle
    [SerializeField]
    private int currentPosition;
    // Référence pour la porte à ouvrir
    [SerializeField]
    private GameObject doorToOpen;
    // Référence à la caméra cinemachine
    private CinemachineVirtualCamera cinemachineVirtualCamera;
    // Référence au texte d'interaction 
    private Interaction interaction;

    private void Awake()
    {
        // On initialise les variables
        interaction = GetComponent<Interaction>();
        hasTouchedTarget = false;
        canPlayerInteract = false;
    }

    void Start()
    {
        // On initialise qui doivent être initialisées à la première frame du jeu
        cinemachineVirtualCamera = GameObject.FindGameObjectWithTag("VirtualCamera").GetComponent<CinemachineVirtualCamera>();
        // On démarre le tir de lumière qui se répètera toutes les 0.5s
        InvokeRepeating("ShootLight", 0f, 0.5f);
        lineRenderer.startWidth = 0.1f;
        UpdateEmitter();
    }

    void Update(){
        // Si l'émetteur a touché la cible, on met à jour son sprite
        if(hasTouchedTarget){
            target.GetComponent<SpriteRenderer>().sprite = targetOn;
        } else {
            // Sinon, on regarde si le joueur est dans la zone
            CheckPresence();
            // Si le joueur appuie sur E
            if(Input.GetKeyDown(KeyCode.E)){
                // Si le joueur peut interagir avec l'émetteur
                if(canPlayerInteract){
                    // On joue le son d'interaction et on modifie la position de l'émetteur
                    AudioManager.instance.Play("Interaction");
                    currentPosition = (currentPosition + 1) % positionInZAxis.Length;
                    UpdateEmitter();
                }
            }
        }
    }   

    // Méthode servant à check la présence du joueur par rapport à l'émetteur
    private void CheckPresence(){
        RaycastHit2D hit = Physics2D.BoxCast(transform.position, new Vector2(1.5f, 2.5f), 0f, new Vector2(0f, 0f), 0f, playerLayerMask);
        // Si le joueur est présent
        if(hit){
            // Si la cible n'a pas été touché et que le joueur vient de rentrer en contact avec l'émetteur
            if(!hasTouchedTarget && canPlayerInteract == false)
                // On affiche le texte
                interaction.PutText();
            // Et on met à jour sa variable
            canPlayerInteract = true;
            return;
        }
        // Sinon, si le joueur peut interagir, mais n'est plus en contact avec l'émetteur
        if(canPlayerInteract){
            // On supprime le texte et on met à jour la variable
            interaction.EraseText();
            canPlayerInteract = false;
        }
    }

    // Méthode servant à tourner l'émetteur en fonction de la rotation actuelle choisie par le joueur
    private void UpdateEmitter(){
        transform.rotation = Quaternion.Euler(0f, 0f, positionInZAxis[currentPosition]);
    }

    // Méthode servant à lancer le rayon de lumière
    private void ShootLight()
    {
        // Si l'émetteur n'a pas touché la cible
        if(!hasTouchedTarget){
            // On fait les calculs pour le rayon de lumière
            Vector3 originalPointShoot = new Vector3(shootPoint.position.x, shootPoint.position.y, 0f);
            Vector3 direction = transform.right;
            Ray2D ray2D = new Ray2D(originalPointShoot, direction);
            Debug.DrawRay(ray2D.origin, ray2D.direction * maxStepDistance, Color.red, 1f);
            lineRenderer.positionCount = 1;
            // On démarre la méthode récursive pour tirer le rayon de lumière
            ShootRay(ray2D.origin, ray2D.direction, 0);
        }
    }

    // Méthode récursive
    // startPosition = Vecteur d'où la lumière part (émetteur ou miroir)
    // direction = la direction que prend la lumière
    // depth = le numéro du reflet actuel
    void ShootRay(Vector3 startPosition, Vector3 direction, int depth){
        // Si la lumière a été réfléchie trop de fois, on s'arrête
        if(depth > maxDepth)
            return;
        // On calcule le raycast de la lumière
        Ray2D nextRaycast = new Ray2D(startPosition, direction);

        // On regarde si on a touché quelque chose
        RaycastHit2D hit = Physics2D.Raycast(nextRaycast.origin, nextRaycast.direction, maxStepDistance, ~(mirrorLayerMask));
        // Si on a touché quelque chose
        if (hit)
        {
            // On calcule le vecteur pour le prochain reflet
            Vector3 nextDirection = Vector3.Reflect(direction, hit.normal);
            // On met à jour le lineRenderer
            lineRenderer.positionCount += 1;
            lineRenderer.SetPosition(depth, startPosition);
            if(depth >= 1){
                lineRenderer.SetPosition(depth+1, hit.point - ((Vector2)nextDirection * 0.02f));
            } else {
                lineRenderer.SetPosition(depth+1, hit.point);
            }
            // Si ce que la lumière touche est la cible
            if (hit.collider.transform.gameObject.Equals(target))
            {   
                // On met à jour les variables et on ouvre la porte
                target.GetComponent<SpriteRenderer>().sprite = targetOn;
                StartCoroutine(OpenDoor());
                hasTouchedTarget = true;
            }
            // Si c'est un miroir, on appelle récursivement la méthode avec le prochain reflet
            else if (hit.collider.CompareTag("Mirror"))
            { 
                ShootRay(hit.point + ((Vector2)nextDirection * 0.02f), nextDirection, depth + 1);
            }
        }
    }

    // Méthode pour ouvrir la porte
    private IEnumerator OpenDoor(){
        // On supprime le texte d'interaction
        interaction.EraseText();
        // On freeze les positions du joueur
        PlayerMovement.instance.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
        yield return new WaitForSecondsRealtime(.5f);
        PlayerMovement.instance.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePosition | RigidbodyConstraints2D.FreezeRotation;
        yield return new WaitForSecondsRealtime(.5f);
        // On focus la caméra sur la porte
        cinemachineVirtualCamera.Follow = doorToOpen.transform;
        cinemachineVirtualCamera.LookAt = doorToOpen.transform;
        yield return new WaitForSecondsRealtime(1.5f);
        // On ouvre la porte
        doorToOpen.GetComponent<Door>().OpenDoor();
        yield return new WaitForSecondsRealtime(3f);

        // On reset les contraintes du joueur et on refocus la caméra sur le joueur 
        PlayerMovement.instance.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
        yield return new WaitForSecondsRealtime(.5f);
        cinemachineVirtualCamera.Follow = PlayerMovement.instance.gameObject.transform;
        cinemachineVirtualCamera.LookAt = PlayerMovement.instance.gameObject.transform;
    }

    // Getter pour hasTouchedTarget;
    public bool HasOpenDoor(){
        return hasTouchedTarget;
    }
}
