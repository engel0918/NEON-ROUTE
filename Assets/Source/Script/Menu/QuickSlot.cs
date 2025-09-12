using System;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.Rendering.VolumeComponent;

public class QuickSlot : MonoBehaviour, IPointerClickHandler
{
    int Order;
    Inven_Ctrl inven;

    private void Start()
    {
        inven = GameObject.FindGameObjectWithTag("InvenCtrl").GetComponent<Inven_Ctrl>();
        Order = transform.GetSiblingIndex();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right) // ��Ŭ��
        {
            if (transform.childCount > 0)
            {
                if (inven == null) { GameObject.FindGameObjectWithTag("InvenCtrl").GetComponent<Inven_Ctrl>(); }

                ItemCtrl it = transform.GetChild(0).GetComponent<ItemCtrl>();
                inven.Quick_to_Mov(Order, it);
            }
            Debug.Log("UI ��� ��Ŭ��!");
        }
    }
}
