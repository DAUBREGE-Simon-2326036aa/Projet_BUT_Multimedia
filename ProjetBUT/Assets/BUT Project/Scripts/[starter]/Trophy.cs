using UnityEngine;

public class Trophy : MonoBehaviour
{
    public float vitesseRotation = 100f;
    public AudioClip sonVictoire;

    void Update()
    {
        transform.Rotate(Vector3.up * vitesseRotation * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Victoire !");
            if (sonVictoire) AudioSource.PlayClipAtPoint(sonVictoire, transform.position);

            BUT.GameManager.Instance.WinLevel();
            Destroy(gameObject);
        }
    }
}