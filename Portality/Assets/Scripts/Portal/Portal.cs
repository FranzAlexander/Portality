using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    int _recursionLimit = 5;
    // Camera's
    private Camera _playerCam;
    private Camera _portalCam;

    // Rendering
    private RenderTexture _viewTexture;
    private MeshRenderer _portalScreen;
    private MeshFilter _portalScreenFilter;


    // Getters and Setter
    public MeshRenderer PortalScreen { get => _portalScreen; set => _portalScreen = value; }

    public Portal OtherPortal;

    private void Awake()
    {
        _playerCam = Camera.main;
        _portalCam = GetComponentInChildren<Camera>();
        _portalScreen = transform.Find("PortalView").GetComponent<MeshRenderer>();
        _portalScreenFilter = _portalScreen.GetComponent<MeshFilter>();
    }

    public void RenderPortal()
    {
        if (!VisableFromCamera(OtherPortal.PortalScreen, _playerCam))
        {
            return;
        }

        CreateViewTexture();


        Matrix4x4 localToWorldMatrix = _playerCam.transform.localToWorldMatrix;
        Vector3[] renderPosition = new Vector3[_recursionLimit];
        Quaternion[] renderRotations = new Quaternion[_recursionLimit];

        int startIndex = 0;
        _portalCam.projectionMatrix = _playerCam.projectionMatrix;

        for (int i = 0; i < _recursionLimit; i++)
        {
            localToWorldMatrix = transform.localToWorldMatrix * OtherPortal.transform.worldToLocalMatrix * localToWorldMatrix;
            int renderOrderIndex = _recursionLimit - i - 1;
            renderPosition[renderOrderIndex] = localToWorldMatrix.GetColumn(3);
            renderRotations[renderOrderIndex] = localToWorldMatrix.rotation; ;

            _portalCam.transform.SetPositionAndRotation(renderPosition[renderOrderIndex], renderRotations[renderOrderIndex]);
            startIndex = renderOrderIndex;
        }

        _portalScreen.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
        OtherPortal.PortalScreen.material.SetInt("displayMask", 0);

        for (int i = startIndex; i < _recursionLimit; i++)
        {
            _portalCam.transform.SetPositionAndRotation(renderPosition[i], renderRotations[i]);
            _portalCam.Render();

            if (i == startIndex)
            {
                OtherPortal.PortalScreen.material.SetInt("displayMask", 1);
            }
        }

        _portalScreen.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;

    }

    private void CreateViewTexture()
    {
        if (_viewTexture == null || _viewTexture.width != Screen.width || _viewTexture.height != Screen.height)
        {
            if (_viewTexture != null)
            {
                _viewTexture.Release();
            }

            _viewTexture = new RenderTexture(Screen.width, Screen.width, 0);

            _portalCam.targetTexture = _viewTexture;
            OtherPortal.PortalScreen.material.SetTexture("_MainTex", _viewTexture);
        }
    }

    private bool VisableFromCamera(Renderer renderer, Camera camera)
    {
        Plane[] franstumPlanes = GeometryUtility.CalculateFrustumPlanes(camera);
        return GeometryUtility.TestPlanesAABB(franstumPlanes, renderer.bounds);
    }
}