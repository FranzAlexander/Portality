using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Portal : MonoBehaviour
{
    // Cameras.
    private Camera _playerCam;

    private Camera _portalCam;

    // The other portal in the scene.
    private Portal _otherPortal;

    // Portal renderering.
    private MeshRenderer _portalScreen;

    private RenderTexture _viewTexture;

    // Portal camera recursion rendering.
    private const int _recursionLimit = 7;

    private Vector3[] _portalCameraPositions;

    private Quaternion[] _portalCameraRotations;

    // Reference to parent.
    private PortalManager _portalManager;

    // List of all objects currently being teleported.
    private List<Teleportable> _teleportables;

    // Getters and setters.
    public Camera PortalCam { get => _portalCam; }
    public Portal OtherPortal { get => _otherPortal; set => _otherPortal = value; }

    public MeshRenderer PortalScreen { get => _portalScreen; }
    public RenderTexture ViewTexture { get => _viewTexture; set => _viewTexture = value; }

    private void Awake()
    {
        // Cameras.
        _playerCam = Camera.main;
        _portalCam = GetComponentInChildren<Camera>();

        // Rendering.
        _portalScreen = gameObject.transform.GetChild(1).GetComponent<MeshRenderer>();

        _viewTexture = new RenderTexture(Screen.width, Screen.height, 24);

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

    private void PortalTextureSetup()
    {
        if (_portalCam.targetTexture != null)
        {
            _viewTexture.Release();
        }

        _viewTexture = new RenderTexture(Screen.width, Screen.height, 24);
        _portalCam.targetTexture = _viewTexture;
        _otherPortal.PortalScreen.material.SetTexture("_MainTex", _viewTexture);
    }

    public void RenderPortal(ScriptableRenderContext context)
    {
        // Matrix4x4 playerCameraMatrix = _playerCam.transform.localToWorldMatrix;

        Vector3 cameraNormalSpace;
        float cameraSpaceDest = 0.0f;

        Vector3 cameraPositionInSourceSpace;
        Quaternion cameraRotationInSourceSpace;

        _portalCam.projectionMatrix = _playerCam.projectionMatrix;
        //int secondIndex = 0;

        // TEST THIS CODE TOMORROW ALEX....

        // First loop recursions to get positions and rotations for the camera.
        for (int i = _recursionLimit - 1; i >= 0; --i)
        {
            cameraPositionInSourceSpace = _otherPortal.transform.InverseTransformPoint(_playerCam.transform.position);
            cameraRotationInSourceSpace = Quaternion.Inverse(_otherPortal.transform.rotation) * _playerCam.transform.rotation;

            _portalCameraPositions[i] = transform.InverseTransformPoint(cameraPositionInSourceSpace);
            _portalCameraRotations[i] = transform.rotation * cameraRotationInSourceSpace;
            // playerCameraMatrix = transform.localToWorldMatrix * _otherPortal.transform.worldToLocalMatrix * playerCameraMatrix;
            // _portalCameraPositions[i] = playerCameraMatrix.GetColumn(3);
            // _portalCameraRotations[i] = playerCameraMatrix.rotation;

        }

        for (int i = 0; i < _recursionLimit; ++i)
        {
            _portalCam.transform.position = _portalCameraPositions[i];
            _portalCam.transform.rotation = _portalCameraRotations[i];

            cameraNormalSpace = _portalCam.worldToCameraMatrix.MultiplyVector(transform.forward) *
                                System.Math.Sign(Vector3.Dot(transform.forward, transform.position - _portalCam.transform.position));

            cameraSpaceDest = -Vector3.Dot(_portalCam.worldToCameraMatrix.MultiplyPoint(transform.position), cameraNormalSpace) + 0.05f;

            if (Mathf.Abs(cameraSpaceDest) > 0.05f)
            {
                _portalCam.projectionMatrix = _playerCam.CalculateObliqueMatrix(
                    new Vector4(
                        cameraNormalSpace.x,
                        cameraNormalSpace.y,
                        cameraNormalSpace.z,
                        cameraSpaceDest
                    )
                );
            }
            else
            {
                _portalCam.projectionMatrix = _playerCam.projectionMatrix;
            }

            UniversalRenderPipeline.RenderSingleCamera(context, _portalCam);
        }
    }

    // private bool VisableFromCamera(Renderer renderer)
    // {
    //     return GeometryUtility.TestPlanesAABB(GeometryUtility.CalculateFrustumPlanes(_portalCam), renderer.bounds);
    // }

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

[BurstCompile]
public struct RenderPortalJob : IJob
{
    public NativeArray<float4> cameraPositions;
    public NativeArray<quaternion> cameraRotations;

    //  public NativeArray<portalcameratr> camMatrix;

    public float4x4 playerCamMatrix;

    public float4x4 currentPortalMatrix;

    public float4x4 otherPortalMatrix;

    public void Execute()
    {
        for (int i = cameraPositions.Length - 1; i >= 0; --i)
        {
            playerCamMatrix = math.mul(math.mul(currentPortalMatrix, otherPortalMatrix), playerCamMatrix);
            cameraPositions[i] = playerCamMatrix.c3;
            cameraRotations[i] = new quaternion(playerCamMatrix);
        }
    }
}