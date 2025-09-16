using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class MenuObj : MonoBehaviour
{
    private const string SETTINGS_FILE = "settings.json";
    private const string Cha_Path = "Character.json";
    SteamSave Steam;

    public GameObject MainUI, SetUI, ExitUI, Inven_UI;

    // 0. ����UI�� ����
    [Header("Menu���� ����")]
    public PageCtrl page;
    public HoverUI hover;

    [Header("�ʼ� ������")]
    public Volume postProcessVolume; // ������ ���� �Ǵ� null ����

    [Header("������")]
    public GameObject Scene_Loading;
    public AudioMixer audioMixer;

    [Header("����UI�� �� ����")]
    public List<Menu> MenuList;

    [Header("����� Count 0 / ���� 0.������, 1.�����ȵ� ����")]
    public List<Sprite> Under_Spr;

    [Header("�����̴�_1��Ī(FPS) ���� ����")]
    public Slider Sld_SS;
    public TMP_InputField Ipf_SS;

    [Header("��Ӵٿ�_�׷��� ǰ��,��Ƽ�����¡, ȭ����, �ػ�")]
    public TMP_Dropdown Dd_GQ;
    public TMP_Dropdown Dd_Aa;
    public TMP_Dropdown Dd_Gm;
    public TMP_Dropdown Dd_Res;

    [Header("��ü ����, ����� ����, ȿ���� ����")]
    public Slider Sld_Mv;
    public TMP_Text txt_Mv;

    public Slider Sld_BGMv;
    public TMP_Text txt_BGMv;

    public Slider Sld_SFXv;
    public TMP_Text txt_SFXv;

    [Header("��Ӵٿ�_����")]
    public TMP_Dropdown Dd_Lang;

    public enum AntiAliasingMode
    { None = 0, FXAA = 1, MSAA2x = 2, MSAA4x = 3, MSAA8x = 4, TAA = 5, }
    AntiAliasingMode postProcessAA = AntiAliasingMode.None;

    private UniversalRenderPipelineAsset urpAsset;

    [System.Serializable]
    public struct Menu
    {
        public string Num;
        public List<GameObject> Left;
        public GameObject Manu_Main;
        public GameObject Under_UI;
    }

    private void Awake()
    {
        urpAsset = GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;

        GameObject volumeObj = GameObject.FindGameObjectWithTag("Volume");
        if (volumeObj != null)
        { postProcessVolume = volumeObj.GetComponent<Volume>(); }
        else { Debug.LogError("Post-processing Volume ������Ʈ�� �����ϴ�."); }

        UI_SetFunc();
    }

    void Start()
    {
        Debug.Log("SteamManager.Initialized: " + SteamManager.Initialized);
        if (!SteamManager.Initialized)
            Debug.LogError("Steam API �ʱ�ȭ ����!");

        if (SceneManager.GetActiveScene().name.Contains("Intro") == false)
        { Start_Set(); }
    }

    void Start_Set()
    {
        //----------------------- ���� �ε� -----------------------
        if (!SteamRemoteStorage.FileExists(SETTINGS_FILE))
        {
            Debug.Log("No settings file found in Steam cloud.");
            return;
        }

        int size = SteamRemoteStorage.GetFileSize(SETTINGS_FILE);
        byte[] buffer = new byte[size];
        SteamRemoteStorage.FileRead(SETTINGS_FILE, buffer, size);

        string json = Encoding.UTF8.GetString(buffer);
        SettingsData data = JsonUtility.FromJson<SettingsData>(json);
        //------------------------------------------------------
        // ���� ���� üũ
        int AAlv = data.AA;
        Aa_Set(AAlv);
    }

    // Intro���� �Ѵ� �����޴�
    public void Set_IntroData()
    {
        // ���� ���� üũ
        int GQlv = QualitySettings.GetQualityLevel();

        AntiAliasingMode currentAA = GetCurrentAA();
        int AAlv = (int)currentAA;

        int GMlv, RESlv;     float Mv, BGMv, SFXv;

        // â���
        if (Screen.fullScreenMode == FullScreenMode.Windowed) 
        { GMlv = 0; Dd_Gm.value = 0; }
        // �׵θ� ���� â ���
        else if (Screen.fullScreenMode == FullScreenMode.FullScreenWindow) 
        { GMlv = 1; Dd_Gm.value = 1; }
        //â ��� ��üȭ��
        else if (Screen.fullScreenMode == FullScreenMode.MaximizedWindow) 
        { GMlv = 2; Dd_Gm.value = 2; }
        // ��üȭ�� 
        else if (Screen.fullScreenMode == FullScreenMode.ExclusiveFullScreen) 
        { GMlv = 3; Dd_Gm.value = 3; }

        if (Screen.width == 3840) { RESlv = 0; }
        else if (Screen.width == 2560) { RESlv = 1; }
        else if (Screen.width == 1920) { RESlv = 2; }
        else if (Screen.width == 1600) { RESlv = 3; }
        else if (Screen.width == 1280) { RESlv = 4; }
        else { RESlv = -1; } // �⺻���� ���� (��: �� �� ����)

        Mv = InitSlider("MasterVolume");
        BGMv = InitSlider("BGMVolume");
        SFXv = InitSlider("SFXVolume");

        //------------------------------------------------------
        // �� UI�� ������� ����

        Sel_Menu("1.0");
        Sel_MainUI("Set");

        Sld_SS.value = 3;
        Ipf_SS.text = Sld_SS.value.ToString("F2");

        Dd_GQ.value = GQlv;
        Dd_Aa.value = AAlv;
        //Dd_Gm.value = GMlv;
        Dd_Res.value = RESlv;

        Sld_Mv.value = Mv;
        txt_Mv.text = Mv.ToString("F0") + "%";

        Sld_BGMv.value = BGMv;
        txt_BGMv.text = BGMv.ToString("F0") + "%";

        Sld_SFXv.value = SFXv;
        txt_SFXv.text = SFXv.ToString("F0") + "%";

        //---------------------------------------------------
    }

    // �޴����� �Ѵ� �����޴�
    public void Set_SetData()
    {
        //----------------------- ���� �ε� -----------------------
        if (!SteamRemoteStorage.FileExists(SETTINGS_FILE))
        {
            Debug.Log("No settings file found in Steam cloud.");
            return;
        }

        int size = SteamRemoteStorage.GetFileSize(SETTINGS_FILE);
        byte[] buffer = new byte[size];
        SteamRemoteStorage.FileRead(SETTINGS_FILE, buffer, size);

        string json = Encoding.UTF8.GetString(buffer);
        SettingsData data = JsonUtility.FromJson<SettingsData>(json);
        //------------------------------------------------------

        // ���� ���� üũ
        float SSlv = data.SS;
        int GQlv = data.GQ;
        int AAlv = data.AA;
        int GMlv = data.GM;
        int RESlv = data.RES;
        float Mv = data.Mv, BGMv = data.BGMv, SFXv = data.SFXv;
        //------------------------------------------------------
        // �� UI�� ������� ����

        Sel_Menu("1.0");
        Sel_MainUI("Set");

        Sld_SS.value = SSlv;
        Ipf_SS.text = Sld_SS.value.ToString("F2");

        Dd_GQ.value = GQlv;

        Dd_Aa.value = AAlv;

        Dd_Gm.value = GMlv;

        Dd_Res.value = RESlv;

        Sld_Mv.value = Mv;
        txt_Mv.text = Mv.ToString("F0") +"%";

        Sld_BGMv.value = BGMv;
        txt_BGMv.text = BGMv.ToString("F0") + "%";

        Sld_SFXv.value = SFXv;
        txt_SFXv.text = SFXv.ToString("F0") + "%";
    }

    void UI_SetFunc()
    {
        // UI�� ����� ���
        Sld_SS.onValueChanged.AddListener((value) => SetMenu_Func("SS_sld"));
        Ipf_SS.onValueChanged.AddListener((value) => SetMenu_Func("SS_ipf"));

        Sld_Mv.onValueChanged.AddListener((value) => SetMenu_Func("Mv"));
        Sld_BGMv.onValueChanged.AddListener((value) => SetMenu_Func("BGMv"));
        Sld_SFXv.onValueChanged.AddListener((value) => SetMenu_Func("SFXv"));

        Dd_GQ.onValueChanged.AddListener((value) => SetMenu_Func("None"));
        Dd_Aa.onValueChanged.AddListener((value) => SetMenu_Func("None"));
        Dd_Gm.onValueChanged.AddListener((value) => SetMenu_Func("None"));
        Dd_Res.onValueChanged.AddListener((value) => SetMenu_Func("None"));

        Dd_Lang.onValueChanged.AddListener((value) => SetMenu_Func("None"));
    }

    void SetMenu_Func(string str)
    {
        if(str == "SS_sld")
        { Ipf_SS.text = Sld_SS.value.ToString(); }
        else if(str == "SS_ipf")
        {
            if (float.TryParse(Ipf_SS.text, out float parsedValue))
            {
                float Value = (float)Math.Round(parsedValue, 2);

                if(Value >= 100)
                {
                    Sld_SS.value = 100f;
                    Ipf_SS.text = "100.00";
                }
                else if(Value <= 0)
                {
                    Sld_SS.value = 0f;
                    Ipf_SS.text = "0.00";
                }
                else { Sld_SS.value = Value; }
            }
        }
        else if (str == "Mv")
        { txt_Mv.text = Sld_Mv.value.ToString("F0") + "%"; }
        else if (str == "BGMv")
        { txt_BGMv.text = Sld_BGMv.value.ToString("F0") + "%"; }
        else if (str == "SFXv")
        { txt_SFXv.text = Sld_SFXv.value.ToString("F0") + "%"; }

        if (SceneManager.GetActiveScene().name.Contains("Intro") == false)
        { page.True_Page(0); }
    }

    // ���� ����
    public void Func_Apply()
    {
        if (Steam == null)
        { Steam = GameObject.FindGameObjectWithTag("Steam").GetComponent<SteamSave>(); }

        List<float> floats = new List<float>();

        floats.Add(Sld_SS.value);
        floats.Add(Sld_Mv.value);
        floats.Add(Sld_BGMv.value);
        floats.Add(Sld_SFXv.value);

        List<int> ints = new List<int>();

        ints.Add(Dd_GQ.value);
        ints.Add(Dd_Aa.value);
        ints.Add(Dd_Gm.value);
        ints.Add(Dd_Res.value);
        ints.Add(Dd_Lang.value);

        Steam.SaveSettings(ints, floats);
        Apply();


        //-----------------------------------------------------

        //�κ��丮 �� ����
        List<string> Inven_Quick = new List<string>();
        List<string> Inven_Wp = new List<string>();
        List<string> Inven_Con = new List<string>();
        List<string> Inven_Val = new List<string>();
        List<string> Inven_MAT = new List<string>();

        List<int> Count_Quick = new List<int>();
        List<int> Count_Con = new List<int>();
        List<int> Count_Val = new List<int>();
        List<int> Count_MAT = new List<int>();

        int G = 0;

        List<string> EquipPT = new List<string>();

        List<string> Head_Col = new List<string>();
        List<string> Core_Col = new List<string>();
        List<string> Body_Col = new List<string>();
        List<string> Arms_Col = new List<string>();
        List<string> Legs_Col = new List<string>();

        for (int i = 0; i <= 5; i++)
        {
            Inven_Quick.Add("");
            Count_Quick.Add(0);
        }

        // �κ��丮
        for (int i = 0; i <= 23; i++)
        {
            Inven_Wp.Add(""); Inven_Con.Add(""); Inven_Val.Add(""); Inven_MAT.Add("");
            Count_Con.Add(0); Count_Val.Add(0); Count_MAT.Add(0);
        }

        // ������ ���
        for (int i = 0; i <= 6; i++)
        {
            EquipPT.Add("null");
        }

        // ������ �÷�
        for (int i = 0; i <= 2; i++)
        {
            Head_Col.Add(""); Core_Col.Add(""); Body_Col.Add("");
            Arms_Col.Add(""); Legs_Col.Add("");

            if (i == 0)
            {
                string col = "#ff0000";

                Head_Col[i] = col; Core_Col[i] = col; Body_Col[i] = col;
                Arms_Col[i] = col; Legs_Col[i] = col;
            }
            if (i == 1) 
            {
                string col = "#ff9100";

                Head_Col[i] = col; Core_Col[i] = col; Body_Col[i] = col;
                Arms_Col[i] = col; Legs_Col[i] = col;
            }
            if (i == 2) 
            {
                string col = "#ffea00";

                Head_Col[i] = col; Core_Col[i] = col; Body_Col[i] = col;
                Arms_Col[i] = col; Legs_Col[i] = col;
            }
        }

        //-----------------------------------------------------
        // ĳ���� ���̺�

        Steam.Save_Cha(Inven_Quick, Inven_Wp, Inven_Con, Inven_MAT, Inven_Val, 
            Count_Quick, Count_Con, Count_MAT, Count_Val,
            G, EquipPT, Head_Col, Core_Col, Body_Col, Arms_Col, Legs_Col);

        //-----------------------------------------------------
    }

    //���� ����
    void Apply()
    {
        // �׷��� ǰ��
        QualitySettings.SetQualityLevel(Dd_GQ.value);

        // ��Ƽ �����¡
        Aa_Set(Dd_Aa.value);

        // â���
        if (Dd_Gm.value == 0) 
        { Screen.fullScreenMode = FullScreenMode.Windowed; }
        // �׵θ� ���� â ���
        else if (Dd_Gm.value == 1)
        { Screen.fullScreenMode = FullScreenMode.FullScreenWindow; }
        //â ��� ��üȭ��
        else if (Dd_Gm.value == 2)
        { Screen.fullScreenMode = FullScreenMode.MaximizedWindow; }
        // ��üȭ�� 
        else if (Dd_Gm.value == 3)
        { Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen; }

        // �ػ� ����
        bool fullscreen = (Dd_Gm.value == 3);

        if (Dd_Res.value == 0) { Screen.SetResolution(3840, 2160, fullscreen); }
        else if (Dd_Res.value == 1) { Screen.SetResolution(2560, 1440, fullscreen); }
        else if (Dd_Res.value == 2) { Screen.SetResolution(1920, 1080, fullscreen); }
        else if (Dd_Res.value == 3) { Screen.SetResolution(1600, 900, fullscreen); }
        else if (Dd_Res.value == 4) { Screen.SetResolution(1280, 720, fullscreen); }

        // ���� ����
        SetVolume("MasterVolume", Sld_Mv.value);
        SetVolume("BGMVolume", Sld_BGMv.value);
        SetVolume("SFXVolume", Sld_SFXv.value);
    }

    void Aa_Set(int i)
    {
        // ��Ƽ �����¡
        var urpCameraData = Camera.main.GetUniversalAdditionalCameraData();


        if (i == 0)
        {
            // None
            urpAsset.msaaSampleCount = 0;
            urpCameraData.antialiasing = AntialiasingMode.None;
        }
        else if (i == 1)
        {
            // FXAA
            urpAsset.msaaSampleCount = 0;
            urpCameraData.antialiasing = AntialiasingMode.FastApproximateAntialiasing;
        }
        else if (i == 2)
        {
            // MSAA 2x
            urpAsset.msaaSampleCount = 2;
            urpCameraData.antialiasing = AntialiasingMode.None;
        }
        else if (i == 3)
        {
            // MSAA 4x
            urpAsset.msaaSampleCount = 4;
            urpCameraData.antialiasing = AntialiasingMode.None;
        }
        else if (i == 4)
        {
            // MSAA 8x
            urpAsset.msaaSampleCount = 8;
            urpCameraData.antialiasing = AntialiasingMode.None;
        }
        else if (i == 5)
        {
            // TAA
            urpAsset.msaaSampleCount = 0;
            urpCameraData.antialiasing = AntialiasingMode.TemporalAntiAliasing;
        }
    }

    public void Func_PageBool(bool b, int i)
    {
        if(b == true) { page.True_Page(i); }
        else { page.False_Page(i); }
    }


    public void Sel_Menu(string str)
    {
        for (int i = 0; i <= (MenuList.Count - 1); i++)
        {
            for (int o = 0; o <= (MenuList[i].Left.Count - 1); o++)
            {
                if (MenuList[i].Left[o] != null)
                { MenuList[i].Left[o].SetActive(false); }
            }

            if (MenuList[i].Manu_Main != null)
            { MenuList[i].Manu_Main.SetActive(false); }

            if(Under_Spr.Count > 0)
            { MenuList[i].Under_UI.GetComponent<Image>().sprite = Under_Spr[1]; }
        }

        if (str != "0.0")
        {
            for (int i = 0; i <= (MenuList.Count - 1); i++)
            {
                if (str == MenuList[i].Num)
                {
                    for (int o = 0; o <= (MenuList[i].Left.Count - 1); o++)
                    {
                        if (MenuList[i].Left[o] != null)
                        { MenuList[i].Left[o].SetActive(true); }
                    }

                    if (MenuList[i].Manu_Main != null)
                    {
                        MenuList[i].Manu_Main.SetActive(true);

                        if (Under_Spr.Count > 0)
                        { MenuList[i].Under_UI.GetComponent<Image>().sprite = Under_Spr[0]; }
                    }

                    break;
                }
            }
        }
        else
        {
           Set_SetData();
            if(SceneManager.GetActiveScene().name.Contains("Intro") == false)
            { page.All_Check(false); }
        }
    }

    public void Sel_MainUI(string str)
    {
        if (MainUI != null) { MainUI.SetActive(false); }
        if (SetUI != null) { SetUI.SetActive(false); }
        if (ExitUI != null) { ExitUI.SetActive(false); }

        if (str == "Main")
        { if (MainUI != null) { MainUI.SetActive(true); } }
        else if (str == "Set")
        { if (SetUI != null) { SetUI.SetActive(true); } }
         else if (str == "Exit")
        { if (ExitUI != null) { ExitUI.SetActive(true); } }
    }

    private float InitSlider(string parameterName)
    {
        if (audioMixer.GetFloat(parameterName, out float dB))
        {
            // dB �� 0~100% ��ȯ
            return Mathf.Pow(10f, dB / 20f) * 100f;
        }
        else
        {
            return 100f; // ���� ������ �⺻ 100%
        }
    }

    /// <summary>
    /// �����̴� % ���� AudioMixer dB ������ ��ȯ�� ����
    /// </summary>
    public void SetVolume(string parameterName, float percent)
    {
        if (percent <= 0.01f)
            audioMixer.SetFloat(parameterName, -80f); // ����
        else
            audioMixer.SetFloat(parameterName, Mathf.Log10(percent / 100f) * 20f);
    }

    AntiAliasingMode GetCurrentAA()
    {
        var urpAsset = GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;
        int msaaLevel = (urpAsset != null) ? urpAsset.msaaSampleCount : 0;

        switch (msaaLevel)
        {
            case 2: return AntiAliasingMode.MSAA2x;
            case 4: return AntiAliasingMode.MSAA4x;
            case 8: return AntiAliasingMode.MSAA8x;
            default:
                // MSAA ������ ����Ʈ ���μ��� �� ���� ����
                if (postProcessAA == AntiAliasingMode.FXAA || postProcessAA == AntiAliasingMode.TAA)
                    return postProcessAA;
                else
                    return AntiAliasingMode.None;
        }
    }

    public IEnumerator LoadSceneAsync(string sceneName)
    {
        Image ProgBar = Instantiate(Scene_Loading, Vector3.zero, Quaternion.Euler(Vector3.zero))
            .GetComponent<SceneLoading>().Loading_Bar;

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

        // �� �ε��� ���� ������ ���
        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            ProgBar.fillAmount = progress; // ����� ������Ʈ
            yield return null;
        }
    }
}
