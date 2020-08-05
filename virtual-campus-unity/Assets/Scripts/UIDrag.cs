using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class UIDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Precision")]
    public bool m_isPrecision;
    private Vector3 m_offset;
    private RectTransform m_rt;

    public int xShift;
    public int yShift;

    void Start()
    {
        m_rt = gameObject.GetComponent<RectTransform>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (m_isPrecision)
        {
            Vector3 tWorldPos;
            RectTransformUtility.ScreenPointToWorldPointInRectangle(m_rt, eventData.position, eventData.pressEventCamera, out tWorldPos);
            m_offset = transform.position - tWorldPos;
        }
        else
        {
            m_offset = Vector3.zero;
        }

        SetDraggedPosition(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        SetDraggedPosition(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        SetDraggedPosition(eventData);
    }

    private void SetDraggedPosition(PointerEventData eventData)
    {
        Vector3 globalMousePos;
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(m_rt, eventData.position, eventData.pressEventCamera, out globalMousePos))
        {
            m_rt.position = globalMousePos + m_offset;
        }
    }

    void Update()
    {

        if (m_rt.anchoredPosition.x > xShift)
        {
            m_rt.anchoredPosition = new Vector2(xShift, m_rt.anchoredPosition.y);
        }
        else if (m_rt.anchoredPosition.x < -xShift)
        {
            m_rt.anchoredPosition = new Vector2(-xShift, m_rt.anchoredPosition.y);
        }
        if (m_rt.anchoredPosition.y > yShift)
        {
            m_rt.anchoredPosition = new Vector2(m_rt.anchoredPosition.x, yShift);
        }
        else if (m_rt.anchoredPosition.y < -yShift)
        {
            m_rt.anchoredPosition = new Vector2(m_rt.anchoredPosition.x, -yShift);
        }
    }
}