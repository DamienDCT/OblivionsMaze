using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointPath : MonoBehaviour
{
    //on y place respectivement les waypoints situés au dessus, à droite, en dessous, et à gauche du waypoint
    public WaypointPath top;
    public WaypointPath right;
    public WaypointPath bottom;
    public WaypointPath left;

    //indique si c'est un waypoint sur lequel le joueur doit s'arrêter
    public bool isIntersection;

    //nom de la scène
    public string SceneLevelName;

    //texte à écrire sur l'écran de chargement
    public string levelNameLoadingScreen;

    //le nombre de pièces que le joueur doit posséder pour entrer dans le niveau
    public int nbCoinsToEnter;

}
