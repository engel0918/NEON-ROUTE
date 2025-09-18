using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class HoverUI : MonoBehaviour
{
    public Transform Inventory;
    public RectTransform Panel;

    [Header("txt_정렬")]
    [SerializeField] List<RectTransform> Align_Txts;
    [SerializeField] float Space;

    Item_Load itemMgr;

    public GameObject Contents;
    public Image Thum;
    [Header("0.tit, 1.type, 2.parts 3.info, 4,description")]
    public List<TMP_Text> txt_List;


    private void Start()
    {
        Contents.SetActive(false);
    }

    void position()
    {
        Vector2 mousePos = Input.mousePosition; // 마우스 좌표
        Panel.position = mousePos;
    }

    public void Txting(int value, Sprite spr, string str)
    {
        MoveBelow();

        position();

        if (itemMgr == null)
        { itemMgr = GameObject.FindGameObjectWithTag("ItList").GetComponent<Item_Load>(); }

        Thum.sprite = spr;

        Txt_(str, value);

        StartCoroutine(sizeCheck());

        Contents.SetActive(true);
    }

    void Txt_(string type, int value)
    {
        if(type == "Part")
        {
            //tit
            txt_List[0].text = itemMgr.Pt_Data[value].Name;
            //type
            txt_List[1].text = itemMgr.Pt_Data[value].Types;
            //parts
            txt_List[2].text = itemMgr.Pt_Data[value].Parts;
            //info
            txt_List[3].text = "";

            if (itemMgr.Pt_Data[value].MaxHP != 0)
            { txt_List[3].text += "최대 생명력: " + itemMgr.Pt_Data[value].MaxHP + System.Environment.NewLine; }

            if (itemMgr.Pt_Data[value].MaxMP != 0)
            { txt_List[3].text += "최대 마력치: " + itemMgr.Pt_Data[value].MaxMP + System.Environment.NewLine; }

            if (itemMgr.Pt_Data[value].STR != 0)
            { txt_List[3].text += "근력: " + itemMgr.Pt_Data[value].STR + System.Environment.NewLine; }

            if (itemMgr.Pt_Data[value].INT != 0)
            { txt_List[3].text += "지능: " + itemMgr.Pt_Data[value].INT + System.Environment.NewLine; }

            if (itemMgr.Pt_Data[value].DEX != 0)
            { txt_List[3].text += "민첩: " + itemMgr.Pt_Data[value].DEX + System.Environment.NewLine; }

            if (itemMgr.Pt_Data[value].ATK != 0)
            { txt_List[3].text += "공격: " + itemMgr.Pt_Data[value].ATK + System.Environment.NewLine; }

            if (itemMgr.Pt_Data[value].DEF != 0)
            { txt_List[3].text += "방어: " + itemMgr.Pt_Data[value].DEF + System.Environment.NewLine; }

            if (itemMgr.Pt_Data[value].Kg != 0)
            { txt_List[3].text += "무게 : " + itemMgr.Pt_Data[value].Kg + "Kg" + System.Environment.NewLine; }

            txt_List[4].text = itemMgr.Pt_Data[value].Description + System.Environment.NewLine;
        }
        else if(type == "Item")
        {
            //tit
            txt_List[0].text = itemMgr.It_Data[value].Name;
            //type
            txt_List[1].text = itemMgr.It_Data[value].Types;
            //parts
            txt_List[2].text = itemMgr.It_Data[value].Parts;
            //info
            txt_List[3].text = "";

            if (itemMgr.It_Data[value].MaxHP != 0)
            { txt_List[3].text += "최대 생명력: " + itemMgr.It_Data[value].MaxHP + System.Environment.NewLine; }

            if (itemMgr.It_Data[value].MaxMP != 0)
            { txt_List[3].text += "최대 마력치: " + itemMgr.It_Data[value].MaxMP + System.Environment.NewLine; }

            if (itemMgr.It_Data[value].HP != 0)
            { txt_List[3].text += "생명력: " + itemMgr.It_Data[value].HP + System.Environment.NewLine; }

            if (itemMgr.It_Data[value].MP != 0)
            { txt_List[3].text += "마력: " + itemMgr.It_Data[value].MP + System.Environment.NewLine; }

            if (itemMgr.It_Data[value].STR != 0)
            { txt_List[3].text += "근력: " + itemMgr.It_Data[value].STR + System.Environment.NewLine; }

            if (itemMgr.It_Data[value].INT != 0)
            { txt_List[3].text += "지능: " + itemMgr.It_Data[value].INT + System.Environment.NewLine; }

            if (itemMgr.It_Data[value].DEX != 0)
            { txt_List[3].text += "민첩: " + itemMgr.It_Data[value].DEX + System.Environment.NewLine; }

            if (itemMgr.It_Data[value].ATK != 0)
            { txt_List[3].text += "공격: " + itemMgr.It_Data[value].ATK + System.Environment.NewLine; }

            if (itemMgr.It_Data[value].DEF != 0)
            { txt_List[3].text += "방어: " + itemMgr.It_Data[value].DEF + System.Environment.NewLine; }

            if (itemMgr.It_Data[value].Kg != 0)
            { txt_List[3].text += "무게 : " + itemMgr.It_Data[value].Kg + "Kg" + System.Environment.NewLine; }

            txt_List[4].text = itemMgr.It_Data[value].Description + System.Environment.NewLine;
        }

    }

    IEnumerator sizeCheck()
    {
        float elapsed = 0f; // 경과 시간 기록

        while (elapsed < 3f) // 3초 동안만 체크
        {
            float size_y = Align_Txts[0].sizeDelta.y + Align_Txts[1].sizeDelta.y + Space;

            if (Panel.sizeDelta.y != size_y)
            {
                Panel.sizeDelta = new Vector2(Panel.sizeDelta.x, size_y);
            }

            elapsed += Time.deltaTime; // 매 프레임 시간 누적
            yield return null;         // 다음 프레임까지 대기
        }

        Debug.Log("코루틴 종료");
    }

    public void MoveBelow()
    {
        Transform panel = Panel.GetComponent<Transform>();

        if (Inventory == null || panel == null)
        {
            Debug.LogWarning("대상 혹은 이동 오브젝트가 설정되지 않았습니다.");
            return;
        }

        // targetSibling의 인덱스를 가져와 +1로 설정
        int targetIndex = Inventory.GetSiblingIndex();
        //panel.SetSiblingIndex(targetIndex + 1);
    }
}
