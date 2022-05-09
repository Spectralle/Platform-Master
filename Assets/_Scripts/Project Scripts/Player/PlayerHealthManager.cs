using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif


public class PlayerHealthManager : MonoBehaviour
{
    [SerializeField, Range(1, 10)] private int _lives = 1;
    public int Lives => _currentLives;

    private int _currentLives;


    private void Awake() => _currentLives = _lives;

    private void Start()
    {
        if (UIManager.Instance)
            UIManager.Instance.UpdateLives(_currentLives, _lives);
        if (RespawnManager.Instance)
            RespawnManager.Instance.SetRespawnPoint(transform.position, null);
    }

    public void TakeDamage(int damage)
    {
        if (_currentLives <= 0)
            return;

        _currentLives = Mathf.Max(_currentLives - damage, 0);
        if (_currentLives > 0)
            Respawn();
        else
            Die();

        if (UIManager.Instance)
            UIManager.Instance.UpdateLives(_currentLives, _lives);
    }

    public void TakeFatalDamage() => TakeDamage(_currentLives);

    private void Respawn()
    {
        if (RespawnManager.Instance)
            RespawnManager.Instance.Respawn(transform);
        else
        {
            Debug.Log("No Respawn Manager. Respawning disabled.");
            EditorApplication.isPaused = true;
        }
    }

    private void Die() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    public void ResetLives()
    {
        Debug.Log($"Lives reset to {_currentLives}");
        _currentLives = _lives;
    }
}
