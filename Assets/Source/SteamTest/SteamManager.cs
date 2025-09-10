using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public class SteamManager : MonoBehaviour
{
    private static SteamManager s_instance;
    private static bool s_EverInitialized;

    public static bool Initialized { get; private set; }

    private void Awake()
    {
        if (s_instance != null)
        {
            Destroy(gameObject);
            return;
        }

        s_instance = this;

        try
        {
            if (SteamAPI.RestartAppIfNecessary((AppId_t)3904900))
            {
                Application.Quit();
                return;
            }
        }
        catch (System.DllNotFoundException e)
        {
            Debug.LogError("steam_api.dll 없음: " + e);
            Application.Quit();
            return;
        }

        Initialized = SteamAPI.Init();
        if (!Initialized)
        {
            Debug.LogError("SteamAPI 초기화 실패");
        }
        else
        {
            Debug.Log("Steam 초기화 성공");
        }

        s_EverInitialized = true;
    }

    private void OnEnable()
    {
        if (s_instance == null) s_instance = this;
    }

    private void OnDestroy()
    {
        if (s_instance != this) return;

        s_instance = null;

        if (Initialized)
        {
            SteamAPI.Shutdown();
        }
    }

    private void Update()
    {
        if (Initialized)
        {
            SteamAPI.RunCallbacks();
        }
    }
}