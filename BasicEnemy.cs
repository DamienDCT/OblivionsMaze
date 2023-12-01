using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class BasicEnemy : Enemy
{
    // Référence à la particule d'atterrissage (uniquement pour GNE-O3X et GNE-O3X chaos)
    [Header("Particules")]
    [SerializeField]
    private ParticleSystem landGroundParticle;

    // Référence à si l'ennemi peut sauter aléatoirement (GNE-O3X et GNE-O3X chaos uniquement)
    [SerializeField]
    private bool canRandomJump;
    // Booléen pour indiquer si l'ennemi est au sol ou non
    [SerializeField]
    private bool isOnGround;
    // Vitesse maximale de l'ennemi
    [SerializeField]
    private float maxSpeed; 
    // Vitesse actuelle de l'ennemi
    private float speed;
    // Waypoints où l'ennemi doit se déplacer
    [SerializeField]
    private Transform[] waypoints;
    // Référence au waypoint vers lequel l'ennemi est en train de se déplacer
    private Transform target;
    // Référence à l'indice de ce waypoint
    private int destPoint = 0;
    // Référence au SpriteRenderer pour pouvoir le flipX
    private SpriteRenderer renderSprite;

    // Animation de saut pour GNE-O3X uniquement
    [SerializeField]
    private Animator animatorGneO3X;

    // Uniquement pour les versions chaos
    [Header("Chaos Version")]
    // Nombre de hit restant avant de tuer l'ennemi
    [SerializeField]
    private int nbHitRemaining;
    // Un booléen pour savoir si le mob est une version chaos
    [SerializeField]
    private bool isChaosVersion;
    // Référence au texte où est affiché le nombre de coups restants
    [SerializeField]
    private Text hitTextIndicator;

    // Son de marche (pour toutes les versions de l'ennemi)
    private AudioSource walkAudioSource;
    // Son d'atterrissage (uniquement pour GNE-O3X et GNE-O3X chaos)
    private AudioSource landAudioSource;
    // Référence au Renderer de l'ennemi)
    private Renderer rendu;

    private void Awake()
    {
        // On prépare les variables de sons pour l'ennemi
        AudioSource[] audioSources = GetComponents<AudioSource>();
        foreach(AudioSource audioSource in audioSources){
            if(audioSource.clip.name == "gnewalk"){
                walkAudioSource = audioSource;
            } else if(canRandomJump && audioSource.clip.name == "gneLanding"){
                landAudioSource = audioSource;
            }
        }
        // On récupère et initialise les variables
        rendu = GetComponent<Renderer>();
        speed = maxSpeed;
        renderSprite = GetComponent<SpriteRenderer>();
        target = waypoints[0];
        destPoint = 0;
        isOnGround = true;
        // On démarre une fonction qui sera utile pour mettre à jour les VFX si le joueur décide 
        // de les désactiver en plein niveau
        if(landGroundParticle != null)
            InvokeRepeating("UpdateVFX", 0f, 0.5f);
        // On met à jour le texte de nombre de coups restants
        if(isChaosVersion)
            hitTextIndicator.text = "" + nbHitRemaining;
    }

    // Méthode pour mettre à jour les VFX
    private void UpdateVFX(){
        if(gameObject != null)
            landGroundParticle.gameObject.SetActive(SettingsJSON.instance.settings.videoSettings.isVFXToggled);
    }

    private void Update()
    {
        Move();
    }

    // Méthode servant à déplacer l'ennemi
    public override void Move()
    {
        // Si l'ennemi est sur le sol
        if (isOnGround)
        {
            // Si il peut sauter (GNE-O3X et GNE-O3X chaos)
            if (canRandomJump)
            {
                // Système de probabilités de saut
                float value = Random.Range(0f, 1f);
                if (value < 0.0025f)
                {
                    isOnGround = false;
                    if(animatorGneO3X != null)
                        animatorGneO3X.SetBool("isOnGround", false);
                    if(walkAudioSource.isPlaying)
                        walkAudioSource.Stop();
                    // Selon le côté où l'ennemi regarde, on le fait sauter dans une direction ou une autre
                    GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                    if (GetComponent<SpriteRenderer>().flipX)
                    {
                        GetComponent<Rigidbody2D>().velocity = Vector2.up * 6f + Vector2.right * 1f;
                    }
                    else
                    {
                        GetComponent<Rigidbody2D>().velocity = Vector2.up * 6f + Vector2.left * 1f;
                    }
                    return; 
                }
            }
            // Si l'ennemi est visible par la caméra, on lui active le son de marche, sinon on le stop
            if(rendu.isVisible){
                if(!walkAudioSource.isPlaying){
                    walkAudioSource.Play();
                }
            } else {
                if(walkAudioSource.isPlaying)
                    walkAudioSource.Stop();
            }
            // Calcul de la distance jusqu'au prochain waypoint
            Vector3 dir = target.position - transform.position;
            // On déplace l'ennemi jusqu'au prochain waypoint
            transform.Translate(dir.normalized * speed * Time.deltaTime, Space.World);

            // Si l'ennemi a atteint son waypoint cible
            if (Vector3.Distance(transform.position, target.position) < 0.3f)
            {
                // On change de waypoint
                destPoint = (destPoint + 1) % waypoints.Length;

                // Si la position du prochain waypoint est à gauche
                if (target.position.x > waypoints[destPoint].position.x)
                {
                    // On fait regarder l'ennemi à gauche
                    renderSprite.flipX = false;
                }
                else
                // Sinon on le fait regarder à droite
                {
                    renderSprite.flipX = true;
                }

                // On change le prochain waypoint
                target = waypoints[destPoint];
            }


        }
    }
    
    // Méthode affectant l'effet de Stun (du StunHammer) à l'ennemi
    public override void Stun()
    {
        // Si l'ennemi existe encore
        if (gameObject != null)
        {
            // On modifie la vitesse de l'ennemi
            speed = 0.5f;
        }
    }


    // Méthode retirant l'effet de Stun (du StunHammer) à l'ennemi
    public override void Destun()
    {
        if(gameObject != null)
            speed = maxSpeed;
    }

    // Méthode appelée à chaque entrée en Collision TRIGGER sur l'ennemi
    public void OnTriggerEnter2D(Collider2D collision)
    {
        // Si le joueur rentre en contact avec l'ennemi
        if (collision.CompareTag("Player"))
        {
            ContactPoint2D[] contactpoint = new ContactPoint2D[10];
            collision.GetContacts(contactpoint);
            Vector2 direction = contactpoint[0].normal;
            // On vérifie si le contact n'est pas au niveau de la tête de l'ennemi, si c'est le cas
            // Le joueur prend des dégâts
            if (direction.y != 0f)
            {
                PlayerHealth.instance.TakeDamage(damageAmount);
            }
            // Sinon
            else
            {
                // On retire un coup au nombre de coup restants si l'ennemi est une version chaos
                // sinon on le tue.
                // On fait également rebondir le joueur à une hauteur différente si le mob est un version chaos
                if(!isChaosVersion){
                    PlayerMovement.instance.GetComponent<Rigidbody2D>().velocity = Vector2.up * 15f;
                    AudioManager.instance.Play("MobHit");
                    Invoke("destroyEnemy", 0.1f);
                }
                else
                {
                    AudioManager.instance.Play("MobHit");
                    PlayerMovement.instance.GetComponent<Rigidbody2D>().velocity = Vector2.up * 30f;
                    nbHitRemaining--;
                    hitTextIndicator.text = "" + nbHitRemaining;
                    if(nbHitRemaining == 0)
                        Invoke("destroyEnemy", 0.1f);
                }
            }
        }
    }

    // Si l'ennemi rentre en contact avec le sol ou une plateforme
    public void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.CompareTag("Ground") || collision.collider.CompareTag("Platform"))
        {
            // On lui remet sa variable isOnGround à true
            isOnGround = true;
            // On remet les animations et le son de l'ennemi au sol
            if(rendu.isVisible)
                landAudioSource.Play();
            if(animatorGneO3X != null)
                animatorGneO3X.SetBool("isOnGround", true);
            if(landGroundParticle != null)
                if(!landGroundParticle.isPlaying)
                    landGroundParticle.Play();
        }
    }

    // Méthode pour détruire le gameObject
    private void destroyEnemy()
    {
        Destroy(gameObject);
    }

    public override void Geyser()
    {
        
    }
}