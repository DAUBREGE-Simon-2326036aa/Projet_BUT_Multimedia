using UnityEngine;
using UnityEngine.SceneManagement; 

public class MainMenu : MonoBehaviour
{
    public string nomSceneJeu = "Level1";

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void LancerJeu()
    {
        Debug.Log("Lancement du jeu...");
        SceneManager.LoadScene(nomSceneJeu);
    }

    public void QuitterJeu()
    {
        Debug.Log("Fermeture du jeu...");
        Application.Quit();
    }
}