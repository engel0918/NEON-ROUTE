using Opsive.UltimateCharacterController.Inventory;
using Steamworks;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuCtrl : MonoBehaviour
{
    public string Num;

    [Header("지정")]
    public GameObject Menu;
    public GameObject Press_F;

    [Header("script")]
    public HitUI hitUI;
    public MenuObj menuObj;

    [Header("txt_플레이 타임")]
    public TMP_Text txt_PLYTM;
    public TMP_Text txt_Life;

    [Header("Exits")]
    public List<TMP_Text> txt_ExitList;
    public Animator ErrorTxt;
    public List<Exit> ExitStr;

    [Header("지정X")]
    public Player_Ctrl Player;
    public string Exits;

    [SerializeField] List<string> Opened_UI = new List<string>();
    [SerializeField] List<string> Close_UI = new List<string>();

    private void Awake()
    {
        Opened_UI = new List<string>();
        Opened_UI.Clear();

        menuObj.Inven_UI.SetActive(false);
        Menu.SetActive(false);

    }

    private void OnEnable()
    {
        Exits = "";
        menuObj.Sel_MainUI("None");
    }

    private void Update()
    {
        Ctrl_Menu();
    }

    void Ctrl_Menu()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // 켜진 UI와 종료UI가 없을 때, 
            if (Opened_UI.Count <= 0 && Close_UI.Count <= 0)
            {
                //설정UI 켜기

                Sel_Menu("0.0");
                Menu_ON();

                if (Cursor.visible == false) { Cursor.visible = true; }
                if (Cursor.lockState == CursorLockMode.Locked) { Cursor.lockState = CursorLockMode.None; }

                Opened_UI.Add("Setting");
            }
            // 켜진 UI가 있을 때, 
            else if (Opened_UI.Count > 0 || Close_UI.Count > 0)
            {
                string lastUI = Opened_UI[Opened_UI.Count - 1];

                // 설정메뉴가 켜져있는 경우
                if (Menu.activeSelf == true)
                {
                    int index = Opened_UI.IndexOf("Setting");
                    Opened_UI.RemoveAt(index);

                    Menu.SetActive(false);
                }
                // 아닌 경우
                else
                {
                    // Last UI 지우기

                    // 설정 UI 지우기
                    if (lastUI == "Setting")
                    {
                        // 설정 UI 지우기
                        Menu.SetActive(false);

                        if (Player != null)
                        {
                            if (Player.Mode.Equals("1인칭시점"))
                            {
                                if (Cursor.visible == true) { Cursor.visible = false; }
                                if (Cursor.lockState != CursorLockMode.Locked) { Cursor.lockState = CursorLockMode.Locked; }
                            }
                        }
                    }
                    // 인벤토리 UI 지우기
                    else if (lastUI == "Inventory")
                    { 
                        menuObj.Inven_UI.SetActive(false);
                        menuObj.hover.Contents.SetActive(false);
                    }

                    // 리스트 제거
                    Opened_UI.RemoveAt(Opened_UI.Count - 1);
                }
            }
        }

        if (Input.inputString == "i" || Input.inputString == "I")
        {
            if (menuObj.Inven_UI.activeSelf == false)
            {
                Opened_UI.Add("Inventory");
                menuObj.Inven_UI.SetActive(true);
            }
            else 
            {
                int index = Opened_UI.IndexOf("Inventory");
                Opened_UI.RemoveAt(index);

                menuObj.hover.Contents.SetActive(false);
                menuObj.Inven_UI.SetActive(false);
            }
        }


    }

    public void Menu_ON()
    {
        Menu.SetActive(true);
        menuObj.Sel_MainUI("Main");
    }

    public void Sel_Menu(string str)
    {
        Num = str;

        if (str != "0.0") { menuObj.Sel_MainUI("Set"); }
        else
        {
            menuObj.Sel_MainUI("None");
        }

        menuObj.Sel_Menu(str);
    }
    
    // 설정 저장
    public void Btn_SaveSetting()
    {
        menuObj.Func_PageBool(false, 0);
        menuObj.Func_Apply();
    }

    public void Sel_Exit(string str)
    {
        if(str.Equals("Home"))
        {
            txt_ExitList[0].text = ExitStr[0].Tit.ToString();
            txt_ExitList[1].text = ExitStr[0].Info.ToString();
        }
        else if(str.Equals("Exit"))
        {
            txt_ExitList[0].text = ExitStr[1].Tit.ToString();
            txt_ExitList[1].text = ExitStr[1].Info.ToString();
        }

        Exits = str;
        menuObj.Sel_MainUI("Exit");
        ErrorTxt.SetTrigger("Act");
    }

    public void Exit_Confirm(string str)
    {
        if (str == "Yes")
        {
            if (Exits.Equals("Home"))
            {
                if (SceneManager.GetActiveScene().name.Equals("Lobby") == false)
                { Player.SceneMove("Lobby"); Debug.Log("Go_Lobby"); }
                else
                { ErrorTxt.SetTrigger("Act"); Debug.Log("Go_Error"); }
            }
            else if (Exits.Equals("Exit"))
            {
                // SteamAPI 종료
                if (SteamManager.Initialized)
                    SteamAPI.Shutdown();

                // Unity 게임 종료
                Application.Quit();
            }
        }
        else
        { menuObj.ExitUI.SetActive(false); }
    }

    public void txt_Timer()
    {
        txt_PLYTM.text = FormatTime(Player.Mgr.Timer);
    }
    public string FormatTime(float time)
    {
        int hours = (int)(time / 3600); // 시간
        int minutes = (int)((time % 3600) / 60); // 분
        int seconds = (int)(time % 60); // 초
        int milliseconds = (int)((time - Mathf.Floor(time)) * 10); // 1/10초

        return string.Format("{0:00}:{1:00}:{2:00}:{3}", hours, minutes, seconds, milliseconds);
    }

}
[System.Serializable]
public struct Exit
{
    public string Tit;
    public string Info;
}