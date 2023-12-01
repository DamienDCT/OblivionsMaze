using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vautour : Enemy
{
    // Booléen indiquant si le vautour voit le joueur
    [SerializeField]
    private bool isTrackingPlayer;
    // Référence au transform du joueur
    private Transform transformPlayer;
    // Référence à la vitesse du vautour
    [SerializeField]
    private float speed;
    // Référence à la distance max à laquelle le vautour peut suivre le joueur
    private float distanceTarget = 10f;
    // Booléen servant à savoir si le vautour a touché verticalement une plateforme
    [SerializeField]
    private bool verticalHit;
    // Booléen servant à savoir si le vautour a touché horizontalement une plateforme
    [SerializeField]
    private bool horizontalHit;
    // Vecteur servant à stocker la position de la dernière frame du vautour
    private Vector2 beforeVautourPosition;
    // Item que peut drop le vautour
    [SerializeField]
    private GameObject dropItem;
    // Source audio pour quand le vautour se déplace
    [SerializeField]
    private AudioSource flyAudioSource;

    [SerializeField]
    private int touch; //1 : top
                       //2 : bottom
                       //3/4 : side
                       //5 : diagonale
    private void Awake()
    {
        // Initialisation des variables
        isTrackingPlayer = false;
        beforeVautourPosition = new Vector2(0f, 0f);
        verticalHit = false;
        horizontalHit = false;
        touch = 0;
        transformPlayer = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Start()
    {
        // On vérifie si le joueur est dans la zone du vautour toutes les 0.5s
        InvokeRepeating("CheckPlayer", 0f, 0.5f);
    }

    private void Update()
    {
        Move();
    }

    private void CheckPlayer()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, distanceTarget);
        // On regarde tous les colliders à côté du vautour
        for (int i = 0; i < colliders.Length; i++)
        {
            // Si un collider correspond au joueur
            if (colliders[i].CompareTag("Player"))
            {
                // Si le joueur vient de rentrer dans la zone
                if (isTrackingPlayer == false)
                {
                    // On met la variable à true
                    isTrackingPlayer = true;
                }
                return;
            }
        }
        isTrackingPlayer = false;
    }

    public override void Stun()
    {

    }

    public override void Destun()
    {

    }

    // Action du geyser sur le vautour
    public override void Geyser()
    {
        // On détruit l'ennemi
        DestroyEnemy();
    }

    public override void Move()
    {
        if (isTrackingPlayer)
        {
            // Si le son du vautour n'est pas en train d'être joué, on le joue
            if(!flyAudioSource.isPlaying){
                flyAudioSource.Play();
            }
            // On calcule la distance normalisée entre le joueur et le vautour
            Vector2 trackVector = (Vector2)(transformPlayer.position - transform.position).normalized;
            // On regarde de quel côté le vautour a touché une plateforme
            switch (touch)
            {
                // S'il a touché une plateforme par le dessous
                case 1:
                    if (transformPlayer.position.y >= beforeVautourPosition.y - 1.5f) // -1.5f pour prendre en compte la taille du vautour
                    {
                        transform.Translate(trackVector * speed * Time.deltaTime, Space.World);
                    }
                    else
                    {
                        VerticalMovement();
                    }
                    break;
                // S'il a touché une plateforme par le dessus
                case 2:
                    if (transformPlayer.position.y <= transform.position.y)
                    {
                        transform.Translate(trackVector * speed * Time.deltaTime, Space.World);
                    }
                    else
                    {
                       VerticalMovement();
                    }
                    break;
                // S'il a touché une plateforme par le côté
                case 3:
                    HorizontalMovement();
                    transform.Translate(trackVector * speed * Time.deltaTime, Space.World);
                    break;
                // S'il a touché une plateforme par le côté
                case 4:
                    HorizontalMovement();
                    transform.Translate(trackVector * speed * Time.deltaTime, Space.World);
                    break;
                // S'il a touché une plateforme par la diagonale
                case 5:
                    // Si le mob n'a rien touché, on le fait déplacer normalement
                    if (!verticalHit && !horizontalHit)
                    {
                        transform.Translate(trackVector * speed * Time.deltaTime, Space.World);
                    }
                    else
                    {
                        // Sinon on applique la méthode de recadrage
                        VerticalMovement();
                        HorizontalMovement();
                    }
                    break;
                default:
                    // Sinon on le fait déplacer normalement vers le joueur
                    transform.Translate(trackVector * speed * Time.deltaTime, Space.World);
                    break;
            }
        // Si le joueur n'est pas dans la zone du vautour, on stop le son
        } else {
            flyAudioSource.Stop();
        }

    }

    // Méthode de recadrage du vautour en cas de hit horizontal
    private void HorizontalMovement()
    {
        // Si le hit horizontal a eu lieu
        if (horizontalHit)
        {
            // On calcule la nouvelle trajectoire du vautour et on le déplace
            if (transformPlayer.position.y >= beforeVautourPosition.y)
            {
                Vector2 direction = new Vector2(0f, 1f);
                transform.Translate(direction.normalized * speed * Time.deltaTime, Space.World);
            }
            else
            {
                Vector2 direction = new Vector2(0f, -1f);
                transform.Translate(direction.normalized * speed * Time.deltaTime, Space.World);
            }
        }
    }

    // Méthode de recadrage en cas de hit vertical
    private void VerticalMovement()
    {
         // Si le hit vertical a eu lieu
        if (verticalHit)
        {
             // On calcule la nouvelle trajectoire du vautour et on le déplace
            if (transformPlayer.position.x >= beforeVautourPosition.x)
            {
                Vector2 direction = new Vector2(1f, 0f);
                transform.Translate(direction.normalized * speed * Time.deltaTime, Space.World);
            }
            else
            {
                Vector2 direction = new Vector2(-1f, 0f);
                transform.Translate(direction.normalized * speed * Time.deltaTime, Space.World);
            }
        }
    }

    // Si le vautour entre en contact avec une plateforme
    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Platform"))
        {
            // On stocke la position de la frame d'avant du vautour
            beforeVautourPosition.x = transform.position.x;
            beforeVautourPosition.y = transform.position.y;
            // On regarde tous les contacts du vautour
            foreach (ContactPoint2D contact in collision.contacts)
            {
                // On récupère la normal du vecteur
                Vector2 contact2 = NormalizeVect(contact.normal);
                // On met à jour les valeurs en fonction du hit
                if (contact2.x == 0f && contact2.y == 1f) //top
                {
                    touch = 1;
                    horizontalHit = false;
                    verticalHit = true;
                }
                else if (contact2.x == 0f && contact2.y == -1f) //bottom
                {
                    touch = 2;
                    horizontalHit = false;
                    verticalHit = true;
                }
                else if (contact2.x == 1f && contact2.y == 0f) // side
                {
                    touch = 3;
                    horizontalHit = true;
                    verticalHit = false;
                }
                else if (contact2.x == -1f && contact2.y == 0f) // side
                {
                    touch = 4;
                    horizontalHit = true;
                    verticalHit = false;
                }
                else //diagonale
                {
                    touch = 5;
                    horizontalHit = true;
                    verticalHit = true;
                }
            }
        }
    }

    // Méthode pour normaliser un vecteur en arrondissant    
    private Vector2 NormalizeVect(Vector2 input)
    {
        Vector2 output = Vector2.ClampMagnitude(input, 1);
        output.x = Mathf.Round(output.x);
        output.y = Mathf.Round(output.y);
        return output;
    }

    // Si le vautour sort de la collision avec une plateforme
    public void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Platform"))
        {
            // Si le vautour avait touché verticalement une plateforme
            if (verticalHit)
            {
                // On le déplace deux fois plus vite que sa vitesse initiale vers la gauche ou la droite
                // selon là où est le joueur
                if (transformPlayer.position.y >= transform.position.y)
                {
                    transform.Translate(Vector2.up * speed * 2f * Time.deltaTime, Space.World);
                }
                else
                {
                    transform.Translate(Vector2.down * speed * 2f * Time.deltaTime, Space.World);
                }
            }
            verticalHit = false;
            horizontalHit = false;
            touch = 0;
        }
    }

    //s'il rentre en collision avec le joueur
    public void OnTriggerEnter2D(Collider2D collider2D){
        if(collider2D.CompareTag("Player")){
            //on fait prendre des dégâts au joueur
            PlayerHealth.instance.TakeDamage(damageAmount);
        }
    }

    //méthode pour détruire l'ennemi
    private void DestroyEnemy(){
        //s'il a un item à faire tomber
        if(dropItem != null)
        {
            //on le fait tomber
            GameObject go = Instantiate(dropItem);
            go.transform.position = transform.position;
        }
        //puis on détruit l'ennemi
        Destroy(gameObject);
    }
}
