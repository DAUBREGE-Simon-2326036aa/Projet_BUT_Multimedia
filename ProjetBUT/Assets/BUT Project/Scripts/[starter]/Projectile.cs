using UnityEngine;

namespace BUT
{
    public class Projectile : MonoBehaviour
    {
        public float vitesse = 20f;
        public float dureeDeVie = 3f;

        void Start()
        {
            Destroy(gameObject, dureeDeVie);
        }

        void Update()
        {
            transform.Translate(Vector3.forward * vitesse * Time.deltaTime);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Enemy"))
            {
                Destroy(other.gameObject); 
                Destroy(gameObject);      
            }
            else if (other.CompareTag("Player"))
            {
                return;
            }
            else if (other.isTrigger)
            {
                return;
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}