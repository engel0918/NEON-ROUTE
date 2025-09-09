using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class Buff_Func : MonoBehaviour
{
    public GameObject Buff_Prefab;
    public Transform Buff_Pos;

    public int Run_Lim;
    public int Run_;

    [Header("버프되는 부분")]
    public float RunSpd;

    private Coroutine buffRunCoroutine; // 현재 실행 중인 코루틴 저장

    [Header("지정X")]
    public Player_Ctrl Player;

    public void BuffOn(string str, int i)
    {
        if (str == "Run")
        { SetRun(i); }
    }

    void SetRun(int Value)
    {
        Run_Lim = Value;
        Run_ = 0;

        bool isCheck = false;

        for (int i = 0; i < Buff_Pos.childCount; i++) // 0부터 시작
        {
            Buff_Img buffImg = Buff_Pos.GetChild(i).GetComponent<Buff_Img>(); // 캐싱
            if (buffImg.Buff.Equals("Run"))
            {
                buffImg.BuffOn("Run", Value);
                isCheck = true;
            }
        }

        if (!isCheck)
        {
            Buff_Img Buff = Instantiate(Buff_Prefab, Buff_Pos).GetComponent<Buff_Img>();
            Buff.BuffOn("Run", Value);
        }

        // 🛑 기존에 실행 중인 코루틴이 있다면 중지
        if (buffRunCoroutine != null)
        {
            StopCoroutine(buffRunCoroutine);
        }

        // ✅ 새 코루틴 실행 후 저장
        buffRunCoroutine = StartCoroutine(Buff_Run());
    }

    IEnumerator Buff_Run()
    {
        while (Run_ < Run_Lim) // Run_Lim이 될 때까지 반복
        {
            if (Player.RunSpd != RunSpd)
            {
                Player.RunSpd = RunSpd;
            }

            Run_++;
            yield return new WaitForSeconds(1f);
        }

        // 버프 종료 시 기본 속도로 복귀
        Player.RunSpd = 1;
    }
}
