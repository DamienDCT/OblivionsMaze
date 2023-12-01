using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Canon : MonoBehaviour
{
    [Header("Rotation variables")]
    // Référence à toutes les cibles que le canon peut atteindre
    [SerializeField]
    private Transform[] angleRotationArray;
    // Référence à l'indice de la cible que vise le canon
    private int indexRotation;
    // Référence à la cible que vise le canon
    [SerializeField]
    private Transform pointTP;
    // Booléen servant à dire si le joueur est dans le canon
    private bool isPlayerInCanon;
    // Référence à la zone d'interaction
    [SerializeField]
    private Vector2 zoneTarget;
    // Booléen servant à dire si le joueur est dans la zone
    private bool isPlayerOnZone;
    // Référence au Layer du joueur
    [SerializeField]
    private LayerMask playerLayerMask;
    // Référence au joueur
    private GameObject player;
    // Référence à la puissance du canon
    [SerializeField]
    private float powerCanon;
    // Référence à la coroutine de tir
    private Coroutine shootCoroutine;
    // Si le booléen est vrai, cela indique que la caméra est en transition sur la nouvelle cible
    private bool isInCinematic;
    // Référence à la caméra pour la transition de caméra
    private CinemachineVirtualCamera cinemachineVirtualCamera;

    [SerializeField]
    private Interaction interactionBeforeIn;
    [SerializeField]
    private Interaction interactionIn;

    [SerializeField]
    private ParticleSystem smokeParticle;

    private void Start(){
        // On setup les variables
        cinemachineVirtualCamera = GameObject.FindGameObjectWithTag("VirtualCamera").GetComponent<CinemachineVirtualCamera>();
        player = PlayerMovement.instance.gameObject;
        indexRotation = 0;
        // On détermine la direction vers la cible
        Vector3 direction = angleRotationArray[indexRotation].position - transform.position;
        // On détermine l'angle entre la direction et l'axe Y
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        // On fait la rotation à partir de l'angle calculé
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle-45));
    }

    // Méthode appelée à chaque frame pour savoir si le joueur est dans la zone du canon
    private void CheckPlayer(){
        RaycastHit2D hit = Physics2D.BoxCast((Vector2)transform.position, zoneTarget, 0f, new Vector2(0f, 0f), 0f, playerLayerMask);
        if(hit.collider != null)
        {
            isPlayerOnZone = true;
            if(!isPlayerInCanon)
                interactionBeforeIn.PutText();
        }
        else {
            if(isPlayerOnZone)
                interactionBeforeIn.EraseText();
            isPlayerOnZone = false;
        }
    }

    private void Update(){
        // On regarde si le joueur est dans la zone
        CheckPlayer();
        // Si le joueur est dans le canon
        if(isPlayerInCanon)
        {
            // Et que le joueur n'a pas tiré
            if(!isInCinematic){
                // Si le joueur appuie sur E, on change la rotation du canon
                if(Input.GetKeyDown(KeyCode.E)){
                    isInCinematic = true;
                    StartCoroutine(ChangeRotation());
                // A l'inverse, s'il appuie sur F, on fait tirer le canon
                } else if(Input.GetKeyDown(KeyCode.F)){
                    ShootCanon();
                }
                return;
            }
        }
        // Si le joueur est dans la zone et qu'il n'est pas dans le canon
        if(isPlayerOnZone)
        {
            // Si le joueur appuie sur E, on fait rentrer le joueur dans le canon
            if(Input.GetKeyDown(KeyCode.E)){
                isPlayerInCanon = true;
                PutPlayerInCanon();
                return;
            }
        }
    }

    // Méthode appelée pour faire rentrer le joueur dans le canon
    private void PutPlayerInCanon(){
        // On met à jour le gameObject du joueur pour lui freeze toutes ses positions et le mettre dans le canon
        player.transform.SetParent(gameObject.transform,true);
        player.transform.GetComponentInChildren<SpriteRenderer>().enabled = false;
        player.transform.position = pointTP.position;
        player.transform.rotation = transform.rotation;
        player.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
        gameObject.tag = "Canon";
        interactionBeforeIn.EraseText();
        interactionIn.PutText();
    }
    
    // Méthode appelée pour changer la rotation du canon pour le tourner vers une cible
    private IEnumerator ChangeRotation(){
        // On change l'indice dans le tableau de cibles
        indexRotation = (indexRotation + 1) % angleRotationArray.Length;
        // On détermine la direction vers la cible
        Vector3 direction = angleRotationArray[indexRotation].position - transform.position;
        // On calcule l'angle entre la direction et l'axe Y
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        // On fait la rotation du canon sur l'angle calculé
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle-45));
        
        // On fait regarder la caméra vers la nouvelle cible
        cinemachineVirtualCamera.Follow = angleRotationArray[indexRotation].transform;
        cinemachineVirtualCamera.LookAt = angleRotationArray[indexRotation].transform;
        yield return new WaitForSecondsRealtime(1f);

        // Puis on fait regarder la caméra vers le joueur
        cinemachineVirtualCamera.Follow = player.transform;
        cinemachineVirtualCamera.LookAt = player.transform;
        yield return new WaitForSecondsRealtime(1f);
        // On reset la variable de cinématique
        isInCinematic = false;
    }

    // Méthode appelée à chaque tir de canon
    private void ShootCanon(){
        if(!smokeParticle.isPlaying)
            smokeParticle.Play();
        player.transform.GetComponentInChildren<SpriteRenderer>().enabled = true;
        // On calcule le vecteur de direction entre le joueur et la cible du canon
        Vector3 direction = angleRotationArray[indexRotation].position - transform.position;
        // On récupère la coroutine pour pouvoir l'arrêter        
        shootCoroutine = StartCoroutine(MovePlayerWithCanon());
        // On remet le joueur en enfant de son gameObject
        player.transform.parent = GameObject.FindGameObjectWithTag("ParentPlayer").transform;
        gameObject.tag = "Canon";
        // On freeze les positions du joueur et on reset sa rotation et sa taille 
        // (à cause du canon)
        player.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
        player.transform.rotation = Quaternion.identity;
        player.transform.localScale = new Vector3(1,1,1);
        AudioManager.instance.Play("CanonShoot");
        isPlayerInCanon = false;
    }

    private IEnumerator MovePlayerWithCanon(){
        // On calcule la distance à parcourir et on translate le joueur
        Vector3 direction = angleRotationArray[indexRotation].position - player.transform.position;
        player.transform.Translate(direction * powerCanon * Time.deltaTime, Space.World);
        // Tant que la distance entre le joueur et la cible est supérieure à 0.5
        while(Vector3.Distance(player.transform.position, angleRotationArray[indexRotation].position) > 0.5f)
        {
            // On recalcule la distance et on refait bouger le joueur
            direction = angleRotationArray[indexRotation].position - player.transform.position;
            player.transform.Translate(direction * powerCanon * Time.deltaTime, Space.World);
            yield return new WaitForSecondsRealtime(.02f);
        }
    }

    // Méthode appelée par les cibles pour arrêter la coroutine de tir
    public void StopShooting(){
        if(shootCoroutine != null)
        {
            StopCoroutine(shootCoroutine);
            shootCoroutine = null;
        }
    }
}
