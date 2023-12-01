using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpawnPoint : MonoBehaviour
{

    //référence au script qui permet de détecter la mort
    [SerializeField]
    private DeathDetection deathDetection;

    //au début on apparaît au point d'apparition
    void Start()
    {
        Respawn();
    }

    //méthode pour réapparaître au point d'apparition
    public void Respawn()
    {
        GameObject go = GameObject.FindGameObjectWithTag("Player");
        //on remet toute la vie au joueur
        PlayerHealth.instance.ResetHealth();
        // Retirer le texte
        GameObject interactionText = GameObject.FindGameObjectWithTag("InteractionText");
        if(interactionText != null)
        {
            interactionText.GetComponent<Text>().text = "";
        }
        //et on le téléporte au point d'apparition
        if(go != null)
        {
            go.transform.position = transform.position;
        }
    }

    public void ChangeSpawn(){
        deathDetection.spawnPoint = this.gameObject;
        GameObject go = GameObject.FindGameObjectWithTag("Player");
        if(go != null)
            go.transform.position = transform.position;
    }   
}
