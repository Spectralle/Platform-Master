using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

[RequireComponent(typeof(Collider))]
public class TriggerInteractor : MonoBehaviour
{
    private enum StateType { OneState, TwoStates }
    [SerializeField, EnumToggleButtons] private StateType _stateType;
    [SerializeField, ShowIf("_stateType", StateType.TwoStates)] private bool _startingState;
    [SerializeField, Range(0,20)] private int _activationCost;
    [SerializeField] private KeyCode _interactKey = KeyCode.E;
    [Title("Collide with Trigger zone:")]
    [SerializeField] private UnityEvent _enterTriggerActions;
    [SerializeField] private UnityEvent _exitTriggerActions;
    [Title("Activate via input:")]
    [SerializeField, ShowIf("_stateType", StateType.TwoStates)] private UnityEvent _activeActions;
    [SerializeField, ShowIf("_stateType", StateType.TwoStates)] private UnityEvent _inactiveActions;
    [Title("Activate via input:")]
    [SerializeField, ShowIf("_stateType", StateType.OneState)] private UnityEvent _actions;

    private bool _isActive;
    private bool _playerNearby;
    private CoinManager _playerCoins;


    private void Awake() => _isActive = _stateType == StateType.TwoStates && _startingState;

    private void Update()
    {
        if (_playerNearby && Input.GetKeyDown(_interactKey))
            if (_activationCost == 0 || (_activationCost > 0 && _playerCoins && _playerCoins.CoinsFound >= _activationCost))
                ToggleActivation();
    }

    private void ToggleActivation()
    {
        if (_stateType == StateType.TwoStates)
        {
            if (_isActive)
                _inactiveActions.Invoke();
            else
                _activeActions.Invoke();
            _isActive = !_isActive;
        }
        else
            _actions.Invoke();
        
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
