using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserWallController : MonoBehaviour
{
    [SerializeField] private AnimationCurve _animation;
    [SerializeField] private float _timeSpan = 1f;
    [SerializeField] private GameObject _animatedLasers;

    private float _timer = 0f;


    private void Start() => StartCoroutine(Animate());

    private IEnumerator Animate()
    {
        while (true)
        {
            if (_timer > _timeSpan)
                _timer = 0f;

            float eval = _animation.Evaluate(_timer / _timeSpan);

            if (eval == 1)
                _animatedLasers.SetActive(true);
            else
                _animatedLasers.SetActive(false);

            _timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }
}
