using UnityEngine;
using TMPro;

[RequireComponent(typeof(Rigidbody))]
public class CollectableController : MonoBehaviour
{
    [SerializeField, Range(0.5f, 10)] private float _floatSpeed = 1;
    [SerializeField, Range(0.1f, 2)] private float _floatScale = 1;
    [SerializeField] private Vector3 _rotateDirAndSpeed = Vector3.forward;
    [Space]
    [SerializeField, Range(1, 100)] private int _value;

    private float _originY;
    private TextMeshPro[] _text;


    private void Awake()
    {
        _originY = transform.position.y;
        _text = GetComponentsInChildren<TextMeshPro>();
        if (_value > 1)
            foreach (TextMeshPro t in _text)
                t.SetText(_value.ToString());
    }

    private void Update()
    {
        transform.position = new Vector3(transform.position.x, _originY + (Mathf.Sin(Time.time * _floatSpeed) * _floatScale), transform.position.z);
        transform.Rotate(_rotateDirAndSpeed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.TryGetComponent(out CoinManager player);
            player?.FoundCoin(_value);
            Destroy(gameObject);
        }
    }
}
