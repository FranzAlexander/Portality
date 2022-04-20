using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Player : MonoBehaviour
{
    private HandController[] _handController;

    [SerializeField]
    private PortalManager _portalManager;

    private Quaternion _cameraRotation;

    // Getters and setters.
    public Quaternion CameraRotation { get => _cameraRotation; private set => _cameraRotation = value; }

    private void Awake()
    {
        _handController = GetComponentsInChildren<HandController>();
        _cameraRotation = transform.rotation;
    }

    // Start is called before the first frame update
    private void Start()
    {
    }

    private void Update()
    {
        CheckCreatePortal();
    }

    private void CheckCreatePortal()
    {
        for (int i = 0; i < _handController.Length; i++)
        {
            if (_handController[i].CreatePortal && _handController[i].PortalCanBePlaced())
            {
                _portalManager.PlacePortal(_handController[i].Hit, i);
                _handController[i].CreatePortal = false;
            }
        }
    }
}
