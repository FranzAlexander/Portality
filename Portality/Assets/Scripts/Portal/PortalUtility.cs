using UnityEngine;

public static class PortalUtility
{
    public static bool VisableFromCamera(Renderer renderer, Camera camera)
    {
        Plane[] franstumPlanes = GeometryUtility.CalculateFrustumPlanes(camera);
        return GeometryUtility.TestPlanesAABB(franstumPlanes, renderer.bounds);
    }


    // public static bool BoundsOverlap(MeshFilter nearObject, MeshFilter farObject, Camera camera)
    // {
    //     var near = GetScreenRectFromBounds();
    // }
}
