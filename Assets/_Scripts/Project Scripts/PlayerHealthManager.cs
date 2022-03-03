using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealthManager : MonoBehaviour
{
    [SerializeField, Range(1, 10)] private int _lives;
    public int Lives => _currentLives;

    private int _currentLives;
    private Vector3 _respawnPoint;


    private void Awake()
    {
        _currentLives = _lives;
        _respawnPoint = transform.position;
    }

    private void Start() => UIManager.Instance?.UpdateLives(_currentLives);

    public void TakeDamage(int damage)
    {
        if (_currentLives <= 0)
            return;

        _currentLives = Mathf.Max(_currentLives - damage, 0);
        if (_currentLives == 0)
            Die();
        else
            Respawn();

        UIManager.Instance.UpdateLives(_currentLives);
    }

    public void TakeFatalDamage() => TakeDamage(_currentLives);

    private void Respawn()
    {
        TryGetComponent(out PlayerController2D pc);
        transform.position = _respawnPoint;
    }

    private void Die() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
}
