using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HitColl : MonoBehaviour
{
    public string Tit;
    public Sprite spr;
    public List<string> Reason;

    [Header("죽음위치의 존재 여부(없는경우 True)")]
    public bool Fallen;
}
