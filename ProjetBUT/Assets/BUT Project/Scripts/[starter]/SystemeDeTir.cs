using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

namespace BUT
{
    public class SystemeDeTir : MonoBehaviour
    {
        [Header("Réglages")]
        public GameObject projectilePrefab;
        public Transform pointDeTir;
        public float delaiEntreTirs = 1f;
        public float delaiAnimation = 1f; // Temps avant que la balle parte

        [Header("Audio")]
        public AudioClip sonTir;

        private float tempsDernierTir = 0f;
        private Animator animator;

        void Start()
        {
            animator = GetComponent<Animator>();
        }

        void Update()
        {
            if (Mouse.current.leftButton.wasPressedThisFrame && Time.time > tempsDernierTir + delaiEntreTirs)
            {
                Tirer();
            }
        }

        void Tirer()
        {
            tempsDernierTir = Time.time;

            if (animator != null)
                animator.SetTrigger("Shoot");

            StartCoroutine(TirDecale());
        }

        IEnumerator TirDecale()
        {
            yield return new WaitForSeconds(delaiAnimation);

            if (pointDeTir != null && projectilePrefab != null)
            {
                Instantiate(projectilePrefab, pointDeTir.position, pointDeTir.rotation);
            }

            if (sonTir) GameManager.Instance.PlaySound(sonTir);
        }
    }
}