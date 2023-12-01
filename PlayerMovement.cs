using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    // Variable pour l'offset de la particule de mouvement
    private float heightOffsetParticle;
    // Particules pour le mouvement 
    [SerializeField]
    private ParticleSystem sprintParticleLeft;
    [SerializeField]
    private ParticleSystem sprintParticleRight;
    // Particule de sprint actuellement jouée
    private ParticleSystem sprintParticle;
    // Singleton
    public static PlayerMovement instance;
    // Booléen pour savoir si le joueur est en gravité inversée ou non
    public bool gravityReversed { get; private set; }
    // LayerMask des plateformes
    public LayerMask platformLayerMask;
    // SpriteRenderer du joueur
    public SpriteRenderer sprite;
    // Booléen indiquant si le joueur regarde à gauche ou à droite
    public bool isWatchingLeft;
    // Vitesse de marche du joueur
    public float movementSpeed;
    // Vitesse de course du joueur
    public float movementSprintSpeed;
    // Vitesse actuelle du joueur
    public float currentSpeed { get; set; }
    // Référence au vecteur nul pour le mouvement
    private Vector3 basicMovement = Vector3.zero;
    // Booléen indiquant si le joueur est au sol ou non
    public bool isGrounded;
    // Nombre de saut actuels réalisés par le joueur
    public int jumpCount = 0;
    // Nombre de saut maximum possible
    public int extraJumps = 1;
    // Force de saut du joueur
    public float forceJump;
    // Booléen indiquant si le joueur a touché le côté d'une plateforme
    public bool hasTouchSide;
    // Booléen indiquant si le joueur est en train de monter une échelle
    public bool isClimbing;
    // Variables de mouvement sur l'axe X et Y du joueur
    private float movementX;
    public float movementY;

    // Référence au rigidbody du joueur
    private Rigidbody2D rb;

    // Référence à l'animator du joueur
    public Animator movementAnimation;
    // Référence au sprite par défaut du joueur
    public Sprite defaultSprite;

    // Référence à l'objet Graphics du joueur
    public GameObject playerGraphics;
    // Booléen pour savoir si le joueur est en haut d'une échelle
    [SerializeField]
    public bool isOnTopLadder;
    // Source audio pour la marche du joueur
    [SerializeField]
    private AudioSource walkAudioSource;
    private void Awake()
    {
        // Unicité du script
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        // Initialisation des variables et mise en place du UpdateVFX pour faire en sorte que les particules soient
        // activées ou désactivés selon les préférences du joueur dans les paramètres
        hasTouchSide = false;
        InvokeRepeating("UpdateVFX", 0f, 0.5f);
    }

    private void UpdateVFX(){
        sprintParticleLeft.gameObject.SetActive(SettingsJSON.instance.settings.videoSettings.isVFXToggled);
        sprintParticleRight.gameObject.SetActive(SettingsJSON.instance.settings.videoSettings.isVFXToggled);
    }


    private void Start()
    {
        // On initialise les variables
        rb = GetComponent<Rigidbody2D>();
        isClimbing = false;
        isWatchingLeft = false;
        isOnTopLadder = false;

        sprintParticle = null;
        heightOffsetParticle = .9f;       
    }

    // On récupère à chaque frame les input verticaux et horizontaux du joueur
    private void FixedUpdate()
    {
        movementX = Input.GetAxis("Horizontal") * currentSpeed * Time.deltaTime;
        movementY = Input.GetAxis("Vertical") * currentSpeed * Time.deltaTime;
        movePlayer(movementX, movementY);
    }


    private void Update()
    {
        // Si le joueur est au dessus d'une échelle, on arrête le son "Ladder"
        if(isOnTopLadder){
            AudioManager.instance.Stop("Ladder");
        }
        // Si on appuie sur la touche pour courir
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.LeftShift))
        {
            // On modifie la vitesse de déplacement du joueur
            currentSpeed = movementSprintSpeed;
        }
        else
        {
            // Si on n'appuie pas sur la touche, on reset la vitesse
            currentSpeed = movementSpeed;
        }

        // Si le joueur est en haut d'une échelle ou qu'il n'est pas en train de grimper sur une échelle
        if (isOnTopLadder || !isClimbing)
        {
            // Et qu'il appuie sur espace, on le fait sauter
            if(Input.GetKeyDown(KeyCode.Space))
                Jump();

        }

        // On met à jour la variable isWatchingLeft et son rendu en fonction de la direction dans laquelle le joueur
        // se déplace
        if (movementX > 0)
        {
            sprite.flipX = false;
            isWatchingLeft = false;
        }
        else if (movementX < 0)
        {
            sprite.flipX = true;
            isWatchingLeft = true;
        }
        // On vérifie si le joueur est sur le sol
        CheckGrounded();
    }

    // Méthode pour sauter
    void Jump()
    {
        // Si le joueur est sur le sol ou qu'il a encore des sauts de disponible
        if (isGrounded || (jumpCount < extraJumps && !hasTouchSide))
        {
            // On fait sauter le joueur
            rb.velocity = new Vector2(rb.velocity.x, forceJump);
            jumpCount++;
        }
    }

    // Méthode pour vérifier si le joueur est sur le sol
    void CheckGrounded()
    {
        // Si le joueur est sur le sol
        if (IsGrounded())
        {
            // Et qu'il vient juste d'atterir
            if(isGrounded == false)
                // On joue le son PlayerLanding
                AudioManager.instance.Play("PlayerLanding");
            // et on met à jour les variables
            isGrounded = true;
            jumpCount = 0;
        }
        else
        {
            // Sinon, on met isGrouded à false
            isGrounded = false;
        }
        // On change l'état de l'animation de saut en fonction de là où est le joueur 
        // (sur le sol ou non)
        movementAnimation.SetBool("Jump", !isClimbing && !isGrounded);
    }

    // Si le joueur rentre en contact avec un objet
    public void OnCollisionEnter2D(Collision2D collision)
    {
        foreach (ContactPoint2D contact in collision.contacts)
        {
            // Si le contact provient du côté
            if (contact.normal.y == 0)
            {
                if (contact.normal.x == 1 || contact.normal.x == -1)
                {
                    // alors on fait en sorte qu'il ne puisse plus sauter
                    jumpCount = 4;
                    hasTouchSide = true;
                }
            }
            // Si le joueur touche le sol depuis le haut et qu'il a sa gravité normale
            else if (contact.normal.y == -1 && !gravityReversed)
            {
                // On lui met à jour sa variable
                hasTouchSide = true;
            // Si le joueur touche le sol depuis le bas et qu'il a sa gravité inversée
            } else if(contact.normal.y == 1 && gravityReversed){
                // On lui met à jour sa variable
                hasTouchSide = true;
            }
            else{
                // Sinon on lui reset sa variable à false
                hasTouchSide = false;
            } 

        }
    }

    // Si le joueur rentre en contact avec un objet de tag "OutLadder"
    public void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("OutLadder"))
        {
            // On met à jour sa variable isOnTopLadder
            isOnTopLadder = true;
        }
    }

    // Si le joueur sort de la boîte de collision trigger "OutLadder"
    public void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.CompareTag("OutLadder"))
        {
            // On met à jour sa variable isOnTopLadder
            isOnTopLadder = false;
        }
    }

    // méthode pour savoir si le joueur est sur le sol
    public bool IsGrounded()
    {
        // On récupère la boîte de collision du joueur
        CapsuleCollider2D box = GetComponent<CapsuleCollider2D>();
        // On regarde si le joueur touche le sol en fonction de si la gravité est inversée ou non
        if (!gravityReversed)
        {
            RaycastHit2D raycastHit = Physics2D.Raycast(box.bounds.center, new Vector3(0f, -box.bounds.size.y, 0f), 1f, platformLayerMask);
            return raycastHit.collider != null;
        }
        else
        {
            RaycastHit2D raycastHit = Physics2D.Raycast(box.bounds.center, new Vector3(0f, box.bounds.size.y, 0f), 1f, platformLayerMask);
            return raycastHit.collider != null;
        }

    }

    // Méthode pour inverser la gravité
    public void SwapGravity(bool isReverseSwapper){
        // Si la gravité est déjà dans son état initiale, on la réinverse pas
        if(isReverseSwapper == gravityReversed){
            // On inverse toutes les variables
            GetComponent<Rigidbody2D>().gravityScale *= -1; 
            forceJump *= -1;
            gravityReversed = !gravityReversed;
            sprite.flipY = !sprite.flipY;
            Vector3 positionParticleLeft = sprintParticleLeft.transform.position;
            Vector3 positionParticleRight = sprintParticleRight.transform.position;
            // On met à jour les positions des particules de course
            if(gravityReversed){
                sprintParticleLeft.gameObject.transform.position = new Vector3(positionParticleLeft.x, transform.position.y + heightOffsetParticle, 0f);
                sprintParticleRight.gameObject.transform.position = new Vector3(positionParticleRight.x, transform.position.y + heightOffsetParticle, 0f);
            } else {
                sprintParticleLeft.gameObject.transform.position = new Vector3(positionParticleLeft.x, transform.position.y - heightOffsetParticle, 0f);
                sprintParticleRight.gameObject.transform.position = new Vector3(positionParticleRight.x, transform.position.y - heightOffsetParticle, 0f);
            }
        }
    }

    // Méthode pour faire bouger le joueur
    private void movePlayer(float mouvementX, float mouvementY)
    {
        // On récupère sa vélocité en x
        float characterVelocity = Mathf.Abs(rb.velocity.x);
        
        // Si la vélocité en x est supérieure à 0.05
        if(characterVelocity > 0.05f)
        {
            // On joue le son de marche si il n'est pas déjà en train d'être joué
            if(!walkAudioSource.isPlaying)
            {
                walkAudioSource.Play();
            }
        } else {
            // Sinon, on arrête le son
            walkAudioSource.Stop();
        }
        // On update l'animator par rapport à la vélocité du joueur
        movementAnimation.SetFloat("Speed", characterVelocity);
        SwapParticlePosition(mouvementX);
        // Si le joueur est sur le sol et sa vitesse est supérieure à 0.5, on lui active sa particule de course
        if(isGrounded && Mathf.Abs(mouvementX) > 0.5f){ 
            if(sprintParticle != null){
                if(!sprintParticle.isPlaying)
                    sprintParticle.Play();
            }
        }
        // Mouvement du joueur en modifiant sa vélocité
        Vector3 velocity = new Vector2(mouvementX, rb.velocity.y);
        rb.velocity = Vector3.SmoothDamp(rb.velocity, velocity, ref basicMovement, .05f);
    }

    // Méthode pour connaître quelle particule de course il faut jouer
    private void SwapParticlePosition(float mouvementX)
    {
        // On change la variable sprintParticle en fonction de la valeur mouvementX
        if(mouvementX > 0.5f){
            sprintParticle = sprintParticleLeft;
        } else if(mouvementX < -0.5f){
            sprintParticle = sprintParticleRight;
        } else {
            sprintParticle = null;
        }
    }
}

