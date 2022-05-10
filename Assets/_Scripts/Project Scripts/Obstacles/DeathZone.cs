using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Events;
using Sirenix.OdinInspector;

public class DeathZone : MonoBehaviour
{
    [Title("Life Damage:")]
    [SerializeField, Min(0)] private int _lifeDamage = 1;
    [Title("Timeline Reset:")]
    [SerializeField] private PlayableDirector _resetTimeline;
    [SerializeField, ShowIf("_resetTimeline", null)] private bool _stopPlaying;
    [Title("Other Reset:")]
    [SerializeField] private UnityEvent _resetActions;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.TryGetComponent(out PlayerHealthManager health);
            if (health)
                health.TakeDamage(_lifeDamage);

            if (_resetTimeline)
            {
                _resetTimeline.time = 0;
                _resetTimeline.Evaluate();
                if (_stopPlaying)
                    _resetTimeline.Stop();
            }

            _resetActions.Invoke();
        }
    }
}
