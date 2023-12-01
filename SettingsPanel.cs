using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsPanel : MonoBehaviour
{
    //instance au panel d'options
    public static SettingsPanel instance;
    void Awake()
    {
        //Singleton pour n'avoir qu'un panel d'options accessible de partout
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        //ne pas le détruire au changement de scène
        DontDestroyOnLoad(this);
    }
}
