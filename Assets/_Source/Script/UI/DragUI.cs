using UnityEngine;
using UnityEngine.EventSystems;

public class DragUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private Canvas canvas;
    private Vector2 originalPosition;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalPosition = rectTransform.anchoredPosition;
        //rectTransform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (canvas == null)
            return;

        // 드래그 후 새로운 위치 계산
        Vector2 newPos = rectTransform.anchoredPosition + eventData.delta / canvas.scaleFactor;

        // 화면 끝 좌표를 고려해서 제한
        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);

        Vector3 bottomLeft = corners[0] + (Vector3)(eventData.delta / canvas.scaleFactor);
        Vector3 topRight = corners[2] + (Vector3)(eventData.delta / canvas.scaleFactor);

        Vector2 screenMin = Vector2.zero;
        Vector2 screenMax = new Vector2(Screen.width, Screen.height);

        Vector2 offset = Vector2.zero;

        if (bottomLeft.x < screenMin.x) offset.x = screenMin.x - bottomLeft.x;
        if (bottomLeft.y < screenMin.y) offset.y = screenMin.y - bottomLeft.y;
        if (topRight.x > screenMax.x) offset.x = screenMax.x - topRight.x;
        if (topRight.y > screenMax.y) offset.y = screenMax.y - topRight.y;

        rectTransform.anchoredPosition = newPos + offset;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // 필요 시 드래그 종료 처리
    }
}
