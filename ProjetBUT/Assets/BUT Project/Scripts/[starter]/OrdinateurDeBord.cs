using UnityEngine;
using UnityEngine.InputSystem;

namespace BUT
{
    public class OrdinateurDeBord : MonoBehaviour
    {
        [Header("Configuration")]
        public Transform pointTeleportation;

        [Header("Audio")]
        public AudioClip sonInteraction;
        public AudioClip sonRefus; 

        private GameObject joueurDetecte;
        private bool joueurEstDevant = false;

        private void Update()
        {
            if (joueurEstDevant && joueurDetecte != null && Keyboard.current.eKey.wasPressedThisFrame)
            {
                if (GameManager.Instance.porteVaisseauOuverte == true)
                {
                    ActiverTeleportation();
                }
                else
                {
                    Debug.Log("L'ordinateur est verrouillé. Ouvrez d'abord la porte du vaisseau !");
                    if (sonRefus) AudioSource.PlayClipAtPoint(sonRefus, transform.position);
                }
            }
        }

        private void ActiverTeleportation()
        {
            Debug.Log("Téléportation...");
            if (sonInteraction) AudioSource.PlayClipAtPoint(sonInteraction, transform.position);

            CharacterController cc = joueurDetecte.GetComponent<CharacterController>();
            if (cc != null) cc.enabled = false;

            joueurDetecte.transform.position = pointTeleportation.position;
            joueurDetecte.transform.rotation = pointTeleportation.rotation;

            if (cc != null) cc.enabled = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                joueurEstDevant = true;
                joueurDetecte = other.gameObject;

                if (GameManager.Instance.porteVaisseauOuverte)
                    Debug.Log("Appuie sur [E] pour entrer");
                else
                    Debug.Log("Ordinateur Inactif (Porte fermée)");
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                joueurEstDevant = false;
                joueurDetecte = null;
            }
        }
    }
}