using UnityEngine;

public class DeathZone : MonoBehaviour
{
    [SerializeField, Min(0)] private int _lifeDamage = 1;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.TryGetComponent(out PlayerHealthManager health);
            if (health)
                health.TakeDamage(_lifeDamage);
        }
    }
}
