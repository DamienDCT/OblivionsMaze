using UnityEngine;

public class Pistolet : Item
{
    // Crosshair du pistolet
    [SerializeField]
    private GameObject crosshair;
    // Camera dans la salle de boss
    [SerializeField]
    private Camera cameraRoom;
    // Prefab de la balle
    [SerializeField]
    private GameObject bulletPrefab;
    // Vecteur stockant la position de la souris à l'écran
    private Vector3 mouseWorldPosition;

    private void Update()
    {
        if(Camera.main != null){
            // A chaque frame, on regarde la position de la souris du joueur
            Vector3 mouseScreenPosition = Input.mousePosition;
            // On stock la position de la souris par rapport à la position dans le monde
            mouseWorldPosition = Camera.main.ScreenToWorldPoint(new Vector3(mouseScreenPosition.x, mouseScreenPosition.y, Camera.main.nearClipPlane));
            // On met à jour la position de la crosshair
            crosshair.transform.position = mouseWorldPosition;
        }
    }

    // Méthode pour utiliser l'item
    public override void UseItem(){
        AudioManager.instance.Play("Pistolet");
        // On récupère la position du joueur
        Vector3 playerPosition = PlayerMovement.instance.gameObject.transform.position;
        // On instancie une balle de pistolet et on la positionne devant le joueur
        GameObject go = Instantiate(bulletPrefab);
        go.transform.position = PlayerMovement.instance.gameObject.transform.position;
        if(mouseWorldPosition.x > playerPosition.x){
            go.transform.position += new Vector3(.5f, 0f, 0f);
            PlayerMovement.instance.sprite.flipX = false;
        } else {
            go.transform.position += new Vector3(-.5f, 0f, 0f);
            PlayerMovement.instance.sprite.flipX = true;
        }
        // On setup la balle pour qu'elle parte dans la direction souhaitée
        go.GetComponent<PistolBullet>().SetupBullet(mouseWorldPosition);
        // On démarre le cooldown de l'item
        StartCoroutine(PlayerPowerup.instance.CooldownTimer(cooldownTimerItem));
    }
}
