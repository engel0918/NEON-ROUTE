using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using System.Security.Cryptography;
using System.IO;


[System.Serializable]

public class SettingsData
{
    public float SS;
    public int GQ, AA, GM, RES;
    public float Mv, BGMv, SFXv;
    public int Lang;
}

public class Data_Cha
{
    // 인벤토리
    public List<string> Inven_Quick, Inven_WP, Inven_CON, Inven_MAT, Inven_VAL;
    public List<int> Count_Quick, Count_CON, Count_MAT, Count_VAL;
    public int Gold;

    // 장착된 장비
    public List<string> EquipPT;

    // 지정된 컬러
    public List<string> HeadCol, Core_Col, Body_Col, Arms_Col, Legs_Col;
}

public static class KeyManager
{
    /// <summary>
    /// Steam ID 기반으로 Key / IV 생성
    /// </summary>
    public static void GetKeyAndIV(out byte[] key, out byte[] iv)
    {
        ulong steamId = SteamUser.GetSteamID().m_SteamID;
        string baseStr = steamId.ToString();

        using (SHA256 sha = SHA256.Create())
        {
            byte[] hash = sha.ComputeHash(Encoding.UTF8.GetBytes(baseStr));
            key = new byte[32];
            iv = new byte[16];

            Array.Copy(hash, 0, key, 0, 32);
            Array.Copy(hash, 0, iv, 0, 16);
        }
    }
}

public static class CryptoUtil
{
    public static byte[] Encrypt(string plainText)
    {
        KeyManager.GetKeyAndIV(out byte[] key, out byte[] iv);

        using (Aes aes = Aes.Create())
        {
            aes.Key = key;
            aes.IV = iv;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
            return encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
        }
    }

    public static string Decrypt(byte[] cipherBytes)
    {
        KeyManager.GetKeyAndIV(out byte[] key, out byte[] iv);

        using (Aes aes = Aes.Create())
        {
            aes.Key = key;
            aes.IV = iv;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            byte[] decryptedBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
            return Encoding.UTF8.GetString(decryptedBytes);
        }
    }
}

[System.Serializable]
public class Ranker
{
    public static List<string> Names = new List<string>();
    public static List<int> Ranks = new List<int>();
    public static List<int> Times = new List<int>();
    public static List<Texture2D> Pics = new List<Texture2D>();
}

[System.Serializable]
public class MyRank
{
    public static string Name;
    public static int Rank;
    public static int Time;
    public static Texture2D Pic;
}

public class SteamSave : MonoBehaviour
{

    public TMP_InputField ipf;

    private const string SETTINGS_FILE = "settings.json";
    private const string Cha_Path = "Character.json";

    private SteamLeaderboard_t m_SteamLeaderboard;
    private CallResult<LeaderboardFindResult_t> m_findResult;
    private CallResult<LeaderboardScoreUploaded_t> m_uploadResult;
    private CallResult<LeaderboardScoresDownloaded_t> m_downloadResult;

    private List<CSteamID> playerSteamIDs = new List<CSteamID>();

    private float pendingScore = -1f;

    public List<RankUI> RankUIs;

    public static SteamSave Instance { get; private set; }
    private Data_Cha loadedData; // 로드된 데이터 저장용



    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    void Start()
    {

        if (!SteamManager.Initialized)
        {
            Debug.LogError("Steam 초기화 안됨");
            return;
        }

        m_findResult = CallResult<LeaderboardFindResult_t>.Create(OnLeaderboardFind);
        m_uploadResult = CallResult<LeaderboardScoreUploaded_t>.Create(OnScoreUploaded);
        m_downloadResult = CallResult<LeaderboardScoresDownloaded_t>.Create(OnScoresDownloaded);

        // 내 이름, 사진 불러오기
        MyRank.Name = SteamFriends.GetPersonaName();

        LoadMyProfilePicture((Texture2D tex) =>
        {
            if (tex != null)
            { MyRank.Pic = tex; }
        });

        Debug.Log("Cloud 사용 가능?: " + SteamRemoteStorage.IsCloudEnabledForApp());
    }

