using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_HitColl : MonoBehaviour
{
    public Player_Ctrl player;

    private void Start()
    {
        transform.tag = "PlayerColl";
    }

    private void OnTriggerEnter(Collider coll)
    {
        if (coll.tag == "Enemy")
        {
            if (player.HitIs == false)
            {
                //Debug.Log("개 쳐맞음");

                if (coll.GetComponent<HitColl>() != null)
                {
                    HitColl hit = coll.GetComponent<HitColl>();

                    player.hitUI.Spr = hit.spr;
                    player.hitUI.tit = hit.Tit;
                    player.hitUI.Reason = hit.Reason;

                    player.hitUI.Fallen = hit.Fallen;

                    player.Hit();
                    //player.Hit(hit.spr, hit.Tit, hit.Reason);
                }
                else
                {
                    Debug.LogWarning("HitColl이 추가되지않았습니다.");
                }
            }
        }
    }
}
