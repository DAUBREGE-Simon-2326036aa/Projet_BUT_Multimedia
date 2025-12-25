using UnityEngine;
using UnityEngine.InputSystem; // Nécessaire car tu utilises le nouveau système d'input

namespace BUT
{
    public class ItemInteractif : MonoBehaviour
    {

        [Header("Audio")]
        public AudioClip sonRamassage;

        private bool joueurEstDansLaZone = false; 

        private void Update()
        {

            // Interaction : Si le joueur est proche ET appuie sur E
            if (joueurEstDansLaZone && Keyboard.current.eKey.wasPressedThisFrame)
            {
                RamasserObjet();
            }
        }

        // Détecte quand le joueur entre dans la zone
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                joueurEstDansLaZone = true;
                Debug.Log("Appuie sur [E] pour ramasser la clé du vaisseau");
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                joueurEstDansLaZone = false;
            }
        }

        private void RamasserObjet()
        {
            Debug.Log("Interaction réussie !");

            GameManager.Instance.PickupKey();

            if (sonRamassage) GameManager.Instance.PlaySound(sonRamassage);
            Destroy(gameObject);
        }
    }
}