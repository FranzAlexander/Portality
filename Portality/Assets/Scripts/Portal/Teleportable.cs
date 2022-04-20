using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleportable : MonoBehaviour
{
    private GameObject _teleportableClone;
    public GameObject TeleportableClone { get => _teleportableClone; }

    public Material[] OriginalMaterials { get; set; }
    public Material[] CloneMaterials { get; set; }

    private new Rigidbody rigidbody;

    private Vector3 oldPosition;
    private Quaternion oldRotation;

    private bool _overlapping;
    private void Awake()
    {
        CreateObjectClone();

        rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (_teleportableClone.activeSelf)
        {
            _teleportableClone.transform.position = oldPosition;
            _teleportableClone.transform.rotation = oldRotation;
        }
    }

    private void CreateObjectClone()
    {
        // if (_teleportableClone == null)
        // {
        //     _teleportableClone = new GameObject(this.transform.name + "clone");
        //     //List<Component> cloneComponets = GetComponents<Component>();
        //     Component[] cloneComponets = GetComponents<Component>();

        //     for (int i = 0; i < cloneComponets.Length; i += 1)
        //     {
        //         if (!cloneComponets[i] == GetComponent<Teleportable>())
        //         {
        //             _teleportableClone.AddComponent(cloneComponets[i]);
        //         }
        //     }
        // }

        _teleportableClone = new GameObject(transform.name + "clone");
        _teleportableClone.SetActive(false);

        // Added Mesh components to clone.
        MeshFilter cloneMeshFilter = _teleportableClone.AddComponent<MeshFilter>();
        MeshRenderer cloneMeshRenderer = _teleportableClone.AddComponent<MeshRenderer>();

        cloneMeshFilter.mesh = GetComponent<MeshFilter>().mesh;
        cloneMeshRenderer.materials = GetComponent<MeshRenderer>().materials;

        _teleportableClone.transform.localScale = transform.localScale;

    }

    public void Teleport(Transform source, Transform destination)//, Vector3 position, Quaternion rotation)
    {
        Vector3 relativePosition = source.InverseTransformPoint(transform.position);
        transform.position = destination.TransformPoint(relativePosition);

        oldPosition = relativePosition;

        Quaternion relativeRotation = Quaternion.Inverse(source.rotation) * transform.rotation;
        transform.rotation = destination.rotation * relativeRotation;

        oldRotation = relativeRotation;

        Vector3 relativeVelocity = source.InverseTransformDirection(rigidbody.velocity);
        rigidbody.velocity = destination.TransformDirection(relativeVelocity);
        // float rotationDiff = -Quaternion.Angle(source.rotation, destination.rotation);
        // rotationDiff += 180;
        // transform.Rotate(Vector3.up, rotationDiff);

        // Vector3 positionOffset = Quaternion.Euler(0f, rotationDiff, 0f) * distanceToTeleportable;
        // transform.position = destination.position + positionOffset;

        // UpdateClone();
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
        _teleportableClone.SetActive(true);
        // }
    }

    public void ExitPortal()
    {
        _teleportableClone.SetActive(false);

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
