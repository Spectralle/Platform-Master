using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Sirenix.OdinInspector;
using TMPro;

public class LoadingSceneManager : MonoBehaviour
{
    [Title("Loading Progress:")]
    [SerializeField] private Slider _progressBar;
    [SerializeField] private TextMeshProUGUI[] _continueText;


    private void Awake() => StartCoroutine(LoadAsyncScene());

    private IEnumerator LoadAsyncScene()
    {
        yield return null;

        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync("Game Scene");
        asyncOperation.allowSceneActivation = false;
        
        while (!asyncOperation.isDone)
        {
            _progressBar.value = asyncOperation.progress;

            if (asyncOperation.progress >= 0.9f)
                break;

            yield return null;
        }

        foreach (var text in _continueText)
            text.SetText("Press SPACE");

        while (!Input.GetKeyDown(KeyCode.Space))
            yield return null;

        asyncOperation.allowSceneActivation = true;
    }
}
