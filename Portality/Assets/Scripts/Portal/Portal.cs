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

    // Holds objects currently teleporting.
    private List<Teleportable> _teleportables;

    private Portal _otherPortal;

    // Getters and Setters.
    public MeshRenderer PortalScreen { get => _portalScreen; set => _portalScreen = value; }
    public Portal OtherPortal { get => _otherPortal; set => _otherPortal = value; }


    private void Awake()
    {
        // Cameras.
        _playerCam = Camera.main;
        _portalCam = GetComponentInChildren<Camera>();

        // Rendering.
        _portalScreen = transform.Find("PortalView").GetComponent<MeshRenderer>();
        _portalScreenFilter = _portalScreen.GetComponent<MeshFilter>();

        // Teleportables.
        _teleportables = new List<Teleportable>();
    }


    public void RenderPortal()
    {
        if (!PortalUtility.VisableFromCamera(_otherPortal.PortalScreen, _playerCam))
        {
            return;
        }

        CreateViewTexture();

        Matrix4x4 localToWorldMatrix = _playerCam.transform.localToWorldMatrix;
        Vector3[] renderPositions = new Vector3[_recursionLimit];
        Quaternion[] renderRotations = new Quaternion[_recursionLimit];
    }

    // Create the view texture for this portal to see the other side.
    private void CreateViewTexture()
    {
        if (_viewTexture == null || _viewTexture.width != Screen.width || _viewTexture.height != Screen.height)
        {
            if (_viewTexture != null)
            {
                _viewTexture.Release();
            }

            _viewTexture = new RenderTexture(Screen.width, Screen.height, 0);

            _portalCam.targetTexture = _viewTexture;
            _otherPortal.PortalScreen.material.SetTexture("_MainTex", _viewTexture);
        }
    }

    // Helper Methods.
    public void SetTransformFromPreview(Transform previewTransfom)
    {
        transform.parent = previewTransfom;
    }

    public void SetPortalActive(bool currentlyActive)
    {
        Debug.Log(currentlyActive);
        gameObject.SetActive(currentlyActive);
    }

}