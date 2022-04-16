using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PlayerCamera : MonoBehaviour
{
    private Portal[] _portals;

    private Camera _playerCam;

    private void Awake()
    {
        _playerCam = GetComponent<Camera>();
    }

    public void SetPortals(Portal[] portals)
    {
        _portals = portals;
    }

    private void OnEnable()
    {
        RenderPipelineManager.beginCameraRendering += CameraRenderUpdate;
    }

    private void OnDisable()
    {
        RenderPipelineManager.beginCameraRendering -= CameraRenderUpdate;
    }

    private void CameraRenderUpdate(ScriptableRenderContext context, Camera camera)
    {
        for (int i = 0; i < _portals.Length; i++)
        {
            _portals[i].RenderPortal(context);
        }
    }
}
