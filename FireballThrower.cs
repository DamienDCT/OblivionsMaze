using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballThrower : MonoBehaviour
{
    //la borne minimum (resp. maximum) correspond au temps minimum (resp. maximum) entre chaque tir de boule de feu
    [SerializeField]
    private float borneTempsMin;
    [SerializeField]
    private float borneTempsMax;
    [SerializeField]
    private GameObject fireballPrefab;

    //booléen qui sert à savoir si le lanceur est situé à gauche ou non
    [SerializeField]
    private bool isLeft;

    //Méthode qui appelle la coroutine pour tirer aléatoirement
    public void StartLaunch(){
        StartCoroutine(RandomThrow());
    }

    // Coroutine qui permet de tirer la boule de feu à un timing aléatoire entre les bornes
    private IEnumerator RandomThrow(){
        while(true){
            yield return new WaitForSecondsRealtime(Random.Range(borneTempsMin, borneTempsMax));
            Fireball go = Instantiate(fireballPrefab).GetComponent<Fireball>();
            go.transform.position = transform.position;
            //si le lanceur est à gauche, on tire à droite, et inversement
            if(isLeft){
                go.LaunchRight();
            } else {
                go.LaunchLeft();
            }
        }
    }
}
