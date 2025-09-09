using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class targetMark : MonoBehaviour
{
    public float DeleteCount;
    float Count;
    // Start is called before the first frame update
    void Start()
    {
        Count = 0f;


    }

    // Update is called once per frame
    void Update()
    {
        if(DeleteCount <= Count)
        { Destroy(gameObject); }
        else { Count += Time.deltaTime; }
    }
}
