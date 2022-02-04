using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class WorldButton : MonoBehaviour
{
    [SerializeField] private bool _startState;
    [SerializeField] private int _activationCost;
    [Space]
    [SerializeField] private UnityEvent _enterTriggerActions;
    [SerializeField] private UnityEvent _exitTriggerActions;
    [Space]
    [SerializeField] private UnityEvent _activeActions;
    [SerializeField] private UnityEvent _inactiveActions;

    private bool _isActive;
    private bool _playerNearby;
    private CoinManager _playerCoins;


    private void Awake() => _isActive = _startState;

    private void Update()
    {
        if (_playerNearby && Input.GetKeyDown(KeyCode.E))
        {
            if (_activationCost == 0)
                ToggleActivation();
            else if (_activationCost > 0 && _playerCoins && _playerCoins.CoinsFound >= _activationCost)
                ToggleActivation();
        }
    }

    private void ToggleActivation()
    {
        if (_isActive)
            _inactiveActions.Invoke();
        else
            _activeActions.Invoke();

        _isActive = !_isActive;
        if (_activationCost > 0 && _playerCoins)
            _playerCoins.UsedCoin(_activationCost);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _playerNearby = true;
            other.TryGetComponent(out _playerCoins);
            _enterTriggerActions.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _playerNearby = false;
            _exitTriggerActions.Invoke();
        }
    }
}
