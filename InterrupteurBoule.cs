using System.Collections;
using Cinemachine;
using System.Collections.Generic;
using UnityEngine;

public class InterrupteurBoule : MonoBehaviour
{

    [SerializeField]
    private GameObject boule;

    [SerializeField]
    private BoxCollider2D boxCollider2D;

    [SerializeField]
    private Sprite spriteClose;
    [SerializeField]
    private Sprite spriteOpen;

    [SerializeField]
    private SpriteRenderer spriteRenderer;

    private CinemachineVirtualCamera cinemachineVirtualCamera;

    //on initialise la caméra cinemachine en la cherchant dans la scène
    private void Start()
    {
        cinemachineVirtualCamera = GameObject.FindGameObjectWithTag("VirtualCamera").GetComponent<CinemachineVirtualCamera>();
    }

    //on vérifie que le joueur appuie sur le bouton par le haut
    public void OnCollisionEnter2D(Collision2D collision2D)
    {
        if (collision2D.collider.CompareTag("Player"))
        {
            foreach (ContactPoint2D contact in collision2D.contacts)
            {
                if (contact.normal.y <= -.3f)
                {
                    PushButton();
                    StartCoroutine(TpBoule());
                    Invoke("PullButton", 1f); //au bout d'une seconde on fait en sorte que le bouton se relache
                }
            }
        }
    }

    //Méthode appelée lorsqu'on presse le bouton
    private void PushButton()
    {
        AudioManager.instance.Play("Interrupteur");
        boxCollider2D.enabled = false;
        spriteRenderer.sprite = spriteClose;
    }

    //Méthode appelée lorsqu'on ne presse plus le bouton
    private void PullButton()
    {
        boxCollider2D.enabled = true;
        spriteRenderer.sprite = spriteOpen;
    }

    //on téléporte la boule assignée a l'interrupteur et on la suit avec la caméra pour voir où.
    private IEnumerator TpBoule()
    {
        //Pendant le changement de caméra on fait en sorte que le joueur ne puisse pas bouger
        PlayerMovement.instance.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
        yield return new WaitForSecondsRealtime(.5f);
        PlayerMovement.instance.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePosition | RigidbodyConstraints2D.FreezeRotation;
        cinemachineVirtualCamera.Follow = boule.transform;
        cinemachineVirtualCamera.LookAt = boule.transform;
        yield return new WaitForSecondsRealtime(1.5f);
        boule.GetComponent<Boule>().Tp();
        yield return new WaitForSecondsRealtime(1f);
        PlayerMovement.instance.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
        yield return new WaitForSecondsRealtime(.5f);
        cinemachineVirtualCamera.Follow = PlayerMovement.instance.gameObject.transform;
        cinemachineVirtualCamera.LookAt = PlayerMovement.instance.gameObject.transform;
    }

}

