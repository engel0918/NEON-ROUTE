using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RankUI_Item : MonoBehaviour
{
    [Header("RectTr Y���̰�")]
    public float tr_Y;

    public TMP_Text txt_Rank;
    public Image img_Rank;

    public RawImage Pic;
    public TMP_Text Name;
    public TMP_Text Times;

    [Header("1, 2, 3�� ��������Ʈ")]
    public List<Sprite> Rank_spr;


    public void Rank_Tit(int i)
    {
        img_Rank.sprite = Rank_spr[i];

        txt_Rank.gameObject.SetActive(false);
        img_Rank.gameObject.SetActive(true);

    }
}
