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

    private int _recursionLimit = 5;

    // Getters and setters.
    public Camera PortalCam { get => _portalCam; }
    public Portal OtherPortal { get => _otherPortal; set => _otherPortal = value; }

    public MeshRenderer PortalScreen { get => _portalScreen; }
    public RenderTexture ViewTexture { get => _viewTexture; set => _viewTexture = value; }

    // Job systems.
    private RenderPortalJob _renderPortalJob;

    private void Awake()
    {
        // Cameras.
        _playerCam = Camera.main;
        _portalCam = GetComponentInChildren<Camera>();

        // Rendering.
        _portalScreen = gameObject.transform.GetChild(1).GetComponent<MeshRenderer>();

        _viewTexture = new RenderTexture(Screen.width, Screen.height, 24);
    }

    private void Start()
    {
        _portalCam.targetTexture = _viewTexture;
        _otherPortal.PortalScreen.material.SetTexture("_MainTex", _viewTexture);
    }

    public void RenderPortal(ScriptableRenderContext context)
    {
        // NativeArray<float4> cameraRenderPositions = new NativeArray<float4>(_recursionLimit, Allocator.TempJob);
        // NativeArray<float4> cameraRenderRotations = new NativeArray<float4>(_recursionLimit, Allocator.TempJob);

        // _renderPortalJob = new RenderPortalJob()
        // {
        //     cameraRenderPositions = cameraRenderPositions,
        //     cameraRenderRotations = cameraRenderRotations,
        //     playerCamMatrix = _playerCam.transform.localToWorldMatrix,
        //     currentPortalMatrix = transform.localToWorldMatrix,
        //     otherPortalMatrix = _otherPortal.transform.worldToLocalMatrix,
        // };

        // _renderPortalJob.Schedule<RenderPortalJob>().Complete();

        Matrix4x4 playerCameraMatrix = _playerCam.transform.localToWorldMatrix;
        Vector3[] portalCameraPositions = new Vector3[_recursionLimit];
        Quaternion[] portalCameraRotations = new Quaternion[_recursionLimit];

        _portalCam.projectionMatrix = _playerCam.projectionMatrix;

        for (int i = _recursionLimit - 1; i >= 0; --i)
        {
            playerCameraMatrix = transform.localToWorldMatrix * _otherPortal.transform.worldToLocalMatrix * playerCameraMatrix;

            portalCameraPositions[i] = playerCameraMatrix.GetColumn(3);
            portalCameraRotations[i] = playerCameraMatrix.rotation;
        }

        for (int i = 0; i < _recursionLimit; ++i)
        {
            _portalCam.transform.position = portalCameraPositions[i];
            _portalCam.transform.rotation = portalCameraRotations[i];
            // _portalCam.transform.SetPositionAndRotation(portalCameraPositions[i], portalCameraRotations[i]);
            //     _portalCam.transform.SetPositionAndRotation(
            //         new Vector3(
            //                     cameraRenderPositions[i].x,
            //                     cameraRenderPositions[i].y,
            //                     cameraRenderPositions[i].z
            //                     ),
            //         new Quaternion(
            //                     cameraRenderRotations[i].x,
            //                     cameraRenderRotations[i].y,
            //                     cameraRenderRotations[i].z,
            //                     cameraRenderRotations[i].w
            //                     ));

            // Vector3 camSpaceNormal = _portalCam.worldToCameraMatrix.MultiplyVector(transform.forward) * System.Math.Sign(Vector3.Dot(transform.forward, transform.position - _portalCam.transform.position));

            // _portalCam.projectionMatrix = _playerCam.CalculateObliqueMatrix(
            //     new Vector4(
            //         camSpaceNormal.x,
            //         camSpaceNormal.y,
            //         camSpaceNormal.z,
            //         -Vector3.Dot(_portalCam.worldToCameraMatrix.MultiplyPoint(transform.position), camSpaceNormal) + 0.05f
            //         ));

            UniversalRenderPipeline.RenderSingleCamera(context, _portalCam);
        }

        // cameraRenderPositions.Dispose();
        //     cameraRenderRotations.Dispose();
    }



    private bool VisableFromCamera(Renderer renderer)
    {
        return GeometryUtility.TestPlanesAABB(GeometryUtility.CalculateFrustumPlanes(_portalCam), renderer.bounds);
    }

    // Create the view texture for this portal to see the other side.
    // private void CreateViewTexture()
    // {
    //     if (_viewTexture == null || _viewTexture.width != Screen.width || _viewTexture.height != Screen.height)
    //     {
    //         if (_viewTexture != null)
    //         {
    //             _viewTexture.Release();
    //         }

    //         _viewTexture = new RenderTexture(Screen.width, Screen.height, 24);

    //         _portalCam.targetTexture = _viewTexture;
    //         _otherPortal.PortalScreen.material.SetTexture("_MainTex", _viewTexture);
    //     }
    // }

}

[BurstCompile]
public struct RenderPortalJob : IJob
{
    [WriteOnly]
    public NativeArray<float4> cameraRenderPositions;

    [WriteOnly]
    public NativeArray<float4> cameraRenderRotations;

    public float4x4 playerCamMatrix;

    public float4x4 currentPortalMatrix;

    public float4x4 otherPortalMatrix;

    public void Execute()
    {
        int recursionLimit = cameraRenderPositions.Length;

        for (int i = recursionLimit - 1; i >= 0; --i)
        {
            playerCamMatrix = currentPortalMatrix * otherPortalMatrix * playerCamMatrix;
            cameraRenderPositions[i] = playerCamMatrix.c3;
            cameraRenderRotations[i] = playerCamMatrix.c2;
        }
    }
}