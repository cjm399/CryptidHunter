using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Axis
{
    X,
    Y,
    Z
}

public class FaceCamera : MonoBehaviour
{
    [SerializeField] private Axis upwardAxis = Axis.Y;

    private Transform cameraTransform;
    private Transform cachedTransform;
    private Vector3 _upAxis;

    private void Start()
    {
        cameraTransform = Camera.main.transform;
        cachedTransform = transform;
    }

    private void FixedUpdate()
    {
        cachedTransform.LookAt(cameraTransform, SetAxis(upwardAxis));
    }

    private Vector3 SetAxis(Axis _a)
    {
        if(_a == Axis.X)
        {
            return cachedTransform.right;
        }
        else if(_a == Axis.Y)
        {
            return cachedTransform.up;
        }
        else
        {
            return cachedTransform.forward;
        }
    }
}
