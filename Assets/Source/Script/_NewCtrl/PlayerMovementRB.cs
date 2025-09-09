using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

//[RequireComponent(typeof(Rigidbody))]
public class PlayerMovementRB : MonoBehaviour
{
    public Transform Y_Axis;
    public Transform cameraTransform;
    private Rigidbody rb;
    public Transform firstPP_Campos;
    public Transform thirdPP_Campos;

    ThirdPersonCamera ThirdPp_Cam;

    public string PP;

    bool MovOn, RotOn, CamMov;

    public float Spd, RunSpd, Y_Rot_Spd, Rot_Spd, CamSpd;

    float Mouse_Y;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        ThirdPp_Cam = cameraTransform.GetComponent<ThirdPersonCamera>();
    }

    private void Update()
    {
        Switching();

        if (PP == "third_PP")
        {
            Mode_Third_PP();


            if (CamMov == true)
            {
                cameraTransform.SetParent(firstPP_Campos);

                CamMov = false;
            }
        }
        else if (PP == "first_PP") 
        {
            Mode_First_PP(); 

            if(CamMov == true)
            {
                cameraTransform.SetParent(firstPP_Campos);

                float dis = Vector3.Distance(firstPP_Campos.position, cameraTransform.position);
                float rot = Quaternion.Angle(firstPP_Campos.rotation, cameraTransform.rotation);

                if (dis > 0.01f || rot > 0.1f) // ȸ���� �ణ�� ������ ũ�� ���� �� �����Ƿ� �� ����
                {
                    cameraTransform.position =
                        Vector3.MoveTowards(cameraTransform.position, firstPP_Campos.position, Time.deltaTime * CamSpd);

                    cameraTransform.rotation =
                        Quaternion.RotateTowards(cameraTransform.rotation, firstPP_Campos.rotation, Time.deltaTime * CamSpd * 50f);
                    // ȸ�� �ӵ��� �ʿ信 ���� CamSpd�� ����� ������ ������ �����ִ� ���� ����
                }
                else { CamMov = false; }
            }
        }

        
    }

    public void Switching()
    {
        if (Input.GetKeyDown(KeyCode.V) == true)
        {
            if (PP == "first_PP")
            {
                PP = "third_PP";
            }
            else if (PP == "third_PP")
            {
                PP = "first_PP";
            }

            CamMov = true;

        }
    }

    void Mode_Third_PP()
    {
        if(ThirdPp_Cam.enabled == false)
        { ThirdPp_Cam.enabled = true; }

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 inputDir = new Vector3(h, 0, v).normalized;

        if (inputDir.magnitude >= 0.1f)
        {
            Vector3 moveDir = Quaternion.Euler(0, cameraTransform.eulerAngles.y, 0) * inputDir;
            Vector3 targetVelocity = moveDir * Spd;

            rb.linearVelocity = new Vector3(targetVelocity.x, rb.linearVelocity.y, targetVelocity.z);

            Quaternion targetRot = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.fixedDeltaTime * 10f);
        }
        else
        {
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
        }
    }

    void Mode_First_PP()
    {
        if (ThirdPp_Cam.enabled == true)
        { ThirdPp_Cam.enabled = false; }


        //if (Stop == false)
        {
            //if (HitIs == false && Die == false)
            {

                // Ctrl �κ�
                float Horizontal = Input.GetAxis("Horizontal");
                float Vertical = Input.GetAxis("Vertical");

                if (Horizontal != 0 || Vertical != 0)
                { MovOn = true; }
                else { MovOn = false; }


                if (Input.GetKey(KeyCode.LeftShift))
                {
                    transform.Translate(Vector3.right * Spd * RunSpd * Horizontal * Time.deltaTime);
                    transform.Translate(Vector3.forward * Spd * RunSpd * Vertical * Time.deltaTime);
                }
                else
                {
                    transform.Translate(Vector3.right * Spd * Horizontal * Time.deltaTime);
                    transform.Translate(Vector3.forward * Spd * Vertical * Time.deltaTime);
                }


                float Mouse_X = Input.GetAxis("Mouse X");
                float Mouse_y = Input.GetAxis("Mouse Y");

                transform.Rotate(Vector3.up * Rot_Spd * Mouse_X * Time.deltaTime);
                Y_Axis.Rotate(Vector3.right * Rot_Spd * -Mouse_y * Time.deltaTime);
                //Jump();

                if (Mouse_X != 0 || Mouse_Y != 0)
                { RotOn = true; }
                else
                { RotOn = false; }

                Mouse_Y += Input.GetAxis("Mouse Y") * Y_Rot_Spd;

                Vector3 Cha_Angle = Y_Axis.localEulerAngles;
                Cha_Angle.x = (Cha_Angle.x > 180) ? Cha_Angle.x - 360 : Cha_Angle.x;
                Cha_Angle.x = Mathf.Clamp(Cha_Angle.x, -60, 60);
                Y_Axis.localEulerAngles = new Vector3(Cha_Angle.x, 0, 0);
            }
        }

    }

}