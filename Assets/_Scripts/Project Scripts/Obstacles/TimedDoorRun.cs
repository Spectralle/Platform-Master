using System.Collections;
using UnityEngine;
using TMPro;

public class TimedDoorRun : MonoBehaviour
{
    [SerializeField] private PlayerController2D _player;
    [SerializeField] private float[] _timings = new float[0];
    [SerializeField] private Animator[] _doors = new Animator[0];

    [SerializeField] private TextMeshPro _text;
    [SerializeField] private float _displayDelay = 2f;
    [SerializeField] private float _timescale = 0.4f;
    [SerializeField] private string[] _messages;

    private bool _hasStarted;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !_hasStarted)
            StartCoroutine(ShowWarning());
    }

    private IEnumerator ShowWarning()
    {
        _player.enabled = false;
        Time.timeScale = _timescale;

        for (int i = 0; i < _messages.Length; i++)
        {
            _text.SetText(_messages[i]);
            yield return new WaitForSecondsRealtime(_displayDelay);
        }

        _text.SetText("");
        _hasStarted = true;
        _player.enabled = true;
        Time.timeScale = 1f;

        StartCoroutine(TimedRun());
    }

    private IEnumerator TimedRun()
    {
        for (int i = 0; i < _timings.Length; i++)
        {
            yield return new WaitForSeconds(_timings[i]);
            _doors[i].Play("Functional Door_Close");
        }
    }

    public void ResetRun()
    {
        StopAllCoroutines();
        _hasStarted = false;
    }
}
