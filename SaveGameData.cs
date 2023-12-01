using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using System;

public class SaveGameData : MonoBehaviour
{
    //instance pour les données à sauvegarder
    public static SaveGameData instance;

    //référence aux informations du joueur
    public PlayerInfoData playerInfoData;

    private void Awake(){
        //on fait un Singleton
        if(instance == null){
            instance = this;
        } else {
            Destroy(gameObject);
        }
        playerInfoData = new PlayerInfoData();
    }

    //on charge les données du joueur
    void Start(){
        LoadPlayerDataFile();
    }

    //méthode pour mettre à jour les données du jeu
    private void UpdateGameDataValues(){
        playerInfoData.nbCoins = PlayerPowerup.instance.GetNbCoins();
        playerInfoData.nbLives = PlayerHealth.instance.nbLives;
    }

    //méthode pour mettre à jour les données du joueur
    public void UpdatePlayerDataFile()
    {
        if(!CreativeMode.instance.isCreativeActivated){
            string filePath = Application.persistentDataPath + "/PlayerInfoData.json";
            UpdateGameDataValues();
            string settingsData = JsonUtility.ToJson(playerInfoData);
            File.WriteAllText(filePath, settingsData);
        }
    }

    //méthode pour charger les données du joueur
    public void LoadPlayerDataFile()
    {
        if(!CreativeMode.instance.isCreativeActivated){
            string filePath = Application.persistentDataPath + "/PlayerInfoData.json";
            if(File.Exists(filePath)){
                string playerInfoDataJSON = "";
                try
                {
                    //on cherche les données du joueur
                    playerInfoDataJSON = File.ReadAllText(filePath);
                } catch(IOException e)
                {
                    UpdatePlayerDataFile();
                    LoadPlayerDataFile();
                    return;
                }

                playerInfoData = JsonUtility.FromJson<PlayerInfoData>(playerInfoDataJSON);
            } else {
                UpdatePlayerDataFile();
                return;
            }
            //on modifie son nombre de pièces et de vie en conséquence
            PlayerPowerup.instance.SetNbCoins(playerInfoData.nbCoins);
            PlayerHealth.instance.nbLives = playerInfoData.nbLives;
        }
    }
}

[System.Serializable]
public class PlayerInfoData{
    //correspond au nombre de pièce du joueur
    public int nbCoins;
    //correspond au nombre de vie restantes du joueur
    public int nbLives;

    public PlayerInfoData(){
        //Au départ, le joueur aura 0 pièces et 5 vies
        nbCoins = 0;
        nbLives = 5;
    }
}
