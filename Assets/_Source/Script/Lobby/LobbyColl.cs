using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyColl : MonoBehaviour
{
    public enum Type{ Play , Character, Setting, Exit }
    public Type Case;
    public bool Trig;

    private Camera mainCamera;
    Lobby lobby;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        lobby = GameObject.FindGameObjectWithTag("Mgr").GetComponent<Lobby>();
    }

    private void OnCollisionEnter(Collision coll)
    {
        if (coll.gameObject.CompareTag("Player") && lobby.Press_F != null) // 특정 오브젝트와 상호작용
        {
            Trig = true;
            ShowUI(coll.collider);
        }
    }

    private void OnCollisionExit(Collision coll)
    {
        if (coll.gameObject.CompareTag("Player") && lobby.Press_F != null)
        {
            Trig = false;
            lobby.Press_F.SetActive(false);
        }
    }

    private void Update()
    {
        if (Trig == true)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                if(Case == Type.Play)
                {
                    lobby.PlayFunc();
                    lobby.Press_F.SetActive(false);
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
        RectTransform uiRect = lobby.Press_F.GetComponent<RectTransform>();
        uiRect.position = screenPos;

        // UI 활성화
        lobby.Press_F.SetActive(true);
    }
}
