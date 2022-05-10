using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MovingPlatform : MonoBehaviour
{
    public delegate void MovingPlatformEventHandler();
    public event MovingPlatformEventHandler OnPlatformStartMoving;
    public event MovingPlatformEventHandler OnPlatformStopMoving;

    [SerializeField] private Vector2[] _waypoints = new Vector2[0];
    [SerializeField, Range(0.5f, 10f)] private float _moveSpeed = 2f;
    [SerializeField, Range(-1f, 5f)] private float _waitAtWaypoints = 0f;
    [SerializeField] private bool _loop;
    [SerializeField, Min(0)] private int _startAtWaypoint = 0;
    [Space]
    [SerializeField] private Color _labelColor = Color.white;

    private bool _movementEnabled = true;
    private int _currentIndex = 1;
    private bool _reversing = false;

    private Dictionary<Transform, Transform> _objectsOnPlatform = new Dictionary<Transform, Transform>();


    private void Start()
    {
        if (_waypoints.Length < 2)
        {
            Debug.Log("Not enough waypoints on " + name, gameObject);
            enabled = false;
            return;
        }

        if (_startAtWaypoint < _waypoints.Length)
        {
            transform.localPosition = _waypoints[_startAtWaypoint];
            _currentIndex = _startAtWaypoint;
        }
        else
            transform.localPosition = _waypoints[0];
    }

    private void FixedUpdate()
    {
        if (!_movementEnabled)
            return;

        if (!_loop)
        {
            if (_currentIndex == 0)
                _reversing = false;
            else if (_currentIndex == _waypoints.Length - 1)
                _reversing = true;

            if (transform.localPosition == (Vector3)_waypoints[_currentIndex])
            {
                OnPlatformStopMoving?.Invoke();

                if (_waitAtWaypoints > 0f)
                    StartCoroutine(WaitAtWaypoint(!_reversing ? 1 : -1));
                else if (_waitAtWaypoints < 0) { /* Platform should not automatically move. */ }
                else
                    _currentIndex += !_reversing ? 1 : -1;
            }
        }
        else
        {
            if (transform.localPosition == (Vector3)_waypoints[_currentIndex])
            {
                OnPlatformStopMoving?.Invoke();

                if (_waitAtWaypoints > 0f)
                    StartCoroutine(WaitAtWaypoint(_currentIndex != _waypoints.Length - 1 ? 1 : -(_waypoints.Length - 1)));
                else if (_waitAtWaypoints < 0) { /* Platform should not automatically move. */ }
                else
                    _currentIndex += _currentIndex != _waypoints.Length - 1 ? 1 : -(_waypoints.Length - 1);
            }
        }

        transform.localPosition = Vector2.MoveTowards(
            transform.localPosition,
            _waypoints[_currentIndex],
            Time.deltaTime * _moveSpeed
        );
    }

    private IEnumerator WaitAtWaypoint(int intToAdd)
    {
        _movementEnabled = false;
        yield return new WaitForSeconds(_waitAtWaypoints);
        _movementEnabled = true;
        GoToWaypoint(_currentIndex + intToAdd);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 3)
        {
            Debug.Log("Player entered");

            if (!_objectsOnPlatform.ContainsKey(other.transform))
            {
                _objectsOnPlatform.Add(other.transform, other.transform.parent);
                other.transform.parent = transform;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 3)
        {
            Debug.Log("Player exited");

            if (_objectsOnPlatform.ContainsKey(other.transform))
            {
                other.transform.parent = _objectsOnPlatform[other.transform];
                _objectsOnPlatform.Remove(other.transform);
            }
        }
    }

    public void GoToWaypoint(int index)
    {
        if (index >= 0 && index < _waypoints.Length)
            _currentIndex = index;

        OnPlatformStartMoving?.Invoke();
    }
    

    private void OnDrawGizmos()
    {
        if (_waypoints.Length == 0)
            return;

        Vector3 parPos = transform.parent ? transform.parent.position : Vector3.zero;
        for (int i = 0; i < _waypoints.Length; i++)
        {
            Gizmos.color = Color.black;
            Gizmos.DrawSphere(parPos + (Vector3)_waypoints[i], 0.25f);

            Gizmos.color = Color.white;
            if (i + 1 < _waypoints.Length)
                Gizmos.DrawLine(parPos + (Vector3)_waypoints[i], parPos + (Vector3)_waypoints[i + 1]);
        }

        if (_loop)
            Gizmos.DrawLine(parPos + (Vector3)_waypoints[0], parPos + (Vector3)_waypoints[_waypoints.Length - 1]); 
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Vector3 parPos = transform.parent ? transform.parent.position : Vector3.zero;
        for (int i = 0; i < _waypoints.Length; i++)
        {
            string labelName = transform.parent ? transform.parent.name : transform.name;
            GizmoLabel(parPos + (Vector3)_waypoints[i], $"{labelName} WP{i}", _labelColor);
        }
    }

    private void GizmoLabel(Vector3 point, string label, Color textColor)
    {
        GUIStyle style = new GUIStyle();
        style.alignment = TextAnchor.MiddleLeft;
        style.normal.textColor = textColor;
        Handles.Label(point, label, style);
    }
#endif
}
