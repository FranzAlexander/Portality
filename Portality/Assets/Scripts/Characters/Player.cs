using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private GameObject _portal;

    // First index is left hand, second is for the right hand.
    private GameObject[] _playerPortals;

    private void Awake()
    {
        _playerPortals = new GameObject[2];
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void createPortal(string handSide, Quaternion handRotation, Vector3 handPosition)
    {
        if (handSide == "Right")
        {
            if (_playerPortals[1] != null)
            {
                GameObject.Destroy(_playerPortals[1]);
            }

            _playerPortals[1] = Instantiate(_portal, handPosition, handRotation);

        }
        else
        {
            if (_playerPortals[0] != null)
            {
                GameObject.Destroy(_playerPortals[0]);
            }
            _playerPortals[0] = Instantiate(_portal, handPosition, handRotation);

        }
    }
}
