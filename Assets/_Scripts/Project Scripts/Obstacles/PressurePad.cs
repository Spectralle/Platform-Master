using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PressurePad : MonoBehaviour
{
    [SerializeField] private UnityEvent _onActivation;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Pushable"))
        {
            float distance = Vector2.Distance(transform.position, other.transform.position);
            if (distance < 0.03f)
            {
                other.TryGetComponent(out Rigidbody rb);
                if (rb)
                {
                    rb.isKinematic = true;
                    _onActivation.Invoke();
                }
            }
        }
    }
}
