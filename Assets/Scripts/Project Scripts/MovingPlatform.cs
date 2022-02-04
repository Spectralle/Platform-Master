using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] private Vector2[] _waypoints = new Vector2[0];
    [SerializeField, Range(0.5f, 10f)] private float _moveSpeed = 2f;
    [SerializeField, Range(0f, 5f)] private float _waitAtWaypoints;
    [SerializeField] private bool _loop;

    private bool _movementEnabled = true;
    private int _currentIndex = 1;
    private bool _reversing = false;

    private Dictionary<Transform, Transform> _objectsOnPlatform = new Dictionary<Transform, Transform>();


    private void Start()
    {
        if (_waypoints.Length < 2)
        {
            enabled = false;
            return;
        }

        transform.position = _waypoints[0];
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

            if (transform.position == (Vector3)_waypoints[_currentIndex])
            {
                if (_waitAtWaypoints > 0f)
                    StartCoroutine(WaitAtWaypoint(!_reversing ? 1 : -1));
                else
                    _currentIndex += !_reversing ? 1 : -1;
            }
        }
        else
        {
            if (transform.position == (Vector3)_waypoints[_currentIndex])
            {
                if (_waitAtWaypoints > 0f)
                    StartCoroutine(WaitAtWaypoint(_currentIndex != _waypoints.Length - 1 ? 1 : -(_waypoints.Length - 1)));
                else
                    _currentIndex += _currentIndex != _waypoints.Length - 1 ? 1 : -(_waypoints.Length - 1);
            }
        }

        transform.position = Vector2.MoveTowards(
            transform.position,
            _waypoints[_currentIndex],
            Time.deltaTime * _moveSpeed
        );
    }

    private IEnumerator WaitAtWaypoint(int intToAdd)
    {
        _movementEnabled = false;
        yield return new WaitForSeconds(_waitAtWaypoints);
        _movementEnabled = true;
        _currentIndex += intToAdd;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 3)
        {
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
            if (_objectsOnPlatform.ContainsKey(other.transform))
            {
                other.transform.parent = _objectsOnPlatform[other.transform];
                _objectsOnPlatform.Remove(other.transform);
            }
        }
    }



    private void OnDrawGizmos()
    {
        if (_waypoints.Length == 0)
            return;
        
        for (int i = 0; i < _waypoints.Length; i++)
        {
            Gizmos.color = Color.black;
            Gizmos.DrawSphere(_waypoints[i], 0.1f);

            if (i + 1 < _waypoints.Length)
                Gizmos.DrawLine(_waypoints[i], _waypoints[i + 1]);
        }

        if (_loop)
            Gizmos.DrawLine(_waypoints[0], _waypoints[_waypoints.Length - 1]);
    }
}
