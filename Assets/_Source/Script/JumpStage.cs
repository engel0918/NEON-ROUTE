using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpStage : MonoBehaviour
{
    public enum Type { type01, type02 }
    public Type type;

    [Header("Type01")]
    public Animator Anim;
    bool Is_Jump = false;
    int Delay = 0;
    public int DelayLim = 3;

    [Header("Type02")]
    public float jumpForce;
    public GameObject player;
    public Rigidbody rb;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void OnCollisionEnter(Collision coll)
    {
        if(coll.gameObject.CompareTag("Player"))
        {
            if(type == Type.type01)
            {
                if (Is_Jump == false)
                {
                    if (Anim != null) { Anim.SetTrigger("Act"); }
                    Is_Jump = true;

                    StartCoroutine(JunpDel());
                }
            }
        }

        if (type == Type.type02)
        {
            if (rb == null)
            { rb = player.GetComponent<Rigidbody>(); }

            if (Input.GetKeyDown(KeyCode.Space))
            { rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse); }
        }
    }

    private void OnCollisionStay(Collision coll)
    {
        if (coll.gameObject.CompareTag("Player"))
        {
            if (type == Type.type02)
            {
                if (rb == null)
                { rb = player.GetComponent<Rigidbody>(); }

                if (Input.GetKeyDown(KeyCode.Space))
                { rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse); }
            }
        }
    }

    IEnumerator JunpDel()
    {
        if(Delay <= DelayLim)
        {
            Delay++;
        }

        yield return new WaitForSeconds(1f);
        if (Delay <= DelayLim) { StartCoroutine(JunpDel()); }
        else if (Delay > DelayLim)
        { Is_Jump = false; Delay = 0; }
    }
}
