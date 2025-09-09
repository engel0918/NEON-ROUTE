using Unity.VisualScripting;
using UnityEngine;

public class Fin_Func : MonoBehaviour
{
    public GameObject Player_Prefab;
    public GameObject Menu_Prefab;

    [Header("시작 위치")]
    public Transform StartPos;

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

    private void Start()
    {
        gameMgr = GameObject.FindGameObjectWithTag("GameMgr").GetComponent<GameMgr>();

        Menu = Instantiate(Menu_Prefab).GetComponent<MenuCtrl>();
        Player = Instantiate(Player_Prefab, StartPos.position, StartPos.rotation).GetComponent<Player_Ctrl>();
        Player.Menu = Menu;

        Press_F = Menu.Press_F;
    }
}
