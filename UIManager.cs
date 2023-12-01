using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    //référence au panel du GameOver
    [SerializeField]
    private GameoverPanelSpawn prefabUIPanel;

    //référence à la zone de mort
    [SerializeField]
    private DeathDetection deathDetection;


    
    void Start(){
        //on créer une instance du panel
        GameoverPanelSpawn go = Instantiate(prefabUIPanel);
        //si la mort est détectée
        if(deathDetection != null){
            //on set le gameoverpanel et le menu pause
            deathDetection.SetGameoverPanel(go.getGameoverPanel());
            deathDetection.SetIngameGUIPanel(go.gameObject);
        }
        go.gameObject.transform.SetParent(GameObject.FindGameObjectWithTag("IngameGUI").transform, false);
    }
}
