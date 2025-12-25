using UnityEngine;
using UnityEngine.InputSystem; // Nécessaire pour la touche E

namespace BUT
{
    public class PorteVaisseau : MonoBehaviour
    {
        [Header("Réglages de la Porte")]
        public GameObject porteMobile; 
        public Vector3 angleOuverture = new Vector3(-49.40f, 0f, 0f); // L'angle final quand elle est ouverte
        public float vitesseOuverture = 2f;

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
            // Si le joueur est devant + appuie sur E + la porte n'est pas déjà ouverte
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
            if (GameManager.Instance.hasKey)
            {
                Debug.Log("Systèmes réparés. Ouverture de la rampe...");
                estOuverte = true;
                if (sonOuverture) audioSource.PlayOneShot(sonOuverture);
            }
            else
            {
                Debug.Log("Accès refusé ! Il manque la batterie (Key).");
                if (sonVerrouille) audioSource.PlayOneShot(sonVerrouille);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                joueurEstDevant = true;
                if (!estOuverte) Debug.Log("Appuie sur [E] pour ouvrir");
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                joueurEstDevant = false;
            }
        }
    }
}