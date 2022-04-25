using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Jobs;
using UnityEngine.Profiling;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public struct CameraMatrix
{
    public float3 position;
    public quaternion rotation;
}

public class Portal : MonoBehaviour
{
    // Main camera.
    private Camera _playerCam;

    private Camera _portalCam;

    // Reference to the other portal.
    private Portal _otherPortal;

    // Portal rendering.
    [SerializeField]
    private MeshRenderer _portalScreen;

    [SerializeField]
    private MeshFilter _portalScreenFilter;

    private RenderTexture _viewTexture;

    private const int _recursionLimit = 7;

    // Position and Rotation for the portal camera.
    private Vector3[] _portalCameraPositions;

    private Quaternion[] _portalCameraRotations;

    // Objects to teleport.
    private List<Teleportable> _teleportables;

    private int _renderIndex;

    #region Getter Setter Properties
    // Getters and Setter auto properties.
    public Camera PortalCamera { get => _portalCam; private set => _portalCam = value; }

    public Portal OtherPortal { get => _otherPortal; set => _otherPortal = value; }

    public MeshRenderer PortalScreen { get => _portalScreen; private set => _portalScreen = value; }
    public MeshFilter PortalScreenFilter { get => _portalScreenFilter; private set => _portalScreenFilter = value; }
    #endregion
    private CustomSampler _portalCameraPRLoop;

    private CustomSampler _portalCameraVisable;

    // Jobs.
    private RenderPortalMatrixJob _renderPortalMatrixJob;

    private JobHandle _jobHandle;

    private NativeArray<CameraMatrix> _cameraMatrix;

    private void Awake()
    {
        // Cameras.
        _playerCam = Camera.main;
        _portalCam = GetComponentInChildren<Camera>();

        // Rendering.
        //_portalScreen = GetComponentInChildren<MeshRenderer>();
        //_portalScreenFilter = _portalScreen.GetComponent<MeshFilter>();

        _portalCameraPositions = new Vector3[_recursionLimit];
        _portalCameraRotations = new Quaternion[_recursionLimit];

        _renderIndex = 0;

        _teleportables = new List<Teleportable>();
    }

    private void Start()
    {
        _portalCameraPRLoop = CustomSampler.Create("First Portal Render Loop");
        _portalCameraVisable = CustomSampler.Create("Portal Visable from camera");
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

        //_otherPortal.PortalScreen.material.mainTexture = _viewTexture;
        _otherPortal.PortalScreen.material.SetTexture("_MainTex", _viewTexture);
    }

    public void RenderPortal(ScriptableRenderContext context, bool floor)
    {
        if (!PortalUtility.PortalVisableByCamera(_otherPortal.PortalScreen, _playerCam)) { return; }

        Matrix4x4 cameraMatrix;

        //Vector3 cameraPosition;
        // Quaternion cameraRotation;

        _portalCameraPRLoop.Begin();

        for (int i = 0; i < _recursionLimit; i++)
        {
            // if (i > 0)
            // {
            //     if (!PortalUtility.BoundsOverlap(_portalScreenFilter, _otherPortal.PortalScreenFilter, _portalCam))
            //     {
            //         break;
            //     }
            // }

            //cameraMatrix = transform.localToWorldMatrix * _otherPortal.transform.worldToLocalMatrix * _playerCam.transform.localToWorldMatrix;

            // Debug.Log("Less complicated: " + (transform.position + (_playerCam.transform.position - _otherPortal.transform.position)));
            // Debug.Log("More complicated: " + _otherPortal.transform.TransformPoint(transform.InverseTransformPoint(_playerCam.transform.position)));
            // Debug.Log("Super complicated: " + cameraMatrix.GetPosition());

            // Debug.Log("Sorta Less Complicated: " + Quaternion.LookRotation(Quaternion.AngleAxis(Quaternion.Angle(transform.rotation, _otherPortal.transform.rotation), transform.up) * _playerCam.transform.forward));
            // Debug.Log("More Complicated: " + (_otherPortal.transform.rotation * (Quaternion.Inverse(transform.rotation) * _playerCam.transform.rotation)));
            // Debug.Log("Super Complicated: " + cameraMatrix.rotation);
            // cameraRotation = Quaternion.Inverse(transform.rotation) * _playerCam.transform.rotation;
            // cameraRotation = Quaternion.Euler(180.0f, 0.0f, 0.0f) * cameraRotation;

            _renderIndex = _recursionLimit - i - 1;

            _portalCameraPositions[i] = (transform.position + (_playerCam.transform.position - _otherPortal.transform.position)) - (-transform.up * 2);
            _portalCameraRotations[i] = Quaternion.Euler(180.0f, 0.0f, 0.0f) * (_otherPortal.transform.rotation * (Quaternion.Inverse(transform.rotation) * _playerCam.transform.rotation));

            //_portalCameraPositions[i] = cameraMatrix.GetPosition() - (-transform.up * 2);
            // _portalCameraRotations[i] = Quaternion.Euler(180.0f, 0.0f, 0.0f) * cameraMatrix.rotation;
            // _portalCameraPositions[i] = (transform.position + (_playerCam.transform.position - _otherPortal.transform.position) - (-transform.up * 2));

            // float angleDiff = Quaternion.Angle(transform.rotation, _otherPortal.transform.rotation);
            // Quaternion portalRotDif = Quaternion.AngleAxis(angleDiff, transform.up);
            // Vector3 newCameraDir = portalRotDif * _playerCam.transform.forward;
            // _portalCameraRotations[i] = Quaternion.LookRotation(newCameraDir, transform.up);
            // _portalCameraRotations[i] = (_otherPortal.transform.rotation * cameraRotation);

            //_portalCameraRotations[i] = _otherPortal.transform.rotation * cameraRotation;

        }
        _portalCameraPRLoop.End();

        for (int i = _renderIndex; i < _recursionLimit; i++)
        {
            _portalCam.transform.position = _portalCameraPositions[i];
            _portalCam.transform.rotation = _portalCameraRotations[i];

            // Setting the camera's oblique view frustum.
            Vector3 cameraSpaceNormal = _portalCam.worldToCameraMatrix.MultiplyVector(transform.up) * System.Math.Sign(Vector3.Dot(transform.up, transform.position - _portalCam.transform.position));
            float cameraSpaceDistance = -Vector3.Dot(_portalCam.worldToCameraMatrix.MultiplyPoint(transform.position), cameraSpaceNormal) + 0.05f;
            if (Mathf.Abs(cameraSpaceDistance) > 0.05f)
            {
                _portalCam.projectionMatrix = _playerCam.CalculateObliqueMatrix(new Vector4(
                    cameraSpaceNormal.x,
                    cameraSpaceNormal.y,
                    cameraSpaceNormal.z,
                    cameraSpaceDistance));
            }
            else
            {
                _portalCam.projectionMatrix = _playerCam.projectionMatrix;
            }

            UniversalRenderPipeline.RenderSingleCamera(context, _portalCam);
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        Teleportable newTeleportable = other.GetComponent<Teleportable>();

        if (newTeleportable)
        {
            newTeleportable.EnterPortal(gameObject.activeSelf && _otherPortal.gameObject.activeSelf ? true : false);
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
public struct RenderPortalMatrixJob : IJob
{
    public float3 portalPosition;
    public float3 otherPortalPosition;
    public float3 playerCameraPosition;

    public quaternion portalRotation;
    public quaternion otherPortalRotation;
    public quaternion playerCameraRotation;

    public void Execute()
    {
        throw new System.NotImplementedException();
    }
}