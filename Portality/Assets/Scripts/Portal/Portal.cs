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

    private void Start()
    {
        PortalTextureSetup();
    }

    private void LateUpdate()
    {
        for (int i = 0; i < _teleportables.Count; ++i)
        {
            _teleportables[i].Teleport(transform, _otherPortal.transform);
        }
    }

    // Sets render texture for the camera and portal.
    public void PortalTextureSetup()
    {
        if (_portalCam.targetTexture != null)
        {
            _viewTexture.Release();
        }

        _viewTexture = new RenderTexture(Screen.width, Screen.height, 24);
        _portalCam.targetTexture = _viewTexture;

        _otherPortal.PortalScreen.material.SetTexture("MainTex", _viewTexture);
    }

    public void RenderPortal(ScriptableRenderContext context, bool floor)
    {
        if (!PortalVisableByCamera()) { return; }

        Matrix4x4 cameraMatrix;
        Vector3 cameraPosition;
        _portalCam.projectionMatrix = _playerCam.projectionMatrix;

        for (int i = 0; i < _recursionLimit; ++i)
        {
            cameraMatrix = transform.localToWorldMatrix * _otherPortal.transform.worldToLocalMatrix * _playerCam.transform.localToWorldMatrix;
            cameraPosition = cameraMatrix.GetPosition();
            cameraPosition.y = cameraPosition.y * -transform.up.y;
            // _portalCameraPositions[i] = Vector3.Scale(transform.TransformPoint(_otherPortal.transform.InverseTransformPoint(_playerCam.transform.position)), -Vector3.up);
            //  Vector3 cameraPosition = transform.TransformPoint(_otherPortal.transform.InverseTransformPoint(_playerCam.transform.position));
            _portalCameraPositions[i] = cameraPosition;
            // transform.rotation * Quaternion.Inverse(_otherPortal.transform.rotation) * _playerCam.transform.rotation;
            _portalCameraRotations[i] = cameraMatrix.rotation;
        }

        for (int i = 0; i < _recursionLimit; ++i)
        {
            _portalCam.transform.position = _portalCameraPositions[i];
            _portalCam.transform.rotation = _portalCameraRotations[i];

            // Setting the camera's oblique view frustum.
            Vector3 cameraSpaceNormal = _portalCam.worldToCameraMatrix.MultiplyVector(transform.up) * System.Math.Sign(Vector3.Dot(transform.up, transform.position - _portalCam.transform.position));

            _portalCam.projectionMatrix = _playerCam.CalculateObliqueMatrix(new Vector4(
                cameraSpaceNormal.x,
                cameraSpaceNormal.y,
                cameraSpaceNormal.z,
                (-Vector3.Dot(_portalCam.worldToCameraMatrix.MultiplyPoint(transform.position), cameraSpaceNormal) + 0.05f)));

            // Vector3 cameraSpacePosition = _portalCam.worldToCameraMatrix.MultiplyPoint(transform.position);
            // float cameraSpaceDestination = -Vector3.Dot(cameraSpacePosition, cameraSpaceNormal) + 0.05f;

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

    private bool PortalVisableByCamera()
    {
        return GeometryUtility.TestPlanesAABB(GeometryUtility.CalculateFrustumPlanes(_playerCam), _otherPortal.PortalScreen.bounds);
    }
}
