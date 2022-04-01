using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(ActionBasedController))]
public class HandController : MonoBehaviour
{
    private ActionBasedController _controller;

    private bool _createPortal;

    private bool _placingPortal;

    public bool CreatePortal { get => _createPortal; set => _createPortal = value; }

    public bool PlacingPortal { get => _placingPortal; }

    private void Awake()
    {
        _controller = GetComponent<ActionBasedController>();
        _createPortal = false;
        _placingPortal = false;
    }

    // Update is called once per frame
    private void Update()
    {
        CheckInput();
    }

    public void CheckInput()
    {
        if (_controller.activateAction.action.IsPressed())
        {
            _placingPortal = true;
        }

        if (_controller.activateAction.action.WasReleasedThisFrame())
        {
            _placingPortal = false;
            _createPortal = true;
        }
    }

}
