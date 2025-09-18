using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RankUI_Item : MonoBehaviour
{
    [Header("RectTr Y길이값")]
    public float tr_Y;

    public TMP_Text txt_Rank;
    public Image img_Rank;

    public RawImage Pic;
    public TMP_Text Name;
    public TMP_Text Times;

    [Header("1, 2, 3위 스프라이트")]
    public List<Sprite> Rank_spr;


    public void Rank_Tit(int i)
    {
        img_Rank.sprite = Rank_spr[i];

        txt_Rank.gameObject.SetActive(false);
        img_Rank.gameObject.SetActive(true);

    }
}
