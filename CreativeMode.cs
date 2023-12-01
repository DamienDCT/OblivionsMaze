using UnityEngine;

public class CreativeMode : MonoBehaviour
{
    // Singleton
    public static CreativeMode instance;

    // Si le booléen est vrai, le mode créatif est activé, sinon, il ne l'est pas
    public bool isCreativeActivated;

    // Mise en place du singleton
    private void Awake(){
        if(instance == null)
            instance = this;
        else 
            Destroy(gameObject);
    }
    
    // Si on appuie sur le toggle du menu principal, on change la valeur du booléen
    public void ChangeCreativeMode(bool value){
        AudioManager.instance.Play("ClickUI");
        isCreativeActivated = value;
    }
}
