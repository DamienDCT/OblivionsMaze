using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ventilateur : MonoBehaviour
{
    //référence à l'animation du ventilateur
    [SerializeField]
    private Animator animationVentilateur;

    //énumération des directions dans laquelle souffle le ventilateur
    private enum DirectionVentilo { NORTH, SOUTH, WEST, EAST }

    //référence à la direction
    [SerializeField]
    private DirectionVentilo directionVentilo;

    //zone dans laquelle le souffle sera appliqué
    [SerializeField]
    private Vector2 zoneTarget;

    //point d'origine de la zone
    [SerializeField]
    private Vector2 originPoint;

    //booléen qui indique si le joueur est dans la zone d'application du souffle
    [SerializeField]
    private bool isPlayerOnZone;

    //Layermask pour n'appliquer la force qu'au joueur
    [SerializeField]
    private LayerMask playerLayerMask;

    //pour que la force soit appliqué graduellement en foncion de la distance par rapport au ventilateur
    [SerializeField]
    private AnimationCurve windForceCurve;

    //force du vent
    [SerializeField]
    private float windForce;

    //booléen qui indique si le ventilateur est activé
    [SerializeField]
    private bool isActivated;

    //référence au son pendant que le ventilateur est activé
    [SerializeField]
    private AudioSource audioSource;

    //référence au sprite du ventilateur
    [SerializeField]
    private Renderer rendu;

    //sert à voir la zone d'application du vent
    public void OnDrawGizmos(){
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube((Vector2)transform.position + originPoint, zoneTarget);
    }

    //Au début, on change l'animation du ventilateur selon s'il est activé ou non
    private void Start(){
        animationVentilateur.SetBool("activation", isActivated);
    }

    private void FixedUpdate(){
        //si le ventilateur n'est plus visible on arrête de jouer l'audio
        if(!rendu.isVisible)
        {
            audioSource.Stop();
            return;
        }
        //si le ventilateur est activé
        if(isActivated){
            //qu'il est visible et que l'audio n'est pas déjà en route, on le lance
            if(rendu.isVisible && !audioSource.isPlaying)
                audioSource.Play();
        }
    }

    //on vérifie de manière constante si le joueur est dans la zone
    void Update()
    {
        if(isActivated){
            CheckPlayer();
            if(isPlayerOnZone)
                //s'il est dans la zone on lui applique la force du vent
                WindForceDirectional();
        }
    }

    //méthode qui vérifie si le joueur est dans la zone et modifie le booléen isPlayerOnZone en conséquence
    private void CheckPlayer(){
        RaycastHit2D hit = Physics2D.BoxCast((Vector2)transform.position + originPoint, zoneTarget, 0f, new Vector2(0f, 0f), 0f, playerLayerMask);
        if(hit.collider != null)
            isPlayerOnZone = true;
        else 
            isPlayerOnZone = false;
    }

    //méthode pour appliquer la force du vent
    private void WindForceDirectional(){
        float endZoneX;
        float endZoneY;
        float percentagePosition;
        //on effectue les calculs selon la direction du ventilateur
        switch(directionVentilo){
            case DirectionVentilo.NORTH:
                endZoneY = (transform.position.y - zoneTarget.y);
                percentagePosition = (endZoneY - PlayerMovement.instance.gameObject.transform.position.y) / (endZoneY - transform.position.y);
                ApplyWindForce(new Vector2(0f, windForce), percentagePosition);
                break;
            case DirectionVentilo.SOUTH:
                endZoneY = (transform.position.y - zoneTarget.y);
                percentagePosition = (PlayerMovement.instance.gameObject.transform.position.y - endZoneY) / (transform.position.y - endZoneY);
                ApplyWindForce(new Vector2(0f, windForce * -1), percentagePosition);
                break;
            case DirectionVentilo.WEST:
                endZoneX = (transform.position.x - zoneTarget.x);
                percentagePosition = (PlayerMovement.instance.gameObject.transform.position.x - endZoneX) / (transform.position.x - endZoneX);
                ApplyWindForce(new Vector2(windForce * - 1, 0f), percentagePosition);
                break;
            case DirectionVentilo.EAST:
                endZoneX = (transform.position.x - zoneTarget.x);
                percentagePosition = (endZoneX - PlayerMovement.instance.gameObject.transform.position.x) / (endZoneX - transform.position.x);
                ApplyWindForce(new Vector2(windForce, 0f), percentagePosition);
                break;
            default:    
                break;
        }

    }

    //méthode pour appliquer la force du vent
    public void ApplyWindForce(Vector2 windForce, float positionJoueur){
        //plus le joueur est loin moins la force est élevée
        float percentagePosition = Mathf.Clamp01(positionJoueur);
        Vector2 forceWind = windForce * windForceCurve.Evaluate(percentagePosition);
        //on ajoute la force après les calculs
        PlayerMovement.instance.gameObject.GetComponent<Rigidbody2D>().AddForce(windForce);
    }

    //méthode appelée lorsqu'on appuie sur un interrupteur relié au ventilateur pour l'arrêter ou le lancer
    public void Switch(){
        isActivated = !isActivated;
        animationVentilateur.SetBool("activation", isActivated);
        
    }
}
