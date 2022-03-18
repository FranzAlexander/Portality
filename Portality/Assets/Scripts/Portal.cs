using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public Portal LinkedPortal;
    public MeshRenderer PortalScreen;
    public int RecursionLimit = 5;

    private RenderTexture _viewTexture;
    private Camera _portalCamera;
    private Camera _playerCamera;

    private RenderTexture _portalTexture;

    private void Awake()
    {
        _playerCamera = Camera.main;
        _portalCamera = GetComponentInChildren<Camera>();
    }

    private void CreatePortalTexture()
    {
        if (_portalTexture == null || _portalTexture.width != Screen.width || _portalTexture.height != Screen.height)
        {
            if (_portalTexture != null)
            {
                _portalTexture.Release();
            }
        }

        _portalTexture = new RenderTexture(Screen.width, Screen.width, 0);

        _portalCamera.targetTexture = _portalTexture;

        LinkedPortal.PortalScreen.material.SetTexture("_MainText", _portalTexture);
    }

    public void Render()
    {
        PortalScreen.enabled = false;
        CreatePortalTexture();

        Matrix4x4 camPos = transform.localToWorldMatrix * LinkedPortal.transform.worldToLocalMatrix * _playerCamera.transform.localToWorldMatrix;
        _portalCamera.transform.SetPositionAndRotation(camPos.GetColumn(3), camPos.rotation);

        _portalCamera.Render();

        PortalScreen.enabled = true;

        // Matrix4x4 camPos = _playerCamera.transform.localToWorldMatrix;
        // Vector3[] renderPos = new Vector3[RecursionLimit];
        // Quaternion[] renderRot = new Quaternion[RecursionLimit];

        // _portalCamera.projectionMatrix = _playerCamera.projectionMatrix;

        // for (int i = 0; i < RecursionLimit; ++i)
        // {

        // }
    }
}
