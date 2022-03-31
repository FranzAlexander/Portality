using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleportable : MonoBehaviour
{
    private GameObject _teleportableClone;

    public GameObject TeleortableCloneGraphics;
    public GameObject TeleportableClone { get => _teleportableClone; }

    public Material[] OriginalMaterials { get; set; }
    public Material[] CloneMaterials { get; set; }

    private new Rigidbody rigidbody;

    private bool _overlapping;
    private void Awake()
    {
        // _teleportableClone = new GameObject();
        rigidbody = GetComponent<Rigidbody>();
    }

    public void Teleport(Transform source, Transform destination, Vector3 distanceToTeleportable)//, Vector3 position, Quaternion rotation)
    {
        float rotationDiff = -Quaternion.Angle(source.rotation, destination.rotation);
        rotationDiff += 180;
        transform.Rotate(Vector3.up, rotationDiff);

        Vector3 positionOffset = Quaternion.Euler(0f, rotationDiff, 0f) * distanceToTeleportable;
        transform.position = destination.position + positionOffset;

        UpdateClone();
        // gameObject.transform.position = position;
        // gameObject.transform.rotation = rotation;

        // transform.position = source.position;
        // transform.position = source.position;
        //    rigidbody.velocity = source.TransformVector(destination.InverseTransformVector(rigidbody.velocity));
        //rigidbody.angularVelocity = source.TransformVector(destination.InverseTransformVector(rigidbody.angularVelocity));
    }

    public void UpdateClone()
    {
        _teleportableClone.GetComponent<Rigidbody>().velocity = rigidbody.velocity;
        _teleportableClone.GetComponent<Rigidbody>().angularVelocity = rigidbody.angularVelocity;
    }

    public void EnterPortal()
    {
        // if (_teleportableClone == null)
        // {
        //     _teleportableClone = Instantiate(TeleortableCloneGraphics);
        //     _teleportableClone.transform.parent = transform;
        // }
        // if (TeleportableClone == null)
        // {
        //     TeleportableClone = Instantiate(TeleportableObject);
        //     TeleportableClone.transform.parent = TeleportableObject.transform.parent;
        //     TeleportableClone.transform.localScale = TeleportableObject.transform.localScale;
        //     OriginalMaterials = GetMaterials(TeleportableObject);
        //     CloneMaterials = GetMaterials(TeleportableClone);
        // }
        // else
        // {
        //     TeleportableClone.SetActive(true);
        // }
    }

    public void ExitPortal()
    {
        // Destroy(_teleportableClone);
    }

    private Material[] GetMaterials(GameObject gameObject)
    {
        MeshRenderer[] meshRenderers = gameObject.GetComponentsInChildren<MeshRenderer>();
        List<Material> material = new List<Material>();

        foreach (var renderer in meshRenderers)
        {
            foreach (var mat in renderer.materials)
            {
                material.Add(mat);
            }
        }

        return material.ToArray();
    }
}
