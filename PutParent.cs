using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PutParent : MonoBehaviour
{
    // Booléen pour savoir si le background est un background pour la salle de boss
    [SerializeField]
    private bool isBossBackground;


    void Start()
    {
        // On met le background 0.1s plus tard pour éviter les bug de caméra qui ne sont pas mise en place
        Invoke("SetBackground", 0.1f);
    }

    // Méthode pour mettre le background à sa bonne position
    private void SetBackground(){
        GameObject cam = GameObject.FindGameObjectWithTag("MainCamera");
        if(cam != null){
            // On retire le background en enfant de la caméra s'il existe
            int nbchild = cam.transform.childCount;
            if(nbchild > 0)
            {
                Destroy(cam.transform.GetChild(nbchild - 1).gameObject);
            }
        }
        // Si le background doit être placé en enfant de la caméra, on le place
        if(!isBossBackground){
            transform.SetParent(GameObject.FindGameObjectWithTag("MainCamera").transform, false);
        }
    }
}
