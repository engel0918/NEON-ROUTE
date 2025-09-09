using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemCtrl : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    Item_Load itemMgr;
    HoverUI hover;

    [Header("���� O")]
    public Image Thum;
    public string Item_ID;

    int It_Num;
    [SerializeField] Sprite Thum_spr;

    private void Start()
    {
        It_Num = -1;

        if (itemMgr == null)
        { itemMgr = GameObject.FindGameObjectWithTag("ItList").GetComponent<Item_Load>(); }

        ItemCheck();
    }

    void ItemCheck()
    {
        if (It_Num == -1)
        {
            for (int i = 0; i <= (itemMgr.It_Data.Count - 1); i++)
            {
                if (itemMgr.It_Data[i].Id == Item_ID)
                {
                    It_Num = i; 
                    break;
                }
            }

            for (int i = 0; i <= (itemMgr.thums.Count - 1); i++)
            {
                if (Item_ID == itemMgr.thums[i].Id)
                {
                    Thum_spr = itemMgr.thums[i].Thumnail;
                    Thum.sprite = Thum_spr;
                }
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (Input.GetMouseButton(0) == false)
        {
            if (hover == null)
            { hover = GameObject.Find("item_hover").GetComponent<HoverUI>(); }

            hover.Txting(It_Num, Thum_spr);
        }

        //Debug.Log("���콺�� �̹��� ���� �ö��");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (hover == null)
        { hover = GameObject.Find("item_hover").GetComponent<HoverUI>(); }

        hover.Contents.SetActive(false);

        //Debug.Log("���콺�� �̹������� ���");
    }
}
