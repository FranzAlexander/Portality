using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PortalManager : MonoBehaviour
{
    private Portal[] _portals;

    // Profiling.
    //   private CustomSampler sampler;

    private void Awake()
    {
        _portals = GetComponentsInChildren<Portal>();

        if (_portals.Length == 2)
        {
            _portals[0].OtherPortal = _portals[1];
            _portals[1].OtherPortal = _portals[0];
        }
        else
        {
            Debug.LogWarning("Wrong number of portals in the scene!");
        }

    }

    // Start is called before the first frame update
    private void Start()
    {
        //        sampler = CustomSampler.Create("RenderPortalSampler");

        RenderPipelineManager.beginCameraRendering += PortalMainCameraRender;
    }

    private void OnDisable()
    {
        RenderPipelineManager.beginCameraRendering -= PortalMainCameraRender;
    }

    private void PortalMainCameraRender(ScriptableRenderContext context, Camera camera)
    {
        for (int i = 0; i < _portals.Length; i += 1)
        {
            _portals[i].RenderPortal(context);

        }
    }

    public void PlacePortal(RaycastHit hit, Quaternion cameraRotation, int portalIndex)
    {
        _portals[portalIndex].transform.up = hit.normal;
        //_portals[portalIndex].transform.rotation = Quaternion.LookRotation(hit.normal);
        _portals[portalIndex].transform.position = hit.point;
    }

    private void CheckValidPositions()
    {
        Vector3[] points =
        {
            new Vector3(-1.1f, 0.0f, 0.1f),
            new Vector3(1.1f, 0.0f, 0.1f),
            new Vector3( 0.0f, -2.1f, 0.1f),
            new Vector3( 0.0f,  2.1f, 0.1f)
        };

        Vector3[] dirs =
        {
            Vector3.right,
            -Vector3.right,
             Vector3.up,
            -Vector3.up
        };

        for (int i = 0; i < 4; i += 1)
        {

        }
    }
}
