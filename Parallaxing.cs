using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Parallaxing : MonoBehaviour
{
    // Position X initiale du gameObject pour la couche de background
    private float startpos;
    // Position Y initiale du gameObject pour la couche de background
    private float startposY;
    // Position initiale de la caméra en Y
    private float startposCamY;
    // Référence à la camera
    private GameObject cam;
    // Variable entre 0 et 1 pour calculer l'effet de parallaxing (0 = bouge en même temps que la caméra, 1 = très décalé)
    public float parallaxEffect;
    // Booléen savoir si l'axe Y du gameObject de la couche de background doit être fixe en Y
    [SerializeField]
    private bool YaxisFixed;
    // Vecteur de calcul de position
    private Vector3 positionCalculated;
    // Référence au joueur
    private GameObject player;

    private void Awake()
    {
        // On initialise les variables
        cam = GameObject.FindGameObjectWithTag("MainCamera");
    }

    private void Start()
    {
        // On initialise les variables 0.1s plus tard le temps que tout soit setup
        Invoke("InitializeVariables", 0.1f);
        player = PlayerMovement.instance.gameObject;
    }

    // Méthode faisant office de "constructeur"
    private void InitializeVariables(){
        startpos = transform.position.x;
        startposY = transform.position.y;
        startposCamY = cam.transform.position.y;
        positionCalculated = new Vector3(0f, 0f, transform.position.z);
    }

    void FixedUpdate()
    {
        // On calcule la prochaine position de la caméra en X en fonction du degré de parallaxing
        float dist = (cam.transform.position.x * parallaxEffect);

        // On met à jour la position en x en fonction de la position de départ
        positionCalculated.x = startpos + dist;
        // Si l'axe Y ne doit pas être fixe, on met la position en y par rapport à la position la caméra en y
        if(!YaxisFixed){
            positionCalculated.y = cam.transform.position.y;
        // Sinon on met la position en y par rapport à la position de base de la caméra en Y
        } else {
            positionCalculated.y = startposCamY;
        }
        // On met à jour la position du GameObject
        transform.position = positionCalculated;
    }
}
