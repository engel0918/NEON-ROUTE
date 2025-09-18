using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.GraphicsBuffer;

public class Btn : MonoBehaviour
{
    public UnityEvent onPress;
    public UnityEvent onRelease;
    AudioSource As;
    bool isPressed;
    public bool Trig;

    Vector3 OriginPos;
    Rigidbody rb;
    public float speed = 5f;

    private void Start()
    {
        OriginPos = transform.position;
        As = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody>();

    }

    void FixedUpdate()
    {
        //if (Trig == false)
        //{
        //    transform.position = Vector3.MoveTowards(transform.position, OriginPos, speed * Time.deltaTime);
        //}
    }

    private void OnTriggerEnter(Collider coll)
    {
        if (!coll.CompareTag("PlayerColl")) return;

        Trig = true;
        if (!isPressed)
        {
            onPress.Invoke();
            As.Play();
            isPressed = true;
        }
    }

    private void OnTriggerStay(Collider coll)
    {
        if (coll.CompareTag("PlayerColl"))
        {
            Trig = true;
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, OriginPos, speed * Time.deltaTime);
        }
    }

    private void OnTriggerExit(Collider coll)
    {
        if (!coll.CompareTag("PlayerColl")) return;

        Trig = false;
        onRelease.Invoke();
        isPressed = false;
    }

    public void Debug_(string str)
    {
        Debug.Log(str);
    }
}
