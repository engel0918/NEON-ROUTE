using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCtrl_Evt : MonoBehaviour
{
    public Player_Ctrl Player;

    public void Stop_true()
    {
        if (Player != null)
        { Player.Stop = true; }
        else { Debug.LogWarning("플레이어가 지정되지 않았습니다."); }
    }

    public void Stop_false()
    {
        if (Player != null)
        { Player.Stop = false; }
        else { Debug.LogWarning("플레이어가 지정되지 않았습니다."); }
    }

    public void PlayerColl_True()
    {
        if (Player != null)
        { Player.ctrl_Coll(true); }
        else { Debug.LogWarning("플레이어가 지정되지 않았습니다."); }
    }
}
