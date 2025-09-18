using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class Player_Ctrl : MonoBehaviour
{
    public string Cha;
    public string Mode;

    [SerializeField] Transform PlayerCamPos;
    public Transform WorldCamPos;
    Transform Cam;

    public int Std_Life, Max_Life, CamSpd;

    public Transform Y_Axis;
    public float Spd, RunSpd, Rot_Spd, jumpForce, rotationSpeed, groundCheckDistance;

    //public float rotationSpeed = 10f; // 회전 속도
    //public float groundCheckDistance = 1.2f; // 바닥 체크 거리

    public Transform AngleTr;
    public Vector3 TargetRot;

    [Header("프리펩")]
    public GameObject Scene_Loading;
    public GameObject GameMgr;

    [Header("지정")]
    public Buff_Func Buff;
    public MenuCtrl Menu;
    public HitUI hitUI;
    public bool DiePos;

    public Animator Stat_Anim;

    [SerializeField] TMP_Text txtLife;
    [SerializeField] List<Color> Txt_Colors;

    public List<Collider> PlayerColls;

    [Header("지정X")]
    public GameMgr Mgr;

    public bool AngleRot, MovOn, RotOn, CamMov, 
        isJump, isGound, Stop, Angle_Move, HitIs, Die;
    public Rigidbody rb;
    public float Y_Rot_Spd, Mouse_Y;
    public StageFunc stage;

    private void Awake()
    {

        if (SceneManager.GetActiveScene().name.Equals("Lobby"))
        {
            if (GameObject.FindGameObjectWithTag("GameMgr") == null)
            { Mgr = Instantiate(GameMgr, Vector3.zero, Quaternion.Euler(Vector3.zero)).GetComponent<GameMgr>(); }
            else
            { Mgr = GameObject.FindGameObjectWithTag("GameMgr").GetComponent<GameMgr>(); }

            if (Mgr.Cha != Cha)
            {
                Mgr.Max_Life = Max_Life;
                Mgr.Cha = Cha;
            }
            Mgr.Life = Std_Life;
            Mgr.Timer = 0;
        }
        else
        {
            if (Mgr == null)
            {
                Mgr = GameObject.FindGameObjectWithTag("GameMgr").GetComponent<GameMgr>();
            }
        }

    }

    private void Start()
    {
        Buff = Menu.GetComponent<Buff_Func>();
        Menu.GetComponent<Buff_Func>().Player = this;
        hitUI = Menu.hitUI;

        txtLife = Menu.txt_Life;

        Menu.txt_Life.text = Mgr.Life.ToString();
        Menu.Player = this;

        Menu.hitUI.Player = this;

        rb = GetComponent<Rigidbody>();
        Cam = Camera.main.transform;
        WorldCamPos = GameObject.Find("WorldCamPos").GetComponent<Transform>();

        AngleTr = WorldCamPos.parent;
        TargetRot = AngleTr.eulerAngles;
    }

    void Update()
    {
        Ctrl_Cursor();

        if (SceneManager.GetActiveScene().name.Contains("Stage"))
        { Mgr.Timer += Time.deltaTime; }


        if (Mode.Contains("인칭") && Input.GetKeyDown(KeyCode.V))
        {
            rb.linearVelocity = Vector3.zero;

            if (Mode.Contains("1인칭"))
            { StartCoroutine(GotoCamPos(WorldCamPos, "3인칭시점")); }

            else if (Mode.Contains("3인칭"))
            { StartCoroutine(GotoCamPos(PlayerCamPos, "1인칭시점")); }

            Mode = "이동중";
        }

        if (Mode.Equals("3인칭시점"))
        {
            Mode_Third_PP();
            Jump();
        }
        else if (Mode.Equals("1인칭시점"))
        {
            Mode_First_PP();
            Jump();
        }

        if (Menu.Menu.activeSelf == true)
        {

            if (Menu.Num.Equals("0.1"))
            {
                Menu.txt_Timer();
            }
        }

        //Revive();
        AngleMove();
        GetGroundRotation();

    }

    void Ctrl_Cursor()
    {
        if (Menu.Menu.activeSelf == false && Die == false)
        {
            if (Mode.Equals("1인칭시점"))
            {
                if (Cursor.visible == true)
                { Cursor.visible = false; }

                if (Cursor.lockState != CursorLockMode.Locked)
                { Cursor.lockState = CursorLockMode.Locked; }
            }
            else if (Mode.Equals("3인칭시점"))
            {
                if (Cursor.visible == true)
                { Cursor.visible = true; }

                if (Cursor.lockState == CursorLockMode.Locked)
                { Cursor.lockState = CursorLockMode.None; }
            }
        }
        else
        {
            if (Cursor.visible == true)
            { Cursor.visible = true; }

            if (Cursor.lockState == CursorLockMode.Locked)
            { Cursor.lockState = CursorLockMode.None; }
        }
    }

    public void Revive(string str)
    {
        if (HitIs == true && Die == false)
        {
            if (Mgr.Life > -1)
            {
                //if (Input.GetKeyDown(KeyCode.X) == true)
                {
                    if (str == "StartPos")
                    {
                        func_Revive();

                        if(stage == null)
                        { stage = GameObject.FindGameObjectWithTag("Mgr").GetComponent<StageFunc>(); }

                        rb.linearVelocity = Vector3.zero; // 움직임 멈추기
                        rb.position = stage.StartPos.position;
                    }
                    else if(str == "FinPos")
                    {
                        func_Revive();
                    }
                }
            }
        }
    }
    void  func_Revive()
    {
        ctrl_Coll(false);
        rb.linearVelocity = Vector3.zero; // 움직임 멈추기
        hitUI.Hitui.SetActive(false);
        hitUI.StartHitCount(false);
        Stat_Anim.SetTrigger("Revive");
        HitIs = false;
    }

    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isJump == false)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z); // Y축 속도 초기화
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);      // 위로 힘 추가
            isJump = true;
        }
    }
    void Mode_Third_PP()
    {
        if (Stop == false)
        {
            if (HitIs == false && Die == false)
            {
                if (RotOn == true) { RotOn = false; }

                // 카메라 기준 방향 설정
                Vector3 camForward = Cam.forward;
                Vector3 camRight = Cam.right;

                // 수직 성분 제거 (Y축 기준으로 투영)
                camForward = Vector3.ProjectOnPlane(camForward, Vector3.up).normalized;
                camRight = Vector3.ProjectOnPlane(camRight, Vector3.up).normalized;

                // 입력 처리
                float horizontal = Input.GetAxis("Horizontal");
                float vertical = Input.GetAxis("Vertical");

                // 입력 방향을 카메라 기준으로 변환
                Vector3 moveDir = (camRight * horizontal) + (camForward * vertical);
                //Jump();

                // 캐릭터 이동
                if (moveDir.magnitude > 0.01f) // 이동 입력이 있을 때만 처리
                {

                    if (Input.GetKey(KeyCode.LeftShift))
                    {
                        //Debug.Log("시프트누름");
                        rb.linearVelocity = new Vector3(moveDir.x * Spd * RunSpd, rb.linearVelocity.y, moveDir.z * Spd * RunSpd);
                    }
                    else
                    {
                        // Rigidbody로 이동 (y 축 속도 유지)
                        rb.linearVelocity = new Vector3(moveDir.x * Spd, rb.linearVelocity.y, moveDir.z * Spd);
                    }

                    // 이동 방향으로 부드럽게 회전
                    Quaternion targetRotation = Quaternion.LookRotation(moveDir);
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Rot_Spd * 10 * Time.deltaTime);
                }
                else
                {
                    // 입력이 없을 경우, 수평 이동을 멈춤
                    if (rb.linearVelocity != Vector3.zero)
                    { rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0); }
                }
            }
        }
    }
    void Mode_First_PP()
    {
        if (Stop == false)
        {
            if (HitIs == false && Die == false)
            {

                // Ctrl 부분
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

    void AngleMove()
    {

        // 현재 회전과 목표 회전의 각도 차이 계산
        float Dis = Quaternion.Angle(AngleTr.rotation, Quaternion.Euler(TargetRot));

        // 부드럽게 목표 회전으로 이동
        if (Dis > 0.1f)
        {
            rb.linearVelocity = Vector3.zero;

            Quaternion Target = Quaternion.Euler(TargetRot);
            AngleTr.rotation = Quaternion.Lerp(AngleTr.rotation, Target, 30 * Time.deltaTime);
        }
        else
        {
            // 회전이 거의 완료되면 정확히 목표 각도로 보정
            if (Dis <= 0.1f)
            {
                AngleTr.rotation = Quaternion.Euler(TargetRot);
                AngleRot = false;
            }
        }

        // 회전 입력 처리 (회전 중이 아닐 때만)
        if (!AngleRot)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                TargetRot = AngleTr.eulerAngles + new Vector3(0, 90, 0);
                NormalizeAngle(ref TargetRot.y);
                AngleRot = true;
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                TargetRot = AngleTr.eulerAngles + new Vector3(0, -90, 0);
                NormalizeAngle(ref TargetRot.y);
                AngleRot = true;
            }
        }
    }

    // 각도를 0~360도로 정규화하는 함수
    void NormalizeAngle(ref float angle)
    {
        if (angle < 0) angle += 360;
        else if (angle >= 360) angle -= 360;
    }

    IEnumerator GotoCamPos(Transform Pos, string str)
    {
        if (Cam.parent != null) { Cam.SetParent(null); }

        float Pos_Dis = Vector3.Distance(Cam.position, Pos.position);
        float Pos_Rot = Vector3.Distance(Cam.eulerAngles, Pos.eulerAngles);

        if (Pos_Dis >= 0.1) { Cam.position = Vector3.Lerp(Cam.position, Pos.position, CamSpd*Time.deltaTime); }
        if (Pos_Rot >= 0.1) { Cam.rotation = Quaternion.Lerp(Cam.rotation, Pos.rotation, CamSpd * Time.deltaTime); }

        if(str.Contains("3인칭"))
        { Y_Axis.rotation = Quaternion.Lerp(Y_Axis.rotation, transform.rotation, (CamSpd * 3) * Time.deltaTime); }

        if (Pos_Dis < 0.1 && Pos_Rot < 0.1)
        { 
            Mode = str;
            if (Mode.Contains("1인칭")) { Cam.SetParent(PlayerCamPos); }
            else if (Mode.Contains("3인칭")) { Cam.SetParent(WorldCamPos); }

            Cam.localPosition = new Vector3(0, 0, 0);
            Cam.localEulerAngles = new Vector3(0, 0, 0);
        }


        yield return null;
        if (Mode.Equals("이동중")) { StartCoroutine(GotoCamPos(Pos, str)); }
    
    }

    void GetGroundRotation()
    {
        // Raycast를 통해 바닥의 법선 벡터를 구함
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 2f)) // 바닥을 찾는 Raycast
        {
            Vector3 groundNormal = hit.normal; // 바닥의 법선 벡터

            // 바닥의 회전 값 구하기
            Quaternion groundRotation = Quaternion.FromToRotation(Vector3.up, groundNormal);

            // 회전 값 출력 (디버깅 용도)
            //Debug.Log("Ground Rotation: " + groundRotation.eulerAngles);
        }
    }

    public void Hit()
    {
        if (HitIs == false && Die == false)
        {
            rb.linearVelocity = Vector3.zero;

            ctrl_Coll(false);

            if (Mgr == null)
            { Mgr = GameObject.FindGameObjectWithTag("GameMgr").GetComponent<GameMgr>(); }

            Mgr.Life--;

            if(Mgr.Life > -1)
            {
                HitIs = true;

                if (Menu.Menu.activeSelf == true)
                { Menu.Menu.SetActive(false); }

                hitUI.Evt_Hit();
                Stat_Anim.SetTrigger("Hit");
            }
            else
            {
                Mgr.Curr_Stage = Mgr.SaveStage;
                hitUI.Evt_Death();
                Stat_Anim.SetTrigger("Die");
            }

            TxtLife();
        }
    }

    public void TxtLife()
    {
        if(Mgr.Life <= -1)
        {
            txtLife.text = "None";
            txtLife.color = Txt_Colors[0];
        }
        else if (Mgr.Life == 0)
        {
            txtLife.text = Mgr.Life.ToString();
            txtLife.color = Txt_Colors[0];
        }
        else if (Mgr.Life >= Mgr.Max_Life)
        {
            txtLife.text = Mgr.Life.ToString();
            txtLife.color = Txt_Colors[2];
        }
        else
        {
            txtLife.text = Mgr.Life.ToString();
            txtLife.color = Txt_Colors[1];
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            isGound = true;
            isJump = false;
            //Debug.Log("벽이나 맵과 충돌했습니다!");
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        isGound = false;
    }

    public void SceneMove(string Scene)
    {
        StartCoroutine(LoadSceneAsync(Scene));
    }

    public void ctrl_Coll(bool b)
    { PlayerColls.ForEach(col => col.enabled = b); }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        Image ProgBar = Instantiate(Scene_Loading, Vector3.zero, Quaternion.Euler(Vector3.zero))
            .GetComponent<SceneLoading>().Loading_Bar;

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

        // 씬 로딩이 끝날 때까지 대기
        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            ProgBar.fillAmount = progress; // 진행률 업데이트
            yield return null;
        }
    }

}
