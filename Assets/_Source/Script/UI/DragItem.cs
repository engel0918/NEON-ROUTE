using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DragItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Canvas canvas;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    public Transform BeforeParents;
    public Transform originalParent; // 무조건 슬롯 저장

    ItemCtrl it;
    Inven_Ctrl inven;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();

        // 시작할 때 이미 슬롯 밑에 있으면 → 그 슬롯을 originalParent로 저장
        originalParent = transform.parent;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // 현재 슬롯 저장
        originalParent = transform.parent;

        // 드래그 중에는 Canvas로 올려서 다른 슬롯 위에 겹치도록 함
        transform.SetParent(canvas.transform, true);

        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // 드롭 실패 → 원래 슬롯으로 돌아가기
        if (transform.parent == canvas.transform)
        {
            transform.SetParent(originalParent);
            rectTransform.anchoredPosition = Vector2.zero;
        }

        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
    }

    // 드롭 성공 시 슬롯에서 호출
    public void SetNewParent(Transform newParent)
    {
        if (newParent.childCount > 0)
        {
            if(inven == null) { inven = GameObject.FindGameObjectWithTag("InvenCtrl").GetComponent<Inven_Ctrl>(); }
            inven.Exchange(newParent, originalParent, transform);
        }
        else
        {
            BeforeParents = originalParent;
            originalParent = newParent;

            if (it == null) { it = GetComponent<ItemCtrl>(); }
            it.inven.MoveSlot(originalParent, BeforeParents);
        }
    }
}