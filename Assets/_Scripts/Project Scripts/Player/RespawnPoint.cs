using UnityEngine;

[RequireComponent(typeof(Collider))]
public class RespawnPoint : MonoBehaviour
{
    private enum CoordinateSource
    {
        TransformPosition,
        ColliderPosition,
        Vector3Variable
    }
    [SerializeField] private CoordinateSource _coordinateSource;
    [SerializeField] private Vector3 _respawnCoordinates;
    [SerializeField] private bool _resetPlayerLives = true;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        switch (_coordinateSource)
        {
            default:
            case CoordinateSource.TransformPosition:
                RespawnManager.Instance.SetRespawnPoint(transform.position, this);
                break;
            case CoordinateSource.ColliderPosition:
                RespawnManager.Instance.SetRespawnPoint(transform.position + GetComponent<Collider>().bounds.center, this);
                break;
            case CoordinateSource.Vector3Variable:
                RespawnManager.Instance.SetRespawnPoint(_respawnCoordinates, this);
                break;
        }

        if (!_resetPlayerLives)
            return;

        other.TryGetComponent(out PlayerHealthManager health);
        if (health)
            health.ResetLives();
    }
}
