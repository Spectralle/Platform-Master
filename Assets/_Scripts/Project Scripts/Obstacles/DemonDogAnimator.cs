using UnityEngine;

[RequireComponent(typeof(Animator))]
public class DemonDogAnimator : MonoBehaviour
{
    [SerializeField] private bool _lieDown;


    private void Start()
    {
        GetComponent<Animator>().SetBool("IdleLieDown", _lieDown);
    }
}