    public void Steam_Set()
    {
        if (!SteamManager.Initialized)
        {
            Debug.LogError("Steam 초기화 안됨");
            return;
        }

        m_findResult = CallResult<LeaderboardFindResult_t>.Create(OnLeaderboardFind);
        m_uploadResult = CallResult<LeaderboardScoreUploaded_t>.Create(OnScoreUploaded);
        m_downloadResult = CallResult<LeaderboardScoresDownloaded_t>.Create(OnScoresDownloaded);

        // 내 이름, 사진 불러오기
        MyRank.Name = SteamFriends.GetPersonaName();

        LoadMyProfilePicture((Texture2D tex) =>
        {
            if (tex != null)
            { MyRank.Pic = tex; }
        });
    }

    public string SetDat_Check()
    {
        Steam_Set();

        ////클라우드 삭제 기능
        //if (SteamRemoteStorage.FileExists(SETTINGS_FILE))
        //{
        //    bool deleted = SteamRemoteStorage.FileDelete(SETTINGS_FILE);
        //    Debug.Log("File Deleted: " + SETTINGS_FILE + deleted);
        //}

        //if (SteamRemoteStorage.FileExists(Cha_Path))
        //{
        //    bool deleted = SteamRemoteStorage.FileDelete(Cha_Path);
        //    Debug.Log("File Deleted: " + Cha_Path + deleted);
        //}


        if (!SteamRemoteStorage.FileExists(SETTINGS_FILE))
        { return "No Data"; }
        else
        { return "Data Available"; }
    }

    // 암호화 - 캐릭터 세이브
    public void Save_Cha(List<string> InvenQuick, List<string> InvenWP, List<string> InvenCON, List<string> InvenMAT, List<string> InvenVAL,
          List<int> CountQuick, List<int> CountCON, List<int> CountMAT, List<int> CountVAL, int G, List<string> EquipPT,
         List<string> Head_Col, List<string> Core_Col, List<string> Body_Col, List<string> Arms_Col, List<string> Legs_Col)
    {
        Data_Cha data = new Data_Cha
        {
            Inven_Quick = InvenQuick,
            Inven_WP = InvenWP,
            Inven_CON = InvenCON,
            Inven_MAT = InvenMAT,
            Inven_VAL = InvenVAL,

            Count_Quick = CountQuick,
            Count_CON = CountCON,
            Count_MAT = CountMAT,
            Count_VAL = CountVAL,

            Gold = G,

            EquipPT = EquipPT,

            HeadCol = Head_Col,
            Core_Col = Core_Col,
            Body_Col = Body_Col,
            Arms_Col = Arms_Col,
            Legs_Col = Legs_Col
        };

        string json = JsonUtility.ToJson(data);
        byte[] encrypted = CryptoUtil.Encrypt(json);

        bool result = SteamRemoteStorage.FileWrite(Cha_Path, encrypted, encrypted.Length);
    }

    public Data_Cha Load_Cha()
    {
        if (!SteamRemoteStorage.FileExists("Character.json"))
        {
            Debug.LogWarning("Inventory file not found.");
            return null;
        }

        int size = SteamRemoteStorage.GetFileSize("Character.json");
        byte[] buffer = new byte[size];
        SteamRemoteStorage.FileRead("Character.json", buffer, size);

        try
        {
            string decrypted = CryptoUtil.Decrypt(buffer);
            loadedData = JsonUtility.FromJson<Data_Cha>(decrypted);
            Debug.Log("Inventory Loaded (Decrypted).");
            return loadedData;
        }
        catch (Exception ex)
        {
            Debug.LogError("Failed to decrypt Inventory: " + ex.Message);
            return null;
        }
    }

    public Data_Cha GetLoadedData()
    {
        return loadedData;
    }

    public void SaveSettings(List<int> val1, List<float> val2)
    {
        SettingsData data = new SettingsData
        {
            GQ = val1[0],
            AA = val1[1],
            GM = val1[2],
            RES = val1[3],

            SS = val2[0],
            Mv = val2[1],
            BGMv = val2[2],
            SFXv = val2[3],
            Lang = val1[4],
        };

        string json = JsonUtility.ToJson(data);
        byte[] bytes = Encoding.UTF8.GetBytes(json);

        bool result = SteamRemoteStorage.FileWrite(SETTINGS_FILE, bytes, bytes.Length);

        Debug.Log("Settings Saved to Steam: " + result);
        //Debug.Log(data.Test);
    }

