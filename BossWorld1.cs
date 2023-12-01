using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossWorld1 : MonoBehaviour
{
    [Header("Rewards")]
    // Prefab de la pièce 
    [SerializeField]
    private GameObject piecePrefab;

    // Référence de la vitesse max de déplacement de karzh'an
    [SerializeField]
    private float maxSpeed;
    // Tableau de waypoints pour le déplacement de karzh'an
    [SerializeField]
    private Transform[] waypoints;
    // Référence à waypoints[destPoint]
    private Transform target;
    // Référence à l'index du waypoint vers lequel karzh'an se déplace
    private int destPoint;
    // Référence à la vitesse de karzh'an
    private float speed;
    // Points de vie de karzh'an
    [SerializeField]
    private int health;
    // Points de vie max de karzh'an
    [SerializeField]
    private int maxHealth;

    [Header("Slider variables")]
    // référence au slider pour sa barre de vie
    [SerializeField]
    private Slider slider;
    // Référence au rectangle pour le slider de sa barre de vie
    [SerializeField]
    private Image sliderRect;

    // Dégâts que fait karzh'an au joueur
    [SerializeField]
    private int damageAmount;


    [Header("Second Phase Variables")]
    // Booléen indiquant si on est en 2ème phase
    private bool secondPhase;
    // Spawnpoint pour la deuxieme phase
    [SerializeField]
    private GameObject spawnPointPhase2;
    // waypoint où doit apparaître karzh'an pour la phase 2
    [SerializeField]
    private GameObject bossSpawnPhase2;
    // Couleur du slider pour la phase 2
    [SerializeField]
    private Color colorSliderSecondPhase;
    // Waypoint pour le trajet aller 
    [SerializeField]
    private Transform[] waypointsPhase2aller;
    // Waypoint pour le trajet retour
    [SerializeField]
    private Transform[] waypointsPhase2retour;
    // Booléen indiquant si karzh'an fonce vers le joueur
    private bool targetPlayer;
    // Référence au tableau (i.e. soit waypointsPhase2aller ou waypointsPhase2retour)
    private Transform[] pathToFollow;
    // Waypoint après avoir foncer sur le mur de gauche 
    [SerializeField]
    private Transform afterTargetLeft;
    // Waypoint après avoir foncer sur le mur de droite
    [SerializeField]
    private Transform afterTargetRight;
    // Référence au mur de gauche
    [SerializeField]
    private Transform leftWall;
    // référence au mur de droite
    [SerializeField]
    private Transform rightWall;
    // Référence au pics de la phase 2
    [SerializeField]
    private PicsBoss picsBossPhase2;
    // Booléen indiquant si le boss est en train de foncer vers un mur
    [SerializeField]
    private bool running;

    // Particules pour quand il touche le côté gauche, le côté droit, et le sol
    [Header("Particules")]
    [SerializeField]
    private ParticleSystem leftSideParticle;
    [SerializeField]
    private ParticleSystem rightSideParticle;
    [SerializeField]
    private ParticleSystem groundLandParticle;
    [Header("Animation")]
    // Animator de karzh'an
    [SerializeField]
    private Animator animator;
    // Référence au Renderer de karzh'an
    [SerializeField]
    private SpriteRenderer graphics;

    private void Awake(){
        // On appelle UpdateVFX toutes les 0.5s pour mettre à jour les graphismes si le joueur décide de retirer les VFX
        InvokeRepeating("UpdateVFX", 0f, 0.5f);
    }

    private void Start(){
        // On initialise les variables
        destPoint = 0;
        target = waypoints[0];
        speed = maxSpeed;
        health = maxHealth;
        slider.value = health;
        secondPhase = false;
        targetPlayer = false;
        waypointsPhase2retour = new Transform[waypointsPhase2aller.Length];
        for(int i = 0; i < waypointsPhase2aller.Length; i++){
            waypointsPhase2retour[waypointsPhase2aller.Length-i-1] = waypointsPhase2aller[i];
        }
        pathToFollow = waypointsPhase2aller;
    }

    private void Update(){
        Move();
    }

    private void UpdateVFX(){
        // On met à jour les VFX
        leftSideParticle.gameObject.SetActive(SettingsJSON.instance.settings.videoSettings.isVFXToggled);
        rightSideParticle.gameObject.SetActive(SettingsJSON.instance.settings.videoSettings.isVFXToggled);
        groundLandParticle.gameObject.SetActive(SettingsJSON.instance.settings.videoSettings.isVFXToggled);
    }

    private void Move(){
        // Si karzh'an a au moins une cible
        if(target != null){
            // On le fait regarder vers sa cible
            if(transform.position.x < target.position.x){
                graphics.flipX = false;
            } else {
                graphics.flipX = true;
            }
        }
        // Si l'ennemi track le joueur
        if(targetPlayer){
            // Calcul de la distance jusqu'au prochain waypoint
            Vector3 dir = target.position - transform.position;
            // On déplace jusqu'au prochain waypoint
            transform.Translate(dir.normalized * speed * Time.deltaTime, Space.World);
        // Sinon si le boss est en première phase
        } else if(!secondPhase){
            // Calcul de la distance jusqu'au prochain waypoint
            Vector3 dir = target.position - transform.position;
            // On déplace jusqu'au prochain waypoint
            transform.Translate(dir.normalized * speed * Time.deltaTime, Space.World);
            // Si il est pas loin d'un waypoint 
            if (Vector3.Distance(transform.position, target.position) < 0.05f)
            {
                // Si il touche un waypoint d'atterrissage
                if(target.CompareTag("MainWaypoint")){
                    // On lui stop sa vitesse et on lui fait reprendre son parcours entre 1 et 2s après
                    speed = 0f;
                    Invoke("ResetSpeed", Random.Range(1f,2f));
                }
                // On lui change son waypoint
                destPoint = (destPoint + 1) % waypoints.Length;

                // On change le prochain waypoint
                target = waypoints[destPoint];
            }
        // Si on est en deuxième phase
        } else {
            // Calcul de la distance jusqu'au prochain waypoint
            Vector3 dir = target.position - transform.position;
            // On déplace jusqu'au prochain waypoint
            transform.Translate(dir.normalized * speed * Time.deltaTime, Space.World);

            // Si le joueur a atteint son waypoint
            if (Vector3.Distance(transform.position, target.position) < 0.05f)
            {
                // Et que le waypoint atteint est un waypoint d'atterrissage
                if(target.CompareTag("MainWaypoint")){
                    // On calcule une probabilité pour que karzh'an fonce vers le joueur pour atterrir sur un mur
                    if(Random.Range(0f, 1f) <= 0.2f){
                        targetPlayer = true;
                        Vector3 playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position;  
                        running = true;
                        animator.SetInteger("AnimationMode", 3);
                        // On modifie le rendu du boss en fonction de là où fonce le boss
                        if(playerPosition.x > transform.position.x){
                            // droite
                            target = rightWall;
                            graphics.flipX = false;
                        } else {
                            // gauche
                            target = leftWall;
                            graphics.flipX = true;
                        }
                        picsBossPhase2.StopSpikes();
                        return;
                    }
                    speed = 0f;
                    Invoke("ResetSpeed", Random.Range(1f,2f));
                }
                destPoint = (destPoint + 1) % pathToFollow.Length;

                // On change le prochain waypoint
                target = pathToFollow[destPoint];
            }
        }
    }

    //méthode pour que le boss perde de la vie
    public void TakeDamage(int damageAmount){
        //on  lui fait perdre selon le nombre de dégats donné
        health -= damageAmount;
        //on ajuste sa barre de vie
        slider.value = health;
        //s'il n'est pas en seconde phase, on vérifie s'il doit y passer
        if(!secondPhase)
            CheckSecondPhase();
        //sinon on vérifie s'il doit mourir
        else 
            CheckDeath();
    }

    //méthode pour vérifier la mort du boss
    private void CheckDeath(){
        //si la vie du boss passe à 0 ou moins
        if(health <= 0f){
            //il fait tomber la pièce pour finir le niveau
            GameObject go = Instantiate(piecePrefab);
            go.transform.position = transform.position;
            //et on le détruit
            Destroy(gameObject);
        }
    }

    //méthode qu'on appelle pour vérifier si on doit passer en seconde phase
    private void CheckSecondPhase(){
        //si le boss a la moitié de sa vie ou moins on passe en seconde phase
        if(health <= (maxHealth/2)){
            secondPhase = true;
            GoSecondPhase();
        }
    }

    //méthode pour aller à la seconde phase du boss
    private void GoSecondPhase(){
        // 1) setup camera
        CameraManager.instance.SetSecondCamera();
        // 2) tp joueur et boss
        spawnPointPhase2.GetComponent<SpawnPoint>().ChangeSpawn();
        transform.position = bossSpawnPhase2.transform.position;
        sliderRect.color = colorSliderSecondPhase;
        target = waypointsPhase2aller[0];
        destPoint = 0;
    }
    
    //méthode pour remettre la vitesse du boss et sa cible à false
    private void ResetSpeed(){
        speed = maxSpeed;
        targetPlayer = false;
    }

    //si le boss entre en collision avec le joueur
    public void OnTriggerEnter2D(Collider2D collider2D){
        if(collider2D.CompareTag("Player")){
            //on fait prendre des dégats au joueur
            PlayerHealth.instance.TakeDamage(damageAmount);
        }
    }

    //si le boss entre en collision
    public void OnCollisionEnter2D(Collision2D collision2D){
        //avec un mur
        if(collision2D.collider.CompareTag("Wall")){
            //on appelle la méthode pour le faire retourner dans un cycle, et on dit qu'il ne court plus
            GoBackCycle(collision2D.collider.name);
            running = false;
        //avec une plateforme ou le sol
        } else if(collision2D.collider.CompareTag("Platform") || collision2D.collider.CompareTag("Ground")){
            //on met l'audio d'atterrissage
            AudioManager.instance.Play("KarzhanLanding");
            //si les particules d'atterrissages ne sont pas déjà en train de jouer, on les met
            if(!groundLandParticle.isPlaying){
                groundLandParticle.Play(); 
            }
            //s'il ne court pas, on change son animation en conséquence
            if(!running)
                animator.SetInteger("AnimationMode", 1);
        }
    }

    //Si on sort d'une collision avec une plateforme ou du sol
    public void OnCollisionExit2D(Collision2D collision2D){
        if(collision2D.collider.CompareTag("Platform") || collision2D.collider.CompareTag("Ground")){
            //on met que le boss ne court pas et on modifie son animation en conséquence
            if(!running)
                animator.SetInteger("AnimationMode", 2);
        }
    }

    //méthode pour que le boss retourne faire un cycle de saut selon le nom du côté
    private void GoBackCycle(string sideName)
    {
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        speed = 0;
        AudioManager.instance.Play("KarzhanHitWall");
        //si le côté est le côté gauche
        if(sideName.Equals("leftWall")){
            leftSideParticle.Play();
            //le chemin à suivre sera le chemin waypointsPhase2retour
            pathToFollow = waypointsPhase2retour;
            target = afterTargetLeft;
            destPoint = System.Array.IndexOf(waypointsPhase2retour, afterTargetLeft);
            //on lui add une force qui le fait retourner sur la plateforme à sa droite
            GetComponent<Rigidbody2D>().AddForce(new Vector2(200f, 550f));
        //sinon si le côté est le côté droit
        } else if(sideName.Equals("rightWall")){
            rightSideParticle.Play();
            //le chemin à suivre sera le chemin waypointsPhase2aller
            pathToFollow = waypointsPhase2aller;
            target = afterTargetRight;
            destPoint = System.Array.IndexOf(waypointsPhase2aller, afterTargetRight);
            //et on lui applique une force qui le fait retourner sur la plateforme à sa gauche
            GetComponent<Rigidbody2D>().AddForce(new Vector2(-200f, 550f));
        }
        //on lui remet sa vitesse et on fait en sorte que les pics puissent sortir de nouveau
        Invoke("ResetSpeed", 3f);
        Invoke("RepopSpikes", 3f);
    }

    //pour que les pics puissent ressortir
    private void RepopSpikes(){
        picsBossPhase2.RestartSpikes();
    }
}
