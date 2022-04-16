using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

internal class PortalBlitPass : ScriptableRenderPass
{
    ProfilingSampler _profilingSampler = new ProfilingSampler("PortalBlit");
    public Material _material = null;
    RenderTargetIdentifier _cameraColorTarget;

    private RenderTargetIdentifier _source { get; set; }
    private RenderTargetIdentifier _dest { get; set; }

    public PortalBlitPass(Material material)
    {
        _material = material;
        this.renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
    }

    public void SetTarget(RenderTargetIdentifier colorHandle)
    {
        _cameraColorTarget = colorHandle;
    }

    public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
    {
        ConfigureTarget(new RenderTargetIdentifier(_cameraColorTarget, 0, CubemapFace.Unknown, -1));
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        Camera camera = renderingData.cameraData.camera;

        RenderTargetIdentifier source = BuiltinRenderTextureType.CameraTarget;
        RenderTargetIdentifier destination = BuiltinRenderTextureType.CurrentActive;

        if (camera.cameraType != CameraType.Game)
        {
            return;
        }

        if (_material == null)
        {
            return;
        }

        CommandBuffer cmd = CommandBufferPool.Get();

        using (new ProfilingScope(cmd, _profilingSampler))
        {
            cmd.SetRenderTarget(new RenderTargetIdentifier(_cameraColorTarget, 0, CubemapFace.Unknown, -1));
            cmd.DrawMesh(RenderingUtils.fullscreenMesh, Matrix4x4.identity, _material);
        }

        context.ExecuteCommandBuffer(cmd);
        cmd.Clear();

        CommandBufferPool.Release(cmd);
    }

}
