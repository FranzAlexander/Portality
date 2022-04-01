using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Player : MonoBehaviour
{
    // [SerializeField]
    // private Portal _portal;

    // // First index is left hand, second is for the right hand.
    // private Portal[] _playerPortals;

    // private void Awake()
    // {
    //     _playerPortals = new Portal[2]
    //     {
    //         Instantiate(_portal, new Vector3(0, 0, 0), Quaternion.identity),
    //         Instantiate(_portal, new Vector3(0, 0, 0), Quaternion.identity)
    //     };
    //     _playerPortals[0].OtherPortal = _playerPortals[1];
    //     _playerPortals[1].OtherPortal = _playerPortals[0];
    // }

    private void Awake()
    {

    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    // void OnPreCull()
    // {
    //     for (int i = 0; i < _playerPortals.Length; i++)
    //     {
    //         _playerPortals[i].RenderPortal();
    //     }
    // }
}
