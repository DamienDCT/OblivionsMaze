using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class BossWorld2 : MonoBehaviour
{
    // Référence au transform du centre de la salle
    [SerializeField]
    private Transform centerActionZone;
    // Référence à la taille de la zone d'action
    [SerializeField]
    private Vector2 actionZoneSize;
    // Booléen indiquant si le joueur est dans la zone
    private bool isPlayerInZone;
    // Référence à la porte pour fermer la salle de boss
    [SerializeField]
    private Door doorLeft;
    // Booléen indiquant si le boss est activé
    [SerializeField]
    private bool isBossActivated;
    // LayerMask du joueur
    [SerializeField]
    private LayerMask playerLayerMask;
    // Référence au joueur
    private GameObject player;

    [Header("Camera Room 1 transition")]
    // Position là où doit arriver la caméra à la fin de la transition
    public Transform positionCameraCentered;
    // Référence à la caméra de la salle
    [SerializeField]
    private Camera cameraRoom1;
    // Vitesse de transition de la caméra
    [SerializeField]
    private float smoothTransitionSpeed;
    // Booléen indiquant si la caméra est en transition ou non
    private bool transitionCamera;
    // Booléen indiquant si le boss est en seconde phase ou non
    private bool isSecondPhase;
    // Booléen indiquant si le boss peut sauter ou non
    private bool canJump;
    // Booléen indiquant si le boss est sur le sol
    private bool isOnGround;
    // Référence au rigidbody2D du boss
    private Rigidbody2D rb2D;
    // Booléen indiquant si le joueur est sur le boss
    private bool playerOnBoss;

    [Header("Health part")]
    // Nombre de points de vie du boss
    private int health;
    // Nombre max de points de vie du boss
    [SerializeField]
    private int maxHealth;
    // Référence au slider du boss
    [SerializeField]
    private Slider healthSlider;

    [Header("Switch Partie")]
    // Référence à l'animator pour la transition en phase 2
    [SerializeField]
    private Animator fadeSystem;
    // Références aux tilemap du niveau
    [SerializeField]
    private Tilemap[] tilemaps;
    // Référence à la couleur des tilemap pour la seconde phase
    [SerializeField]
    private Color colorSecondPhase;
    // Référence à la couleur de la barre de vie pour la phase 2
    [SerializeField]
    private Color healthBarColorSP;
    // référence à l'image du slider
    [SerializeField]
    private Image rectFill;
    // Référence au renderer du background
    [SerializeField]
    private SpriteRenderer imageBackground;
    // Sprite du background de la seconde phase
    [SerializeField]
    private Sprite imageBackgroundSecondPhase;
    // Animator de karak
    [SerializeField]
    private Animator switchAnimator;
    // Particule d'atterrissage de karak
    [SerializeField]
    private ParticleSystem landParticle;
    // Référence au renderer de karak
    [SerializeField]
    private SpriteRenderer graphics;
    // GameObject de la partie graphique de karak
    private GameObject graphicsTransform;
    // référence au script CameraShake de la caméra
    private CameraShake cameraShake;
    // Tous les sprites de karak
    [SerializeField]
    private Sprite leftSprite1stphase;
    [SerializeField]
    private Sprite rightSprite1stphase;
    [SerializeField]
    private Sprite defaultSpritePhase1;
    [SerializeField]
    private Sprite defaultSpritePhase2;
    // Booléen indiquant si le boss est en transition pour la phase 2
    private bool isPhase2InTransition;

    [Header("BoxCollider karak")]
    // BoxCollider2D pour les différentes phases
    [SerializeField]
    private BoxCollider2D firstPhaseBoxCollider;
    [SerializeField] 
    private BoxCollider2D secondPhaseBoxCollider;
    // Etat actuel actif de l'animator de karak
    private string currentState;
    // Booléen indiquant si karak peut bouger
    private bool canMove;
    // Booléen indiquant si karak est stun
    private bool isStun;
    // Coroutine pour le stun de karak
    private Coroutine stunCoroutine;
    // Prefab de la pièce que karak drop à la fin
    [SerializeField]
    private GameObject piecePrefab;


    private void Start(){
        // Initialisation des variables
        canMove = true;
        isPhase2InTransition = false;
        isStun = false;
        cameraShake = cameraRoom1.GetComponent<CameraShake>();
        graphicsTransform = graphics.gameObject;
        Debug.Log(graphicsTransform.name);
        playerOnBoss = false;
        isOnGround = true;
        canJump = true;
        rb2D = GetComponent<Rigidbody2D>();
        isSecondPhase = false;
        isBossActivated = false;
        player = PlayerMovement.instance.gameObject;
        isPlayerInZone = false;
        transitionCamera = false;
        health = maxHealth;
    }

    // On check si le joueur rentre dans la zone
    private void CheckPlayer(){
        RaycastHit2D hit = Physics2D.BoxCast(centerActionZone.position, actionZoneSize, 0f, new Vector2(0f, 0f), 0f, playerLayerMask);
        if(hit.collider != null)
            // Si le joueur rentre dans la zone, on ferme la porte
            if(!isPlayerInZone && player.transform.position.x >= (centerActionZone.position.x - actionZoneSize.x))
            {
                isPlayerInZone = true;
                StartCoroutine(CloseDoor());
            } 
    }

    // méthode de coroutine pour démarrer la transition
    private IEnumerator CloseDoor(){
        // On ferme la porte
        doorLeft.Switch();
        player.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
        yield return new WaitForSecondsRealtime(.5f);
        player.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
        // On démarre la transition de la caméra
        transitionCamera = true;
        yield return new WaitForSecondsRealtime(3f);       
        // On active le boss 3s après
        player.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
        isBossActivated = true;
        
        // On active les tireurs de boule de feu
        FireballThrower[] fireballThrowers = FindObjectsOfType<FireballThrower>();
        foreach(FireballThrower fireballThrower in fireballThrowers)
        {
            fireballThrower.StartLaunch();
        }
    }

    // A chaque frame, on vérifie où est le joueur, et on déplace Karak
    private void Update(){
        CheckPlayer();
        Move();
        // Si on est en transition de camera, on déplace la camera vers son point central jusqu'à ce qu'elle le touche
        if(transitionCamera)
        {
            Vector3 distance = positionCameraCentered.position - cameraRoom1.gameObject.transform.position;
            cameraRoom1.gameObject.transform.Translate(distance.normalized * smoothTransitionSpeed * Time.deltaTime, Space.World);
            if(Vector3.Distance(positionCameraCentered.position, cameraRoom1.gameObject.transform.position) < .5f){
                transitionCamera = false;
            }
        }
    }
    
    private void Move(){
        // Si le boss ne peut pas bouger, ou qu'on est en transition de phase 2, on ne fait rien
        if(!canMove)
            return;
        if(isPhase2InTransition)
            return;
        // Si le boss est en première phase
        if(!isSecondPhase){
            // Si le boss est activé
            if(isBossActivated){
                // Si le boss est sur le sol
                if(isOnGround){
                    // Probabilité de saut
                    if(Random.Range(0f, 1f) < 0.0005f){
                        // Si le boss peut sauter et le joueur n'est pas sur sa tête
                        if(canJump && !playerOnBoss){
                            // On le fait sauter vers le joueur
                            isOnGround = false;
                            canJump = false;
                            if(player.transform.position.x < transform.position.x){
                                rb2D.AddForce(new Vector2(-4f * rb2D.mass, 10f * rb2D.mass), ForceMode2D.Impulse);
                            } else {
                                rb2D.AddForce(new Vector2(4f * rb2D.mass, 10f * rb2D.mass), ForceMode2D.Impulse);
                            }
                            ChangeAnimationState("Jump1stphase");
                        }
                    // Sinon si le joueur n'est pas sur le boss
                    } else {
                        if(!playerOnBoss){
                            // On fait bouger le boss vers le joueur en changeant son animation
                            Vector3 direction = player.transform.position - transform.position;
                            if(direction.x < 0)
                            {
                                ChangeAnimationState("movingLeft1stphase");
                            } else {
                                ChangeAnimationState("movingRight1stphase");
                            }
                            direction.y = 0f;
                            transform.Translate(direction.normalized * 0.45f * Time.deltaTime, Space.World);
                        } else {
                            ChangeAnimationState("Jump1stphase");
                        }
                    }
                }
            }
        } else {
            // Si on est en phase 2, et que le boss est activé, et au sol
            if(isBossActivated){
                if(isOnGround){
                    // Si le boss a la probabilité de sauter
                    if(Random.Range(0f, 1f) < 0.001f){
                        // Si le boss peut sauter et que le joueur n'est pas sur le boss
                        if(canJump && !playerOnBoss){
                            // On fait sauter le boss vers le joueur
                            isOnGround = false;
                            canJump = false;
                            if(player.transform.position.x < transform.position.x){
                                rb2D.AddForce(new Vector2(-4f * rb2D.mass, 10f * rb2D.mass), ForceMode2D.Impulse);
                            } else {
                                rb2D.AddForce(new Vector2(4f * rb2D.mass, 10f * rb2D.mass), ForceMode2D.Impulse);
                            }
                            ChangeAnimationState("Jump2ndphase");
                        }
                    } else {
                        // Si le boss n'a pas sauté, et que le joueur n'est pas sur sa tête, on déplace le boss vers le joueur
                        if(!playerOnBoss){
                            Vector3 direction = player.transform.position - transform.position;
                            if(direction.x < 0)
                            {
                                ChangeAnimationState("movingLeft2ndphase");
                            } else {
                                ChangeAnimationState("movingRight2ndphase");
                            }
                            direction.y = 0f;
                            transform.Translate(direction.normalized * 0.9f * Time.deltaTime, Space.World);
                        } else {
                            ChangeAnimationState("Jump2ndphase");
                        }
                    }
                }
            }
        }
    }

    // Si le boss touche le sol
    public void OnCollisionEnter2D(Collision2D collision2D){
        if(collision2D.collider.CompareTag("Ground")){
            // Si le boss est stun, il fait une onde de choc au sol
            if(isStun)
            {
                ShockWave();
                return;
            }
            // Si le boss n'est pas stun, mais qu'il est en transition pour la phase 2, on termine la phase de sortie de ses pics
            if(isPhase2InTransition)
            {
                EndExitPiques();
                return;
            }
            // On lui fait retrouver sa capacité de saut au bout de 3 secondes
            Invoke("ResetJump", 3f);
            // On fait la bonne animation d'atterrissage en fonction de la phase du boss
            if(!isSecondPhase){
                ChangeAnimationState("Landing");
            } else {
                ChangeAnimationState("Landing2ndphase");
            }
            // Si la particule d'atterrissage n'est pas en train d'être joué, on la joue
            if(!landParticle.isPlaying)
                landParticle.Play();
            // On fait une onde de choc
            ShockWave();
        // Si le boss touche le joueur
        } else if(collision2D.collider.CompareTag("Player")){
            ContactPoint2D contactPoint2D = collision2D.GetContact(0);
            // Si le contact a lieu avec le joueur sous le boss
            if(contactPoint2D.normal.y == 1){
                // On fait propulser le joueur vers le côté opposé de là où il est
                float forceMagnitude = 9.81f*7.5f;
                Vector3 playerPos = PlayerMovement.instance.gameObject.transform.position;
                if(playerPos.x > positionCameraCentered.position.x){
                    PlayerMovement.instance.gameObject.GetComponent<Rigidbody2D>().velocity = (Vector2.left * forceMagnitude * 4 + Vector2.up * (forceMagnitude / 4f));
                } else {
                    PlayerMovement.instance.gameObject.GetComponent<Rigidbody2D>().velocity = (Vector2.right * forceMagnitude * 4 + Vector2.up * (forceMagnitude / 4f));
                }
                // et on fait prendre des dégâts au joueur
                PlayerHealth.instance.TakeDamage(10);
            // Si le joueur atterrit sur la tête du boss
            } else if(contactPoint2D.normal.y == -1){
                // Si le boss est en seconde phase
                if(isSecondPhase){
                    // Et que le boss n'est pas stun, le joueur prend des dégâts
                    if(!isStun){
                        PlayerHealth.instance.TakeDamage(10);
                    // Sinon on met à jour la variable playerOnBoss
                    } else {
                        playerOnBoss = true;
                    }
                // Si le boss est en première phase, on met directement à jour la variablel playerOnBoss
                } else {
                    playerOnBoss = true;
                }
            // Si le contact se fait sur le côté du boss, on empoisonne le joueur
            } else if(contactPoint2D.normal.x == 1 || contactPoint2D.normal.x == -1)
            {
                PlayerHealth.instance.Poison();
            }
        }
    }

    // Si le joueur sort de sa collision avec le boss
    public void OnCollisionExit2D(Collision2D collision2D){
        if(collision2D.collider.CompareTag("Player")){
            // On remet la variable playerOnBoss à false
            playerOnBoss = false;
        }
    }

    //méthode appelée quand le boss atterri 
    private void ShockWave(){
        //si le boss est actif
        if(isBossActivated){
            //on fait une secousse de caméra, on active les particules d'atterrissage 
            cameraShake.StartCoroutine(cameraShake.NonCinemachineShake(1f));
            landParticle.Play();
            //si le joueur se trouve sur le sol au moment de l'impact
            if(player.GetComponent<PlayerMovement>().IsGrounded()){
                //on fait sauter le joueur et prendre des dégats
                float speedX = player.GetComponent<Rigidbody2D>().velocity.x;
                player.GetComponent<Rigidbody2D>().velocity = new Vector2(speedX, 25f);
                player.GetComponent<PlayerHealth>().TakeDamage(10);
            }
        }
    }

    //méthode pour quand on utilise la foreuse
    public int Hit(){
        //si on est pas en seconde phase
        if(!isSecondPhase){
            //et que le joueur se trouve sur le boss
            if(playerOnBoss){
                //on enlève de la vie au boss et on modifie sa barre de vie en conséquence
                health -= 10;
                healthSlider.value = health;
                //on fait un effet de secousse de caméra
                cameraShake.StartCoroutine(cameraShake.NonCinemachineShake(1f));
                if(health <= (maxHealth/2)){
                    //si la vie du boss passe en dessous de la moitié de sa vie, on passe en seconde phase
                    Invoke("SwitchSecondPhase", 0.5f);
                    Invoke("JumpBoss", 1.5f);
                }
                //on retourne true car on a touché le boss
                return 1;
            //et que le joueur ne se trouve pas sur le boss
            } else {
                //on retourne false car on n'a pas touché le boss
                return 0;
            }
        //sinon si on est en seconde phase
        } else {
            //si le boss n'est pas étourdi
            if(!isStun)
            {
                //on vérifie la distance entre le joueur et le boss
                float distance = Mathf.Abs(player.transform.position.x - transform.position.x);
                //si elle est < 15f et que le joueur est sur le sol
                if(distance < 15f && isOnGround){
                    //on fait une secousse de caméra et on étourdi le boss
                    cameraShake.StartCoroutine(cameraShake.NonCinemachineShake(1f));
                    FlipKarak();
                    return 2;
                }
                return 0;
            //si le boss est étourdi et que le joueur est sur le boss
            } else if(isStun && playerOnBoss) {
                //on enlève de la vie au boss et on modifie sa barre de vie en conséquence
                health -= 10;
                healthSlider.value = health; 
                //si sa vie passe à 0 ou en dessous, on fait mourir le boss   
                if(health <= 0){
                    Death();
                    return 1;
                //sinon on fait une secousse de caméra
                } else {

                    cameraShake.StartCoroutine(cameraShake.NonCinemachineShake(1f));
                    //et si le boss est étourdi on lui enlève l'étourdissement
                    if(stunCoroutine != null)
                        StopCoroutine(stunCoroutine);
                    stunCoroutine = null;
                    StartCoroutine(StunDestun(true));
                }
                return 1;
            }
            return 0;
        }
    }

    //méthode pour la mort du boss
    private void Death(){
        //il fait tomber sa pièce
        GameObject go = Instantiate(piecePrefab);
        if(go != null)
            go.transform.position = transform.position;
        //puis on détruit le boss
        Destroy(gameObject);
    }

    //méthode pour retourner le boss quand on le stun
    private void FlipKarak(){
        //on indique que le boss est stun et qu'il ne peut plus bouger
        isStun = true;
        canMove = false;
        StartCoroutine(StunDestun(false));
    }

    //on enlève l'étourdissement du boss au bout d'un temps donné
    private IEnumerator DestunByTime(){
        yield return new WaitForSecondsRealtime(3f);
        StartCoroutine(StunDestun(true));
    }

    //Coroutine pour Stun ou Destun le boss
    private IEnumerator StunDestun(bool destun){
        //on lui fait faire un petit saut
        GetComponent<Rigidbody2D>().velocity = Vector2.up * 5f;
        yield return new WaitForSecondsRealtime(0.5f);
        if(destun){
            //animation pour retourner à l'endroit
            ChangeAnimationState("destunKarak"); 
        } else {
            //animation pour retourner à l'envers
            ChangeAnimationState("stunKarak");
            //on appelle la coroutine pour le destun au bout d'un certain temps
            stunCoroutine = StartCoroutine(DestunByTime());
        }
    }

    //à la fin du Destun quand le boss est retourné, on met qu'il n'est plus étourdi et qu'il peut bouger
    public void EndDestun(){
        isStun = false;
        canMove = true;
    }

    //On fait un effet de fade entre la première et la deuxième phase
    private void SwitchSecondPhase(){
        fadeSystem.SetTrigger("fade");
    }

    //méthode pour modifier le thème de la salle du boss lors du passage à la 2e phase
    public void SetSecondPhaseTheme(){
        //on modifie la couleur de la barre de vie du boss
        rectFill.color = healthBarColorSP;
        //on modifie l'image du background
        imageBackground.sprite = imageBackgroundSecondPhase;
        //on modifie la couleur de la tilemap
        foreach(Tilemap tilemap in tilemaps){
            tilemap.color = colorSecondPhase;
        }
    }

    //méthode pour faire le saut du boss pendant la transition
    public void JumpBoss(){
        AnimationCurve curve = AnimationCurve.Constant(0, 1, transform.position.x);
        isPhase2InTransition = true;
        StartCoroutine(SecondPhase());
    }

    //Coroutine pour passer à la seconde phase
    private IEnumerator SecondPhase(){
        //on donne de la vitesse au boss vers le haut
        GetComponent<Rigidbody2D>().velocity = Vector2.up * 15f;
        yield return new WaitForSecondsRealtime(0.5f);
        switchAnimator.Play("SwitchPhase");
    }

    //méthode pour jouer l'animation après le saut de la transition à la 2e phase
    public void EndExitPiques(){
        ChangeAnimationState("Jump2ndphase");
        isSecondPhase = true;
        firstPhaseBoxCollider.enabled = false;
        secondPhaseBoxCollider.enabled = true;
        isPhase2InTransition = false;
    }

    //méthode pour faire que le boss puisse sauter
    private void ResetJump(){
        canJump = true;
    }

    //méthode pour changer la position des Graphics du boss
    public void SetGraphicsPosition(float value){
        graphicsTransform.transform.localPosition = new Vector3(graphicsTransform.transform.localPosition.x, value, graphicsTransform.transform.localPosition.z);
    }

    //On jour l'animation ExitPiques, qui fait sortir les pics du boss
    public void ExitPiques(){
        ChangeAnimationState("ExitPiques");
    }

    //méthode qu'on appelle à la fin de l'atterrissage
    public void EndLand(){
        if(!isPhase2InTransition){
            ChangeAnimationState("emptyState");
            //le boss est sur le sol
            isOnGround = true;
        }
    }

    //on change l'animation du boss
    private void ChangeAnimationState(string newState){
        if(currentState == newState) return;

        switchAnimator.Play(newState);
        currentState = newState;
    }
}
