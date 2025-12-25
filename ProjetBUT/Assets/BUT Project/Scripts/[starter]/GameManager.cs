using UnityEngine;
using TMPro;

namespace BUT
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        [Header("Interface Utilisateur")]
        public TextMeshProUGUI textScore; 
        public GameObject panelVictoire;  

        [Header("Données de Jeu")]
        public int score = 0;
        public bool hasKey = false;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        private void Start()
        {
            if (panelVictoire != null)
                panelVictoire.SetActive(false);

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
            Debug.Log("Batterie récupérée !");
        }

        public void WinLevel()
        {
            Debug.Log("Victoire !");

            if (panelVictoire != null)
                panelVictoire.SetActive(true);

            Time.timeScale = 0f;

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
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