using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemCtrl : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    HoverUI hover;

    [Header("Fill O")]
    public Image Thum;
    public TMP_Text txt_Count;
    public string Item_ID;

    [Header ("Fill X")]
    public Item_Load ItList;
    public int Count;
    int It_Num;
    public string Type;

    [SerializeField] Sprite Thum_spr;

    private void OnEnable()
    {
        if(Thum_spr == null)
        { ItemCheck(Item_ID); }
    }

    public void ItemCheck(string str)
    {
        if (ItList == null)
        { ItList = GameObject.FindGameObjectWithTag("ItList").GetComponent<Item_Load>(); }

        Item_ID = str;

        for (int i = 0; i <= (ItList.thums.Count - 1); i++)
        {
            if (Item_ID == ItList.thums[i].Id)
            {
                Thum_spr = ItList.thums[i].Thumnail;
                Thum.sprite = ItList.thums[i].Thumnail;

                break;
            }
        }

        for (int i = 0; i <= (ItList.It_Data.Count - 1); i++)
        {
            if (ItList.It_Data[i].Id == Item_ID)
            {
                It_Num = i;

                Type = ItList.It_Data[i].Types;

                // 장비인 경우, Count text 안보이게
                if (ItList.It_Data[i].Types == "장비")
                { txt_Count.gameObject.SetActive(false); }

                break;
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (Input.GetMouseButton(0) == false)
        {
            if (hover == null)
            { hover = GameObject.Find("item_hover").GetComponent<HoverUI>(); }

            if (It_Num != -1)
            { hover.Txting(It_Num, Thum_spr); }
        }

        //Debug.Log("마우스가 이미지 위에 올라옴");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (hover == null)
        { hover = GameObject.Find("item_hover").GetComponent<HoverUI>(); }

        hover.Contents.SetActive(false);

        //Debug.Log("마우스가 이미지에서 벗어남");
    }
}