    public void LoadSettings()
    {
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

        Debug.Log("Settings Loaded from Steam.");
        //Debug.Log(data.Test);
    }

    /// <summary>
    /// 테스트용. 유저가 임의로 입력한 점수 업로드를 방지합니다.
    /// </summary>
    public void ipf_Save_Score()
    {
        //Debug.LogWarning("직접 입력은 허용되지 않습니다.");

        // 테스트 목적이 아니라면 주석을 해제하지 마십시오.
        if (float.TryParse(ipf.text, out float score))
        {
            UploadMyScore(score);
        }
        else
        {
            Debug.LogError("입력값이 올바른 숫자가 아닙니다: " + ipf.text);
        }
    }

    /// <summary>
    /// 점수는 시스템 내부에서만 호출되도록 메서드를 보호합니다.
    /// </summary>
    public void UploadMyScore(float timeSeconds)
    {
        if (!SteamManager.Initialized)
        {
            Debug.LogError("Steam 초기화 안됨");
            return;
        }
        if (timeSeconds <= 0f) // 0미만은 허용하지 않음 (기본 검증)
        {
            Debug.LogError("잘못된 점수 값");
            return;
        }

        pendingScore = timeSeconds;

        SteamAPICall_t handle = SteamUserStats.FindOrCreateLeaderboard(
            "ClearTime",
            ELeaderboardSortMethod.k_ELeaderboardSortMethodAscending,
            ELeaderboardDisplayType.k_ELeaderboardDisplayTypeTimeSeconds
        );
        m_findResult.Set(handle);
    }

    private void OnLeaderboardFind(LeaderboardFindResult_t result, bool failure)
    {
        if (failure || result.m_bLeaderboardFound == 0)
        {
            Debug.LogError("리더보드 찾기 실패");
            return;
        }

        m_SteamLeaderboard = result.m_hSteamLeaderboard;

        int timeInMs = Mathf.RoundToInt(pendingScore * 1000f);

        var handle = SteamUserStats.UploadLeaderboardScore(
            m_SteamLeaderboard,
            // 점수 타입

            // 낮을수롣 상위 랭킹
            ELeaderboardUploadScoreMethod.k_ELeaderboardUploadScoreMethodKeepBest,

            // 높을수록 상위 랭킹
            //ELeaderboardUploadScoreMethod.k_ELeaderboardUploadScoreMethodForceUpdate, 
            timeInMs,
            null,
            0
        );
        m_uploadResult.Set(handle);

        Debug.Log("리더보드 연결 및 점수 업로드 중...");
    }

    private void OnScoreUploaded(LeaderboardScoreUploaded_t result, bool failure)
    {
        if (failure || result.m_bSuccess == 0)
        {
            Debug.LogError("점수 업로드 실패");
            return;
        }

        Debug.Log($"점수 업로드 성공! 순위: {result.m_nGlobalRankNew}");

        SteamAPICall_t handle = SteamUserStats.DownloadLeaderboardEntries(
            m_SteamLeaderboard,
            ELeaderboardDataRequest.k_ELeaderboardDataRequestGlobal,
            1,
            100
        );
        m_downloadResult.Set(handle);
    }

    private void OnScoresDownloaded(LeaderboardScoresDownloaded_t result, bool failure)
    {
        if (failure)
        {
            Debug.LogError("랭킹 다운로드 실패");
            return;
        }

        RankReset();
        playerSteamIDs.Clear();

        Debug.Log("===== 상위 100명 랭킹 =====");
        for (int i = 0; i < result.m_cEntryCount; i++)
        {
            LeaderboardEntry_t entry;
            int[] details = new int[0];
            SteamUserStats.GetDownloadedLeaderboardEntry(result.m_hSteamLeaderboardEntries, i, out entry, details, 0);

            // 내 정보만 처리
            if (entry.m_steamIDUser == SteamUser.GetSteamID())
            {
                //int rank = entry.m_nGlobalRank;
                int time = entry.m_nScore / 1000;

                // Debug.Log($"내 순위: {rank}위, 기록: {time:F3}초");
                MyRank.Rank = (i);
                MyRank.Time = time;
            }

            int timeSec = entry.m_nScore / 1000;
            string playerName = SteamFriends.GetFriendPersonaName(entry.m_steamIDUser);
            Debug.Log($"{i + 1}위 - {playerName} (SteamID: {entry.m_steamIDUser}), 시간: {timeSec:F3}초");

            Ranker.Ranks.Add(i);
            Ranker.Names.Add(playerName);
            Ranker.Times.Add(timeSec);

            playerSteamIDs.Add(entry.m_steamIDUser);
        }

        StartCoroutine(LoadAvatars());
    }

