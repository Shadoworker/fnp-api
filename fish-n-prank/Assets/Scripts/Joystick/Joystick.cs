using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Joystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public float m_horizontal { get { return (m_snapX) ? SnapFloat(input.x, AxisOptions.Horizontal) : input.x; } }
    public float m_vertical { get { return (m_snapY) ? SnapFloat(input.y, AxisOptions.Vertical) : input.y; } }
    public Vector2 Direction { get { return new Vector2(m_horizontal, m_vertical); } }

    public float m_HandleRange
    {
        get { return m_handleRange; }
        set { m_handleRange = Mathf.Abs(value); }
    }

    public float m_DeadZone
    {
        get { return m_deadZone; }
        set { m_deadZone = Mathf.Abs(value); }
    }

    public AxisOptions m_AxisOptions { get { return m_AxisOptions; } set { m_axisOptions = value; } }
    public bool m_SnapX { get { return m_snapX; } set { m_snapX = value; } }
    public bool m_SnapY { get { return m_snapY; } set { m_snapY = value; } }

    [SerializeField] private float m_handleRange = 1;
    [SerializeField] private float m_deadZone = 0;
    [SerializeField] private AxisOptions m_axisOptions = AxisOptions.Both;
    [SerializeField] private bool m_snapX = false;
    [SerializeField] private bool m_snapY = false;

    [SerializeField] protected RectTransform m_background = null;
    [SerializeField] private RectTransform m_handle = null;
    private RectTransform m_baseRect = null;

    private Canvas m_canvas;
    private Camera m_cam;

    private Vector2 input = Vector2.zero;

    protected virtual void Start()
    {
        m_HandleRange = m_handleRange;
        m_DeadZone = m_deadZone;
        m_baseRect = GetComponent<RectTransform>();
        m_canvas = GetComponentInParent<Canvas>();
        if (m_canvas == null)
            Debug.LogError("The Joystick is not placed inside a canvas");

        Vector2 center = new Vector2(0.5f, 0.5f);
        m_background.pivot = center;
        m_handle.anchorMin = center;
        m_handle.anchorMax = center;
        m_handle.pivot = center;
        m_handle.anchoredPosition = Vector2.zero;
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        m_cam = null;
        if (m_canvas.renderMode == RenderMode.ScreenSpaceCamera)
            m_cam = m_canvas.worldCamera;

        Vector2 position = RectTransformUtility.WorldToScreenPoint(m_cam, m_background.position);
        Vector2 radius = m_background.sizeDelta / 2;
        input = (eventData.position - position) / (radius * m_canvas.scaleFactor);
        FormatInput();
        HandleInput(input.magnitude, input.normalized, radius, m_cam);
        m_handle.anchoredPosition = input * radius * m_handleRange;
    }

    protected virtual void HandleInput(float magnitude, Vector2 normalised, Vector2 radius, Camera cam)
    {
        if (magnitude > m_deadZone)
        {
            if (magnitude > 1)
                input = normalised;
        }
        else
            input = Vector2.zero;
    }

    private void FormatInput()
    {
        if (m_axisOptions == AxisOptions.Horizontal)
            input = new Vector2(input.x, 0f);
        else if (m_axisOptions == AxisOptions.Vertical)
            input = new Vector2(0f, input.y);
    }

    private float SnapFloat(float value, AxisOptions snapAxis)
    {
        if (value == 0)
            return value;

        if (m_axisOptions == AxisOptions.Both)
        {
            float angle = Vector2.Angle(input, Vector2.up);
            if (snapAxis == AxisOptions.Horizontal)
            {
                if (angle < 22.5f || angle > 157.5f)
                    return 0;
                else
                    return (value > 0) ? 1 : -1;
            }
            else if (snapAxis == AxisOptions.Vertical)
            {
                if (angle > 67.5f && angle < 112.5f)
                    return 0;
                else
                    return (value > 0) ? 1 : -1;
            }
            return value;
        }
        else
        {
            if (value > 0)
                return 1;
            if (value < 0)
                return -1;
        }
        return 0;
    }

    public virtual void OnPointerUp(PointerEventData eventData)
    {
        input = Vector2.zero;
        m_handle.anchoredPosition = Vector2.zero;
    }

    protected Vector2 ScreenPointToAnchoredPosition(Vector2 screenPosition)
    {
        Vector2 localPoint = Vector2.zero;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(m_baseRect, screenPosition, m_cam, out localPoint))
        {
            Vector2 pivotOffset = m_baseRect.pivot * m_baseRect.sizeDelta;
            return localPoint - (m_background.anchorMax * m_baseRect.sizeDelta) + pivotOffset;
        }
        return Vector2.zero;
    }
}

public enum AxisOptions { Both, Horizontal, Vertical }