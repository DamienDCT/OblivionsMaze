using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameoverPanelSpawn : MonoBehaviour
{
    // Script servant à rediriger le panel de Gameover dans un autre script
    // Référence au panel de gameover
    [SerializeField]
    private GameObject gameoverPanel;

    // Getter servant à récupérer le panel de gameover depuis un autre script
    public GameObject getGameoverPanel(){
        return gameoverPanel;
    }
}
