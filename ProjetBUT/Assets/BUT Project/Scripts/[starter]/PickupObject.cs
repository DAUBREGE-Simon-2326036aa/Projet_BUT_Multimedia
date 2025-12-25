using UnityEngine;

namespace BUT
{
    public class PickupObject : MonoBehaviour
    {
        public enum PickupType { Piece, Trophee }

        [Header("Réglages")]
        public PickupType typeObjet;
        public float vitesseRotation = 50f;

        [Header("Audio")]
        public AudioClip sonRamassage;

        private void Update()
        {
            transform.Rotate(Vector3.up * vitesseRotation * Time.deltaTime);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                if (typeObjet == PickupType.Trophee)
                {
                    Debug.Log("COMMANDES ACTIVÉES : DÉCOLLAGE !");
                    GameManager.Instance.WinLevel(); 
                }
                else if (typeObjet == PickupType.Piece)
                {
                    GameManager.Instance.AddScore(1);
                }

                if (sonRamassage) GameManager.Instance.PlaySound(sonRamassage);

                Destroy(gameObject); 
            }
        }
    }
}