using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Transform cameraTransform;  // ���� ī�޶� Transform

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
            // ī�޶� �������� ���� ���
            Vector3 moveDir = Quaternion.Euler(0, cameraTransform.eulerAngles.y, 0) * inputDir;

            controller.Move(moveDir * moveSpeed * Time.deltaTime);

            // �÷��̾ �̵� ������ �ٶ󺸰� ȸ��
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(moveDir), Time.deltaTime * 10f);
        }
    }
}