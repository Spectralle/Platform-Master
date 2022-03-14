using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Volume))]
public class VignetteAutomator : MonoBehaviour
{
    [SerializeField, Range(0f, 1f)] private float _intensityRange = 0.1f;
    [SerializeField] private float _timeMultiplier = 1;

    private Vignette _vignette;
    private float _startIntensity;


    private void Awake()
    {
        GetComponent<Volume>().profile.TryGet(out _vignette);
        _startIntensity = _vignette.intensity.value;
    }

    private void LateUpdate() => _vignette.intensity.value = _startIntensity + (Mathf.Sin(Time.timeSinceLevelLoad * _timeMultiplier) * _intensityRange);
}
