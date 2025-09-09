using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public enum Type { Ptc, Audio, Both }
    public Type Case;

    public GameObject Ptc;
    public AudioClip Clip;

    public string Buff;
    public int BuffTime;

    Buff_Func Buff_func;

    public void GetItem()
    {
        if (Case == Type.Ptc || Case == Type.Both)
        {
            GameObject PTC = Instantiate(Ptc, transform.position, transform.rotation);
            PTC.AddComponent<Ins_Effect>().PtcPlay(PTC.GetComponent<ParticleSystem>());
        }

        if (Case == Type.Audio || Case == Type.Both)
        {
            if (Clip != null)
            {
                GameObject Audio = new GameObject(transform.name + "_Audio");
                Audio.AddComponent<Ins_Effect>().AudioPlay(Clip);
            }
        }

        if(Buff!= "" && BuffTime > 0)
        {
            Buff_func.BuffOn(Buff, BuffTime);
        }

        Destroy(this.gameObject);
    }

    private void OnTriggerEnter(Collider Coll)
    {
        if (Coll.tag == "Player")
        {
            Buff_func = Coll.GetComponent<Player_Ctrl>().Buff; 
            GetItem();
        }
    }
}
