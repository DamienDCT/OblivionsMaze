using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public string WorldSelectMenu;

    [SerializeField]
    private Toggle creativeToggle;

    private void Start(){
        creativeToggle.isOn = CreativeMode.instance.isCreativeActivated;
    }

    public void GoToWorldSelect()
    {
        SettingsJSON.instance.LoadSettingsFile();
        SceneManager.LoadScene(WorldSelectMenu);
    }

    public void GoToTestArea(){
        AudioManager.instance.Play("ClickUI");
        SettingsJSON.instance.LoadSettingsFile();
        LevelLoader.instance.LoadLevel("TestArea", "", true, null);
    }
}
