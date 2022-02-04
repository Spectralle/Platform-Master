using UnityEngine;

public class DeathZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.TryGetComponent(out PlayerHealthManager ph);
            ph?.TakeDamage(1);
        }
    }
}
