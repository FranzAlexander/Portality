using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleportable : MonoBehaviour
{
    private GameObject _teleportableClone;

    private Rigidbody _rigidbody;

    private Vector3 oldPosition;
    private Quaternion oldRotation;

    public GameObject TeleportableClone { get => _teleportableClone; private set => _teleportableClone = value; }

    private void Awake()
    {
        CreateObjectClone();

        _rigidbody = GetComponent<Rigidbody>();
    }

    private void CreateObjectClone()
    {
        _teleportableClone = new GameObject(transform.name + "clone");
        _teleportableClone.SetActive(false);

        MeshFilter cloneMeshFilter = _teleportableClone.AddComponent<MeshFilter>();
        MeshRenderer cloneMeshRenderer = _teleportableClone.AddComponent<MeshRenderer>();

        cloneMeshFilter.mesh = GetComponent<MeshFilter>().mesh;
        cloneMeshRenderer.materials = GetComponent<MeshRenderer>().materials;

        _teleportableClone.transform.localScale = transform.localScale;
    }

    public void EnterPortal()
    {
        _teleportableClone.SetActive(true);
    }

    public void ExitPortal()
    {
        _teleportableClone.SetActive(false);
    }

    public void Teleport(Transform source, Transform destination)
    {
        Vector3 relativePosition = source.InverseTransformPoint(transform.position);
        transform.position = destination.TransformPoint(relativePosition);

        oldPosition = relativePosition;

        Quaternion relativeRotation = Quaternion.Inverse(source.rotation) * transform.rotation;
        transform.rotation = destination.rotation * relativeRotation;

        oldRotation = relativeRotation;

        Vector3 relativeVelocity = source.InverseTransformDirection(_rigidbody.velocity);
        _rigidbody.velocity = destination.TransformDirection(relativeVelocity);
    }
}