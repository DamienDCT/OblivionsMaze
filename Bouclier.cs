using System.Collections;
using UnityEngine;

public class Bouclier : Item
{
    // Booléen servant à indiquer si on peut utiliser le powerup
    private bool canUseItem;
    // Référence au joueur
    private GameObject player;

    // Référence à l'objet du bouclier qui doit apparaître devant le joueur
    [SerializeField]
    private GameObject shieldGameobject;

    void Awake()
    {
        // On setup les variables
        canUseItem = true;
        player = PlayerPowerup.instance.gameObject;
        shieldGameobject.SetActive(false);
    }

    void Update()
    {
        // On positionne le bouclier en fonction de là où regarde le joueur
        if(PlayerMovement.instance.isWatchingLeft)
            shieldGameobject.transform.position = new Vector2(player.transform.position.x - 0.5f, player.transform.position.y);
        else 
            shieldGameobject.transform.position = new Vector2(player.transform.position.x + 0.5f, player.transform.position.y);
    }

    // Méthode appelée à chaque utilisation du powerup
    public override void UseItem()
    {
        if(!canUseItem)
            return;

        // On joue le son et on sort le bouclier
        AudioManager.instance.Play("ExitShield");
        shieldGameobject.SetActive(true);
        // On démarre la coroutine pour le cooldown
        StartCoroutine(PlayerPowerup.instance.CooldownTimer(cooldownTimerItem));
        // On démarre la coroutine pour rentrer le bouclier
        StartCoroutine(ResetShield());
    }

     private IEnumerator ResetShield(){
        yield return new WaitForSecondsRealtime(.5f);
        shieldGameobject.SetActive(false);
    }
}
