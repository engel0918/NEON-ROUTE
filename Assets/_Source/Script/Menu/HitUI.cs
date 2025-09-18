using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HitUI : MonoBehaviour
{
    [Header("Hit Count")]
    public float Count;
    public float CountMax;

    [Header("Hit UI")]
    public GameObject Hitui;
    public Image Cnt_Gage;

    [Header("Death UI")]
    public GameObject DieUI;

    public Image Tit_Img;
    public TMP_Text Tit_txt;
    public TMP_Text Reason_txt;

    public Sprite Death_spr;

    [Header("0.���� 1.���� ����.. ��ư")]
    public List<GameObject> BtnList;

    [Header("����X")]
    public Player_Ctrl Player;

    public Sprite Spr;
    public string tit;
    public List<string> Reason;
    public bool Fallen;

    private Coroutine hitCoroutine;

    private void Start()
    {
        DieUI.SetActive(false);
        Hitui.SetActive(false);
    }

    public void Evt_Death()
    {
        Player.rb.linearVelocity = Vector3.zero;

        Player.Mgr.Life = -1;

        Tit_Img.sprite = Spr;
        Tit_txt.text = tit.ToString();

        for(int i = 0; i<= (Reason.Count-1); i++)
        {
            if(i == 0) 
            {
                Reason_txt.text = Reason[i].ToString();
            }
            else
            {
                Reason_txt.text = Reason_txt.text + "\n" + Reason[i].ToString();
            }
        }

        Player.Die = true;

        Player.TxtLife();

        if (Cursor.visible == false) { Cursor.visible = true; }
        if (Cursor.lockState == CursorLockMode.Locked) { Cursor.lockState = CursorLockMode.None; }

        DieUI.SetActive(true);
        Hitui.SetActive(false);
    }

    public void Evt_Hit()
    {
        Count = 0;
        StartHitCount(true);
        Hitui.SetActive(true);
    }

    public void Evt_MovScene(string str)
    {
        if (str.Equals("Lobby"))
        { Player.SceneMove("Lobby"); }
        else if(str.Equals("Save"))
        {
            GameMgr mgr = Player.Mgr;
            Player.SceneMove(mgr.Stage[mgr.SaveStage]);
        }
    }

    public void StartHitCount(bool b)
    {
        if (b == true)
        {
            if (hitCoroutine != null) StopCoroutine(hitCoroutine); // ���� ���� ���� �ڷ�ƾ ����
            hitCoroutine = StartCoroutine(Hit_Count()); // �� �ڷ�ƾ ����
        }
        else
        {
            if (hitCoroutine != null) // ���� ���� �ڷ�ƾ�� ������ ����
            {
                StopCoroutine(hitCoroutine);
                hitCoroutine = null;
            }
        }
    }

    public void Revive(string str)
    {
        Player.Revive(str);
    }


    public IEnumerator Hit_Count()
    {
        while (Count < CountMax) // Count�� CountMax���� ���� ���� �ݺ� ����
        {
            Count += Time.deltaTime; // ������ ������ Count ����
            Cnt_Gage.fillAmount = Count / CountMax;

            //Player.rb.velocity = Vector3.zero;
            yield return null; // ���� �����ӱ��� ���
        }
        // Count�� CountMax�� �����ϸ� ������ �ڵ�
        Evt_Death();
    }
}
