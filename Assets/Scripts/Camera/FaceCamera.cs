using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    private Transform _mainCameraTransform;

    private Transform _transform;

    private void Start()
    {
        _mainCameraTransform = Camera.main.transform;

        _transform = transform;
    }

    private void LateUpdate()
    {
        _transform.LookAt(
            _transform.position + _mainCameraTransform.rotation * Vector3.forward,
            _mainCameraTransform.rotation * Vector3.up);
    }
}
