using UnityEngine;
using UnityEngine.InputSystem; // Pour la touche E

namespace BUT
{
    public class OrdinateurDeBord : MonoBehaviour
    {
        [Header("Audio")]
        public AudioClip sonDecollage; 

        private bool joueurEstDevant = false;
        private bool estActive = false;

        private void Update()
        {
            if (joueurEstDevant && !estActive && Keyboard.current.eKey.wasPressedThisFrame)
            {
                LancerDecollage();
            }
        }

        private void LancerDecollage()
        {
            estActive = true;
            Debug.Log("Séquence de décollage initiée...");

            if (sonDecollage) AudioSource.PlayClipAtPoint(sonDecollage, transform.position);

            GameManager.Instance.WinLevel();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                joueurEstDevant = true;
                if (!estActive) Debug.Log("Appuie sur [E] pour décoller");
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