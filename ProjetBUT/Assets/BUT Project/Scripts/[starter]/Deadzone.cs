using UnityEngine;

namespace BUT
{
    public class DeadZone : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                GameManager.Instance.DeclencherMort();
            }
        }
    }
}