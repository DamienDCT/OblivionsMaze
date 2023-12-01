using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    // Singleton
    public static CameraManager instance;

    // Mise en place du singleton
    private void Awake(){
        if(instance == null){
            instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    // Référence à la première caméra
    [SerializeField]
    private Camera cameraRoom1;
    // Référence à la deuxième caméra
    [SerializeField]
    private Camera cameraRoom2;

    // Référence à la caméra principale (CinemachineCamera)
    private Camera mainCamera;

    // On setup les variables
    void Start(){
        mainCamera = Camera.main;
        mainCamera.tag = "Untagged";
        mainCamera.gameObject.SetActive(false);
        cameraRoom1.gameObject.tag = "MainCamera";
        cameraRoom1.gameObject.SetActive(true);
    }

    // Pour reset les cameras, on doit retirer le tag MainCamera et les désactiver
    // Afin de réactiver la CinemachineCamera
    public void ResetCameras(){
        if(cameraRoom1 != null){
            cameraRoom1.gameObject.tag = "Untagged";
            cameraRoom1.gameObject.SetActive(false);
        }
        if(cameraRoom2 != null){
            cameraRoom2.gameObject.tag = "Untagged";
            cameraRoom2.gameObject.SetActive(false);
        }
        mainCamera.tag = "MainCamera";
        mainCamera.gameObject.SetActive(true);
    }

    // Méthode appelée pour setup la deuxième camera
    public void SetSecondCamera(){
        cameraRoom1.tag = "Untagged";
        cameraRoom1.gameObject.SetActive(false);
        cameraRoom2.tag = "MainCamera";
        cameraRoom2.gameObject.SetActive(true);
    }
}
