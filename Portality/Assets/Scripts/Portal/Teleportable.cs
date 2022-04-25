using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleportable : MonoBehaviour
{
    private GameObject _teleportableClone;

    private Rigidbody _rigidbody;

    private bool _bothPortalsActive;

    private Vector3 oldPosition;
    private Quaternion oldRotation;

    public GameObject TeleportableClone { get => _teleportableClone; private set => _teleportableClone = value; }

    private void Awake()
    {
        CreateObjectClone();

        _rigidbody = GetComponent<Rigidbody>();

        _bothPortalsActive = false;
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

    public void EnterPortal(bool bothPortalsActive)
    {
        _bothPortalsActive = bothPortalsActive;
        _teleportableClone.SetActive(true);
    }

    public void ExitPortal()
    {
        _teleportableClone.SetActive(false);
    }

    private void LateUpdate()
    {
        if (!_bothPortalsActive) { return; }

        if (_teleportableClone.activeSelf && _bothPortalsActive)
        {
            _teleportableClone.transform.position = oldPosition;
            _teleportableClone.transform.rotation = oldRotation;
        }
    }

    public void Teleport(Transform source, Transform destination)
    {
        Vector3 portalOffset = transform.position - source.transform.position;

        if (Vector3.Dot(transform.up, portalOffset) < 0f)
        {
            Matrix4x4 prMatrix = destination.localToWorldMatrix * source.worldToLocalMatrix * transform.localToWorldMatrix;
            oldPosition = transform.position;
            oldRotation = transform.rotation;

            transform.position = prMatrix.GetPosition();
            transform.rotation = prMatrix.rotation;

            Vector3 relativeVelocity = source.TransformDirection(_rigidbody.velocity);
            relativeVelocity = Quaternion.Euler(0.0f, 180.0f, 0.0f) * relativeVelocity;
            _rigidbody.velocity = destination.InverseTransformDirection(relativeVelocity);

        }

        // Vector3 relativePosition = source.TransformPoint(transform.position);
        // transform.position = destination.InverseTransformPoint(relativePosition);

        // oldPosition = relativePosition;

        // Quaternion relativeRotation = Quaternion.Inverse(source.rotation) * transform.rotation;
        // transform.rotation = destination.rotation * relativeRotation;

        // oldRotation = relativeRotation;

        // Vector3 relativeVelocity = source.TransformDirection(_rigidbody.velocity);
        // _rigidbody.velocity = destination.InverseTransformDirection(relativeVelocity);
    }

    private void OnDestroy()
    {
        GameObject.Destroy(_teleportableClone);
    }
}