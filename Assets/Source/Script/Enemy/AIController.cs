using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class AIController : MonoBehaviour
{
    [Header("���� ����")]
    public float viewRadius = 10f;      // ���� ����

    [Header("���� ����")]
    public float viewAngle = 90f;       // ���� ����

    [Header("��ֹ� ����")]
    public LayerMask obstacleMask;      // ��ֹ� ���̾�

    [Header("���� ���")]
    public Transform[] patrolPoints;    // ���� ���

    [Header("����ǥ�� �ִϸ�")]
    public Animator Emotji_Anim;        // �̸��� �ִϸ�����

    [Header("������")]
    public float Delay = 3f;            // ������ ��ȯ �� ��� �ð�

    private NavMeshAgent agent;
    private Player_Ctrl playerCtrl;
    private Transform player;
    private int patrolIndex = 0;
    private bool isChasing = false;     // ���� �� ����
    private float lastSeenTime = 0f;    // ���������� �÷��̾ �� �ð�
    private float chaseDuration = 5f;   // �÷��̾� ��ģ �� ������ ��ȯ �ð�
    private Vector3 lastKnownPosition;  // ���������� �÷��̾ �� ��ġ
    private bool isSearching = false;   // ���� ��ġ Ž�� ����

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        playerCtrl = GameObject.FindGameObjectWithTag("Player").GetComponent<Player_Ctrl>(); // �÷��̾� ã��
        GoToNextPatrolPoint(); // ù ���� �������� �̵�
    }

    void Update()
    {
        if (CanSeePlayer()) // �÷��̾ �߰��ߴ��� Ȯ��
        {
            if (!isChasing)
            {
                // �÷��̾� �߰� �� ����ǥ �ִϸ��̼�
                Emotji_Anim.SetTrigger("EX_mark");
                //Debug.Log("�÷��̾� �߰�! ���� ����!");
            }

            isChasing = true;
            lastKnownPosition = player.transform.position; // ������ ��ġ ����
            lastSeenTime = Time.time; // ���� �ð� ����
        }

        if (isChasing)
        {
            if (player != null)
            { agent.SetDestination(player.transform.position); } // �÷��̾� ����

            // ���� �ð� ���� �÷��̾ �� ���� ���� ����
            if (Time.time - lastSeenTime > chaseDuration)
            {
                isChasing = false;
                isSearching = true; // ���� ��ġ Ž�� ����
                Emotji_Anim.SetTrigger("Q_mark"); // ����ǥ �ִϸ��̼�
                //Debug.Log("�÷��̾� ��ħ. ���� ��ġ�� �̵�...");

                agent.SetDestination(lastKnownPosition);
                Invoke(nameof(StopSearching), Delay); // ������ �� ���� �簳
            }
        }
        else if (isSearching) // ���� ��ġ�� �̵� ��
        {
            // ���� ��ġ�� ���������� ���� �Ϸ�
            if (!agent.pathPending && agent.remainingDistance < 0.5f)
            {
                StopSearching();
            }
        }
        else if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            GoToNextPatrolPoint(); // ���� ���� �������� �̵�
        }
    }

    private void FixedUpdate()
    {
        if(playerCtrl.HitIs == true || playerCtrl.Die)
        { player = null; }
        else { player = playerCtrl.transform; }

    }

    // �÷��̾� ���� ����
    bool CanSeePlayer()
    {
        if (player == null) return false;

        Vector3 dirToPlayer = (player.transform.position - transform.position).normalized;

        // �þ߰� ���� �ִ��� Ȯ��
        if (Vector3.Angle(transform.forward, dirToPlayer) < viewAngle / 2)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

            // ��ֹ��� ���� ��� ���� ����
            if (!Physics.Raycast(transform.position, dirToPlayer, distanceToPlayer, obstacleMask))
            {
                return true; // �÷��̾� ����
            }
        }
        return false; // ���� ����
    }

    // ���� �������� �̵�
    void GoToNextPatrolPoint()
    {
        if (patrolPoints.Length == 0) return;

        agent.SetDestination(patrolPoints[patrolIndex].position);
        patrolIndex = (patrolIndex + 1) % patrolPoints.Length; // ���� ��� ��ȯ
    }

    // ���� ���� �� ������ ����
    void StopSearching()
    {
        if (!isChasing) // �̹� �ٽ� ���� ���� �ƴϸ� ������ ����
        {
            isSearching = false;
            GoToNextPatrolPoint();
            Debug.Log("���� ����. ���� �簳...");
        }
    }

    // �þ߰� �ð�ȭ (����׿�)
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, viewRadius);

        Vector3 leftBoundary = Quaternion.Euler(0, -viewAngle / 2, 0) * transform.forward;
        Vector3 rightBoundary = Quaternion.Euler(0, viewAngle / 2, 0) * transform.forward;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + leftBoundary * viewRadius);
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary * viewRadius);
    }
}