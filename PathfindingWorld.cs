using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PathfindingWorld : MonoBehaviour
{
    // Référence au waypoint d'intersection
    public WaypointPath intersection;
    // Référence au transform du joueur (flèche rouge)
    public Transform transformJoueur;
    // Vitesse dans le monde
    public float speedWorld;
    // Booléen indiquant si on peut appuyer sur une touche 
    public bool isPossibleToMove;
    // Transform pour savoir où on se situe actuellement sur l'arbre des WaypointPath
    public Transform currentTarget;
    // waypoint actuel où se situe le joueur
    public WaypointPath currentWaypoint;
    // Vecteur de référence
    private Vector3 referenceVector = Vector3.zero;
    // Entier pour savoir la direction
    private int direction;

    private void Awake()
    {
        // Initialisation des variables
        isPossibleToMove = true;
        currentWaypoint = intersection;
    }

    private void OnEnable()
    {
        // Lorsqu'on active le GameObject, on replace le joueur à sa position sur la map
        transformJoueur.position = currentWaypoint.transform.position;
    }

    private void Update()
    {
        // Si on peut bouger
        if (isPossibleToMove)
        {
            // Si on décide d'aller à gauche
            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.Q))
            {
                // Si notre waypoint actuel possède un waypoint à sa gauche
                if (currentWaypoint.left != null)
                {
                    // On met à jour les variables
                    isPossibleToMove = false;
                    currentTarget = currentWaypoint.left.transform;
                    currentWaypoint = currentWaypoint.left;
                    direction = 1;
                }
            }
            // Si on décide d'aller à droite
            else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            {
                // Si notre waypoint actuel possède un waypoint à sa droite
                if (currentWaypoint.right != null)
                {
                    // On met à jour les variables
                    isPossibleToMove = false;
                    currentTarget = currentWaypoint.right.transform;
                    currentWaypoint = currentWaypoint.right;
                    direction = 2;
                }
            }
            // Si on décide d'aller en haut
            else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.Z))
            {
                // Si notre waypoint actuel possède un waypoint au dessus
                if (currentWaypoint.top != null)
                {
                    // On met à jour les variables
                    isPossibleToMove = false;
                    currentTarget = currentWaypoint.top.transform;
                    currentWaypoint = currentWaypoint.top;
                    direction = 3;
                }
            }
            // Si on décide d'aller en bas
            else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
            {
                // Si notre waypoint actuel possède un waypoint en dessous
                if (currentWaypoint.bottom != null)
                {
                    // On met à jour les variables
                    isPossibleToMove = false;
                    currentTarget = currentWaypoint.bottom.transform;
                    currentWaypoint = currentWaypoint.bottom;
                    direction = 4;
                }
            // Si on appuie sur A
            } else if (Input.GetKeyDown(KeyCode.A))
            {
                // et que notre waypoint actuel est une intersection de niveau
                if(currentWaypoint.SceneLevelName != "")
                {
                    // On regarde si le joueur a assez de pièces pour rentrer dans le niveau
                    if(CreativeMode.instance.isCreativeActivated || PlayerPowerup.instance.GetNbCoins() == currentWaypoint.nbCoinsToEnter){
                        // On charge le niveau 
                        LevelLoader.instance.LoadLevel(currentWaypoint.SceneLevelName, currentWaypoint.levelNameLoadingScreen, true, null);
                    }
                }
            }
        }
        // Si la target est différent de null
        if (currentTarget != null)
        {
            // On calcule la distance à parcourir entre la position actuelle du joueur et le prochain waypoint
            Vector3 dir = currentTarget.position - transformJoueur.position;
            transformJoueur.Translate(dir.normalized * 5f * Time.deltaTime, Space.World);
            // Si le joueur est très proche du prochain waypoint
            if (Vector3.Distance(transformJoueur.position, currentTarget.position) < 0.1f)
            {
                // Si le waypoint de destination n'est pas une intersection
                if (!currentWaypoint.isIntersection)
                {
                    // On regarde où le joueur a décidé d'aller, et on change le transform en fonction de là où
                    // le joueur a décidé de partir
                    switch (direction)
                    {
                        case 1:
                            currentWaypoint = currentWaypoint.left;
                            currentTarget = currentWaypoint.transform;
                            break;
                        case 2:
                            currentWaypoint = currentWaypoint.right;
                            currentTarget = currentWaypoint.transform;
                            break;
                        case 3:
                            currentWaypoint = currentWaypoint.top;
                            currentTarget = currentWaypoint.transform;
                            break;
                        case 4:
                            currentWaypoint = currentWaypoint.bottom;
                            currentTarget = currentWaypoint.transform;
                            break;
                        default:
                            return;
                    }
                // Sinon, on est sur intersection, et donc on remet la possibilité au joueur de se déplacer
                } else
                {
                    isPossibleToMove = true;
                    currentTarget = null;
                    direction = -1;
                }
            }
        }
    }
}
