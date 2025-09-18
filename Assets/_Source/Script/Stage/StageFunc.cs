using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StageFunc : MonoBehaviour
{
    public GameObject Player_Prefab;
    public GameObject Menu_Prefab;

    [Header("시작 위치")]
    public Transform StartPos;

    [Header("AI NPC")]
    public List<Enemy> enemyList;

    [Header("지정X")]
    MenuCtrl Menu;
    public GameObject Press_F;

    Player_Ctrl Player;
    GameMgr gameMgr;
    SteamSave steam;

    private void Awake()
    {
        steam = new GameObject("SteamMgr").AddComponent<SteamManager>().AddComponent<SteamSave>();
        steam.tag = "Steam";
    }

    // Start is called before the first frame update
    void Start()
    {
        gameMgr = GameObject.FindGameObjectWithTag("GameMgr").GetComponent<GameMgr>();

        Menu = Instantiate(Menu_Prefab).GetComponent<MenuCtrl>();
        Player = Instantiate(Player_Prefab, StartPos.position, StartPos.rotation).GetComponent<Player_Ctrl>();
        Player.Menu = Menu;

        Press_F = Menu.Press_F;

        for (int i = 0; i <= (enemyList.Count - 1); i++)
        {
            GameObject enemy = Instantiate(enemyList[i].Enemy_Prefab, enemyList[i].Ins_Pos.position, enemyList[i].Ins_Pos.rotation).gameObject;
            enemy.GetComponent<AIController>().patrolPoints = enemyList[i].Patrol_Points;
        }
    }

    public void Goal_Func()
    {
        gameMgr.Curr_Stage++;

        if (gameMgr.Stage.Count <= gameMgr.Curr_Stage)
        { All_Clear(); }
        else
        { Next_Stage(gameMgr.Stage[gameMgr.Curr_Stage]); }

    }
    void All_Clear()
    { StartCoroutine(LoadSceneAsync(gameMgr.FIN_Scene)); }

    void Next_Stage(string str)
    {
        StartCoroutine(LoadSceneAsync(str));
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        Image ProgBar = Instantiate(Player.Scene_Loading, Vector3.zero, Quaternion.Euler(Vector3.zero))
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

[System.Serializable]
public struct Enemy
{
    public GameObject Enemy_Prefab;
    public Transform Ins_Pos;
    public Transform[] Patrol_Points;
}
