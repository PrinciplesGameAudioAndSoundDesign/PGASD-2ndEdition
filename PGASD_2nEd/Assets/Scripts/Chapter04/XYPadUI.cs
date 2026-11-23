using UnityEngine;
using UnityEngine.EventSystems;

public class XYPadUI : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    [Header("Refs")]
    public RectTransform padRect;
    public RectTransform handleRect;
    public XYEngine engine;

    void Reset()
    {
        padRect = GetComponent<RectTransform>();
    }

    public void OnPointerDown(PointerEventData eventData) => UpdateFromPointer(eventData);
    public void OnDrag(PointerEventData eventData) => UpdateFromPointer(eventData);

    private void UpdateFromPointer(PointerEventData eventData)
    {
        if (!padRect || !handleRect || !engine) return;

        // Convert pointer -> local pad space
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            padRect,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 local);

        // local is centered; map to 0..1
        Rect r = padRect.rect;
        float x01 = Mathf.InverseLerp(r.xMin, r.xMax, local.x);
        float y01 = Mathf.InverseLerp(r.yMin, r.yMax, local.y);

        x01 = Mathf.Clamp01(x01);
        y01 = Mathf.Clamp01(y01);

        // Move handle
        float hx = Mathf.Lerp(r.xMin, r.xMax, x01);
        float hy = Mathf.Lerp(r.yMin, r.yMax, y01);
        handleRect.anchoredPosition = new Vector2(hx, hy);

        // Drive engine
        engine.SetXY(new Vector2(x01, y01));
    }
}
