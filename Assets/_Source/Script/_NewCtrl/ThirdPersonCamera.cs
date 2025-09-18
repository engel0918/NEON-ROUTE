using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform target;
    public float distance = 3.0f;
    public float height = 1.5f;
    public float sensitivity = 3.0f;
    public LayerMask collisionMask;

    float yaw = 0f;
    float pitch = 0f;

    void LateUpdate()
    {
        yaw += Input.GetAxis("Mouse X") * sensitivity;
        pitch -= Input.GetAxis("Mouse Y") * sensitivity;
        pitch = Mathf.Clamp(pitch, -30f, 60f);

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);
        Vector3 desiredPos = target.position - rotation * Vector3.forward * distance + Vector3.up * height;

        // 카메라 충돌 처리
        RaycastHit hit;
        Vector3 direction = desiredPos - (target.position + Vector3.up * height);
        if (Physics.Raycast(target.position + Vector3.up * height, direction.normalized, out hit, distance, collisionMask))
        {
            desiredPos = hit.point;
        }

        transform.position = desiredPos;
        transform.rotation = rotation;
    }
}