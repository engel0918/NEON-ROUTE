using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DragItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Canvas canvas;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    public Transform BeforeParents;
    public Transform originalParent; // ������ ���� ����

    ItemCtrl it;
    Inven_Ctrl inven;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();

        // ������ �� �̹� ���� �ؿ� ������ �� �� ������ originalParent�� ����
        originalParent = transform.parent;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // ���� ���� ����
        originalParent = transform.parent;

        // �巡�� �߿��� Canvas�� �÷��� �ٸ� ���� ���� ��ġ���� ��
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
        // ��� ���� �� ���� �������� ���ư���
        if (transform.parent == canvas.transform)
        {
            transform.SetParent(originalParent);
            rectTransform.anchoredPosition = Vector2.zero;
        }

        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
    }

    // ��� ���� �� ���Կ��� ȣ��
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