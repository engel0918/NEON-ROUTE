using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class RankUI : MonoBehaviour
{
    SteamSave Save;

    [SerializeField] ScrollView scroll;
    [SerializeField] Transform Contents;

    [Header("프리펩, 스크롤안에 들어가는 Cell")]
    [SerializeField] GameObject Item;

    [Header("내 기록이 들어간 오브젝트")]
    [SerializeField] RankUI_Item MyItem;

    bool StartOn = false;

    private void Start()
    {
        Save = GameObject.FindGameObjectWithTag("Save").GetComponent<SteamSave>();
        Save.RankUIs.Add(this);

        RankCheck();
        StartOn = true;
    }

    private void OnEnable()
    {
        if(StartOn == true)
        {

        }
    }

    public void RankCheck()
    {
        if(MyItem != null)
        {
            MyItem.Name.text = MyRank.Name;
            MyItem.Pic.texture = MyRank.Pic;
            SecondsToHHMMSS(MyRank.Time, MyItem.Times);

            if (MyRank.Rank >= 0 && MyRank.Rank < 3)
            {
                MyItem.Rank_Tit(MyRank.Rank);
            }
            else
            {
                MyItem.txt_Rank.text = ((MyRank.Rank + 1) + "th").ToString();

                MyItem.txt_Rank.gameObject.SetActive(true);
                MyItem.img_Rank.gameObject.SetActive(false);
            }
        }

        foreach (Transform child in Contents)
        { GameObject.Destroy(child.gameObject); }

        if (Item != null)
        {
            RectTransform Box = Contents.GetComponent<RectTransform>();
            float Y = (Item.GetComponent<RankUI_Item>().tr_Y * Ranker.Names.Count);
            Box.sizeDelta = new Vector2(Box.sizeDelta.x, Y);

            if (Ranker.Names.Count > 0)
            {
                for (int i = 0; i <= (Ranker.Names.Count - 1); i++)
                {
                    RankUI_Item item = Instantiate(Item, Contents).GetComponent<RankUI_Item>();

                    item.Name.text = Ranker.Names[i];
                    SecondsToHHMMSS(Ranker.Times[i], item.Times);
                    item.Pic.texture = Ranker.Pics[i];

                    if (i >= 0 && i < 3)
                    {
                        item.Rank_Tit(i);
                    }
                    else
                    {
                        item.txt_Rank.text = ((i + 1) + "th").ToString();

                        item.txt_Rank.gameObject.SetActive(true);
                        item.img_Rank.gameObject.SetActive(false);
                    }
                }

            }
        }
    }

    void DelAllChild(GameObject parent)
    {
        foreach (Transform child in parent.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }

    public string SecondsToHHMMSS(float totalSecondsFloat, TMP_Text txt)
    {
        int totalSeconds = Mathf.FloorToInt(totalSecondsFloat); // 내림 처리, 반올림은 Mathf.RoundToInt

        int hours = totalSeconds / 3600;
        int minutes = (totalSeconds % 3600) / 60;
        int seconds = totalSeconds % 60;

        string timeStr = $"{hours:D2}:{minutes:D2}:{seconds:D2}";
        txt.text = timeStr;
        return timeStr;
    }
}
