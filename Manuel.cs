using UnityEngine;
using UnityEngine.UI;

public class Manuel : MonoBehaviour
{
    // Référence aux pages du manuel
    [SerializeField]
    private GameObject[] pagesManuel;
    // Référence à l'inputField du canvas
    [SerializeField]
    private InputField inputField;
    // Référence aux boutons previous et next page
    [SerializeField]
    private Button previousButton;
    [SerializeField]
    private Button nextButton;

    [SerializeField]
    private Text leftPageText;
    [SerializeField]
    private Text rightPageText;
    // Entier pour savoir à quel page on est
    private int currentPage;

    private void Update(){
        if((currentPage % 2) == 0)
        {
            leftPageText.text = ""+currentPage;
            rightPageText.text = ""+(currentPage + 1);
        } else {
            leftPageText.text = ""+(currentPage-1);
            rightPageText.text = ""+currentPage;
        }
        // Si on est sur la première page
        if(currentPage == 0 || currentPage == 1){
            // On active les bons boutons
            previousButton.gameObject.SetActive(false);
            nextButton.gameObject.SetActive(true);
        // Si on est sur la dernière page
        } else if(currentPage == (pagesManuel.Length-2) || currentPage == (pagesManuel.Length-1))
        {
            // on active les bons boutons
            previousButton.gameObject.SetActive(true);
            nextButton.gameObject.SetActive(false);
        } else {
            // Sinon, on active les deux
            previousButton.gameObject.SetActive(true);
            nextButton.gameObject.SetActive(true);
        }
    }

    // Au démarrage, on active la première page du manuel et on restreint la zone de texte de manière
    // à voir que des chiffres
    private void Start(){
        inputField.characterValidation = InputField.CharacterValidation.Integer;
        DisableAllPages();
        ActivePage(0);
    }

    // Méthode pour désactiver toutes les pages du manuel
    private void DisableAllPages(){
        foreach(GameObject page in pagesManuel){
            page.SetActive(false);
        }
    }

    // Méthode utilisée pour activer une page
    // indexPage = index de la page à activer
    private void ActivePage(int indexPage){
        // Si indexPage est pair (=> si c'est une page qui est à gauche)
        if((indexPage % 2) == 0){
            // On active indexPage et la page à sa droite
            pagesManuel[indexPage].SetActive(true);
            pagesManuel[indexPage+1].SetActive(true);
        } else {
            // Sinon, c'est qu'on a saisit une page qui est à droite du livre
            // Alors on active indexPage et indexPage-1 qui est la page à sa gauche
            pagesManuel[indexPage-1].SetActive(true);
            pagesManuel[indexPage].SetActive(true);
        }
    }

    // Méthode appelée lors du clic sur le bouton pour aller à une page directement
    public void ClickGoToButton(){

        AudioManager.instance.Play("ClickUI");
        // S'il n'y a pas de texte dans l'input field, on return
        if(inputField.textComponent.text == "")
            return;
        // Sinon on récupère l'entier dans la zone de texte
        int indexOnField = int.Parse(inputField.textComponent.text);
        // On vérifie si l'entier est bien un numéro de page valide
        if(indexOnField >= 0 && indexOnField < pagesManuel.Length)
        {
            // Si c'est le cas, on met à jour le numéro de la page, on désactive toutes les pages pour activer la bonne page
            currentPage = indexOnField;
            DisableAllPages();
            ActivePage(currentPage);
        }
    }

    public void ClickSummary(int nbPage){
        // récup info input field
        AudioManager.instance.Play("ClickUI");
        currentPage = nbPage;
        DisableAllPages();
        ActivePage(currentPage);
    }

    public void GoToPage(int index){
        AudioManager.instance.Play("ClickUI");
        if((currentPage + index) >= (pagesManuel.Length+1) || (currentPage + index < 0))
            return; 
        if((currentPage % 2) == 1)
        {
            currentPage -= 1;
        }
        currentPage += index;
        DisableAllPages();
        ActivePage(currentPage);
    }

}
