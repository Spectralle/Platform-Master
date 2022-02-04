using UnityEngine;


[RequireComponent(typeof(Camera))]
public class CameraController2D : MonoBehaviour
{
    [SerializeField] private Transform _player;
    [SerializeField] private Vector3 _cameraOffset;
    [SerializeField] private Vector2 _cameraSmoothing;
    [SerializeField] private Vector2 _cameraDeadZone;

    private Vector3 _camPos;


    private void FixedUpdate()
    {
        CalculateCameraPosition();
        transform.position = _camPos;
    }

    private void CalculateCameraPosition()
    {
        _camPos.x = Mathf.Lerp(
            transform.position.x,
            _player.position.x + _cameraOffset.x,
            Time.deltaTime * _cameraSmoothing.x
        );
        _camPos.y = Mathf.Lerp(
            transform.position.y,
            _player.position.y + _cameraOffset.y,
            Time.deltaTime * _cameraSmoothing.y
        );

        if (_camPos.x - _player.position.x > _cameraDeadZone.x)
            _camPos.x = _player.position.x + _cameraDeadZone.x;
        else if (_camPos.x - _player.position.x < -_cameraDeadZone.x)
            _camPos.x = _player.position.x - _cameraDeadZone.x;

        if (_camPos.y - _player.position.y > _cameraDeadZone.y)
            _camPos.y = _player.position.y + _cameraDeadZone.y;
        else if (_camPos.y - _player.position.y < -_cameraDeadZone.y)
            _camPos.y = _player.position.y - _cameraDeadZone.y;

        _camPos.z = _cameraOffset.z;
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(transform.position, _cameraDeadZone);
    }
}
