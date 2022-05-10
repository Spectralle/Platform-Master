using UnityEngine;

public class RespawnManager : Singleton<RespawnManager>
{
    private Vector3 _respawnPoint = Vector3.zero;
    private RespawnPoint _lastPointPassed = null;


    public void SetRespawnPoint(Vector3 newRespawnPoint, RespawnPoint respawnPoint)
    {
        if (_lastPointPassed == respawnPoint)
            return;

        _respawnPoint = newRespawnPoint;
        _lastPointPassed = respawnPoint;
    }

    public Vector3 GetRespawnPoint() => _respawnPoint;

    public void Respawn(Transform respawnable) =>
        respawnable.transform.SetPositionAndRotation(_respawnPoint, Quaternion.identity);
}
