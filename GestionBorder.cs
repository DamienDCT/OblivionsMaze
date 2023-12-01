using UnityEngine;
using Cinemachine;

public class GestionBorder : MonoBehaviour
{
    // Référence au confiner de caméra d'un niveau
    [SerializeField]
    private PolygonCollider2D confiner;
    // Référence au confiner de la partie haute (que pour la testarea)
    [SerializeField]
    private PolygonCollider2D topConfiner;
    // Référence à la Camera Cinemachine
    private CinemachineConfiner cinemachineConfiner;

    // Booléen servant à savoir si le script est sur la scène de TestArea
    [SerializeField]
    private bool isTestAreaBorder;

    private void Start()
    {
        // Initialisation des variables et du confiner
        cinemachineConfiner = GameObject.FindGameObjectWithTag("VirtualCamera").GetComponent<Cinemachine.CinemachineConfiner>();
        cinemachineConfiner.m_BoundingShape2D = confiner;
    }

    // A chaque frame, pour la testarea uniquement, on vérifie où est le joueur pour positionner le bon confiner
    private void FixedUpdate(){
        if(isTestAreaBorder){
            if(transform.position.y > PlayerMovement.instance.gameObject.transform.position.y){
                cinemachineConfiner.m_BoundingShape2D = confiner;
            } else {
                cinemachineConfiner.m_BoundingShape2D = topConfiner;
            }
        }
    }
}
