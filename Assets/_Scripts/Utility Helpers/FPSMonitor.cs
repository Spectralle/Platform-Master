using UnityEngine;
using TMPro;

public class FPSMonitor : Singleton<FPSMonitor>
{
    public static float FPS = 0;
    [SerializeField] private TextMeshProUGUI _FPSUi;
    [SerializeField] private string _prefix;
    [SerializeField] private string _suffix;

    private int _lastFrameIndex;
    private float[] _frameDeltaTimeArray;

    protected override void Awake()
    {
        base.Awake();
        _frameDeltaTimeArray = new float[50];
    }

    private void Update()
    {
        _frameDeltaTimeArray[_lastFrameIndex] = Time.deltaTime;
        _lastFrameIndex = (_lastFrameIndex + 1) % _frameDeltaTimeArray.Length;

        FPS = Mathf.RoundToInt(CalculateAverageFPS());
        _FPSUi.SetText(_prefix + FPS.ToString() + _suffix);
    }

    private float CalculateAverageFPS()
    {
        float total = 0;
        foreach (float deltaTime in _frameDeltaTimeArray)
            total += deltaTime;
        return _frameDeltaTimeArray.Length / total;
    }
}
