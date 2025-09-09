using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Intro : MonoBehaviour
{
    MenuObj menuObj;
    SteamSave SteamMgr;
    PageCtrl pagectrl;

    private void Start()
    {
        pagectrl = GetComponent<PageCtrl>();
        menuObj = GetComponent<MenuObj>();

        gameObject.AddComponent<SteamManager>();
        SteamMgr = gameObject.AddComponent<SteamSave>();
        transform.tag = "Steam";

        menuObj.Sel_MainUI("Main");
        SetData_Check();
    }

    void SetData_Check()
    {
       Debug.Log(SteamMgr.SetDat_Check());

        if (SteamMgr.SetDat_Check() == "No Data")
        { No_Data(); }
        else if (SteamMgr.SetDat_Check() == "Data Available")
        { Data_Available(); }
    }

    void No_Data()
    {
        menuObj.Set_IntroData();
        pagectrl.func_PageCtrl(0);
    }

    void Data_Available()
    {
        SceneMove("Lobby");
    }

    public void Btn_SetFin()
    {
        menuObj.Func_Apply();
        StartCoroutine(Wait());
    }

    private IEnumerator Wait()
    {
        yield return new WaitForSeconds(1f);
        SceneMove("Lobby");
    }

    public void SceneMove(string Scene)
    {
        StartCoroutine(LoadSceneAsync(Scene));
    }
    private IEnumerator LoadSceneAsync(string sceneName)
    {
        Image ProgBar = Instantiate(menuObj.Scene_Loading, Vector3.zero, Quaternion.Euler(Vector3.zero))
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
