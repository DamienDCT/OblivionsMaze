using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Key : MonoBehaviour
{
    // Particule de glow de la clé
    [SerializeField]
    private ParticleSystem glowEffect;
    // Référence aux portes ouvrables par la clé
    [SerializeField]
    private GameObject[] doorsToOpen;
    // Référence à la camera cinemachine
    private CinemachineVirtualCamera cinemachineVirtualCamera;

    void Awake(){
        // On démarre l'appel à intervalle de 0.5s pour update les graphismes de la clé 
        // (au cas où le joueur change les VFX pendant un niveau)
        InvokeRepeating("UpdateVFX", 0f, 0.5f);
    }

    private void Start()
    {
        // On initialise les variables
        cinemachineVirtualCamera = GameObject.FindGameObjectWithTag("VirtualCamera").GetComponent<CinemachineVirtualCamera>();
    }

    private void UpdateVFX(){
        // On met à jour le glowEffect en fonction de la valeur des VFX dans les paramètres
        glowEffect.gameObject.SetActive(SettingsJSON.instance.settings.videoSettings.isVFXToggled);
    }

    // Si le joueur rentre en contact avec la clé
    public void OnTriggerEnter2D(Collider2D collider2D){
        if(collider2D.CompareTag("Player")){
            AudioManager.instance.Play("PieceKey");
            // Si la clé a au moins une porte à ouvrir, on démarre la coroutine
            if(doorsToOpen.Length > 0)
                StartCoroutine(SwitchDoors());
            // On heal le joueur de 50%
            PlayerHealth.instance.AddHealth(PlayerHealth.instance.maxHealth / 2);
            // On augmente le nombre de clé du joueur
            PlayerPowerup.instance.IncrementNbKeys();
            // Si le nombre de porte est égal à 0, on détruit la clé
            if(doorsToOpen.Length == 0)
                Destroy(gameObject);
        }
    }

    // Méthode servant à switch les portes en prenant une clé
    private IEnumerator SwitchDoors()
    {
        // On freeze les positions du joueur
        PlayerMovement.instance.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
        yield return new WaitForSecondsRealtime(.5f);
        PlayerMovement.instance.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePosition | RigidbodyConstraints2D.FreezeRotation;
        // Pour chaque porte, on focus la caméra dessus
        foreach (GameObject door in doorsToOpen)
        {
            cinemachineVirtualCamera.Follow = door.transform;
            cinemachineVirtualCamera.LookAt = door.transform;
            yield return new WaitForSecondsRealtime(1.5f);
            door.GetComponent<Door>().Switch();
            yield return new WaitForSecondsRealtime(3f);
        }

        // On remet les bonnes contraintes au joueur
        PlayerMovement.instance.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
        yield return new WaitForSecondsRealtime(.5f);
        // On refocus la caméra sur le joueur et on détruit la clé
        cinemachineVirtualCamera.Follow = PlayerMovement.instance.gameObject.transform;
        cinemachineVirtualCamera.LookAt = PlayerMovement.instance.gameObject.transform;
        yield return new WaitForSecondsRealtime(.2f);
        if(gameObject != null)
            Destroy(gameObject);
    }
}
