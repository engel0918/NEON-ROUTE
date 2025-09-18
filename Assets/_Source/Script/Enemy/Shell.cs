using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shell : MonoBehaviour
{
    public AudioClip clip;
    public GameObject Ptc;

    Rigidbody rb;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Rigidbody�� velocity ������ ���մϴ�.
        Vector3 velocityDirection = rb.linearVelocity.normalized;

        // velocityDirection�� ���ϵ��� ȸ������ �����մϴ�.
        if (velocityDirection.sqrMagnitude > 0.01f) // �ӵ��� 0�� �ƴ� ���� ȸ��
        {
            Quaternion targetRotation = Quaternion.LookRotation(velocityDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }
    }


    private void OnCollisionEnter(Collision collision)
    {
        GameObject Audio = new GameObject(transform.name + "_Audio");
        Audio.AddComponent<Ins_Effect>().AudioPlay(clip);

        GameObject PTC = Instantiate(Ptc, transform.position, transform.rotation);
        PTC.AddComponent<Ins_Effect>().PtcPlay(PTC.GetComponent<ParticleSystem>());

        Destroy(gameObject);
    }
}