using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalManager : MonoBehaviour
{
    // Zero index is always left hand, First index is always right hand.

    [SerializeField]
    private Portal[] _portals;

    [SerializeField]
    private GameObject[] _previewPortals;

    [SerializeField]
    private HandController[] _handControllers;

    private void Awake()
    {

        // _portals[0].OtherPortal = _portals[1];
        // _portals[1].OtherPortal = _portals[0];

        _previewPortals[0].SetActive(false);
        _previewPortals[1].SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        PortalPreview();
    }

    private void PortalPreview()
    {
        for (int i = 0; i < _handControllers.Length; i += 1)
        {
            if (_handControllers[i].PlacingPortal)
            {
                PlacePortalPreview(i);

                if (!_previewPortals[i].activeInHierarchy)
                {
                    _previewPortals[i].SetActive(true);
                }
            }
            else
            {
                if (_previewPortals[i].activeInHierarchy)
                {
                    _previewPortals[i].SetActive(false);
                }
            }
        }
    }

    private void PlacePortalPreview(in int index)
    {
        if (_handControllers[index].PortalPreviewCanBePlaced())
        {
            if (_handControllers[index].Hit.normal == Vector3.up)
            {
                _previewPortals[index].transform.position = _handControllers[index].Hit.point;
                _previewPortals[index].transform.rotation = Quaternion.Euler(new Vector3(0, _handControllers[index].transform.rotation.eulerAngles.z, 0));
            }
        }
    }

    private void CreatePortal()
    {
        for (int i = 0; i < _handControllers.Length; i += 1)
        {
            if (_handControllers[i].CreatePortal)
            {
                _portals[i].SetTransformFromPreview(_previewPortals[i].transform);

                if (_previewPortals[i].activeInHierarchy)
                {
                    _previewPortals[i].SetActive(false);
                }
                _portals[i].SetPortalActive(true);
            }
        }
    }
}
