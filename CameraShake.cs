using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraShake : MonoBehaviour
{
    // Référence à la caméra Cinemachine
    private CinemachineVirtualCamera cinemachineVirtualCamera;

    // Booléen servant à dire si le script est placé en composant de la CinemachineCamera ou d'une camera classique
    [SerializeField]
    private bool isNonCinemachineCamera;
    // Référence à une courbe pour le shake de la caméra classique
    [SerializeField]
    private AnimationCurve curve;
    // Temps total du shake
    private float shakeTimer = -1f;
    // Start is called before the first frame update
    void Awake()
    {
        // On setup la variable si besoin
        if(!isNonCinemachineCamera)
            cinemachineVirtualCamera = GetComponent<CinemachineVirtualCamera>();
    }

    // ShakeCamera pour la caméra Cinemachine
    public void ShakeCamera(float intensity, float time){
        // On récupère le composant pour ajouter l'amplitude 
        CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = intensity;

        // On met à jour le temps 
        shakeTimer = time;
    }

    private void Update(){
        // A chaque frame, on retire Time.deltaTime au shakeTimer, 
        if(shakeTimer > 0f){
            shakeTimer -= Time.deltaTime;
            // Si le temps est arrivé à 0, on retire l'amplitude du shake de la caméra
            if(shakeTimer <= 0f){
                CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
                cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 0f;
            }
        }
    }

    // Méthode de Shake pour les caméras classiques
    public IEnumerator NonCinemachineShake(float duration){
        // On récupère la position de base de la caméra
        Vector3 originalPos = transform.position;
        // On met à zéro le temps écoulé
        float tpsEcoule = 0.0f;
        // Tant que le temps écoulé est inférieur au temps total souhaité
        while(tpsEcoule < duration){
            // On incrémente le temps écoulé du temps depuis la dernière frame
            tpsEcoule += Time.deltaTime;
            // On utilise la courbe pour avoir une secousse convenable
            float strength = curve.Evaluate(tpsEcoule / duration);
            // On modifie la position de la caméra
            transform.position = originalPos + Random.insideUnitSphere * strength;
            yield return null;
        }
        // A la fin du temps, on remet la caméra à sa position initiale
        transform.position = originalPos;
    }
}
