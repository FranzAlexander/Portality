using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class HandController : MonoBehaviour
{
    private ActionBasedController _controller;

    private bool _createPortal;
    private bool _previewPortal;

    private RaycastHit _hit;
    private const float _range = 50f;

    public bool CreatePortal { get => _createPortal; set => _createPortal = value; }
    public bool PreviewPortal { get => _previewPortal; set => _previewPortal = value; }

    public RaycastHit Hit { get => _hit; }

    private void Awake()
    {
        _controller = GetComponent<ActionBasedController>();
        _controller.activateAction.action.performed += _ => ProjectPortalPlacement();
        _controller.activateAction.action.canceled += _ => ConfirmPortalPlacement();

        _createPortal = false;
        _previewPortal = false;
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
        _previewPortal = true;
    }

    private void ConfirmPortalPlacement()
    {
        _previewPortal = false;
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

}
