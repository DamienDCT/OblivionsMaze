using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchRoomTheme : MonoBehaviour
{
    //référence au boss du monde 2
    [SerializeField]
    private BossWorld2 bossWorld2;

    //méthode pour changer le thème de la salle du boss lors du passage à la seconde phase
    public void SwitchTheme(){
        bossWorld2.SetSecondPhaseTheme();
    }
}
