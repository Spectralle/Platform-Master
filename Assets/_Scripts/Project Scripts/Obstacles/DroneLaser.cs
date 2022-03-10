using UnityEngine;

public class DroneLaser : MonoBehaviour
{
    [SerializeField] private float _range = 15f;
    private LineRenderer _lineRenderer;


    private void Awake() => _lineRenderer = GetComponent<LineRenderer>();

    void Update()
    {
        if (Physics.Raycast(transform.position, transform.up, out RaycastHit hit, _range))
        {
            _lineRenderer.SetPosition(1, new Vector3(0, hit.distance));
            hit.transform.TryGetComponent(out PlayerHealthManager health);
            if (health)
                health.TakeDamage(1);
        }
        else
            _lineRenderer.SetPosition(1, new Vector3(0, _range));
    }
}
