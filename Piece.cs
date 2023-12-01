using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Piece : MonoBehaviour
{   
    // Référence à l'effet de la lumière de la pièce
    [SerializeField]
    private GameObject lightEffect;
    // Booléen pour indiquer si la pièce est une pièce qui apparaît dans la TestArea ou non
    [SerializeField]
    private bool isTestAreaCoin;

    // Méthode qui sert à afficher les VFX selon ce que le joueur souhaite afficher    
    void Start(){
        InvokeRepeating("UpdateVFX", 0f, 0.5f);
    }

    private void UpdateVFX(){
        // On active les VFX ou non selon les paramètres du joueur
        lightEffect.SetActive(SettingsJSON.instance.settings.videoSettings.isVFXToggled);
    }

    // Si le joueur entre en contact avec la pièce
    public void OnTriggerEnter2D(Collider2D collider){
        if(collider.CompareTag("Player")){
            AudioManager.instance.Play("PieceKey");
            // Si c'est une pièce qui n'est pas dans la testarea
            if(!isTestAreaCoin){
                // On incrémente le nombre de pièces du joueur, on met à jour son fichier de jeu et on retourne au menu principal
                PlayerPowerup.instance.IncrementNbCoins();
                SaveGameData.instance.UpdatePlayerDataFile();
                GameObject.FindGameObjectWithTag("IngameUIPrefab").GetComponent<PauseMenu>().GoToMainMenu();
            } else {
                // Sinon on retourne à la testarea
                GameObject.FindGameObjectWithTag("IngameUIPrefab").GetComponent<PauseMenu>().GoToTestArea();
            }
        }
    }
}
