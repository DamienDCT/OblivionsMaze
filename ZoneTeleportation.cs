using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneTeleportation : MonoBehaviour
{
    //booléen qui indique si le joueur est dans la zone de téléportation
    [SerializeField]
    private bool isPlayerOnZone;

    //Waypoint qui a les coordonnées où on veut téléporter le joueur
    [SerializeField]
    private Transform waypointTP;

    //Nom de la scène à charger (si on téléporte dans la scène pour un boss)
    [SerializeField]
    private string sceneToLoad;

    //correspond au texte qui montre l'interaction qui doit être faite
    private Interaction interaction;

    //on initialise l'interaction
    private void Start(){
        interaction = GetComponent<Interaction>();
    }

    //on vérifie si le joueur est dans la zone de manière constante, et s'il appuie sur [E], on le téléporte
    void Update(){
        if(isPlayerOnZone){
            if(Input.GetKeyDown(KeyCode.E)){
                Teleport();
            }
        }
    }

    //méthode pour téléporter le joueur
    private void Teleport(){
        //on enlève le texte car on va sortir de la zone
        interaction.EraseText();
        AudioManager.instance.Play("Teleporter");
        //on enlève quelconque powerup qu'aurait le joueur
        PlayerPowerup.instance.ResetPowerup();  
        //on téléporte le joueur s'il y a un waypoint assigné    
        if(waypointTP != null){
            PlayerMovement.instance.gameObject.transform.position = waypointTP.position;
        } else { //sinon c'est qu'on veut aller dans un autre niveau
            // Si on est sur un niveau avec un manager de camera
            if(CameraManager.instance != null){
                CameraManager.instance.ResetCameras();
            }
            Destroy(GameObject.FindGameObjectWithTag("IngameUIPrefab"));
            GameObject go = GameObject.FindGameObjectWithTag("HealthBar");
            if(go != null) go.SetActive(false);
            //on load le niveau
            LevelLoader.instance.LoadLevel(sceneToLoad, "", true, go);
        }
    }

    //Si le joueur entre dans la zone, on met le texte lié à l'interaction, et on indique qu'il est dans la zone
    public void OnTriggerEnter2D(Collider2D collider2D){
        if(collider2D.CompareTag("Player"))
        {
            interaction.PutText();
            isPlayerOnZone = true;
        }
    }

    //Si le joueur sort de la zone, on enlève le texte lié à l'interaction, et on indique qu'il est sorti de la zone
    public void OnTriggerExit2D(Collider2D collider2D){
        if(collider2D.CompareTag("Player"))
        {
            interaction.EraseText();
            isPlayerOnZone = false;
        }
    }
}
