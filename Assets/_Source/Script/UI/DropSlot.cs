using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DropSlot : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            DragItem dragItem = eventData.pointerDrag.GetComponent<DragItem>();
            if (dragItem == null) return; // DragItem이 아닌 경우 무시

            // 슬롯으로 아이템 옮기기
            eventData.pointerDrag.transform.SetParent(transform);
            eventData.pointerDrag.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

            // 원래 슬롯 갱신
            dragItem.SetNewParent(transform);
        }
    }
}