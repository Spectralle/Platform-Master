using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManualOcclusion : MonoBehaviour
{
    [SerializeField] private MovingPlatform _listenTarget;

    public struct OcclusionGroup
    {
        public int PlatformIndexTarget;
        public GameObject[] OcclusionTargets;
    }


    private void Awake()
    {
        
    }

    private void OnMove(int platformIndex)
    {

    }

    private void OnStop(int platformIndex)
    {

    }
}
