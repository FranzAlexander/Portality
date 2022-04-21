using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Portal : MonoBehaviour
{
    // Main camera.
    private Camera _playerCam;

    private Camera _portalCam;

    // Reference to the other portal.
    private Portal _otherPortal;

    // Portal rendering.
    private MeshRenderer _portalScreen;
    private RenderTexture _viewTexture;

    private const int _recursionLimit = 7;

    // Position and Rotation for the portal camera.
    private Vector3[] _portalCameraPositions;

    private Quaternion[] _portalCameraRotations;

    // Objects to teleport.
    private List<Teleportable> _teleportables;

    // Getters and Setter auto properties.
    public Camera PortalCamera { get => _portalCam; private set => _portalCam = value; }

    public Portal OtherPortal { get => _otherPortal; set => _otherPortal = value; }

    public MeshRenderer PortalScreen { get => _portalScreen; private set => _portalScreen = value; }


    private void Awake()
    {
        // Cameras.
        _playerCam = Camera.main;
        _portalCam = GetComponentInChildren<Camera>();

        // Rendering.
        _portalScreen = GetComponentInChildren<MeshRenderer>();

        _portalCameraPositions = new Vector3[_recursionLimit];
        _portalCameraRotations = new Quaternion[_recursionLimit];

        _teleportables = new List<Teleportable>();
    }

    private void LateUpdate()
    {
        for (int i = 0; i < _teleportables.Count; ++i)
        {
            _teleportables[i].Teleport(transform, _otherPortal.transform);
        }
    }

    // Sets render texture for the camera and portal.
    public void PortalTextureSetup(Portal otherPortal)
    {
        if (_portalCam.targetTexture != null)
        {
            _viewTexture.Release();
        }

        _viewTexture = new RenderTexture(Screen.width, Screen.height, 24);
        _portalCam.targetTexture = _viewTexture;

        otherPortal.PortalScreen.material.SetTexture("_MainTex", _viewTexture);
    }

    public void RenderPortal(ScriptableRenderContext context, bool floor)
    {
        _portalCam.projectionMatrix = _playerCam.projectionMatrix;


        for (int i = 0; i < _recursionLimit; ++i)
        {
            _portalCameraPositions[i] = transform.TransformPoint(_otherPortal.transform.InverseTransformPoint(_playerCam.transform.position));

            _portalCameraRotations[i] = transform.rotation * Quaternion.Inverse(_otherPortal.transform.rotation) * _playerCam.transform.rotation;
        }

        for (int i = 0; i < _recursionLimit; ++i)
        {
            _portalCam.transform.position = _portalCameraPositions[i];
            _portalCam.transform.rotation = _portalCameraRotations[i];

            UniversalRenderPipeline.RenderSingleCamera(context, _portalCam);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Teleportable newTeleportable = other.GetComponent<Teleportable>();

        if (newTeleportable)
        {
            _teleportables.Add(newTeleportable);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Teleportable newTeleportable = other.GetComponent<Teleportable>();

        if (_teleportables.Contains(newTeleportable))
        {
            _teleportables.Remove(newTeleportable);
        }
    }
}
