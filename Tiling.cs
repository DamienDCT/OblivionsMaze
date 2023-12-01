using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(SpriteRenderer))]
public class Tiling : MonoBehaviour
{
    //décalage qui correspond à une marge pour éviter de créer une nouvelle tile quand on voit du vide
    public int offsetX = 2;

    //booléen qui indique si le background a une "suite" à sa droite
    public bool hasARightTile = false;

    //booléen qui indique si le background a une "suite" à sa gauche
    public bool hasALeftTile = false;

    //booléen qui indique si le background doit être reverse (dans le cas où le background n'est pas déjà en miroir)
    public bool reverseScale = false;

    //correspond à la taille pour savoir quand on doit créer un nouveau background à côté
    private float spriteWidth;

    //référence à la caméra
    private Camera cam;

    // on initialise la camera
    void Awake()
    {
        cam = Camera.main;
    }

    //on initialise le sprite ainsi que sa taille
    private void Start()
    {
        SpriteRenderer sRenderer = GetComponent<SpriteRenderer>();
        spriteWidth = sRenderer.sprite.bounds.size.x;
    }

    void Update()
    {
        //si le background n'a rien à sa gauche où à sa droite
        if (!hasALeftTile || !hasARightTile) 
        {
            //on calcule l'extension de la camera en fonction de la taille de l'écran
            float camHorizontalExtend = cam.orthographicSize * Screen.width / Screen.height;
            //et on calcule un nouveau background à gauche et à droite selon la taille du background et sa position
            float edgeVisiblePositionRight = (transform.position.x + spriteWidth / 2) - camHorizontalExtend;
            float edgeVisiblePositionLeft = (transform.position.x - spriteWidth / 2) + camHorizontalExtend;

            //si la position de la caméra en x est supérieur à la position droite et que le background n'a pas de tile à sa droite
            if(cam.transform.position.x >= edgeVisiblePositionRight - offsetX && !hasARightTile)
            {
                //on lui instancie une nouvelle tile à sa droite, et on l'indique en passant le booléen à true
                InstanceNewTile(1);
                hasARightTile = true;
            }
            //si la position de la caméra en x est inférieure à la position droite et que le background n'a pas de tile à sa gauche
            else if(cam.transform.position.x <= edgeVisiblePositionLeft + offsetX && !hasALeftTile)
            {
                //on lui instancie une nouvelle tile à sa gauche, et on l'indique en passant le booléen à true
                InstanceNewTile(-1);
                hasALeftTile = true;
            }

        }
    }

    //méthode pour créer une nouvelle tile à gauche ou à droite du background
    private void InstanceNewTile(int rightOrLeft)
    {
        //on calcule la position de la tile selon la taille du background
        Vector3 newPosition = new Vector3(transform.position.x + spriteWidth * rightOrLeft, transform.position.y, transform.position.z);
        Transform newTile = (Transform)Instantiate(transform, newPosition, transform.rotation);

        //si elle on veut un background en miroir
        if(reverseScale == true)
        {
            //on inverse de sens en x la nouvelle tile
            newTile.localScale = new Vector3(newTile.localScale.x * -1, newTile.localScale.y, newTile.localScale.z);
        }
        //et on indique que son parent est le background actuel
        newTile.parent = transform.parent;
        //si rightOrLeft est > 0
        if(rightOrLeft > 0)
        {
            //on indique à la nouvelle tile qui est donc à droite, qu'elle à une tile à sa gauche
            newTile.GetComponent<Tiling>().hasALeftTile = true;
        } else
        {
            //on indique à la nouvelle tile qui est donc à gauche, qu'elle à une tile à sa droite
            newTile.GetComponent<Tiling>().hasARightTile = true;
        }
    }
}
