using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class AIController : MonoBehaviour
{
    [Header("감지 범위")]
    public float viewRadius = 10f;      // 감지 범위

    [Header("감지 각도")]
    public float viewAngle = 90f;       // 감지 각도

    [Header("장애물 감지")]
    public LayerMask obstacleMask;      // 장애물 레이어

    [Header("순찰 경로")]
    public Transform[] patrolPoints;    // 순찰 경로

    [Header("감정표현 애니메")]
    public Animator Emotji_Anim;        // 이모지 애니메이터

    [Header("딜레이")]
    public float Delay = 3f;            // 순찰로 전환 전 대기 시간

    private NavMeshAgent agent;
    private Player_Ctrl playerCtrl;
    private Transform player;
    private int patrolIndex = 0;
    private bool isChasing = false;     // 추적 중 여부
    private float lastSeenTime = 0f;    // 마지막으로 플레이어를 본 시간
    private float chaseDuration = 5f;   // 플레이어 놓친 후 순찰로 전환 시간
    private Vector3 lastKnownPosition;  // 마지막으로 플레이어를 본 위치
    private bool isSearching = false;   // 예상 위치 탐색 여부

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        playerCtrl = GameObject.FindGameObjectWithTag("Player").GetComponent<Player_Ctrl>(); // 플레이어 찾기
        GoToNextPatrolPoint(); // 첫 순찰 지점으로 이동
    }

    void Update()
    {
        if (CanSeePlayer()) // 플레이어를 발견했는지 확인
        {
            if (!isChasing)
            {
                // 플레이어 발견 시 느낌표 애니메이션
                Emotji_Anim.SetTrigger("EX_mark");
                //Debug.Log("플레이어 발견! 추적 시작!");
            }

            isChasing = true;
            lastKnownPosition = player.transform.position; // 마지막 위치 저장
            lastSeenTime = Time.time; // 감지 시간 갱신
        }

        if (isChasing)
        {
            if (player != null)
            { agent.SetDestination(player.transform.position); } // 플레이어 추적

            // 일정 시간 동안 플레이어를 못 보면 수색 시작
            if (Time.time - lastSeenTime > chaseDuration)
            {
                isChasing = false;
                isSearching = true; // 예상 위치 탐색 시작
                Emotji_Anim.SetTrigger("Q_mark"); // 물음표 애니메이션
                //Debug.Log("플레이어 놓침. 예상 위치로 이동...");

                agent.SetDestination(lastKnownPosition);
                Invoke(nameof(StopSearching), Delay); // 딜레이 후 순찰 재개
            }
        }
        else if (isSearching) // 예상 위치로 이동 중
        {
            // 예상 위치에 도착했으면 수색 완료
            if (!agent.pathPending && agent.remainingDistance < 0.5f)
            {
                StopSearching();
            }
        }
        else if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            GoToNextPatrolPoint(); // 다음 순찰 지점으로 이동
        }
    }

    private void FixedUpdate()
    {
        if(playerCtrl.HitIs == true || playerCtrl.Die)
        { player = null; }
        else { player = playerCtrl.transform; }

    }

    // 플레이어 감지 로직
    bool CanSeePlayer()
    {
        if (player == null) return false;

        Vector3 dirToPlayer = (player.transform.position - transform.position).normalized;

        // 시야각 내에 있는지 확인
        if (Vector3.Angle(transform.forward, dirToPlayer) < viewAngle / 2)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

            // 장애물이 없는 경우 감지 성공
            if (!Physics.Raycast(transform.position, dirToPlayer, distanceToPlayer, obstacleMask))
            {
                return true; // 플레이어 감지
            }
        }
        return false; // 감지 실패
    }

    // 순찰 지점으로 이동
    void GoToNextPatrolPoint()
    {
        if (patrolPoints.Length == 0) return;

        agent.SetDestination(patrolPoints[patrolIndex].position);
        patrolIndex = (patrolIndex + 1) % patrolPoints.Length; // 순찰 경로 순환
    }

    // 수색 종료 후 순찰로 복귀
    void StopSearching()
    {
        if (!isChasing) // 이미 다시 추적 중이 아니면 순찰로 복귀
        {
            isSearching = false;
            GoToNextPatrolPoint();
            Debug.Log("수색 종료. 순찰 재개...");
        }
    }

    // 시야각 시각화 (디버그용)
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