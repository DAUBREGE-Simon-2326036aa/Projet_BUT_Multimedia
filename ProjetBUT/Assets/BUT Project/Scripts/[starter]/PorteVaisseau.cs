using UnityEngine;
using UnityEngine.InputSystem;

namespace BUT
{
    public class PorteVaisseau : MonoBehaviour
    {
        [Header("Réglages de la Porte")]
        public GameObject porteMobile;
        public Vector3 angleOuverture = new Vector3(-49.40f, 0f, 0f);
        public float vitesseOuverture = 2f;
        public int coutPieces = 5;

        [Header("Audio")]
        public AudioClip sonOuverture;
        public AudioClip sonVerrouille;

        private bool joueurEstDevant = false;
        private bool estOuverte = false;
        private AudioSource audioSource;

        private void Start()
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        private void Update()
        {
            if (joueurEstDevant && !estOuverte && Keyboard.current.eKey.wasPressedThisFrame)
            {
                TenterOuverture();
            }

            if (estOuverte && porteMobile != null)
            {
                Quaternion targetRotation = Quaternion.Euler(angleOuverture);
                porteMobile.transform.localRotation = Quaternion.Lerp(
                    porteMobile.transform.localRotation,
                    targetRotation,
                    Time.deltaTime * vitesseOuverture
                );
            }
        }

        private void TenterOuverture()
        {
            int scoreActuel = GameManager.Instance.score;
            bool aLaCle = GameManager.Instance.hasKey;

            if (aLaCle && scoreActuel >= coutPieces)
            {
                Debug.Log("Systèmes réparés. Ouverture de la rampe...");
                estOuverte = true;

                if (sonOuverture) audioSource.PlayOneShot(sonOuverture);

                Invoke("DebloquerOrdinateur", 1.5f);
            }
            else
            {
                if (!aLaCle) Debug.Log("Accès refusé ! Il manque la puce.");
                else if (scoreActuel < coutPieces) Debug.Log($"Manque de pièces ({scoreActuel}/{coutPieces})");

                if (sonVerrouille) audioSource.PlayOneShot(sonVerrouille);
            }
        }

        private void DebloquerOrdinateur()
        {
            GameManager.Instance.porteVaisseauOuverte = true;
            Debug.Log("Ordinateur de bord : EN LIGNE (Prêt à téléporter)");
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                joueurEstDevant = true;
                if (!estOuverte) Debug.Log($"Appuie sur [E] pour ouvrir (Coût: {coutPieces} pièces)");
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player")) joueurEstDevant = false;
        }
    }
}