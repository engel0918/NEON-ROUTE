using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameMgr : MonoBehaviour
{
    public string Cha;
    public int Life;
    public int Max_Life;
    public int SaveStage;

    public int Curr_Stage;
    public string FIN_Scene;

    public List<string> Stage;

    public float Timer;

    private void Awake()
    {
        SaveStage = 0;
        DontDestroyOnLoad(transform);
    }
}
