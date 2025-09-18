using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class AutoCannon : MonoBehaviour
{
    public Transform baseYaw;           // ��ž�� Yaw ��
    public Transform cannonPitch;       // ��ž�� Pitch ��
    public Transform firePoint;         // ��ž�� �߻� ����
    public GameObject shellPrefab;      // �߻�ü ������
    public GameObject markerPrefab;     // ��Ŀ ������
    public float detectRadius = 20f;    // ���� �ݰ�
    public float fireAngle = 45f;       // �߻� ����
    public float rotationSpeed = 90f;   // ȸ�� �ӵ�
    public float fireCooldown = 2f;     // �߻� ��Ÿ��
    public LayerMask obstacleMask;      // ��ֹ� ����ũ (�÷��̾� ����)

    private Transform playerTarget;     // �÷��̾��� ��ġ
    private bool canFire = true;        // �߻� ���� ����
    public Transform FirePTC_Pos;       // �߻� ����Ʈ ��ġ
    public AudioClip Fire_clip;         // �߻� �Ҹ�
    public GameObject Fire_Ptc;         // �߻� ��ƼŬ ����Ʈ

    void Update()
    {
        // �� �����Ӹ��� �÷��̾ Ž���ϰ� �߻縦 �õ�
        DetectAndFire();
    }

    // �������� �þ� ������ �÷��̾ �����ϰ� �߻��ϴ� �Լ�
    void DetectAndFire()
    {
        // ��ü���� ���� ������ ��� ������Ʈ�� Ȯ��
        Collider[] hits = Physics.OverlapSphere(transform.position, detectRadius);

        // ���� ���� �ִ� ��� ��ü���� Ȯ��
        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                Vector3 playerPos = hit.transform.position;

                // ��ֹ��� ������ �÷��̾ �����Ͽ� �߻�
                if (IsPlayerInSight(playerPos))
                {
                    playerTarget = hit.transform;

                    // �߻� ������ �Ǹ� �߻�
                    if (canFire && !playerTarget.GetComponent<Player_Ctrl>().HitIs && !playerTarget.GetComponent<Player_Ctrl>().Die)
                    {
                        StartCoroutine(FireAtPlayer());
                    }
                    break; // �÷��̾ ã������ �� �̻� �˻��� �ʿ� ����
                }
            }
        }
    }

    // ��ֹ��� ������ �÷��̾ �߻� ������� �����ϴ� �Լ�
    bool IsPlayerInSight(Vector3 playerPos)
    {
        Vector3 dir = (playerPos - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, playerPos);

        // ��ֹ� üũ: �÷��̾���� ���� ��ο� ��ֹ��� ������ Ȯ��
        if (!Physics.Raycast(transform.position, dir, distance, obstacleMask))
        {
            return true;
        }

        return false;
    }

    // �÷��̾ �����ϰ� �߻��ϴ� �Լ�
    IEnumerator FireAtPlayer()
    {
        canFire = false;

        // �߻� ������ ��Ŀ�� ǥ��
        Vector3 targetGroundPos = new Vector3(playerTarget.position.x, 0, playerTarget.position.z);
        Instantiate(markerPrefab, targetGroundPos, Quaternion.identity);

        // �ε巴�� ȸ���Ͽ� ��ǥ�� ����
        yield return StartCoroutine(SmoothRotateTowards(targetGroundPos));

        // �߻�
        FireShell(targetGroundPos);

        // �߻� �� ��Ÿ�� ���
        yield return new WaitForSeconds(fireCooldown);

        canFire = true;
    }

    // ��ǥ�� ���� �ε巴�� ȸ���ϴ� �Լ�
    IEnumerator SmoothRotateTowards(Vector3 targetPos)
    {
        Vector3 flatDir = new Vector3(targetPos.x, baseYaw.position.y, targetPos.z) - baseYaw.position;
        Quaternion yawTargetRot = Quaternion.LookRotation(flatDir);

        float distance = Vector3.Distance(transform.position, playerTarget.position);
        float targetPitchAngle = Mathf.Lerp(-80f, -30f, Mathf.InverseLerp(detectRadius, 0f, distance));
        Quaternion pitchTargetRot = Quaternion.Euler(targetPitchAngle, 0f, 0f);

        while (Quaternion.Angle(baseYaw.rotation, yawTargetRot) > 1f ||
               Quaternion.Angle(cannonPitch.localRotation, pitchTargetRot) > 1f)
        {
            baseYaw.rotation = Quaternion.RotateTowards(baseYaw.rotation, yawTargetRot, rotationSpeed * Time.deltaTime);
            cannonPitch.localRotation = Quaternion.RotateTowards(cannonPitch.localRotation, pitchTargetRot, rotationSpeed * Time.deltaTime);

            yield return null;
        }
    }

    // ��ǥ�� ���� ��ź�� �߻��ϴ� �Լ�
    void FireShell(Vector3 targetPos)
    {
        Vector3 dir = targetPos - firePoint.position;
        dir.y = 0;
        float distance = dir.magnitude;
        float angleRad = fireAngle * Mathf.Deg2Rad;
        float g = Mathf.Abs(Physics.gravity.y);

        // �߻� �ӵ� ���
        float velocity = Mathf.Sqrt((distance * g) / Mathf.Sin(2 * angleRad));
        if (float.IsNaN(velocity)) return;

        Vector3 velocityVec = dir.normalized * velocity * Mathf.Cos(angleRad);
        velocityVec.y = velocity * Mathf.Sin(angleRad);

        // ��ź ���� �� �߻�
        GameObject shell = Instantiate(shellPrefab, firePoint.position, Quaternion.identity);
        Rigidbody rb = shell.GetComponent<Rigidbody>();
        rb.linearVelocity = velocityVec;

        // �߻� ����Ʈ
        InsEffect(FirePTC_Pos);
    }

    // �߻� ����Ʈ�� �����ϴ� �Լ�
    void InsEffect(Transform tr)
    {
        GameObject Audio = new GameObject(transform.name + "_Audio");
        Audio.transform.position = tr.position;
        Audio.AddComponent<Ins_Effect>().AudioPlay(Fire_clip);

        GameObject PTC = Instantiate(Fire_Ptc, tr.position, tr.rotation);
        PTC.AddComponent<Ins_Effect>().PtcPlay(PTC.GetComponent<ParticleSystem>());
    }

    // Unity �����Ϳ��� ���� ������ �ð������� ǥ��
    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectRadius);  // ���� ���� ǥ��
    }
}