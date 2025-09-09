using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCam : MonoBehaviour
{
    Transform Cam;
    // Start is called before the first frame update
    void Start()
    {
        Cam = Camera.main.transform;
    }

    private void FixedUpdate()
    {
        transform.LookAt(Cam);
    }
}
