using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

namespace BUT
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        [Header("Interface Utilisateur")]
        public TextMeshProUGUI textScore;
        public GameObject panelVictoire;
        public GameObject panelGameOver;

        [Header("Données de Jeu")]
        public int score = 0;
        public bool hasKey = false;
        public bool porteVaisseauOuverte = false;

        public string nomSceneMenu = "Menu";

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        private void Start()
        {
            if (panelVictoire != null) panelVictoire.SetActive(false);
            if (panelGameOver != null) panelGameOver.SetActive(false);

            UpdateUI();
        }

        public void AddScore(int amount)
        {
            score += amount;
            UpdateUI();
        }

        public void PickupKey()
        {
            hasKey = true;
        }

        public void WinLevel()
        {
            if (panelVictoire != null)
                panelVictoire.SetActive(true);

            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        public void DeclencherMort()
        {
            if (panelGameOver != null)
                panelGameOver.SetActive(true);

            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        public void BoutonRecommencer()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void BoutonRetourMenu()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(nomSceneMenu);
        }

        private void UpdateUI()
        {
            if (textScore != null)
            {
                textScore.text = "Pièces : " + score;
            }
        }

        public void PlaySound(AudioClip clip)
        {
            AudioSource source = GetComponent<AudioSource>();

            if (source != null && clip != null)
            {
                source.PlayOneShot(clip);
            }
        }
    }
}