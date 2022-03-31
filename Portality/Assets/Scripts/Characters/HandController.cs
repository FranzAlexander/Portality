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

    private Transform _handPos;

    [SerializeField]
    private string _handSide;

    // Start is called before the first frame update
    void Start()
    {
        _controller = GetComponent<ActionBasedController>();
        _handPos = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_controller.selectAction.action.IsPressed())
        {
            Debug.Log("Grip trigger");
            //_player.createPortal(_handSide, _handPos);
        }
    }
}
