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

    // Update is called once per frame
    private void Update()
    {
        //CheckInput();
    }

    // public void CheckInput()
    // {
    //     if (_controller.activateAction.action.IsPressed())
    //     {
    //         _placingPortal = true;
    //     }

    //     if (_controller.activateAction.action.WasReleasedThisFrame())
    //     {
    //         _placingPortal = false;
    //         _createPortal = true;
    //     }
    // }

    private void ProjectPortalPlacement()
    {
        _placingPortal = true;
    }

    private void ConfirmPortalPlacement()
    {
        _placingPortal = false;
        _createPortal = true;
    }

    public bool PortalPreviewCanBePlaced()
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

    // Old method.
    // public void GetPortalPlacementPosition(ref Vector3 position, ref Quaternion rotation)
    // {
    //     RaycastHit hit;

    //     if (Physics.Raycast(transform.position, transform.forward, out hit, _range))
    //     {
    //         if (hit.transform.gameObject.CompareTag("Environment"))
    //         {
    //             position = hit.point;

    //             if (hit.normal == Vector3.up)
    //             {

    //                 rotation.y = transform.rotation.eulerAngles.y;
    //             }

    //             if (hit.normal == Vector3.right)
    //             {
    //                 rotation.Set(transform.position.x, 0f, 0f, 0f);
    //             }
    //         }
    //     }
    // }
}
