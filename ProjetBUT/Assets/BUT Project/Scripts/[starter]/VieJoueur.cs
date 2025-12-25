using UnityEngine;
using UnityEngine.SceneManagement; 

namespace BUT
{
    public class VieJoueur : MonoBehaviour
    {
        public int vieMax = 100;
        private int vieActuelle;

        void Start()
        {
            vieActuelle = vieMax;
        }

        public void RecevoirDegats(int montant)
        {
            vieActuelle -= montant;
            Debug.Log("Aïe ! Vie restante : " + vieActuelle);

            if (vieActuelle <= 0)
            {
                Mourir();
            }
        }

        void Mourir()
        {
            Debug.Log("GAME OVER");
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}