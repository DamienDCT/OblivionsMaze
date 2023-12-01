using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class LevelLoader : MonoBehaviour
{
    // Singleton
    public static LevelLoader instance;

    // Référence aux textes affichés sur l'écran de chargement
    [SerializeField]
    private Text nbLifeAmountText;
    [SerializeField]
    private Text nameSceneLoadedText;

    // Initialisation du singleton
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Référence au slider de chargement
    [SerializeField]
    private Slider loadingSlider;
    // Référence au panel de l'écran de chargement
    [SerializeField]
    private GameObject loadingPanel;
    // Référence à un potentiel panel à désactiver pendant le chargement
    [SerializeField]
    private GameObject overpanel;
    // Booléen pour savoir si on est en train de charger une scène
    private bool isCurrentlyLoading;

    private void Start(){
        // initialisation de la variable
        isCurrentlyLoading = false;
    }

    // Méthode servant à charger un niveau
    // levelName = Le nom de la scène à charger
    // levelNameLoading = le nom de la scène à afficher lors de l'écran de chargement
    // loadGameData = savoir si on doit activer ou non le gameObject GameData dans le DontDestroyOnLoad
    // objectToDisable = GameObject a désactivé pendant le chargement de la scène (visuel ou non)
    public void LoadLevel(string levelName, string levelNameLoading, bool loadGameData, GameObject objectToDisable)
    {
        // Si on est pas en train de charger une scène
        if(!isCurrentlyLoading){
            // On stop la musique en cours s'il y en a une
            GameObject go = GameObject.FindGameObjectWithTag("Music");
            if(go != null)
            {
                go.GetComponent<AudioSource>().Stop();
            }
            // On démarre la coroutine de chargement de scène de manière asynchrone
            StartCoroutine(LoadAsynchronousely(levelName, loadGameData, objectToDisable));
            // On met à jour les variables et l'affichage
            nameSceneLoadedText.text = levelNameLoading;
            nbLifeAmountText.text = ""+PlayerHealth.instance.nbLives;
            loadingPanel.SetActive(true);
            if(objectToDisable != null){
                objectToDisable.SetActive(false);
            }
            if (overpanel != null)
                overpanel.SetActive(false);
        }
    }

    private IEnumerator LoadAsynchronousely(string levelName, bool loadGameData, GameObject objectToDisable)
    {
        // On récupère la healthBar
        GameObject go = GameObject.FindGameObjectWithTag("HealthBar");
        // On met à jour la variable isCurrentlyLoading pour éviter de charger plein de scènes à la fois
        isCurrentlyLoading = true;
        // On démarre le chargement de scène en asynchrone
        AsyncOperation operation = SceneManager.LoadSceneAsync(levelName);
        // On désactive l'activation de la scène à la fin du chargement (pour avoir un temps minimal afin de voir les informations à l'écran)
        operation.allowSceneActivation = false;
        // Si on doit désactiver le GameData
        if (!loadGameData)
        {
            // On le désactive
            GameObject gameData = GameData.instance.gameObject;
            if (gameData != null) gameData.SetActive(false);
        }
        // Tant que le chargement de la scène n'est pas terminé
        while (!operation.isDone)
        {
            // On fait les calculs pour bien afficher la barre de progression
            float loadValue = Mathf.Clamp01(operation.progress / 0.9f);
            loadingSlider.value = loadValue;
            // Si le chargement des données en mémoire est terminé
            if(operation.progress >= 0.9f){
                // On attends 3 secondes (temps minimum de chargement)
                yield return new WaitForSecondsRealtime(3f);
                // Puis on active l'activation de la scène, et on réactive tous les gameObject avant de finir le chargement
                operation.allowSceneActivation = true;
                if(objectToDisable != null)
                    objectToDisable.SetActive(true);
                if(loadGameData){
                    GameObject gameData = GameData.instance.gameObject;
                    if (gameData != null) gameData.SetActive(true);
                }
                isCurrentlyLoading = false;
            }
            yield return null;
        }
    }
}

