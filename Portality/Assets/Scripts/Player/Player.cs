using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private HandController[] _handControllers;

    [SerializeField]
    private PortalManager _portalManager;

    private void Awake()
    {
        _handControllers = GetComponentsInChildren<HandController>();
    }

    private void Update()
    {
        CheckCreatePortal();
    }

    private void CheckCreatePortal()
    {
        for (int i = 0; i < _handControllers.Length; ++i)
        {
            if (_handControllers[i].CreatePortal && _handControllers[i].PortalCanBePlaced())
            {
                _portalManager.PlacePortal(_handControllers[i].Hit, i);
                _handControllers[i].CreatePortal = false;
            }
        }
    }
}
