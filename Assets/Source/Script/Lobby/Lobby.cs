using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class Lobby : MonoBehaviour
{
    public GameObject Player_Prefab;
    public GameObject Menu_Prefab;

    public Transform Player_Pos;

    Player_Ctrl Player;
    MenuCtrl Menu;

    [Header("상호작용 UI")]
    public GameObject Press_F;

    SteamSave steam;

    private void Awake()
    {
        steam = new GameObject("SteamMgr").AddComponent<SteamManager>().AddComponent<SteamSave>();
        steam.tag = "Steam";
    }

    // Start is called before the first frame update
    void Start()
    {
        Menu = Instantiate(Menu_Prefab).GetComponent<MenuCtrl>();
        Player = Instantiate(Player_Prefab, Player_Pos.position, Player_Pos.rotation).GetComponent<Player_Ctrl>();
        Player.Menu = Menu;

        Press_F = Menu.Press_F;

    }

    public void PlayFunc()
    {
        Player.SceneMove("Stage 0.0.1");
    }

}