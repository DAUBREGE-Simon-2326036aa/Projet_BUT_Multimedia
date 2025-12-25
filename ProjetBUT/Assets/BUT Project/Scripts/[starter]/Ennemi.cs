using UnityEngine;
using UnityEngine.AI;

namespace BUT
{
    public class Ennemi : MonoBehaviour
    {
        [Header("Réglages")]
        public Transform cible;
        private NavMeshAgent agent;

        void Start()
        {
            agent = GetComponent<NavMeshAgent>();

            if (cible == null)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null) cible = player.transform;
            }
        }

        void Update()
        {
            if (cible == null || agent == null) return;

            if (!agent.isOnNavMesh || !agent.isActiveAndEnabled)
            {
                return;
            }

            agent.SetDestination(cible.position);
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                Debug.Log("Touché !");
            }
        }
    }
}