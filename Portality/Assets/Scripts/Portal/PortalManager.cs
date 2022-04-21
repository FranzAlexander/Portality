using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PortalManager : MonoBehaviour
{
    private Portal[] _portals;

    private const int _otherPortalIndex = 1;

    private bool _onFloor;

    private void Awake()
    {
        _portals = GetComponentsInChildren<Portal>();

        if (_portals.Length != 2)
        {
            Debug.LogWarning("Wrong number of portals in the scene!");
        }
        else
        {
            _portals[0].OtherPortal = _portals[1];
            _portals[1].OtherPortal = _portals[0];
        }

        _onFloor = false;
    }

    private void OnDisable()
    {
        RenderPipelineManager.beginCameraRendering -= PortalMainCameraRender;
    }

    private void Start()
    {
        _portals[0].PortalTextureSetup(_portals[1]);
        _portals[1].PortalTextureSetup(_portals[0]);

        RenderPipelineManager.beginCameraRendering += PortalMainCameraRender;
    }

    private void PortalMainCameraRender(ScriptableRenderContext context, Camera camera)
    {
        for (int i = 0; i < _portals.Length; ++i)
        {
            _portals[i].RenderPortal(context, _onFloor);
        }
    }

    // hit normal on ground 0, 1, 0
    public void PlacePortal(RaycastHit hit, int portalIndex)
    {
        if (hit.normal == Vector3.up)
        {
            _onFloor = true;
        }
        else
        {
            _onFloor = false;
        }
        _portals[portalIndex].transform.up = hit.normal;
        _portals[portalIndex].transform.position = new Vector3(hit.point.x, hit.point.y + 0.01f, hit.point.z);
    }
}