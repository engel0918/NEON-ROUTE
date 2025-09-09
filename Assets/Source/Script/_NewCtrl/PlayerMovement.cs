using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Transform cameraTransform;  // 메인 카메라 Transform

    private CharacterController controller;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 inputDir = new Vector3(h, 0, v).normalized;

        if (inputDir.magnitude >= 0.1f)
        {
            // 카메라 기준으로 방향 계산
            Vector3 moveDir = Quaternion.Euler(0, cameraTransform.eulerAngles.y, 0) * inputDir;

            controller.Move(moveDir * moveSpeed * Time.deltaTime);

            // 플레이어가 이동 방향을 바라보게 회전
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(moveDir), Time.deltaTime * 10f);
        }
    }
}