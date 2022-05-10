using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button _playButton;
    [SerializeField] private Button _quitButton;


    private void Awake()
    {
        if (_playButton)
            _playButton.onClick.AddListener(PlayGame);
        else
            Debug.Log("No Play button defined.");

        if (_quitButton)
            _quitButton.onClick.AddListener(QuitGame);
        else
            Debug.Log("No Quit button defined.");
    }

    private void PlayGame() => SceneManager.LoadScene("Loading Scene");

    private void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#endif
    }
}