    IEnumerator LoadAvatars()
    {
        for (int i = 0; i < playerSteamIDs.Count; i++)
        {
            int imageId = SteamFriends.GetLargeFriendAvatar(playerSteamIDs[i]);

            while (imageId == -1)
            {
                yield return new WaitForSeconds(0.1f);
                imageId = SteamFriends.GetLargeFriendAvatar(playerSteamIDs[i]);
            }

            if (SteamUtils.GetImageSize(imageId, out uint width, out uint height))
            {
                byte[] image = new byte[width * height * 4];
                if (SteamUtils.GetImageRGBA(imageId, image, image.Length))
                {
                    Texture2D avatar = new Texture2D((int)width, (int)height, TextureFormat.RGBA32, false);
                    avatar.LoadRawTextureData(image);
                    avatar.Apply();

                    Ranker.Pics.Add(avatar);
                }
            }

            yield return null;
        }
        RankApply();
    }

    // 내 정보 들고오기
    public void LoadMyProfilePicture(Action<Texture2D> onAvatarReady = null)
    {
        CSteamID mySteamId = SteamUser.GetSteamID();
        int avatarInt = SteamFriends.GetLargeFriendAvatar(mySteamId); // Large: 184x184

        if (avatarInt == -1)
        {
            Debug.LogWarning("아바타 로딩 대기 중...");
            StartCoroutine(WaitForAvatar(mySteamId, onAvatarReady));
            return;
        }

        if (SteamUtils.GetImageSize(avatarInt, out uint width, out uint height))
        {
            byte[] imageData = new byte[width * height * 4];

            if (SteamUtils.GetImageRGBA(avatarInt, imageData, imageData.Length))
            {
                Texture2D avatarTex = new Texture2D((int)width, (int)height, TextureFormat.RGBA32, false);
                avatarTex.LoadRawTextureData(imageData);
                avatarTex.Apply();

                onAvatarReady?.Invoke(avatarTex);
            }
        }
    }

    private IEnumerator WaitForAvatar(CSteamID steamId, Action<Texture2D> callback)
    {
        int avatarInt = -1;
        while (avatarInt == -1)
        {
            yield return new WaitForSeconds(0.1f);
            avatarInt = SteamFriends.GetLargeFriendAvatar(steamId);
        }

        if (SteamUtils.GetImageSize(avatarInt, out uint width, out uint height))
        {
            byte[] imageData = new byte[width * height * 4];

            if (SteamUtils.GetImageRGBA(avatarInt, imageData, imageData.Length))
            {
                Texture2D avatarTex = new Texture2D((int)width, (int)height, TextureFormat.RGBA32, false);
                avatarTex.LoadRawTextureData(imageData);
                avatarTex.Apply();

                callback?.Invoke(avatarTex);
            }
        }
    }

    void RankApply()
    {
        for (int i = 0; i <= (RankUIs.Count - 1); i++)
        {
            if (RankUIs != null)
            {
                RankUIs[i].RankCheck();
            }
        }
    }

    void RankReset()
    {
        Ranker.Names.Clear();
        Ranker.Pics.Clear();
        Ranker.Ranks.Clear();
        Ranker.Times.Clear();
    }

    void Update()
    {
        if (SteamManager.Initialized)
            SteamAPI.RunCallbacks();
    }

    public void ResetMyScoreToMin(float score)
    {
        float maxScore = 9999999f;
        UploadMyScore(maxScore);
    }
}