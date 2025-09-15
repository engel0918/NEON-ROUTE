using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DropSlot : MonoBehaviour, IDropHandler
{
    public string Type;

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null) return;

        DragItem dragItem = eventData.pointerDrag.GetComponent<DragItem>();
        if (dragItem == null) return; // DragItem이 없는 경우 무시

        ItemCtrl itCtrl = dragItem.GetComponent<ItemCtrl>();
        if (itCtrl == null) return; // ItemCtrl 없는 경우 무시

        if (Type != "Quick" && itCtrl.Type != Type) return; // 타입 검사

        // 슬롯이 비어있으면 아이템 옮기기
        if (transform.childCount <= 0)
        {
            eventData.pointerDrag.transform.SetParent(transform);
            eventData.pointerDrag.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        }

        // 원래 슬롯 갱신
        dragItem.SetNewParent(transform);
    }
}