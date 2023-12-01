using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathDetection : MonoBehaviour
{
    // Référence au panel de GameOver
    [SerializeField]
    private GameObject gameoverPanel;
    // Référence au GUI de menu pause qui est instancié au début de chaque scène
    [SerializeField]
    private GameObject ingameGUIpanel;
    public GameObject spawnPoint; // GameObject du spawnPoint du joueur

    // Référence au nom de la scène afin de mettre à jour l'écran de chargement
    [SerializeField]
    private string levelSceneName;
    
    // Méthode pour set le panel de gameOver
    public void SetGameoverPanel(GameObject panel){
        gameoverPanel = panel;
    }

    // Méthode pour set le GUI de menu de pause
    public void SetIngameGUIPanel(GameObject panel){
        ingameGUIpanel = panel;
    }

    // Si un objet entre dans la zone de détection de mort
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Si cet objet est un joueur
        if (collision.CompareTag("Player"))
        {
            // On le fait mourir
            Death();

        } else if(collision.CompareTag("Ennemi")){
            // On le détruit
            Destroy(collision.gameObject);
        }
    }

    // Méthode qui sert à faire mourir le joueur
    public void Death()
    {
        if(!CreativeMode.instance.isCreativeActivated){
            // On décrémente de 1 le nombre de vies restantes
            PlayerHealth.instance.nbLives--;
        }
        // On remet la gravité du joueur au cas où il meurt en ayant la gravité inversée
        PlayerMovement.instance.SwapGravity(true);
        // On fait respawn le joueur
        spawnPoint.GetComponent<SpawnPoint>().Respawn();
        // On lui fait perdre son powerup
        PlayerPowerup.instance.ResetPowerup();
        // On update le fichier de donnéees du joueur
        SaveGameData.instance.UpdatePlayerDataFile();
        
        // Si le joueur a encore de la vie
        if(PlayerHealth.instance.nbLives >= 0)
        {
            // On supprime le IngameUI prefab pour éviter d'avoir des doublons dans le DontDestroyOnLoad
            Destroy(ingameGUIpanel);
            // Si on meurt dans une salle de boss, on reset les caméras
            if(CameraManager.instance != null){
                CameraManager.instance.ResetCameras();
            }
            // S'il existe un chargeur de niveau
            if(LevelLoader.instance != null){
                // On supprime le powerup du niveau
                PlayerPowerup.instance.ResetPowerup();
                // On récupère la barre de vie du canvas
                GameObject go = GameObject.FindGameObjectWithTag("HealthBar");
                // On recharge la scène actuelle
                LevelLoader.instance.LoadLevel(SceneManager.GetActiveScene().name, levelSceneName, true, go);
            }
        }
        // Et on regarde si le joueur est en gameOver
        CheckGameover();
    }

    // Méthode pour vérifier le gameOver
    public void CheckGameover()
    {
        // Si le joueur n'a plus de vies
        if(PlayerHealth.instance.nbLives < 0)
        {
            // On a un gameOver
            Gameover();
        }
    }

    // Méthode pour activer le panel du gameover
    private void Gameover()
    {
        // On remet le nombre de vie pour le prochain niveau
        PlayerHealth.instance.nbLives = PlayerHealth.instance.maxLives;
        // On retire une pièce au joueur
        PlayerPowerup.instance.DecrementNbCoins();
        // On met à jour les data du joueur
        SaveGameData.instance.UpdatePlayerDataFile();
        // On active le GUI du menu gameOver
        gameoverPanel.SetActive(true);
 
        // On fige le temps pour éviter que le joueur se déplace pendant que le menu est actif
        Time.timeScale = 0;
    }
}
