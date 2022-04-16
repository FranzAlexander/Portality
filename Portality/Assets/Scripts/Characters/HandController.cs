using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using static UnityEngine.InputSystem.InputAction;

[RequireComponent(typeof(ActionBasedController))]
public class HandController : MonoBehaviour
{
    private ActionBasedController _controller;

    private bool _createPortal;

    private bool _placingPortal;

    private RaycastHit _hit;

    private float _range = 50f;

    public bool CreatePortal { get => _createPortal; set => _createPortal = value; }

    public bool PlacingPortal { get => _placingPortal; }

    public RaycastHit Hit { get => _hit; }

    private void Awake()
    {
        _controller = GetComponent<ActionBasedController>();
        _controller.activateAction.action.performed += _ => ProjectPortalPlacement();
        _controller.activateAction.action.canceled += _ => ConfirmPortalPlacement();

        _createPortal = false;
        _placingPortal = false;
    }

    private void OnEnabled()
    {
        _controller.activateAction.action.Enable();
    }

    private void OnDisabled()
    {
        _controller.activateAction.action.Disable();
    }

    private void ProjectPortalPlacement()
    {
        _placingPortal = true;
    }

    private void ConfirmPortalPlacement()
    {
        _placingPortal = false;
        _createPortal = true;
    }

    public bool PortalCanBePlaced()
    {
        if (Physics.Raycast(transform.position, transform.forward, out _hit, _range))
        {
            if (_hit.transform.gameObject.CompareTag("Environment"))
            {
                return true;
            }
        }

        return false;
    }

    public Quaternion GetHandRotation()
    {
        return transform.rotation;
    }
}
