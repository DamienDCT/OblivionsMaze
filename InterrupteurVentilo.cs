using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class InterrupteurVentilo : MonoBehaviour
{
    // Référence aux ventilateurs à inverser lors de l'appui sur l'interrupteur
    [SerializeField]
    private Ventilateur[] ventilateurs;
    // Référence à la boîte de collision de l'interrupteur
    [SerializeField]
    private BoxCollider2D boxCollider2D;
    // Référence aux sprites de l'interrupteur
    [SerializeField]
    private Sprite spriteClose;
    [SerializeField]
    private Sprite spriteOpen;
    // Rendu de l'interrupteur
    [SerializeField]
    private SpriteRenderer spriteRenderer;

    // Référence à la camera cinemachine
    private CinemachineVirtualCamera cinemachineVirtualCamera;

    private void Start()
    {
        // On met à jour les variables
        cinemachineVirtualCamera = GameObject.FindGameObjectWithTag("VirtualCamera").GetComponent<CinemachineVirtualCamera>();
    }


    public void OnCollisionEnter2D(Collision2D collision2D)
    {
        // Si le joueur entre en contact avec l'interrupteur
        if (collision2D.collider.CompareTag("Player"))
        {
            // On regarde les points de contacts de la collision
            foreach (ContactPoint2D contact in collision2D.contacts)
            {
                if (contact.normal.y <= -.3f)
                {
                    // On fait l'action de l'interrupteur
                    PushButton();
                    StartCoroutine(SwitchVentilo());
                    Invoke("PullButton", 1f);
                }
            }
        }
    }

    // Méthode servant à appuyer sur le bouton (effets visuels)
    private void PushButton()
    {
        AudioManager.instance.Play("Interrupteur");
        boxCollider2D.enabled = false;
        spriteRenderer.sprite = spriteClose;
    }

    // Méthode servant à remettre le bouton en place (effets visuels)
    private void PullButton()
    {
        boxCollider2D.enabled = true;
        spriteRenderer.sprite = spriteOpen;
    }

    // Méthode servant à gérer les ventialteurs liés à l'interrupteur    
    private IEnumerator SwitchVentilo()
    {
        // On freeze les positions du joueur
        PlayerMovement.instance.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
        yield return new WaitForSecondsRealtime(.5f);
        PlayerMovement.instance.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePosition | RigidbodyConstraints2D.FreezeRotation;
        // Pour chaque ventilateur, on focus la caméra dessus pendant 2.5s le temps que le joueur voit l'action se faire
        foreach (Ventilateur ventilo in ventilateurs)
        {
            cinemachineVirtualCamera.Follow = ventilo.gameObject.transform;
            cinemachineVirtualCamera.LookAt = ventilo.gameObject.transform;
            yield return new WaitForSecondsRealtime(1.5f);
            ventilo.Switch();
            yield return new WaitForSecondsRealtime(1f);
        }

        // On remet les bonnes contraintes du joueur et la caméra focus de nouveau le joueur
        PlayerMovement.instance.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
        yield return new WaitForSecondsRealtime(.5f);
        cinemachineVirtualCamera.Follow = PlayerMovement.instance.gameObject.transform;
        cinemachineVirtualCamera.LookAt = PlayerMovement.instance.gameObject.transform;
    }
}
