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

    // Holds the current teleportable objects.
    private List<Teleportable> _teleportables;

    public List<Teleportable> Teleportables { get => _teleportables; private set => _teleportables = value; }

    // Profiling.
    private CustomSampler sampler;

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

        _teleportables = new List<Teleportable>();

    }

    // Start is called before the first frame update
    private void Start()
    {
        sampler = CustomSampler.Create("RenderPortalSampler");

        RenderPipelineManager.beginCameraRendering += PortalMainCameraRender;
    }

    // private void FixedUpdate()
    // {
    //     for (int i = 0; i < _teleportables.Count; ++i)
    //     {
    //     }
    // }

    private void OnDisable()
    {
        RenderPipelineManager.beginCameraRendering -= PortalMainCameraRender;
    }

    private void PortalMainCameraRender(ScriptableRenderContext context, Camera camera)
    {
        sampler.Begin();
        for (int i = 0; i < _portals.Length; i += 1)
        {
            _portals[i].RenderPortal(context);

        }
        sampler.End();
    }

    public void PlacePortal(RaycastHit hit, int portalIndex)
    {
        _portals[portalIndex].transform.up = hit.normal;

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
