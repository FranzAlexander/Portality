using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

internal class PortalRendererFeature : ScriptableRendererFeature
{
    public Shader m_Shader;
    Material _material;

    PortalBlitPass _renderPass = null;

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (renderingData.cameraData.cameraType == CameraType.Game)
        {
            _renderPass.ConfigureInput(ScriptableRenderPassInput.Color);
            _renderPass.SetTarget(renderer.cameraColorTarget);
            renderer.EnqueuePass(_renderPass);
        }
    }

    public override void Create()
    {
        if (m_Shader != null)
        {
            _material = new Material(m_Shader);
        }

        _renderPass = new PortalBlitPass(_material);
    }

    protected override void Dispose(bool disposing)
    {
        CoreUtils.Destroy(_material);
    }

}
