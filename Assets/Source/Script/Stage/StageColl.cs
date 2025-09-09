using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageColl : MonoBehaviour
{
    public enum Type { Goal, }
    public Type Case;
    public bool Trig;

    private Camera mainCamera;
    StageFunc Stage;
    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        Stage = GameObject.FindGameObjectWithTag("Mgr").GetComponent<StageFunc>();
    }

    private void OnCollisionEnter(Collision coll)
    {
        if (coll.gameObject.CompareTag("Player") && Stage.Press_F != null) // 특정 오브젝트와 상호작용
        {
            Trig = true;
            ShowUI(coll.collider);
        }
    }

    private void OnCollisionExit(Collision coll)
    {
        if (coll.gameObject.CompareTag("Player") && Stage.Press_F != null)
        {
            Trig = false;
            Stage.Press_F.SetActive(false);
        }
    }

    private void Update()
    {
        if (Trig == true)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                if (Case == Type.Goal)
                {
                    Stage.Goal_Func();
                    Stage.Press_F.SetActive(false);
                    Trig = false;
                }
            }
        }
    }

    void ShowUI(Collider col)
    {

        // 월드 좌표 → 스크린 좌표 변환
        Vector3 screenPos = mainCamera.WorldToScreenPoint(col.transform.position);

        // UI 위치 설정
        RectTransform uiRect = Stage.Press_F.GetComponent<RectTransform>();
        uiRect.position = screenPos;

        // UI 활성화
        Stage.Press_F.SetActive(true);
    }
}
