using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class ChangeWorld : MonoBehaviour
{
    public int currentWorld = 1; // Variable du monde actuel affich�
    public World[] worldArray; // Tableau contenant tous les mondes disponibles
    public TextMeshProUGUI titleWorld; // Texte pour afficher le num�ro du monde actuellement affich�

    [SerializeField]
    private Animator animatorWorld2;

    [Header("World 1 sprites/image")]
    [SerializeField]
    private SpriteRenderer spriteWorld1;
    [SerializeField]
    private Sprite spriteWorld1Day;
    [SerializeField]
    private Sprite spriteWorld1Night;

    [SerializeField]
    private Image imageArrowLeft;
    [SerializeField]
    private Image imageArrowRight;
    [SerializeField]
    private Sprite arrowRightDark;
    [SerializeField]
    private Sprite arrowRightLight;
    [SerializeField]
    private Sprite arrowLeftDark;
    [SerializeField]
    private Sprite arrowLeftLight;
    private void Start()
    {
        Invoke("UpdateDayNightTheme", 0.01f);
        // On affiche le monde
        AfficheWorld();
        // On regarde si le GameData (qui doit être sauvegardé entre les différentes scènes du jeu) existe
        GameObject menuUI = GameObject.Find("GameData");
        if (menuUI)
        {
            // Si il existe, on récupère notre joueur
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player)
            {
                // Et on lui reset ses powerups entre deux niveaux
                player.GetComponent<PlayerPowerup>().ResetPowerup();
            }
            // Puis on désactive le gameData pour éviter les conflits entre scènes
            menuUI.SetActive(false);
        } 
    }

    // Méthode appelée pour modifier le thème selon les préférences du joueur
    private void UpdateDayNightTheme(){
        if(SettingsJSON.instance.settings.videoSettings.isDayToggled){
            spriteWorld1.sprite = spriteWorld1Day;
            imageArrowLeft.sprite = arrowLeftDark;
            imageArrowRight.sprite = arrowRightDark;
            animatorWorld2.SetTrigger("Jour");
        } else {
            spriteWorld1.sprite = spriteWorld1Night;
            imageArrowLeft.sprite = arrowLeftLight;
            imageArrowRight.sprite = arrowRightLight;
            animatorWorld2.SetTrigger("Nuit");
        }
    }

    public void PreviousWorld()
    {
        // Calcul du prochain indice de monde
        currentWorld = Mathf.Abs(((currentWorld - 1) % (worldArray.Length)));
        // Si l'indice est 0, on revient au dernier indice du monde
        if (currentWorld == 0) currentWorld = worldArray.Length;
        // Puis on affiche le monde
        AfficheWorld();
    }

    private void Update(){
        if(Input.GetKeyDown(KeyCode.Escape)){
            SceneManager.LoadScene("MainMenu");
        }
    }

    public void NextWorld()
    {
        // Calcul du prochain indice de monde
        currentWorld = ((currentWorld + 1) % (worldArray.Length+1));
        // Si l'indice est 0, on met l'indice à 1
        if (currentWorld == 0) currentWorld = 1;
        // Puis on affiche le monde
        AfficheWorld();
    }

    public void AfficheWorld()
    {
        // On change le titre du monde
        titleWorld.text = "Monde " + currentWorld;
        // On retire l'affichage de tous les mondes
        clearPanels();
        // On affiche le panel du monde actuel
        affichePanels();
        // On met à jour le thème 
        UpdateDayNightTheme();
    }    

    // Méthode pour clear les panels des mondes
    private void clearPanels()
    {
        // Pour chaque monde dans le tableau des mondes
        foreach(World world in worldArray)
        {
            // On désactive le worldPanel associé au monde "world"
            world.worldPanel.SetActive(false);
        }
    }

    // Méthode pour afficher le méthode actuel
    private void affichePanels()
    {
        // On va chercher le worldPanel associé et on l'affiche
        worldArray[currentWorld-1].worldPanel.SetActive(true);
    }
   
}

[System.Serializable]
public struct World
{
    public GameObject worldPanel;
}
