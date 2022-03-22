using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public Portal OtherPortal;
    public int RecursionLimit = 5;

    private Camera _playerCam;
    private Camera _portalCam;
    private Transform _portalPos;
    private Transform _otherPortal;

    private RenderTexture _viewTexture;
    private MeshRenderer _portalScreen;
    private MeshFilter _portalScreenFilter;

    private bool _exists;

    public MeshRenderer PortalScreen { get => _portalScreen; set => _portalScreen = value; }

    private void Awake()
    {
        _playerCam = Camera.main;
        _portalCam = GetComponentInChildren<Camera>();
        _portalScreen = GetComponent<MeshRenderer>();
        _portalScreenFilter = _portalScreen.GetComponent<MeshFilter>();
        _exists = false;
    }

    private void LateUpdate()
    {

    }

    public void CreatePortal()
    {
        _playerCam = Camera.main;
    }

    public void RenderPortal()
    {
        CreateViewTexture();

        Matrix4x4 localToWorldMatrix = _playerCam.transform.localToWorldMatrix;
        Vector3[] renderPosition = new Vector3[RecursionLimit];
        Quaternion[] renderRotations = new Quaternion[RecursionLimit];

        int startIndex = 0;
        _portalCam.projectionMatrix = _playerCam.projectionMatrix;

        for (int i = 0; i < RecursionLimit; i++)
        {
            localToWorldMatrix = transform.localToWorldMatrix * OtherPortal.transform.worldToLocalMatrix * localToWorldMatrix;
            int renderOrderIndex = RecursionLimit - i - 1;
            renderPosition[renderOrderIndex] = localToWorldMatrix.GetColumn(3);
            renderRotations[renderOrderIndex] = localToWorldMatrix.rotation; ;

            _portalCam.transform.SetPositionAndRotation(renderPosition[renderOrderIndex], renderRotations[renderOrderIndex]);
            startIndex = renderOrderIndex;
        }

        _portalScreen.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
        OtherPortal.PortalScreen.material.SetInt("displayMask", 0);

        for (int i = startIndex; i < RecursionLimit; i++)
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
}
