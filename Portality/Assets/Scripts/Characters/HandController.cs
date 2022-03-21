using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(ActionBasedController))]
public class HandController : MonoBehaviour
{
    [SerializeField]
    private Player _player;
    private ActionBasedController _controller;

    [SerializeField]
    private string _handSide;

    // Start is called before the first frame update
    void Start()
    {
        _controller = GetComponent<ActionBasedController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_controller.selectAction.action.IsPressed())
        {
            _player.createPortal(_handSide, _controller.rotationAction.action.ReadValue<Quaternion>(), _controller.positionAction.action.ReadValue<Vector3>());
        }
    }
}
