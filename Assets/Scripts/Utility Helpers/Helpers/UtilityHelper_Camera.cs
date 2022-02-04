using UnityEngine;

namespace Utilities
{
    public static partial class UtilityHelper
    {
        public static void ShakeCamera(float intensity, float timer)
        {
            Vector3 lastCameraMovement = Vector3.zero;
            UtilityHelper_FunctionUpdater.Create(delegate () {
                timer -= Time.unscaledDeltaTime;
                Vector3 randomMovement = new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f)).normalized * intensity;
                Camera.main.transform.position = Camera.main.transform.position - lastCameraMovement + randomMovement;
                lastCameraMovement = randomMovement;
                return timer <= 0f;
            }, "CAMERA_SHAKE");
        }
    }
}