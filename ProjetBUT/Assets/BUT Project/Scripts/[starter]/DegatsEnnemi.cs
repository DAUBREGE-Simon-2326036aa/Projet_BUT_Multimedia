using UnityEngine;

namespace BUT
{
    public class DegatsEnnemi : MonoBehaviour
    {
        public int degats = 10;
        public float delaiEntreDegats = 1.0f;
        private float tempsDernierCoup = 0f;

        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("Player") && Time.time > tempsDernierCoup + delaiEntreDegats)
            {
                VieJoueur vie = other.GetComponent<VieJoueur>();
                if (vie != null)
                {
                    vie.RecevoirDegats(degats);
                    tempsDernierCoup = Time.time;
                }
            }
        }
    }
}