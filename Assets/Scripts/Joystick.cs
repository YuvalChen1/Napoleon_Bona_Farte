using UnityEngine;
using UnityEngine.EventSystems;

public class Joystick : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    public RectTransform background;
    private RectTransform knob;
    private Vector2 inputVector;

    private void Start()
    {
        knob = GetComponent<RectTransform>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 position = eventData.position - (Vector2)background.position;
        float radius = background.sizeDelta.x / 2;
        inputVector = (position.magnitude > radius) ? position.normalized : position / radius;
        knob.anchoredPosition = inputVector * radius;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        inputVector = Vector2.zero;
        knob.anchoredPosition = Vector2.zero;
    }

    public Vector2 GetInputVector()
    {
        return inputVector;
    }
}
